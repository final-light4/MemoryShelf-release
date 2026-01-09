using UnityEngine;

namespace Global.Scripts
{
    public class Debug_RaiseEvent : MonoBehaviour
    {
        public String_IntSO _OnAddToCart;
        public string[] _itemName;
        public NPC.Scripts.NPC_Controller m_NPC_Controller;
        
        public void RaiseAddToCart()
        {
            _OnAddToCart.RaiseEvent(_itemName[0], 1);
        }

        public void RaiseAddToCart1()
        {
            _OnAddToCart.RaiseEvent(_itemName[1], 1);
        }
        public void RaiseAddToCart2()
        {
            _OnAddToCart.RaiseEvent(_itemName[2], 1);
        }
        public void RaiseAddToCart3()
        {
            _OnAddToCart.RaiseEvent(_itemName[3], 1);
        }   
        public void RaiseAddToCart4()
        {
            _OnAddToCart.RaiseEvent(_itemName[4], 1);
        }   
        public void RaiseAddToCart5()
        {
            _OnAddToCart.RaiseEvent(_itemName[5], 1);
        }
        public void RaiseAddToCart6()
        {
            _OnAddToCart.RaiseEvent(_itemName[6], 1);
        }   
        public void RaiseAddToCart7()
        {
            _OnAddToCart.RaiseEvent(_itemName[7], 1);
        }
        public void RaiseAddToCart8()
        {
            _OnAddToCart.RaiseEvent(_itemName[8], 1);
        }
        public void SwitchToNextTask()
        {
            m_NPC_Controller.SwitchToNextTask();
            m_NPC_Controller.ChangeState(NPC.Scripts.StateType.TaskAvaliable);
        }
    }
}