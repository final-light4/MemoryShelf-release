using System.Collections;
using System.Collections.Generic;
using Global.Scripts;
using MarkingList.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

namespace NPC.Scripts
{
    public class ConversationManager : MonoBehaviour
    {
        public static ConversationManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(this);
        }

        #region Configuration Parameters
        [Header("Settings: Key Code")]
        [SerializeField] private KeyCode m_ShowPanelKey = KeyCode.Tab; // Main UI toggle key
        [SerializeField] private KeyCode m_nextLineKey = KeyCode.Space; // Next dialogue line key

        [Header("Settings: Log Info")]
        [SerializeField] private GameObject m_uiInfo; // Basic UI toggle object
        [SerializeField] private ConversationListData m_conversationListData;
        [Header("Settings: Bubble Info")]
        private DialogueDataSO m_dialogueData; // Current active dialogue data
        [SerializeField] private GameObject m_bubble; // Root object of dialogue bubble UI
        [SerializeField] private TextMeshProUGUI m_contentText; // Dialogue content text (TMP)
        [SerializeField] private TextMeshProUGUI m_speaker; // Speaker name text (TMP)
        [SerializeField] private Image m_speakerIcon; // Speaker icon (UGUI Image)

        [Header("Typewriter Settings")]
        [SerializeField] private float m_typewriterSpeed = 30f; // Typewriter effect speed (chars/second)
        #endregion

        #region Private State Variables
        [SerializeField] private int m_currentLineIndex = 0; // Index of current dialogue line
        [SerializeField] private Coroutine m_typewriterCoroutine; // Reference to typewriter coroutine
        [SerializeField] private bool m_isDialoguePlaying = false; // Flag: is dialogue actively playing
        [SerializeField] private bool m_isBubbleActive = false; // Flag: is dialogue bubble visible
        #endregion
        [Header("Event System")]
        public VoidSO _OnConversationEND;
        private void Start()
        {
            // Initialize UI states on startup
            m_uiInfo?.SetActive(false);
            m_bubble?.SetActive(false);
            
            // Validate critical component assignments
            ValidateComponents();
        }
        private void Update()
        {
            if (Input.GetKeyDown(m_ShowPanelKey))
            {
                m_uiInfo.SetActive(!m_uiInfo.activeSelf);
            }
            // Navigate to next dialogue line with Space key (while dialogue is active)
            if (m_isDialoguePlaying && Input.GetKeyDown(m_nextLineKey) && !m_dialogueData.autoPlayNext)
            {
                ShowNextDialogueLine();
            }

            /*// Close dialogue bubble with Tab key (only in non-auto-play mode)
            if (m_isBubbleActive && Input.GetKeyDown(m_ShowPanelKey) && !m_dialogueData.autoPlayNext)
            {
                StopAllCoroutines();
                HideDialogueBubble();
            }*/
        }

        #region Core Functional Methods

        /// <summary>
        /// Change active dialogue data (external call)
        /// </summary>
        /// <param name="dialogueData">New dialogue data to load</param>
        public void ChangeDialogueData(DialogueDataSO dialogueData)
        {
            if (dialogueData == null)
            {
                Debug.LogWarning("ConversationManager: Passed dialogue data is null!");
            }

            m_dialogueData = dialogueData;
            m_currentLineIndex = 0; // Reset dialogue line index
        }

        /// <summary>
        /// Show dialogue bubble and start playing conversation
        /// </summary>
        public void ShowDialogueBubble()
        {
            if (m_dialogueData == null || m_dialogueData.dialogueLines.Count == 0)
            {
                Debug.LogWarning("ConversationManager: Dialogue data is null or empty!");
                return;
            }

            // Initialize dialogue playback state
            m_isDialoguePlaying = true;
            m_isBubbleActive = true;
            m_currentLineIndex = 0;

            // Show dialogue bubble UI
            m_bubble.SetActive(true);

            // Play first dialogue line
            PlayCurrentDialogueLine();
        }

        /// <summary>
        /// Hide dialogue bubble and end conversation
        /// </summary>
        public void HideDialogueBubble()
        {
            m_isDialoguePlaying = false;
            m_isBubbleActive = false;
            m_currentLineIndex = 0;

            // Stop all active coroutines
            if (m_typewriterCoroutine != null)
            {
                StopCoroutine(m_typewriterCoroutine);
                m_typewriterCoroutine = null;
            }

            // Hide bubble UI
            m_bubble.SetActive(false);

            // Clear text fields
            m_contentText.text = string.Empty;
            m_speaker.text = string.Empty;

            // Clear speaker icon
            m_speakerIcon.sprite = null;
            m_speakerIcon.enabled = false;
        }

