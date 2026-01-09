using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarkingList.Scripts
{
    [Serializable]
    public class ItemCountInfo
    {
        public string _name;
        public int _count;
        public ItemCountInfo DeepClone()
        {
            return new ItemCountInfo()
            {
                _name = this._name,   // string 是不可变类型，直接赋值即可
                _count = this._count  // 值类型直接拷贝
            };
        }
    }
    [CreateAssetMenu(fileName = "ShoppingList", menuName = "MarkingList/Shopping List")]
    public class ShoppingListSO : ScriptableObject
    {
        public List<ItemCountInfo> m_NeedList = new List<ItemCountInfo>();
    }
}