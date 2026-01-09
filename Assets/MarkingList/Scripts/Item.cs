using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MarkingList.Scripts
{
    public class Item : MonoBehaviour
    {
        [Header("Item Info")] [SerializeField] [ReadOnly]
        private string m_name;

        [SerializeField] [ReadOnly] private string m_region;
        [SerializeField] [ReadOnly] private int m_count;

        [SerializeField] [ReadOnly] private bool m_isEqual;

        // 添加UI引用，用于显示物品信息
        [Header("Debug Reference")] 
        [SerializeField] private TextMeshProUGUI m_itemNameText;

        [SerializeField] private TextMeshProUGUI m_itemRegionText;
        [SerializeField] private TextMeshProUGUI m_itemCountText;
        [SerializeField] private Image m_itemIcon;
        [SerializeField] private Image m_itemCheck;

        void Awake()
        {
            // 初始化UI引用
            m_itemNameText = transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            m_itemRegionText = transform.Find("ItemRegion").GetComponent<TextMeshProUGUI>();
            m_itemCountText = transform.Find("ItemCount").GetComponent<TextMeshProUGUI>();
            m_itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
            m_itemCheck = transform.Find("ItemCheck").GetComponent<Image>();
        }

        public void SetItemInfo(string objName, Sprite icon, List<string> region, int count, bool isAdded)
        {
            m_name = objName;
            m_region = string.Join(", ", region);
            m_count = count >= 0 ? count : 0;
            m_itemIcon.sprite = icon;
            m_isEqual = m_count == 0;
            UpdateItemUI();
        }

        public void SetAddedInfo(bool isAdded)
        {
            m_isEqual = isAdded;
            UpdateItemUI();
        }

        void UpdateItemUI()
        {
            // 更新UI显示
            m_itemNameText.text = m_name;
            m_itemRegionText.text = m_region;
            m_itemCountText.text = m_count.ToString();
            m_itemIcon.sprite = m_itemIcon.sprite;
            m_itemCheck.gameObject.SetActive(m_isEqual);
        }
    }
}