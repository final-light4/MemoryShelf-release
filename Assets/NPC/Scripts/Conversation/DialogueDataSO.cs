using System;
using System.Collections.Generic;
using UnityEngine;

namespace NPC.Scripts
{
    [CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue System/Dialogue Data")]
    public class DialogueDataSO : ScriptableObject
    {
        public bool autoPlayNext = false;

        public List<DialogueLine> dialogueLines = new List<DialogueLine>();

        // 单条对话的数据结构
        [System.Serializable]
        public class DialogueLine
        {
            public string speakerName = "NPC";
            public string dialogueText = "content";

            public Sprite speakerIcon;
            public AudioClip speakerClip;
        }
    }

    [Serializable]
    public class ConversationInfo
    {
        public string _name;
        public Sprite _sprite;
    }

    [CreateAssetMenu(fileName = "HeadIconSettings", menuName = "Dialogue System/Head Icon Settings")]
    public class HeadIconSettingsSO : ScriptableObject
    {
        public List<ConversationInfo> m_HeadIconSettings = new List<ConversationInfo>();
    }

}