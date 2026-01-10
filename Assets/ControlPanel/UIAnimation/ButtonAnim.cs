using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

// DOTween namespace

namespace ControlPanel.UIAnimation
{
    public class ButtonAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
    {
        [SerializeField] private float m_animTime = 0.2f;  
        
        [Header("Pointer Enter")] 
        [SerializeField] private bool m_playScale = false;
        [SerializeField] private float m_scaleBig = 1.1f;  
        
        [SerializeField] Ease m_scaleType = Ease.OutQuad; 

        [Header("Fade")]
        [SerializeField] bool m_playFade = true;
        [SerializeField] float m_targetAlpha = 1f; 
        [SerializeField] float m_originAlpha = 0.8f;

        [Header("Move distance")]
        [SerializeField] bool m_playMove = true;

        [SerializeField] private Vector3 m_moveDirection = Vector3.left;
        [SerializeField] float m_moveDistance = 25f;
        [SerializeField] Ease m_moveType = Ease.OutQuad;
        private Vector3 m_originScale;
        private Vector3 m_originPosition;
        private RectTransform m_rectTransform;
        private CanvasGroup m_canvasGroup;// 控制透明度的组件

        void Start()
        {
            // 初始化：记录UI的原始缩放
            m_originScale = transform.localScale;
            // 初始化：记录UI的原始位置
            m_originPosition = transform.position;
            // 获取CanvasGroup组件
            m_canvasGroup = GetComponent<CanvasGroup>();
            if (m_canvasGroup == null) m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
            m_canvasGroup.alpha = m_originAlpha;
            // 获取RectTransform组件
            m_rectTransform = GetComponent<RectTransform>();
        }

        #region 鼠标进入UI区域时调用 【核心方法】
        // 鼠标移入：鼠标指针进入当前UI物体时，自动执行这个方法
        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOKill();
            Debug.Log("鼠标移入UI区域，播放动画");
            // 1. 播放【缩放动画】：放大+缓动效果
            if (m_playScale)
            {
                transform.DOScale(Vector3.one * m_scaleBig, m_animTime).SetEase(m_scaleType);
            }
            transform.DOScale(Vector3.one * m_scaleBig, m_animTime).SetEase(m_scaleType);
            // 2. 播放【透明度动画】
            if (m_playFade)
            {
                m_canvasGroup.DOFade(m_targetAlpha, m_animTime);
            }
            //3. 播放【物体移动动画】
            if (m_playMove)
            {
                transform.DOMove(transform.position + m_moveDirection * m_moveDistance, m_animTime).SetEase(m_scaleType);
            }
            // 拓展：可加颜色渐变/位移等任意DOTween动画
            // GetComponent<Image>().DOColor(Color.yellow, animTime);
        }
        #endregion

        #region 鼠标离开UI区域时调用 【核心方法】
        // 鼠标移出：鼠标指针离开当前UI物体时，自动执行这个方法
        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOKill();
            Debug.Log("鼠标移出UI区域，还原动画");
            // 关键：【动画还原】所有动画恢复到初始状态
            // 1. 播放【缩放动画】：恢复到原始大小+缓动效果
            if (m_playScale)
            {
                transform.DOScale(m_originScale, m_animTime).SetEase(m_scaleType);
            }
            // 2. 播放【透明度动画】
            if (m_playFade)
            {
                m_canvasGroup.DOFade(m_originAlpha, m_animTime);
            }
            // 3. 播放【物体移动动画】
            if (m_playMove)
            {
                transform.DOMove(m_originPosition, m_animTime).SetEase(m_moveType);
            }
            // 拓展：颜色还原
            // GetComponent<Image>().DOColor(Color.white, animTime);
        }
        #endregion

        #region 内存泄漏防护
        // 脚本销毁时，清理当前UI的所有DOTween动画，杜绝内存泄漏
        private void OnDestroy()
        {
            transform.DOKill();       // 销毁缩放动画
            m_canvasGroup.DOKill();     // 销毁透明度动画
            // 如果加了其他动画，这里也要Kill：GetComponent<Image>().DOKill();
        }
        #endregion

        public void OnPointerClick(PointerEventData eventData)
        {
            
        }
    }
}