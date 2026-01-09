using System;
using System.Collections;
using System.Collections.Generic;
using Global.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace NPC.Scripts
{
    //state type
    public enum StateType
    {
        Idle,
        Shopping,
        GoToAimedArea,
        TaskAvaliable,
        OnTask
    }

    public enum NPCType
    {
        ChooseOne,
        BuyAll
    }
    // Base State
    public abstract class BaseState{
        public NPC_Controller m_NPC_Controller;
        public string m_NPC_Anim = "Base";
        public abstract void Enter();   // 进入状态

        public abstract void Execute();
        public BaseState(NPC_Controller npcController, string anim)
        {
            m_NPC_Controller = npcController;
            m_NPC_Anim = anim;
        }
        public virtual void OnTriggerEnter(Collider other)
        {
            // Debug.Log("Entering BaseState");
        }
        public virtual void OnTriggerExit(Collider other)
        {
            // Debug.Log("Exiting BaseState");
        }
        public abstract void Exit();    // 退出状态
    }

    [Serializable]
    public class IdleContext
    {
        public float _idleTime = 3f;
        public bool _isChangeState = true;
        public string _idleAnim = "Idle";
    }
    // State: Idle
    /// <summary>
    /// watching props
    /// </summary>
    public class IdleBaseState : BaseState {
        IdleContext m_idleContext;
        private float m_timer;
        public IdleBaseState(NPC_Controller npcController, IdleContext context) : base(npcController, context._idleAnim)
        {
            m_idleContext = context;
        }
        public override void Enter() {
            // Debug.Log("Entering IdleState");
            m_NPC_Controller.m_animator.Play(m_NPC_Anim);
            m_timer = 0f;
        }
        public override void Execute() {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
            // Debug.Log("Execute IdleState:"+m_timer);
            if(!m_idleContext._isChangeState)
            {
                return;
            }
            m_timer += Time.deltaTime;
            if(m_timer >= m_idleContext._idleTime)
            {
                m_NPC_Controller.ChangeState(StateType.Shopping);
            }
        }
        public override void Exit()
        {
            m_timer = 0f;
        }
    }
    
    [Serializable]
    public class WanderContext
    {
        public NavMeshAgent _agent;   // NPC的寻路组件
        public Transform _transform;  // NPC的Transform
        public Transform[] _goal;       // 目标点（对应原代码的goal）
        public string _wanderAnim = "Wander";
    }
    // State: Shopping
    /// <summary>
    /// wandering in the shop
    /// </summary>
    public class ShoppingBaseState : BaseState {
        WanderContext m_routeInfo;
        int m_curGoalIndex = 0;
        Vector3 m_curAimPos;
        public ShoppingBaseState(NPC_Controller npcController, WanderContext context) : base(npcController, context._wanderAnim)
        {
            m_routeInfo = context;
            m_curGoalIndex = 0;
        }
        public override void Enter() {
            // Debug.Log("Entering ShoppingState");
            m_NPC_Controller.m_animator.Play(m_NPC_Anim);
            if(m_routeInfo._goal.Length > 0)
            {
                m_routeInfo._agent.destination = m_routeInfo._goal[m_curGoalIndex].position;
                m_curAimPos = m_routeInfo._goal[m_curGoalIndex].position;
            }
        }
        public override void Execute() {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
            // Debug.Log("Execute ShoppingState");
            if(m_routeInfo._goal.Length <= 0)
            {
                return;
            }
            Vector2 curPlanetPos = new Vector2(m_routeInfo._transform.position.x, m_routeInfo._transform.position.z);
            Vector2 aimPlanePos = new Vector2(m_routeInfo._goal[m_curGoalIndex].position.x, m_routeInfo._goal[m_curGoalIndex].position.z);
            float distance = Vector2.Distance(curPlanetPos, aimPlanePos);
            // Debug.Log("Distance to goal: " + distance+" "+m_curGoalIndex);
            if (distance < 0.5f)
            {
                m_NPC_Controller.ChangeState(StateType.Idle);
            }
        }
        public override void Exit() {
            // Debug.Log("Exiting ShoppingState");
            m_curGoalIndex++;
            if(m_curGoalIndex >= m_routeInfo._goal.Length)
            {
                m_curGoalIndex = 0;
            }
        }
    }
    
    [Serializable]
    public class AimContext
    {
        public NavMeshAgent _agent;   // NPC的寻路组件
        public Transform _transform;  // NPC的Transform
        public Transform _goal;       // 目标点（对应原代码的goal）
        public bool _isTask = true;
        public string _gotoAimAnim = "Aim";
    }
    // State: Go to Aimed Area
    /// <summary>
    /// go to the aimed area,find best direction
    /// </summary>
    public class GoToAimedAreaBaseState : BaseState {
        AimContext m_routeInfo;
        public GoToAimedAreaBaseState(NPC_Controller npcController, AimContext context) : base(npcController, context._gotoAimAnim)
        {
            m_routeInfo = context;
        }
        public override void Enter() {
            // Debug.Log("Entering GoToAimedAreaState");
            m_NPC_Controller.m_animator.Play(m_NPC_Anim);
            m_routeInfo._agent.destination = m_routeInfo._goal.position;    
        }
        public override void Execute() {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
            // Debug.Log("Execute GoToAimedAreaState");
            Vector2 curPlanetPos = new Vector2(m_routeInfo._transform.position.x, m_routeInfo._transform.position.z);
            Vector2 aimPlanePos = new Vector2(m_routeInfo._goal.position.x, m_routeInfo._goal.position.z);
            float distance = Vector2.Distance(curPlanetPos, aimPlanePos);
            // Debug.Log("Distance to goal: " + distance);
            if (distance < 0.5f)
            {
                if(m_routeInfo._isTask)
                {
                    m_NPC_Controller.ChangeState(StateType.TaskAvaliable);
                }
                else
                {
                    m_NPC_Controller.ChangeState(StateType.Idle);
                }
            }
        }
        public override void Exit() {
            // Debug.Log("Exiting GoToAimedAreaState");
            m_routeInfo._agent.destination = m_routeInfo._transform.position;
        }
    }

    [Serializable]
    public class TaskAvaliableContext
    {
        //text
        public List<string> _preDialog;

        public float _textShowTime = 2.0f;
        public float _speakingSpeed = 1.0f;
        //component
        public TextMeshProUGUI _text;
        public GameObject _bubleUI;
        public Collider _collider;
        
        public DialogueDataSO _taskDialogue;
        
        //task accept
        public Button _acceptButton;
        
        public VoidSO _OnConversationEND;
        public VoidSO _OnTaskAccept;
        public string _taskAvaliableAnim = "Accept";
    }
    //State : publish task
    /// <summary>
    /// player get task
    /// </summary>
    public class TaskAvaliable : BaseState {
        TaskAvaliableContext _mTaskAvaliableContext;
        private float m_timer;
        bool isAccept = false;  
        // 当前播放的文本索引
        private int m_currentTextIndex;
        public TaskAvaliable(NPC_Controller npcController, TaskAvaliableContext taskAvaliableContext) : base(npcController, taskAvaliableContext._taskAvaliableAnim)
        {
            _mTaskAvaliableContext = taskAvaliableContext;
            m_timer = 0f;
            m_currentTextIndex = 0;
        }
        public override void Enter() {
            // Debug.Log("Entering TaskState");
            m_NPC_Controller.m_animator.Play(m_NPC_Anim);
            //show ui
            _mTaskAvaliableContext._bubleUI.SetActive(true);
            _mTaskAvaliableContext._acceptButton.gameObject.SetActive(false);
            //refresh text
            if(_mTaskAvaliableContext._preDialog.Count != 0)
                _mTaskAvaliableContext._text.text = _mTaskAvaliableContext._preDialog[0];
            //event bind
            _mTaskAvaliableContext._acceptButton.onClick.AddListener(RaiseTaskAcceptEvent);
        }
        public override void Execute() {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
            // Debug.Log("Execute TaskState");
            m_timer += Time.deltaTime;
            // Debug.Log("isAccept: " + isAccept + " m_timer: " + m_timer);
            // 检查是否点击了接受按钮
            if (isAccept)
            {
                //切换状态
                m_NPC_Controller.ChangeState(StateType.OnTask);
            }

            //TODO: 逐字显示文本
            if(_mTaskAvaliableContext._preDialog.Count == 0)
                return;
            if (m_timer >= _mTaskAvaliableContext._textShowTime)
            {
                // 切换到下一个文本索引
                m_currentTextIndex++;
                // 索引越界时重置为0，实现循环
                if (m_currentTextIndex >= _mTaskAvaliableContext._preDialog.Count)
                {
                    m_currentTextIndex = 0;
                }
                
                //refresh text
                _mTaskAvaliableContext._text.text = _mTaskAvaliableContext._preDialog[m_currentTextIndex];
                m_timer = 0f;
            }
           
        }
        public override void OnTriggerEnter(Collider other) {
            // Debug.Log("Entering Trigger TaskState");
            
            ModernSupermarket.Scripts.player.PlayerInteraction playerInteraction = 
                other.GetComponent<ModernSupermarket.Scripts.player.PlayerInteraction>();
            if (other.CompareTag("Player"))
            {
                _mTaskAvaliableContext._OnConversationEND.action += OnConversationEnd;
                _mTaskAvaliableContext._OnTaskAccept.action += OnTaskAccept;
                // Debug.Log("Player:Entering Trigger TaskState");
               
                if (playerInteraction.Player == ModernSupermarket.Scripts.player.PlayerStatus.Idle)
                {
                    ConversationManager.Instance.PlayDialogue(_mTaskAvaliableContext._taskDialogue);
                }
                else
                {
                    playerInteraction.ShowWarning();
                }
            }
        }

        public override void OnTriggerExit(Collider other)
        {
            // Debug.Log("Exiting TaskState");
           
            if (other.CompareTag("Player"))
            { 
                _mTaskAvaliableContext._OnConversationEND.action -= OnConversationEnd;
                _mTaskAvaliableContext._OnTaskAccept.action -= OnTaskAccept;
                // Debug.Log("Player:Exiting Trigger TaskState");
                ConversationManager.Instance.ForceEndDialogue();
            }
            _mTaskAvaliableContext._acceptButton.gameObject.SetActive(false);
        }
        #region Event 
        //raise event
        private void RaiseTaskAcceptEvent()
        {
            _mTaskAvaliableContext._acceptButton.gameObject.SetActive(false);
           _mTaskAvaliableContext._OnTaskAccept.RaiseEvent();
        }
        //accept task
        private void OnTaskAccept()
        {
            // Debug.Log("OnTaskAccept");
            if(GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused)
                return;
            isAccept = true;
            
        }
        private void OnConversationEnd()
        {
            _mTaskAvaliableContext._acceptButton.gameObject.SetActive(true);
        }
        #endregion
        public override void Exit() {
            // Debug.Log("Exiting TaskState");
            //hide ui
            _mTaskAvaliableContext._bubleUI.SetActive(false);
            _mTaskAvaliableContext._acceptButton.gameObject.SetActive(false);
            //event unbind
            _mTaskAvaliableContext._acceptButton.onClick.RemoveListener(RaiseTaskAcceptEvent);
            _mTaskAvaliableContext._OnConversationEND.action -= OnConversationEnd;
            _mTaskAvaliableContext._OnTaskAccept.action -= OnTaskAccept;
        }
    }
    
}