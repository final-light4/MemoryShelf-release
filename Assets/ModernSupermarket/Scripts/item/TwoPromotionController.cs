using UnityEngine;

public class TwoPromotionController : MonoBehaviour
{
    [Header("Sale Signs (两个标志)")]
    public GameObject saleSignA;
    public GameObject saleSignB;

    [Header("Flash Sale Config")]
    [Range(0f, 1f)] public float triggerProbability = 0.5f;
    public float checkInterval = 30f;
    public float saleDuration = 30f;
    public float discount = 0.5f;

    [Header("Runtime")]
    public bool isOnSale = false;

    private PickableItem[] items;
    private float timer;

    private void Awake()
    {
        items = GetComponentsInChildren<PickableItem>(true);
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
        if (saleSignA != null) saleSignA.SetActive(active);
        if (saleSignB != null) saleSignB.SetActive(active);
    }

    public void RefreshItems()
    {
        items = GetComponentsInChildren<PickableItem>(true);
    }
}

