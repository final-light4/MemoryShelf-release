using UnityEngine;

namespace NPC.Scripts
{
    public class NPC_Female:NPC_Controller
    {
        [Header("Settings: State")]
        public OnTask1FemaleContext c_OnTask1Context;
        public OnTask2FemaleContext c_OnTask2Context;
        public TaskAvaliableContext[] c_TaskAvaliableContext;
        [SerializeField] private int m_taskIndex = 0;
        protected override void ProcessStateType(StateType newState)
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
                        m_states.Add(StateType.TaskAvaliable, new TaskAvaliable(this, c_TaskAvaliableContext[m_taskIndex]));
                       
                        break;
                    case StateType.OnTask:
                        m_states.Add(StateType.OnTask, new NPC_FemaleTask1(this, c_OnTask1Context));
                        break;
                    default:
                        Debug.LogError("Invalid State Type");
                        break;
                }
            }
            m_curState = m_states[newState];
            m_stateType = newState;
        }

        public override void SwitchToNextTask()
        {
            //pre-check: if m_taskIndex is out of range, reset it to 0
            if (m_taskIndex >= 2)
            {
                m_taskIndex = 0;
            }
            m_taskIndex++;
            switch (m_taskIndex)
            {
                case 0:
                    m_states[StateType.OnTask] = new NPC_FemaleTask1(this, c_OnTask1Context);
                    break;
                case 1:
                    m_states[StateType.OnTask] = new NPC_FemaleTask2(this, c_OnTask2Context);
                    break;
            }
            m_states[StateType.TaskAvaliable] =  new TaskAvaliable(this, c_TaskAvaliableContext[m_taskIndex]);
        }
    }
}