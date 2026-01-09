using System;
using Global.Scripts;
using UnityEngine;

namespace MarkingList.Scripts
{
    public class ShoppingListController : MonoBehaviour
    {
        [Header("Event")]
        [SerializeField] private String_IntSO m_addToCartEvent;
        [SerializeField] private String_IntSO m_removeFromCartEvent;

        [SerializeField] private VoidSO m_taskAcceptEvent;
        [SerializeField] private VoidSO m_taskEndEvent;
        
        [SerializeField] private VoidSO m_gameEndEvent;
        [Header("Reference")]
        private ShoppingListData m_shoppingListData;
        private ShoppingScrollList m_shoppingList;
        [Header("Status")]
        private bool m_isTaskAccept = false;
        private void Start()
        {
            m_shoppingListData = FindObjectOfType<ShoppingListData>();
            m_shoppingList = FindObjectOfType<ShoppingScrollList>();
        }

        private void OnEnable()
        {
            m_taskAcceptEvent.action += OnTaskAccept;
            m_taskEndEvent.action += OnTaskEnd;
            m_addToCartEvent.action += RemoveNeed;
            m_removeFromCartEvent.action += AddNeed;
        }

        private void OnDisable()
        {
            m_taskAcceptEvent.action -= OnTaskAccept;
            m_taskEndEvent.action -= OnTaskEnd;
            m_addToCartEvent.action -= RemoveNeed;
            m_removeFromCartEvent.action -= AddNeed;
        }

        private void SendDataToGameManager()
        {
            int cnt = m_shoppingListData.AchieveNeed();
            GameManager.Instance.Update_Shopping(cnt);
        }

        private void OnTaskEnd()
        {
            m_isTaskAccept = false;
        }

        private void OnTaskAccept()
        {
            m_isTaskAccept = true;
        }

        private void AddNeed(string productName,int count)
        {
            
        }
        private void RemoveNeed(string productName,int count)
        {
            if (m_isTaskAccept) return;
            m_shoppingListData.RemoveNeed(productName, count);
            m_shoppingList.UpdateItemPool();
            SendDataToGameManager();
        }
    }
}