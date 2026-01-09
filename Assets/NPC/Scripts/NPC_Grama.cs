using UnityEngine;

namespace NPC.Scripts
{
    public class NPC_Grama:NPC_Controller
    {
        [Header("Settings: State")]
        public OnTaskGramaContext c_OnTaskContext;
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
                        m_states.Add(StateType.TaskAvaliable, new TaskAvaliable(this, m_TaskAvaliableContext));
                       
                        break;
                    case StateType.OnTask:
                        m_states.Add(StateType.OnTask, new NPC_GramaTask(this, c_OnTaskContext));
                        break;
                    default:
                        Debug.LogError("Invalid State Type");
                        break;
                }
            }
            m_curState = m_states[newState];
            m_stateType = newState;
        }
    }
}