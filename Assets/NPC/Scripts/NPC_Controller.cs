using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace NPC.Scripts
{
    public class NPC_Controller:MonoBehaviour
    {
        [Header("All Base")]
        protected BaseState m_curState;
        
        [Header("Settings: State Type")]
        [SerializeField] protected StateType m_stateType = StateType.Idle;
        [Header("Settings: State")]
        public AimContext m_routeInfo;
        public TaskAvaliableContext m_TaskAvaliableContext;
        public WanderContext m_wanderInfo;
        public IdleContext m_idleInfo;



        [Header("Debug")] 
        [SerializeField, ReadOnly(true)] private string m_name;
        public Animator m_animator;
        
        protected Dictionary<StateType,BaseState> m_states = new Dictionary<StateType, BaseState>();
        protected virtual void ProcessStateType(StateType newState)
        {
            if (!m_states.ContainsKey(newState))
            {
                switch (newState)
                {
                    case StateType.Idle:

                        m_states.Add(StateType.Idle, new IdleBaseState(this, m_idleInfo));

                        break;
                    case StateType.Shopping:
                        m_states.Add(StateType.Shopping, new ShoppingBaseState(this, m_wanderInfo));
                       
                        break;
                    case StateType.GoToAimedArea:
                        m_states.Add(StateType.GoToAimedArea, new GoToAimedAreaBaseState(this, m_routeInfo));
                       
                        break;
                    case StateType.TaskAvaliable:
                        m_states.Add(StateType.TaskAvaliable, new TaskAvaliable(this, m_TaskAvaliableContext));
                       
                        break;
                    case StateType.OnTask:
                        Debug.LogError("This is base state,choose NPC_xxx instead");
                        break;
                    default:
                        Debug.LogError("Invalid State Type");
                        break;
                }
            }
            m_curState = m_states[newState];
            m_stateType = newState;
        }
        private void Awake()
        {
            m_animator = GetComponentInChildren<Animator>();
        }
        private void Start()
        {
            ProcessStateType(m_stateType);
            m_curState.Enter();
        }
        private void Update()
        {
            m_curState.Execute();
        }
        public void ChangeState(StateType newStateType)
        {
            m_curState.Exit();
            ProcessStateType(newStateType);
            m_curState.Enter();
        }
        public virtual void SwitchToNextTask()
        {
           //an interface for switch to next task
           //it should be implemented in derived class
        }
        private void OnTriggerEnter(Collider other)
        {
            m_curState.OnTriggerEnter(other);
        }
        private void OnTriggerExit(Collider other)
        {
            m_curState.OnTriggerExit(other);
        }
    }
}