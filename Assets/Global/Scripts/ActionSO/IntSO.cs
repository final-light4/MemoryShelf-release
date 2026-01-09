using UnityEngine;
using UnityEngine.Events;

namespace Global.Scripts
{
    [CreateAssetMenu(fileName = "IntSO", menuName = "Event System/IntSO")]
    public class IntSO : ScriptableObject
    {
        public UnityAction<int> action;

        public void RaiseEvent(int i)
        {
            action?.Invoke(i);
        }
    }
}