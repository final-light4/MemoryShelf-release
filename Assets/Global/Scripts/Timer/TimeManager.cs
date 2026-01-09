using TMPro;
using UnityEngine;

namespace Global.Scripts.Timer
{
    public class TimeManager:MonoBehaviour
    {
        static TimeManager _instance;
        public static TimeManager Instance => _instance;
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }
        [Header("CountDown: Component")]
        [SerializeField] private GameObject m_timeCountDownUI;
        [SerializeField] private TextMeshProUGUI m_timeCountDownminutes;
        [SerializeField] private TextMeshProUGUI m_timeCountDownseconds;
        
        private float m_countDownTime;
        private float m_countDownWarningTime = 10;
        private bool m_countDownisStared = false;

        [Header("Game Time: Component")] 
        [SerializeField] private GameObject m_gameTimeUI;
        [SerializeField] private TextMeshProUGUI m_gameTimeMinutes;
        [SerializeField] private TextMeshProUGUI m_gameTimeSeconds;
        [SerializeField] private float m_gameEndTime = 120.0f;
        [SerializeField] private VoidSO m_gameEnd;
        
        private float m_gameTime = 0.0f;
        private bool m_gameTimeStared = true;
        private void Update()
        {
            if (!m_gameTimeStared) return;
            if(m_gameTime > m_gameEndTime)
            {
                m_gameEnd.RaiseEvent();
                return;
            }
            m_gameTime += Time.deltaTime;
            UpdateGameTimeUI();
            
            if (!m_countDownisStared) return;
            if (m_countDownTime <= 0)
            {
                m_countDownTime = 0;
                m_countDownisStared = false;
            }
            m_countDownTime -= Time.deltaTime;
            UpdateCountDownUI();
            
        }

        private void UpdateCountDownUI()
        {
            int minutes = Mathf.FloorToInt(m_countDownTime / 60);
            int seconds = Mathf.FloorToInt(m_countDownTime % 60);
            m_timeCountDownminutes.text = minutes.ToString("00");
            m_timeCountDownseconds.text = seconds.ToString("00");
        }

        private void UpdateGameTimeUI()
        {
            int minutes = Mathf.FloorToInt(m_gameTime / 60);
            int seconds = Mathf.FloorToInt(m_gameTime % 60);
            m_gameTimeMinutes.text = minutes.ToString("00");
            m_gameTimeSeconds.text = seconds.ToString("00");
        }
        /// <summary>
        /// time = seconds
        /// </summary>
        /// <param name="time"></param>
        public void StartCountDown(float time)
        {
            m_timeCountDownUI.SetActive(true);
            m_countDownTime = time;
            m_countDownisStared = true;
        }
        /// <summary>
        /// kill count down
        /// </summary>
        public void StopCountDown()
        {
            m_timeCountDownUI.SetActive(false);
            m_countDownisStared = false;
        }
        /// <summary>
        /// continue going
        /// </summary>
        public void StartGameTime()
        {
            m_gameTimeStared = true;
        }
        /// <summary>
        /// stop going
        /// </summary>
        public void StopGameTime()
        {
            m_gameTimeStared = false;
        }
    }
}