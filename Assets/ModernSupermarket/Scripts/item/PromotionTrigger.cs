using UnityEngine;

public class SaleTrigger : MonoBehaviour
{
    public GameObject saleSign;  // 拖入促销标志物体

    void Start()
    {
        // 1. 确保是触发器
        GetComponent<Collider>().isTrigger = true;

        // 2. 隐藏促销标志
        if (saleSign != null)
            saleSign.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera") && saleSign != null)
            saleSign.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera") && saleSign != null)
            saleSign.SetActive(false);
    }
}


