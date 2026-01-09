using System.Collections;
using System.Collections.Generic;
using Global.Scripts;
using UnityEngine;

namespace NPC.Scripts
{
    /// <summary>
    /// 对话音频管理器：负责对话语音的播放、自动/手动切换、打断逻辑
    /// </summary>
    public class ConversationAudioManager : MonoBehaviour
    {
        public static ConversationAudioManager Instance { get; private set; }

        private void Awake()
        {
            // 单例模式（与原管理器保持一致）
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
        [SerializeField] private KeyCode m_nextLineKey = KeyCode.Space; // 下一句语音触发键

        [Header("Settings: Audio Source")]
        [SerializeField] private AudioSource m_audioSource; // 语音播放音频源（建议禁用Loop）
        [SerializeField] private bool m_autoPlayAudio = true; // 是否默认自动播放语音

        [Header("Settings: Audio Delay")]
        [SerializeField] private float m_autoPlayDelay = 0.2f; // 自动播放下一句的延迟（语音结束后）
        #endregion

        #region Private State Variables
        [SerializeField] private List<AudioClip> m_currentAudioClips; // 当前对话的语音列表
        [SerializeField] private int m_currentAudioIndex = 0; // 当前播放的语音索引
        [SerializeField] public bool m_isAudioPlaying = false; // 是否正在播放语音
        [SerializeField] private bool m_isAutoPlayMode = false; // 是否开启自动播放模式
        [SerializeField] private Coroutine m_autoPlayCoroutine; // 自动播放协程引用
        #endregion

        /*[Header("Event System")]
        public VoidSO _OnAudioConversationEND; // 语音对话结束事件*/
        private void Start()
        {
            // 初始化音频源
            if (m_audioSource == null)
            {
                m_audioSource = GetComponent<AudioSource>();
                if (m_audioSource == null)
                {
                    m_audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
            m_audioSource.loop = false; // 强制关闭循环（语音播放不循环）
            m_audioSource.playOnAwake = false;

            // 验证核心组件
            ValidateComponents();
        }

        /*private void Update()
        {
            // 空格键切下一句：打断当前语音 + 播放下一句
            if (m_isAudioPlaying && Input.GetKeyDown(m_nextLineKey))
            {
                PlayNextAudioClip();
            }
        }*/

        #region Core Functional Methods

        /// <summary>
        /// 切换当前语音列表（外部调用）
        /// </summary>
        /// <param name="audioClips">新的语音列表</param>
        /// <param name="autoPlayMode">是否开启自动播放模式</param>
        public void ChangeAudioClips(List<AudioClip> audioClips, bool autoPlayMode = true)
        {
            if (audioClips == null || audioClips.Count == 0)
            {
                Debug.LogWarning("ConversationAudioManager: Passed audio clip list is null or empty!");
                return;
            }

            // 重置状态
            m_currentAudioClips = audioClips;
            m_currentAudioIndex = 0;
            m_isAutoPlayMode = autoPlayMode;

            // 停止现有播放
            StopAllCoroutines();
            StopCurrentAudio();
        }

        /// <summary>
        /// 开始播放语音列表（外部调用）
        /// </summary>
        public void StartAudioPlayback()
        {
            if (m_currentAudioClips == null || m_currentAudioClips.Count == 0)
            {
                Debug.LogWarning("ConversationAudioManager: No audio clips to play!");
                return;
            }

            m_isAudioPlaying = true;
            PlayCurrentAudioClip();
        }

        /// <summary>
        /// 播放当前索引的语音
        /// </summary>
        private void PlayCurrentAudioClip()
        {
            // ========== 新增：1. 校验音频列表是否为 null（核心修复空引用） ==========
            if (m_currentAudioClips == null)
            {
                Debug.LogError("ConversationAudioManager: m_currentAudioClips is null!");
                EndAudioConversation();
                return;
            }

            // 索引越界：结束语音播放
            if (m_currentAudioIndex >= m_currentAudioClips.Count)
            {
                EndAudioConversation();
                return;
            }

            AudioClip currentClip = m_currentAudioClips[m_currentAudioIndex];
            if (currentClip == null)
            {
                Debug.LogWarning($"ConversationAudioManager: Audio clip at index {m_currentAudioIndex} is null!");
                m_currentAudioIndex++;
                PlayCurrentAudioClip();
                return;
            }

            // ========== 新增：2. 校验 AudioSource 是否为 null（核心修复空引用） ==========
            if (m_audioSource == null)
            {
                Debug.LogError("ConversationAudioManager: m_audioSource is null!");
                EndAudioConversation();
                return;
            }

            // 播放当前语音
            StopCurrentAudio(); // 确保停止上一句
            m_audioSource.clip = currentClip;
            m_audioSource.Play();

            // 自动播放模式：启动"等待语音结束"协程
            if (m_isAutoPlayMode)
            {
                // ========== 新增：3. 协程空引用保护 ==========
                if (m_autoPlayCoroutine != null)
                {
                    StopCoroutine(m_autoPlayCoroutine);
                }
                m_autoPlayCoroutine = StartCoroutine(WaitForAudioCompleteThenPlayNext());
            }
        }

        /// <summary>
        /// 播放下一句语音（手动触发时打断当前播放）
        /// </summary>
        public void PlayNextAudioClip()
        {
            Debug.LogError("ConversationAudioManager: PlayNextAudioClip");
            // 停止自动播放协程
            if (m_autoPlayCoroutine != null)
            {
                StopCoroutine(m_autoPlayCoroutine);
                m_autoPlayCoroutine = null;
            }

            // 打断当前语音
            StopCurrentAudio();

            // 切换到下一句
            m_currentAudioIndex++;
            PlayCurrentAudioClip();
        }

        /// <summary>
        /// 停止当前播放的语音
        /// </summary>
        private void StopCurrentAudio()
        {
            if (m_audioSource.isPlaying)
            {
                m_audioSource.Stop();
            }
        }

        /// <summary>
        /// 结束语音对话
        /// </summary>
        private void EndAudioConversation()
        {
            m_isAudioPlaying = false;
            m_currentAudioIndex = 0;

            // 停止所有协程和音频
            StopAllCoroutines();
            StopCurrentAudio();

            /*// 修复：先判空 _OnAudioConversationEND，再调用 RaiseEvent
            if (_OnAudioConversationEND != null)
            {
                // 额外检查 VoidSO 内部委托是否为空（可选，看 VoidSO 实现）
                _OnAudioConversationEND.RaiseEvent(); 
            }
            else
            {
                Debug.LogWarning("ConversationAudioManager: _OnAudioConversationEND is null! Assign in Inspector.");
            }*/

            Debug.Log("ConversationAudioManager: Audio conversation completed!");
        }

        /// <summary>
        /// 强制结束语音播放（外部调用）
        /// </summary>
        public void ForceEndAudioPlayback()
        {
            StopAllCoroutines();
            StopCurrentAudio();
            m_isAudioPlaying = false;
            m_currentAudioIndex = 0;
        }

        #endregion

        #region Coroutines

        /// <summary>
        /// 等待当前语音播放完毕后，自动播放下一句
        /// </summary>
        private IEnumerator WaitForAudioCompleteThenPlayNext()
        {
            // 等待语音播放完毕
            while (m_audioSource.isPlaying && m_isAudioPlaying)
            {
                yield return null;
            }

            // 延迟后播放下一句
            yield return new WaitForSeconds(m_autoPlayDelay);

            // 切换到下一句
            m_currentAudioIndex++;
            PlayCurrentAudioClip();
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 验证核心组件是否赋值
        /// </summary>
        private void ValidateComponents()
        {
            if (m_audioSource == null)
            {
                Debug.LogError("ConversationAudioManager: AudioSource (m_audioSource) is unassigned!");
            }
        }

        #endregion

        #region External API Methods

        /// <summary>
        /// 快速播放指定语音列表（外部快捷调用）
        /// </summary>
        /// <param name="audioClips">要播放的语音列表</param>
        /// <param name="autoPlayMode">是否自动播放</param>
        public void PlayAudioConversation(List<AudioClip> audioClips, bool autoPlayMode = true)
        {
            ChangeAudioClips(audioClips, autoPlayMode);
            StartAudioPlayback();
        }

        #endregion
    }
}