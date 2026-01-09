using System;
using UnityEngine;
using UnityEngine.Audio;

namespace Global.Scripts
{
    public class AudioManager: MonoBehaviour
    {
        public static AudioManager instance;
        [Header("Event")]
        [SerializeField] AudioClipSO FXEvent;
        [SerializeField] AudioClipSO BGMEvent;
        [SerializeField] private VoidSO gameEndEvent;
        [Header("Component")]
        [SerializeField] AudioSource BGMSource;
        [SerializeField] AudioSource FXSource;
        [SerializeField] AudioSource RadioSource;
        [SerializeField] AudioSource NoiseSource;
        [Header("Music Volume Settings")]
        [SerializeField] [Range(0,1)] private float m_BGMVolume = 0.5f;
        [SerializeField] [Range(0,1)] private float m_FXMVolume = 0.5f;
        [SerializeField] [Range(0,1)] private float m_RadioVolume = 0.5f;
        [SerializeField] [Range(0,1)] private float m_NoiseVolume = 0.5f;
        [Header("BGM Settings")]
        [SerializeField] private bool m_isLoopBGM = false;
        [SerializeField] private AudioListSO m_audioClips;
        private int m_currentBGMIndex = 0;
        public bool m_isPlaying => BGMSource.isPlaying;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ApplyAudioSourceSetting();
        }
        private void Update()
        {
            // 若音频源未播放、且有音频列表数据 → 切换下一首
            if (!BGMSource.isPlaying && m_audioClips._clips.Count > 0 && BGMSource.clip != null)
            {
                PlayNextAudio();
            }
        }
        public void PlayNextAudio()
        {
            if (m_audioClips._clips.Count == 0)
            {
                Debug.LogWarning("音频列表为空，无法切换下一首！");
                return;
            }

            // 索引+1（切换下一首）
            m_currentBGMIndex++;
            PlayCurrentAudio();
        }
        private void PlayCurrentAudio()
        {
            if (m_currentBGMIndex >= m_audioClips._clips.Count)
            {
                if (m_isLoopBGM)
                {
                    // 循环列表：回到第一首
                    m_currentBGMIndex = 0;
                }
                else
                {
                    // 不循环：停止播放
                    Debug.Log("音频列表播放完毕");
                    BGMSource.Stop();
                    return;
                }
            }

            // 赋值并播放当前音频
            AudioClip currentClip = m_audioClips._clips[m_currentBGMIndex];
            BGMSource.clip = currentClip;
            BGMSource.Play();



            Debug.Log($"正在播放：{currentClip.name}（索引：{m_currentBGMIndex}）");
        }
        private void ApplyAudioSourceSetting()
        {
            BGMSource.volume = m_BGMVolume;
            FXSource.volume = m_FXMVolume;
            RadioSource.volume = m_RadioVolume;
            NoiseSource.volume = m_NoiseVolume;
            if(BGMSource.clip == null)
            {
                BGMSource.clip = m_audioClips._clips[m_currentBGMIndex];
            }
        }
        private void OnEnable()
        {
            FXEvent.action += OnFXEvent;
            BGMEvent.action += OnBGMEvent;
            gameEndEvent.action += OnGameEndEvent;
        }


        private void OnDisable()
        {
            FXEvent.action -= OnFXEvent;
            BGMEvent.action -= OnBGMEvent;
        }
        private void OnGameEndEvent()
        {
            BGMSource.Stop();
            FXSource.Stop();
            RadioSource.Stop();
            NoiseSource.Stop();
        }
        private void OnBGMEvent(AudioClip clip)
        {
            BGMSource.clip = clip;
            BGMSource.Play();
        }

        private void OnFXEvent(AudioClip clip)
        {
            FXSource.clip = clip;
            FXSource.Play();
        }
    }
}