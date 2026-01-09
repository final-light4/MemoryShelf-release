using UnityEngine;
using UnityEngine.Events;

namespace Global.Scripts
{
    [CreateAssetMenu(fileName = "AudioClipSO", menuName = "Event System/AudioClipSO")]
    public class AudioClipSO : ScriptableObject
    {
        public UnityAction<AudioClip> action;

        public void RaiseEvent(AudioClip audioClip)
        {
            action?.Invoke(audioClip);
        }
    }
}