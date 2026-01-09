using System;
using System.Collections.Generic;
using Global.Scripts;
using Global.Scripts.Timer;
using MarkingList.Scripts;
using TMPro;
using UnityEngine;

namespace NPC.Scripts
{
    [Serializable]
    public class OnTaskContext
    {
        public String_IntSO _addToCart;
        public String_IntSO _removeFromCart;
        public MarkingList.Scripts.ShoppingListSO _markingList;
        public AudioClipSO _chooseAction;
        public ConAudioDialogueSO _conversationMapping;
        public string _onTaskAnim = "OnTask";
        public VoidSO  _onTaskEnd;
    }
    public abstract class NPC_OnTask : BaseState
    {
        protected OnTaskContext _mOnTaskContext;
        protected List<ItemCountInfo> m_shoppingList;
        public NPC_OnTask(NPC_Controller npcController, OnTaskContext onTaskContext) : base(npcController, onTaskContext._onTaskAnim)
        {
            //deep copy shopping list
            m_shoppingList = new List<ItemCountInfo>();
            if (onTaskContext._markingList?.m_NeedList != null) // 空值保护
            {
                foreach (var item in onTaskContext._markingList.m_NeedList)
                {
                    m_shoppingList.Add(item.DeepClone()); // 关键：调用深拷贝，而非直接Add
                }
            }
            _mOnTaskContext = onTaskContext;
        }
        public override void Enter() {
            Debug.Log("Entering OnTaskState");
            m_NPC_Controller.m_animator.Play(m_NPC_Anim);
            //event bind
            _mOnTaskContext._addToCart.action += AddToCart;
            _mOnTaskContext._removeFromCart.action += RemoveFromCart;
        }

        public override void Execute()
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
            if (CanChangeState())
            {
                ChangeState();
            }
        }

        protected abstract bool CanChangeState();
        protected abstract void ChangeState();
        public override void Exit()
        {
            Debug.Log("Exiting OnTaskState");
            _mOnTaskContext._onTaskEnd.RaiseEvent();
            //event unbind
            _mOnTaskContext._addToCart.action -= AddToCart;
            _mOnTaskContext._removeFromCart.action -= RemoveFromCart;
        }

        protected abstract void AddToCart(string itemName, int quantity);
        private void RemoveFromCart(string itemName, int quantity)
        {
            
        }
        /// <summary>
        /// s is condition(itemName)
        /// </summary>
        /// <param name="s"></param>

