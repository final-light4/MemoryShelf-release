using System;
using System.Collections.Generic;
using NPC.Scripts;
using UnityEngine;

namespace NPC.Scripts
{
    [Serializable]
    public class ConAudioDialogue
    {
        public string _condition;
        public AudioClip _audio;
        public DialogueDataSO _dialogueData;
    }

    [CreateAssetMenu(fileName = "ConAudioDialogue", menuName = "Dialogue System/Con Audio Dialogue")]
    public class ConAudioDialogueSO : ScriptableObject
    {
        [SerializeField]
        public List<ConAudioDialogue> m_ConAudioDialogue = new List<ConAudioDialogue>();
    }
}
