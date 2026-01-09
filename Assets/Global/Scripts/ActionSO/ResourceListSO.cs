using System.Collections.Generic;
using UnityEngine;

namespace Global.Scripts
{
    [CreateAssetMenu(fileName = "AudioListSO", menuName = "Resource List SO/Audio List")]
    public class AudioListSO: ScriptableObject
    {
        public List<AudioClip> _clips;
    }
}