using UnityEngine;

public class PromotionController : MonoBehaviour
{
    [Header("Sale Sign (子物体)")]
    public GameObject saleSign; // 拖你这个父物体下面的 SALE 标志进来

    [Header("Flash Sale Config")]
    [Range(0f, 1f)] public float triggerProbability = 0.5f; // 每次检查触发概率(50%)
    public float checkInterval = 30f;   // 检查周期(秒)
    public float saleDuration = 30f;    // 促销持续时间(秒)
    public float discount = 0.5f;       // 折扣(0.5=半价)

    [Header("Runtime")]
    public bool isOnSale = false;

    private PickableItem[] items;
    private float timer;

    private void Awake()
    {
        // 只获取“这个父物体”下面的商品
        items = GetComponentsInChildren<PickableItem>(true);
    }

    private void Start()
    {
        timer = checkInterval;

        // 默认先关掉促销标志
        if (saleSign != null) saleSign.SetActive(false);

        // 保底：开局全部恢复到非促销（避免你之前全局脚本残留状态）
        foreach (var it in items)
        {
            if (it != null) it.RestorePrice();
        }
    }

    private void Update()
    {
        // 促销期间不再重复触发
        if (isOnSale) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            timer = checkInterval;
            TryStartSale();
        }
    }

    private void TryStartSale()
    {
        if (isOnSale) return;

        // Random.value 是 0~1
        if (Random.value <= triggerProbability)
        {
            StartCoroutine(SaleRoutine());
        }
    }

    private System.Collections.IEnumerator SaleRoutine()
    {
        isOnSale = true;

        if (saleSign != null) saleSign.SetActive(true);

        // 开始促销：只影响这个父物体下的商品
        foreach (var it in items)
        {
            if (it != null) it.ApplyDiscount(discount);
        }

        // 等待 saleDuration 秒
        yield return new WaitForSeconds(saleDuration);

        // 结束促销：恢复原价 + 关键取消 isDiscounted
        foreach (var it in items)
        {
            if (it != null) it.RestorePrice();
        }

        if (saleSign != null) saleSign.SetActive(false);

        isOnSale = false;
        timer = checkInterval; // 结束后从头再等一个周期
    }

    // 如果你运行中动态生成/添加商品，可以点一下这个重新扫描
    public void RefreshItems()
    {
        items = GetComponentsInChildren<PickableItem>(true);
    }
}
