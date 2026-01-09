using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PromotionEventManager : MonoBehaviour
{
    public static PromotionEventManager Instance;  // 单例实例
    private List<PickableItem> pickableItems = new List<PickableItem>(); // 存储所有商品

    // 存储“Tag大类 → 对应子商品”的映射（用你已设置的Tag）
    private Dictionary<string, List<PickableItem>> _tagToItems = new Dictionary<string, List<PickableItem>>();

    [Header("促销基础配置")]
    public float defaultDiscountRate = 0.5f; // 默认5折
    public int targetSelectedCount = 7; // 固定选7个大类

    [Header("自动触发配置")]
    public float checkInterval = 30f; // 每30秒检查一次
    public float promotionDuration = 30f; // 每次促销持续30秒
    public float triggerProbability = 0.1f; // 10%触发概率（0.1=10%）

    private bool isPromotionActive = false; // 当前是否有促销在进行
    private float promotionTimer = 0f; // 促销持续计时器
    private List<string> currentSelectedTags = new List<string>(); // 本次促销选中的大类

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 确保不被销毁
        }
        else
        {
            Destroy(gameObject);  // 如果已有实例，则销毁新创建的
        }

        // 获取场景中的所有 PickableItem 对象并加入列表
        pickableItems.AddRange(FindObjectsOfType<PickableItem>());
        Debug.Log("收集到 " + pickableItems.Count + " 个商品");

        // 自动绑定“Tag大类 → 子商品”（用你已设置的Tag）
        BindTagToItems();
    }

    void Start()
    {
        // 启动30秒循环检查（重复调用CheckPromotionTrigger方法）
        InvokeRepeating("CheckPromotionTrigger", 0f, checkInterval);
    }

    void Update()
    {
        // 如果促销正在进行，倒计时
        if (isPromotionActive)
        {
            promotionTimer += Time.deltaTime;
            // 促销时间到，恢复原价
            if (promotionTimer >= promotionDuration)
            {
                EndPromotion();
            }
        }
    }

    // 绑定：按父物体的Tag分类子商品
    private void BindTagToItems()
    {
        foreach (PickableItem item in pickableItems)
        {
            Transform parent = item.transform.parent;
            if (parent == null) continue;

            string parentTag = parent.tag;
            if (string.IsNullOrEmpty(parentTag) || parentTag == "Untagged") continue;

            if (!_tagToItems.ContainsKey(parentTag))
            {
                _tagToItems[parentTag] = new List<PickableItem>();
            }
            _tagToItems[parentTag].Add(item);
        }

        // 打印绑定结果（方便调试）
        foreach (var pair in _tagToItems)
        {
            Debug.Log($"大类Tag【{pair.Key}】绑定了 {pair.Value.Count} 个商品");
        }
    }

    // 每30秒执行一次：检查是否触发促销
    private void CheckPromotionTrigger()
    {
        // 如果当前已有促销在进行，跳过本次检查
        if (isPromotionActive)
        {
            Debug.Log($"当前已有促销进行中，跳过本次检查（剩余时间：{promotionDuration - promotionTimer:F1}秒）");
            return;
        }

        // 10%概率触发（Random.value返回0-1之间的随机数）
        float randomValue = Random.value;
        Debug.Log($"促销触发检查：随机概率 {randomValue:F2}（阈值：{triggerProbability:F2}）");

        if (randomValue <= triggerProbability)
        {
            // 触发促销
            StartPromotion();
        }
        else
        {
            Debug.Log("本次检查未触发促销，30秒后再次检查");
        }
    }

    // 开始促销：随机选7个大类打五折
    private void StartPromotion()
    {
        int totalTags = _tagToItems.Count;
        if (totalTags == 0)
        {
            Debug.LogError("没有找到带Tag的商品大类！无法触发促销");
            return;
        }

        // 计算实际选中数量（不足7个则选全部）
        int actualSelectedCount = Mathf.Min(targetSelectedCount, totalTags);

        // 随机选7个大类
        List<string> allTags = new List<string>(_tagToItems.Keys);
        currentSelectedTags = allTags.OrderBy(x => Random.value).Take(actualSelectedCount).ToList();

        // 给选中的大类商品打五折
        foreach (string tag in currentSelectedTags)
        {
            List<PickableItem> targetItems = _tagToItems[tag];
            foreach (var item in targetItems)
            {
                if (item != null)
                {
                    item.ApplyDiscount(defaultDiscountRate);
                    Debug.Log($"【促销触发】{tag}类商品 {item.itemName} 五折后价格：{item.GetCurrentPrice():F2}");
                }
            }
            Debug.Log($"【促销触发】{tag}大类已打五折，共 {targetItems.Count} 个商品");
        }

        // 标记促销状态，启动计时器
        isPromotionActive = true;
        promotionTimer = 0f;

        // 打印促销开始信息
        Debug.Log($"===== 促销开始！共选中 {currentSelectedTags.Count} 个大类，持续 {promotionDuration} 秒 =====");
    }

    // 结束促销：恢复所有商品原价
    private void EndPromotion()
    {
        RestoreAllItemsToItemPrice();
        isPromotionActive = false;
        currentSelectedTags.Clear();
        Debug.Log("===== 促销结束！所有商品已恢复原价 =====");
    }

    // 手动触发促销（用于按钮测试，可选保留）
    public void ApplyDiscountTo7RandomTags()
    {
        if (isPromotionActive)
        {
            Debug.Log("当前已有促销进行中，无法手动触发");
            return;
        }
        StartPromotion();
    }

    // 恢复所有商品原价
    public void RestoreAllItemsToItemPrice()
    {
        foreach (var item in pickableItems)
        {
            if (item != null)
            {
                // 注意：这里修复了你原来的bug！原来写的item.itemPrice = item.itemPrice等于没改
                item.promotionPrice = item.itemPrice; // 促销价重置为原价（适配你的PickableItem逻辑）
                item.isDiscounted = false; // 取消促销标记
                item.UpdatePriceUI(); // 更新UI显示原价
                Debug.Log($"商品 {item.itemName} 已恢复原价：{item.itemPrice:F2}");
            }
        }
    }
}



