 using System;
 using System.Collections.Generic;
 using UnityEngine;
public abstract class ListDataBase<T> : MonoBehaviour
{
    public abstract void SetItemInfo(RectTransform rec, int index);
    [Header("Debug: Running")]
    public List<T> m_allItem = new List<T>();
}