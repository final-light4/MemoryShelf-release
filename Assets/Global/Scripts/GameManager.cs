using System;
using MarkingList.Scripts;
using NPC.Scripts;
using TMPro;
using UnityEngine;

namespace Global.Scripts
{
    public class GameManager:MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager Instance=>_instance;

        private void Awake()
        {
            if (Instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        [Header("Event: Update Achievement")] 
        [SerializeField] private IntSO m_achive_grama;

        [SerializeField] private IntSO m_achive_shopping;
        [SerializeField] private VoidSO m_achive_female;
        
        [SerializeField] private VoidSO m_gameEnd;
        [Header("Debug: Data")]
        //shoppingList Data.cs send result to it
        [SerializeField] private int m_shoppingListCount;
        private int m_shopping_achived = 0;
        
        private bool m_grama_achived = false;
        private int m_grama_cnt;
        private bool m_female_achived = false;
        [Header("Component")] [SerializeField] private GameObject m_gameEndPanel;
        
        [SerializeField] private GameObject m_shoppingUI;
        [SerializeField] private TextMeshProUGUI m_shoppingEndText;
        [SerializeField] private string[] m_shoppingEndTexts;
        
        [SerializeField] private GameObject m_femaleUI;
        [SerializeField] private TextMeshProUGUI m_femaleEndText;
        [SerializeField] private string m_femaleEndTexts;
        
        [SerializeField] private GameObject m_gramaUI;
        [SerializeField] private TextMeshProUGUI m_gramaEndText;
        [SerializeField]  private string[] m_gramaEndTexts;

        [SerializeField] private ShoppingListData m_shoppingList;

        [Header("Game Status")] 
        [SerializeField] private bool m_isGameOver;
        public bool IsGameOver => m_isGameOver;
        [SerializeField] private bool m_isGamePaused;
        public bool IsGamePaused => m_isGamePaused;
        private void Start()
        {
            
        }

        public void Initialize_ListCount()
        {
            m_shoppingListCount = m_shoppingList.m_shoppingListCount;
        }
        private void OnEnable()
        {
            m_achive_grama.action += Update_Grama;
            m_achive_female.action += Update_Female;
            m_achive_shopping.action += Update_Shopping;
            m_gameEnd.action += ShowEndUI;
        }

        private void OnDisable()
        {
            m_achive_grama.action -= Update_Grama;
            m_achive_female.action -= Update_Female;
            m_achive_shopping.action -= Update_Shopping;
            m_gameEnd.action -= ShowEndUI;
        }
        private void Update_Grama(int value)
        {
            m_grama_achived = true;
            m_grama_cnt = value;
        }
        private void Update_Female()
        {
            m_female_achived = true;
        }
        public void Update_Shopping(int value)
        {
            m_shopping_achived = value;
        }

        private void ShowEndUI()
        {
            m_isGameOver = true;
            ShowShoppingEndUI();
            ShowFemaleEndUI();
            ShowGramaEndUI();
            m_gameEndPanel.SetActive(true);
        }
        private void ShowShoppingEndUI()
        {
            m_shoppingUI.SetActive(true);
            int result = m_shoppingListCount - m_shopping_achived;
            if (result == 0)
            {
                m_shoppingEndText.text = m_shoppingEndTexts[0];
                return;
            }
            if(result < m_shoppingEndTexts.Length / 2)
            {
                m_shoppingEndText.text = m_shoppingEndTexts[1];
                return;
            }
            m_shoppingEndText.text = m_shoppingEndTexts[2];
        }

        private void ShowFemaleEndUI()
        {
            if (!m_female_achived) return;
            
            m_femaleUI.SetActive(true);
            m_femaleEndText.text = m_femaleEndTexts;
        }

        private void ShowGramaEndUI()
        {
            if (!m_grama_achived) return;
            m_gramaUI.SetActive(true);
            switch (m_grama_cnt)
            {
                case 1:
                    m_gramaEndText.text = m_gramaEndTexts[0];
                    break;
                case 2:
                    m_gramaEndText.text = m_gramaEndTexts[1];
                    break;
                default:
                    m_gramaEndText.text = m_gramaEndTexts[2];
                    break;
            }
        }
    }
}