using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class CustomScrollList<T> : MonoBehaviour,IEndDragHandler,IBeginDragHandler,IDragHandler
{
    private float m_itemHeight = 100f; 
    private float m_itemWidth = 100f;
    private ListDataBase<T> m_listData;
    private List<RectTransform> m_VisibalPool = new List<RectTransform>();
    private int m_startIndex = 0;//m_VisibalPool[0] index in m_AllData
    
    [Header("Scroll Settings")]
    [SerializeField] private RectTransform m_viewPort;
    [SerializeField] private RectTransform m_content;
    [SerializeField] private float m_intensity = 1;
    [SerializeField] private RectTransform m_itemPrefab;
    [SerializeField] private bool m_isVertical = true;
    [SerializeField] private float m_spacing = 0f;

    [Header("Settings: Visible Item Count")]
    [SerializeField] private int m_visibleItemCount = 5;
    [SerializeField] private Vector2 m_offset = Vector2.zero;
    
    private void Awake()
    {
        m_listData = GetComponent(typeof(ListDataBase<T>)) as ListDataBase<T>;
    }

    private void Start()
    {
        m_itemHeight =  m_itemPrefab.GetComponent<RectTransform>().rect.height;
        m_itemWidth =  m_itemPrefab.GetComponent<RectTransform>().rect.width;
        InitializeViewport();
        InitializeItemMapping();
        InitializePool();
    }

    void InitializeItemMapping()
    {
        /*foreach (var item in m_DataSettings)
        {
            m_ItemMapping.Add(item._name, item);
        }*/
    }
    //auto change viewport size and position
    void InitializeViewport()
    {
        //con:use top-left
        if (m_isVertical)
        {
            float viewPortHeight = (m_visibleItemCount - 1) * (m_itemHeight + m_spacing) - m_spacing;
            m_viewPort.sizeDelta = new Vector2(m_itemWidth, viewPortHeight);
            m_viewPort.anchoredPosition = new Vector2(m_itemWidth / 2, -viewPortHeight / 2);
        }
        else
        {
            float viewPortWidth = (m_visibleItemCount - 1) * (m_itemWidth + m_spacing) - m_spacing;
            m_viewPort.sizeDelta = new Vector2(viewPortWidth, m_itemHeight);
            m_viewPort.anchoredPosition = new Vector2(viewPortWidth / 2, -m_itemHeight / 2);
        }
        // 偏移量
        m_viewPort.anchoredPosition += m_offset;
    }
    void InitializePool()
    {
        for (int i = 0; i < m_visibleItemCount; i++)
        {
            var item = CreateItem();
            m_VisibalPool.Add(item);
            
            // 设置初始位置
            if (m_isVertical)
            {
                float pos = i * (m_itemHeight + m_spacing) + m_itemHeight / 2;
                 item.anchoredPosition = new Vector2(m_itemWidth / 2, -pos);
            }
            else
            {
                float pos = i * (m_itemWidth + m_spacing) + m_itemWidth / 2;
                 item.anchoredPosition = new Vector2(pos, - m_itemHeight / 2);
            }
               
            
            // 设置初始数据
            SetItemInfo(item, i);
            
        }
    }
    
    RectTransform CreateItem(int index = -1)
    {
        var item = Instantiate(m_itemPrefab, m_content);
        item.gameObject.SetActive(true);
        SetItemInfo(item, index);
        return item;
    }

    #region EdgeHandle
    void PhysicalMoveBottomToTop()//physical move
    {
        m_startIndex = (m_startIndex - 1 + m_visibleItemCount) % m_visibleItemCount;
        Debug.LogWarning("physical move bottom to top, m_startIndex: " + m_startIndex);
        //获得顶部元素位置
        float topPos;
        if (m_isVertical)
        {
            topPos = m_VisibalPool[0].anchoredPosition.y;
        }
        else
        {
            topPos = m_VisibalPool[0].anchoredPosition.x;
        }
        // 计算新位置
        float newPos;
        if (m_isVertical)
        {
            newPos = topPos + (m_itemHeight + m_spacing);
        }
        else
        {
            newPos = topPos - (m_itemWidth + m_spacing);
        }
        
        // 更新底部部元素位置
        if (m_isVertical)
        {
            m_VisibalPool[m_visibleItemCount - 1].anchoredPosition = new Vector2(m_itemWidth / 2, newPos);
        }
        else
        {
            m_VisibalPool[m_visibleItemCount - 1].anchoredPosition = new Vector2(newPos, -m_itemHeight / 2);
        }
        RectTransform temp= m_VisibalPool[m_visibleItemCount - 1];
        m_VisibalPool.RemoveAt(m_visibleItemCount - 1);
        m_VisibalPool.Insert(0, temp);
        UpdateItemPool();
    }
    void PhysicalMoveTopToBottom()
    {
        m_startIndex = (m_startIndex + 1 + m_visibleItemCount) % m_visibleItemCount;
        Debug.LogWarning("physical move top to bottom, m_startIndex: " + m_startIndex);
        //获得底部元素位置
        float bottomPos;
        if (m_isVertical)
        {
            bottomPos = m_VisibalPool[m_visibleItemCount - 1].anchoredPosition.y;
        }
        else
        {
            bottomPos = m_VisibalPool[m_visibleItemCount - 1].anchoredPosition.x;
        }
        // 计算新位置
        float newPos;
        if (m_isVertical)
        {
            newPos = bottomPos - (m_itemHeight + m_spacing);
        }
        else
        {
            newPos = bottomPos + (m_itemWidth + m_spacing);
        }
        
        // 更新顶部元素位置
        if (m_isVertical)
        {
            m_VisibalPool[0].anchoredPosition = new Vector2(m_itemWidth / 2, newPos);
        }
        else
        {
            m_VisibalPool[0].anchoredPosition = new Vector2(newPos, -m_itemHeight / 2);
        }
        RectTransform temp= m_VisibalPool[0];
        m_VisibalPool.Insert(m_visibleItemCount, temp);
        m_VisibalPool.RemoveAt(0);
        UpdateItemPool();
    }
    void UpdatePosition()
    {
        // 根据第二个元素的位置进行计算当前应该显示的起始索引，预留缓冲
        if (m_isVertical)
        {
            if (m_VisibalPool[1].anchoredPosition.y >= -m_itemHeight / 2)
            {
                Debug.Log("m_content.anchoredPosition.y: " + m_content.anchoredPosition.y);
                Debug.Log("m_VisibalPool[1].anchoredPosition.y: " + m_VisibalPool[1].anchoredPosition.y);
                PhysicalMoveTopToBottom();
            }

            // 第二个元素位置过低
            if (m_VisibalPool[1].anchoredPosition.y <= -3 * m_itemHeight / 2)
            {
                PhysicalMoveBottomToTop();
            }

        }
        else
        {
            if (m_VisibalPool[1].anchoredPosition.x <= m_itemWidth / 2)
            {
                PhysicalMoveTopToBottom();
            }
               
            
            if (m_VisibalPool[1].anchoredPosition.x >= 3 * m_itemWidth / 2)
            {
                PhysicalMoveBottomToTop();
            }

        }
    }
    
    #endregion
    /// <summary>
    /// refresh item pool
    /// </summary>
    /// <param name="startIndex"></param>
    public void UpdateItemPool()
    {
        for (int i = 0; i < m_visibleItemCount; i++)
        {
            int cnt = Mathf.Max(m_visibleItemCount, m_listData.m_allItem.Count);
            int index = (m_startIndex + i + cnt) % cnt;
            SetItemInfo(m_VisibalPool[i], index);
        }
    }
    void SetItemInfo(RectTransform rec, int index)
    {
        m_listData.SetItemInfo(rec, index);
        /*Item item = rec.GetComponent<Item>();
        if (index == -1 || index >= CustomScrollListData.Instance.m_ItemOrder.Count)
        {
            // 设置默认值
            item.SetItemInfo("None", null, new List<RegionTag>(), 0,false);
            return;
        }
        
        // 设置物品信息
        string objName = CustomScrollListData.Instance.m_ItemOrder[index];
        Sprite sprite = null;
        List<RegionTag> region = new List<RegionTag>();
        if (m_ItemMapping.ContainsKey(objName))
        {
            sprite = m_ItemMapping[objName]._sprite;
            region = m_ItemMapping[objName]._region;
        }

        bool isEqual = CustomScrollListData.Instance.m_curNeedCount[objName] == 0;
        item.SetItemInfo(objName, sprite, region, CustomScrollListData.Instance.m_curNeedCount[objName],isEqual); */
    }

    #region DragHandle

        private Vector3 t_dragStartPos;
        private Vector3 t_dragEndPos;
        
        // 最终计算出的拖动向量（方向：开始 → 结束）
        public Vector3 DragVector { get; private set; }
        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("OnBeginDrag");
            t_dragStartPos = eventData.position;
        }
    
        public void OnEndDrag(PointerEventData eventData)
        {
            
        }

        public void OnDrag(PointerEventData eventData)
        {
            t_dragEndPos = eventData.position;
            DragVector = t_dragEndPos - t_dragStartPos;
            Debug.Log($"Drag Vector: {DragVector}");
            foreach (var item in m_VisibalPool)
            {
                Vector3 offset = DragVector * m_intensity;
                if (m_isVertical)
                {
                    item.anchoredPosition += new Vector2(0, offset.y);
                }
                else
                {
                    item.anchoredPosition += new Vector2(offset.x, 0);
                }
            }
            t_dragStartPos = eventData.position;
            UpdatePosition();
        }

        #endregion DragHandle
}