        /// <summary>
        /// Play dialogue line at current index
        /// </summary>
        private void PlayCurrentDialogueLine()
        {
            // Check if index is out of bounds (end of conversation)
            if (m_currentLineIndex >= m_dialogueData.dialogueLines.Count)
            {
                HideDialogueBubble();
                Debug.Log("ConversationManager: Conversation completed!");
                //TODO:tell npc, conversation completed
                ConversationAudioManager.Instance.ForceEndAudioPlayback();
                _OnConversationEND.RaiseEvent();
                return;
            }

            // Get current dialogue line data
            var currentLine = m_dialogueData.dialogueLines[m_currentLineIndex];
            // Update speaker UI
            m_speaker.text = currentLine.speakerName;

            // Update speaker icon
            if (m_speakerIcon != null)
            {
                m_speakerIcon.sprite = currentLine.speakerIcon;
                m_speakerIcon.enabled = currentLine.speakerIcon != null;
            }

            // Stop any active typewriter effect
            if (m_typewriterCoroutine != null)
            {
                StopCoroutine(m_typewriterCoroutine);
            }

            // Start typewriter effect for current line
            m_typewriterCoroutine = StartCoroutine(TypewriterEffect(currentLine.dialogueText));

            // Auto-play logic
            if (m_dialogueData.autoPlayNext)
            {
                float delay = 1.0f;
                if (m_dialogueData.dialogueLines[m_currentLineIndex].speakerClip != null)
                {
                    delay = m_dialogueData.dialogueLines[m_currentLineIndex].speakerClip.length;
                }
                Invoke(nameof(ShowNextDialogueLine), delay);
            }
            // ========== 新增：确保音频和文本行同步 ==========
            if (ConversationAudioManager.Instance != null && m_isDialoguePlaying)
            {
                // 手动模式下，确保音频停在当前行；自动模式下交给音频管理器处理
                if (!m_dialogueData.autoPlayNext)
                {
                    ConversationAudioManager.Instance.PlayNextAudioClip();
                }
            }
            // Add conversation to log
            m_conversationListData.AddConversation(currentLine.speakerName, currentLine.dialogueText);
        }

        /// <summary>
        /// Navigate to next dialogue line
        /// </summary>
        public void ShowNextDialogueLine()
        {
            if (!m_isDialoguePlaying) return;
            
            // If typewriter is active, show full text immediately
            if (m_typewriterCoroutine != null)
            {
                StopCoroutine(m_typewriterCoroutine);
                m_typewriterCoroutine = null;
                m_contentText.text = m_dialogueData.dialogueLines[m_currentLineIndex].dialogueText;

                // Cancel pending auto-play invoke
                CancelInvoke(nameof(ShowNextDialogueLine));
            }
            else
            {
                // Move to next line
                m_currentLineIndex++;
                PlayCurrentDialogueLine();
            }
            /*// ========== 新增：切换下一段音频 ==========
            ConversationAudioManager.Instance?.PlayNextAudioClip();*/
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Coroutine for typewriter text reveal effect
        /// </summary>
        /// <param name="targetText">Full text to reveal</param>
        private IEnumerator TypewriterEffect(string targetText)
        {
            m_contentText.text = string.Empty;
            float charInterval = 1f / m_typewriterSpeed; // Time between each character

            for (int i = 0; i < targetText.Length; i++)
            {
                // Add one character at a time
                m_contentText.text += targetText[i];
                yield return new WaitForSeconds(charInterval);

                // Exit if dialogue is interrupted
                if (!m_isDialoguePlaying)
                {
                    yield break;
                }
            }

            // Release coroutine reference when complete
            m_typewriterCoroutine = null;
        }

        /// <summary>
        /// Validate critical UI component assignments
        /// </summary>
        private void ValidateComponents()
        {
            if (m_bubble == null) Debug.LogError("ConversationManager: Dialogue bubble (m_bubble) is unassigned!");
            if (m_contentText == null)
                Debug.LogError("ConversationManager: Content text (m_contentText) is unassigned!");
            if (m_speaker == null) Debug.LogError("ConversationManager: Speaker text (m_speaker) is unassigned!");
            if (m_speakerIcon == null)
                Debug.LogWarning("ConversationManager: Speaker icon (m_speakerIcon) is unassigned!");
        }

        #endregion

        #region External API Methods

        /// <summary>
        /// Quick play specified dialogue (external call)
        /// </summary>
        /// <param name="dialogueData">Dialogue data to play</param>
        public void PlayDialogue(DialogueDataSO dialogueData)
        {
            if(dialogueData == null)
            {
                Debug.LogWarning("ConversationManager: Passed dialogue data is null!");
                return;
            }
            ChangeDialogueData(dialogueData);
            ShowDialogueBubble();
            // ========== 新增：启动音频播放 ==========
            if (ConversationAudioManager.Instance != null)
            {
                // 提取对话数据中的所有语音片段
                List<AudioClip> audioClips = new List<AudioClip>();
                foreach (var line in dialogueData.dialogueLines)
                {
                    // 兼容空语音片段（避免列表中断）
                    audioClips.Add(line.speakerClip);
                }
                
                // 调用音频管理器播放，自动播放模式与对话保持一致
                ConversationAudioManager.Instance.PlayAudioConversation(audioClips, dialogueData.autoPlayNext);
            }
            else
            {
                Debug.LogWarning("ConversationManager: ConversationAudioManager Instance is null!");
            }
        }

        /// <summary>
        /// Force end current conversation
        /// </summary>
        public void ForceEndDialogue()
        {
            CancelInvoke();
            if (m_typewriterCoroutine != null)
            {
                StopCoroutine(m_typewriterCoroutine);
            }

            HideDialogueBubble();
            // ========== 新增：强制结束音频播放 ==========
            ConversationAudioManager.Instance?.ForceEndAudioPlayback();
        }

        #endregion
    }

}
