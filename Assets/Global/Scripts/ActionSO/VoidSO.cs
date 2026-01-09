using UnityEngine;
using UnityEngine.Events;

namespace Global.Scripts
{
    [CreateAssetMenu(fileName = "VoidSO", menuName = "Event System/VoidSO")]
    public class VoidSO : ScriptableObject
    {
        public UnityAction action;
    
        public void RaiseEvent()
        {
            action?.Invoke();
        }
    }
}