        protected virtual void Raise_Audio_Conversation(string s)
        {
            //if s is empty, play default audio??
            if(string.IsNullOrEmpty(s))
            {
                //TODO: whether to play default audio
                _mOnTaskContext._chooseAction.RaiseEvent(_mOnTaskContext._conversationMapping.m_ConAudioDialogue[0]._audio);
                ConversationManager.Instance.PlayDialogue(_mOnTaskContext._conversationMapping.m_ConAudioDialogue[0]._dialogueData);
                return;
            }
            //matching
            foreach(var item in _mOnTaskContext._conversationMapping.m_ConAudioDialogue)
            {
                if(item._condition == s)
                {
                    _mOnTaskContext._chooseAction.RaiseEvent(item._audio);
                    ConversationManager.Instance.PlayDialogue(item._dialogueData);
                    return;
                }
            }
                //TODO: whether to play default audio
                _mOnTaskContext._chooseAction.RaiseEvent(_mOnTaskContext._conversationMapping.m_ConAudioDialogue[0]._audio);
                ConversationManager.Instance.PlayDialogue(_mOnTaskContext._conversationMapping.m_ConAudioDialogue[0]._dialogueData);
        }
    }
    [Serializable]
    public class OnTaskGramaContext:OnTaskContext
    {
       public IntSO _taskEnd;
    }
    public class NPC_GramaTask : NPC_OnTask
    {
        private bool m_isFind = false;
        private int m_chooseCount = 0;
        private IntSO m_taskEnd;
        public NPC_GramaTask(NPC_Grama npcController, OnTaskGramaContext onTaskContext) : base(npcController, onTaskContext)
        {
            m_taskEnd = onTaskContext._taskEnd;
        }

        public override void Exit()
        {
            base.Exit();
            m_taskEnd.RaiseEvent(m_chooseCount);
        }

        protected override void AddToCart(string itemName, int quantity)
        {
            m_chooseCount++;
            //matching
            foreach (var item in m_shoppingList)
            {
                if (item._name == itemName)
                {
                    Raise_Audio_Conversation(itemName);
                    //change status
                    m_isFind = true;
                    return;
                }
            }
            //can't find item
            Raise_Audio_Conversation("");
        }

        protected override bool CanChangeState()
        {
            return m_isFind;
        }

        protected override void ChangeState()
        {
            m_NPC_Controller.ChangeState(StateType.Shopping);
        }
    }
    #region female
    [Serializable]
    public class OnTask1FemaleContext:OnTaskContext
    {
        
    }
    public class NPC_FemaleTask1 : NPC_OnTask
    {
        public NPC_FemaleTask1(NPC_Female npcController, OnTask1FemaleContext onTaskContext) : base(npcController, onTaskContext)
        {
        }
        
        protected override void AddToCart(string itemName, int quantity)
        {
            //matching
            foreach (var item in m_shoppingList)
            {
                if (item._name == itemName && item._count >= quantity)
                {
                    Raise_Audio_Conversation(itemName);
                    //reduce need count
                    item._count -= quantity;
                    if(item._count <= 0)
                    {
                        //remove from need list
                        m_shoppingList.Remove(item);
                    }
                    return;
                }
            } 
            //can't find item
            Raise_Audio_Conversation("");
        }

        protected override bool CanChangeState()
        {
            return m_shoppingList.Count == 0;
        }

        protected override void ChangeState()
        {
            //change to task2 state
            m_NPC_Controller.SwitchToNextTask();
            m_NPC_Controller.ChangeState(StateType.TaskAvaliable);
        }
    }
    [Serializable]
    public class OnTask2FemaleContext:OnTaskContext
    {
        public ConAudioDialogueSO _taskEndDiaglogueSO;
        public VoidSO _taskEnd;
        public float _timeLimit = 40.0f;
    }
    public class NPC_FemaleTask2 : NPC_OnTask
    {
        private float m_timeLimit = 0.0f;
        private float m_timer = 0.0f;
        private VoidSO m_taskEnd;
        private List<ConAudioDialogue> m_taskEndDialogueList;
        public NPC_FemaleTask2(NPC_Female npcController, OnTask2FemaleContext onTaskContext) : base(npcController, onTaskContext)
        {
            m_timeLimit = onTaskContext._timeLimit;
            m_taskEnd = onTaskContext._taskEnd;
            m_taskEndDialogueList = onTaskContext._taskEndDiaglogueSO.m_ConAudioDialogue;
        }

        protected override bool CanChangeState()
        {
            return m_timer >= m_timeLimit || m_shoppingList.Count == 0;
        }

        public override void Enter()
        {
            base.Enter();
            //reset timer
            m_timer = 0.0f;
            TimeManager.Instance.StartCountDown(m_timeLimit);
        }

        public override void Execute()
        {
            base.Execute();
            m_timer += Time.deltaTime;
        }
        protected override void ChangeState()
        {
            //change to task3 state
            m_NPC_Controller.ChangeState(StateType.Shopping);
        }

        protected override void AddToCart(string itemName, int quantity)
        {
            //matching
            foreach (var item in m_shoppingList)
            {
                if (item._name == itemName && item._count >= quantity)
                {
                    Raise_Audio_Conversation(itemName);
                    //reduce need count
                    item._count -= quantity;
                    if(item._count <= 0)
                    {
                        //remove from need list
                        m_shoppingList.Remove(item);
                    }
                    return;
                }
            } 
            //can't find item
            Raise_Audio_Conversation("");
        }

        public override void Exit()
        {
            base.Exit();
            m_timer = 0.0f;
            ShowEndPanel();
            m_taskEnd.RaiseEvent();
        }
        private void ShowEndPanel()
        {
            Debug.LogError($"Shopping List Count: {m_shoppingList.Count}");
            switch(m_shoppingList.Count)
            {
                case 0:
                    Raise_End_Conversation("perfect");
                    break;
                case 1:
                    Raise_End_Conversation("great");
                    break;
                case 2: 
                    Raise_End_Conversation("great"); 
                    break;
                case 3:
                    Raise_End_Conversation("good");
                    break;
                case 4:
                    Raise_End_Conversation("good");
                    break;
                default:
                    Debug.LogError("Plz check the task context, there are more than 4 items in need list.");
                    break;
            }
            TimeManager.Instance.StopCountDown();
        }

        private void Raise_End_Conversation(string s)
        {
            //if s is empty, play default audio??
            if(string.IsNullOrEmpty(s))
            {
                //TODO: whether to play default audio
                _mOnTaskContext._chooseAction.RaiseEvent(m_taskEndDialogueList[0]._audio);
                ConversationManager.Instance.PlayDialogue(m_taskEndDialogueList[0]._dialogueData);
                return;
            }
            //matching
            foreach(var item in m_taskEndDialogueList)
            {
                if(item._condition == s)
                {
                    _mOnTaskContext._chooseAction.RaiseEvent(item._audio);
                    ConversationManager.Instance.PlayDialogue(item._dialogueData);
                    return;
                }
            }
            //TODO: whether to play default audio
            _mOnTaskContext._chooseAction.RaiseEvent(m_taskEndDialogueList[0]._audio);
            ConversationManager.Instance.PlayDialogue(m_taskEndDialogueList[0]._dialogueData);
        }
    }
    #endregion
}