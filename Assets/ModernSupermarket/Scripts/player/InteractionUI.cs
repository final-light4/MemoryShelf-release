using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public Text promptText;
    public Text itemInfoText;

    void Start()
    {
        HidePrompt();
        HideItemInfo();
    }

    public void ShowPrompt(string msg)
    {
        if (promptText)
        {
            promptText.gameObject.SetActive(true);
            promptText.text = msg;
        }
    }

    public void HidePrompt()
    {
        if (promptText) promptText.gameObject.SetActive(false);
    }

    public void ShowItemInfo(string name, float price, string desc, bool isDiscounted, float promotionPrice)
    {
        if (itemInfoText)
        {
            itemInfoText.gameObject.SetActive(true);

            // 如果商品有折扣，显示删除线的原价和促销价
            if (isDiscounted)
            {
                // 显示带删除线的原价和促销价
                itemInfoText.text = $"{name}\n<color=red><s>{price:F2}</s></color>\n促销价: {promotionPrice:F2}\n{desc}";
            }
            else
            {
                // 如果没有折扣，只显示原价
                itemInfoText.text = $"{name}\n价格: {price:F2}\n{desc}";
            }
        }
    }


    public void HideItemInfo()
    {
        if (itemInfoText) itemInfoText.gameObject.SetActive(false);
    }
}
