using UnityEngine;
using UnityEngine.Events;

namespace Global.Scripts
{
    [CreateAssetMenu(fileName = "StringSO", menuName = "Event System/StringSO")]
    public class StringSO : ScriptableObject
    {
        public UnityAction<string> action;

        public void RaiseEvent(string s)
        {
            action?.Invoke(s);
        }
    }
}