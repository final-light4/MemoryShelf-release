using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarkingList.Scripts
{
    [Serializable]
    public class ProductInfo
    {
        public string _name;
        public Sprite _sprite;
        public List<string> _region;
    }
    [CreateAssetMenu(fileName = "PropSettings", menuName = "MarkingList/Prop Settings")]
    public class PropSettingsSO : ScriptableObject
    {
        public List<ProductInfo> m_PropSettings = new List<ProductInfo>();
    }

    [Serializable]
    public class PackageItemInfo
    {
        
    }
}