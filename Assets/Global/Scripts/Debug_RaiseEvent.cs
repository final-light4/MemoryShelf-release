using UnityEngine;

namespace Global.Scripts
{
    public class Debug_RaiseEvent : MonoBehaviour
    {
        public String_IntSO _OnAddToCart;
        public string[] _itemName;
        
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
        public void RaiseAddToCart9()
        {
            _OnAddToCart.RaiseEvent(_itemName[9], 1);
        }   
        public void RaiseAddToCart10()
        {
            _OnAddToCart.RaiseEvent(_itemName[10], 1);
        }   
        public void RaiseAddToCart11()  
        {
            _OnAddToCart.RaiseEvent(_itemName[11], 1);
        }
        public void RaiseAddToCart12()
        {
            _OnAddToCart.RaiseEvent(_itemName[12], 1);
        }
        public void RaiseAddToCart13()
        {
            _OnAddToCart.RaiseEvent(_itemName[13], 1);  
        }
        public void RaiseAddToCart14()
        {
            _OnAddToCart.RaiseEvent(_itemName[14], 1);
        }
        public void RaiseAddToCart15()
        {
            _OnAddToCart.RaiseEvent(_itemName[15], 1);
        }
    }
}