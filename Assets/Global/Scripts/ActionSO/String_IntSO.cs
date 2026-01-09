using UnityEngine;
using UnityEngine.Events;

namespace Global.Scripts
{
    [CreateAssetMenu(fileName = "String_IntSO", menuName = "Event System/String_IntSO")]
    public class String_IntSO : ScriptableObject
    {
        public UnityAction<string, int> action;

        public void RaiseEvent(string s, int i)
        {
            action?.Invoke(s, i);
        }
    }
}