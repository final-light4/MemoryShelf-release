using System;
using System.Collections.Generic;
using UnityEngine;

namespace NPC.Scripts
{
    public class ConversationListData:ListDataBase<KeyValuePair<string,string>>
    {
       
        [Header("Settings: Item Data Mapping")]
        [SerializeField] private HeadIconSettingsSO m_HeadIconSettings;
        [Header("Running")]
        Dictionary<string,Sprite> m_HeadIconMapping = new Dictionary<string, Sprite>();
        private void Awake()
        {
            // 初始化映射
            foreach (ConversationInfo info in m_HeadIconSettings.m_HeadIconSettings)
            {
                m_HeadIconMapping.Add(info._name, info._sprite);
            }
        }
        
        public override void SetItemInfo(RectTransform rec, int index)
        {
            Dialog item = rec.GetComponent<Dialog>();
            if (index == -1 || index >= m_allItem.Count)
            {
                // 设置默认值
                item.SetDialogInfo("None", null, null,0);
                return;
            }
        
            // 设置对话信息
            KeyValuePair<string,string> conversation = m_allItem[index];
            Sprite sprite = null;
            if (m_HeadIconMapping.ContainsKey(conversation.Key))
            {
                sprite = m_HeadIconMapping[conversation.Key];
            }

            item.SetDialogInfo(conversation.Key, sprite, conversation.Value, index + 1);
        }

        public void AddConversation(string name, string content)
        {
            m_allItem.Add(new KeyValuePair<string, string>(name, content));
        }
    }
}