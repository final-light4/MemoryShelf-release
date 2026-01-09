using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NPC.Scripts
{
    public class Dialog : MonoBehaviour
    {
        [Header("Dialog Info")]

        [Header("Debug: Reference")]
        private TextMeshProUGUI m_nameText;

        private TextMeshProUGUI m_contentText;
        private Image m_headIcon;
        private TextMeshProUGUI m_indexText;

        void Awake()
        {
            // 初始化UI引用
            m_nameText = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            m_contentText = transform.Find("Content").GetComponent<TextMeshProUGUI>();
            m_headIcon = transform.Find("HeadIcon").GetComponent<Image>();
            m_indexText = transform.Find("Index").GetComponent<TextMeshProUGUI>();
        }

        public void SetDialogInfo(string name, Sprite icon, string content, int index)
        {
            m_nameText.text = name;
            m_contentText.text = content;
            m_headIcon.sprite = icon;
            if (index < 10)
            {
                m_indexText.text = "0" + index.ToString();
            }
            else
            {
                m_indexText.text = index.ToString();
            }
        }
    }
}