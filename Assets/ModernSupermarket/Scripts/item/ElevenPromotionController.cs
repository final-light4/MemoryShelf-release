using UnityEngine;

public class ElevenPromotionController : MonoBehaviour
{
    [Header("Sale Signs (十一个标志)")]
    public GameObject saleSignA;
    public GameObject saleSignB;
    public GameObject saleSignC;
    public GameObject saleSignD;
    public GameObject saleSignE;
    public GameObject saleSignF;
    public GameObject saleSignG;
    public GameObject saleSignH;
    public GameObject saleSignI;
    public GameObject saleSignJ;
    public GameObject saleSignK;

    [Header("Flash Sale Config")]
    [Range(0f, 1f)] public float triggerProbability = 0.5f;
    public float checkInterval = 30f;
    public float saleDuration = 30f;
    public float discount = 0.5f;

    [Header("Runtime")]
    public bool isOnSale = false;

    private PickableItem[] items;
    private float timer;

    // 存储所有11个标志的数组（方便统一控制）
    private GameObject[] allSaleSigns;

    private void Awake()
    {
        items = GetComponentsInChildren<PickableItem>(true);

        // 初始化11个标志的数组
        allSaleSigns = new GameObject[]
        {
            saleSignA, saleSignB, saleSignC, saleSignD, saleSignE,
            saleSignF, saleSignG, saleSignH, saleSignI, saleSignJ, saleSignK
        };
    }

    private void Start()
    {
        timer = checkInterval;

        SetSignsActive(false);

        foreach (var it in items)
        {
            if (it != null) it.RestorePrice();
        }
    }

    private void Update()
    {
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

        if (Random.value <= triggerProbability)
        {
            StartCoroutine(SaleRoutine());
        }
    }

    private System.Collections.IEnumerator SaleRoutine()
    {
        isOnSale = true;

        SetSignsActive(true);

        foreach (var it in items)
        {
            if (it != null) it.ApplyDiscount(discount);
        }

        yield return new WaitForSeconds(saleDuration);

        foreach (var it in items)
        {
            if (it != null) it.RestorePrice();
        }

        SetSignsActive(false);

        isOnSale = false;
        timer = checkInterval;
    }

    private void SetSignsActive(bool active)
    {
        // 遍历所有11个标志，统一设置激活状态
        foreach (var sign in allSaleSigns)
        {
            if (sign != null)
            {
                sign.SetActive(active);
            }
        }
    }

    public void RefreshItems()
    {
        items = GetComponentsInChildren<PickableItem>(true);
    }
}
