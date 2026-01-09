using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarkingList.Scripts
{
   public class ShoppingListData:ListDataBase<string>
 {
     [Header("Settings: Initial Shopping list")] 
     [SerializeField] private ShoppingListSO m_NeedList;
     [HideInInspector] public int m_shoppingListCount;
     
     /*[Header("Debug: Running")]*/
     /*public List<string> m_ItemOrder = new List<string>();*/

     private Dictionary<string,int> m_curNeedCount = new Dictionary<string, int>();
     
     [Header("Settings: Item Data Mapping")] 
     [SerializeField] PropSettingsSO m_DataSettings;
    
     [Header("Running")]
     Dictionary<string,ProductInfo> m_ItemMapping = new Dictionary<string, ProductInfo>();
     
     private void Awake()
     {
         foreach (var item in m_NeedList.m_NeedList)
         {
             m_curNeedCount.Add(item._name, item._count);
             m_allItem.Add(item._name);
         }
         foreach (var item in m_DataSettings.m_PropSettings)
         {
             m_ItemMapping.Add(item._name, item);
         }
         m_shoppingListCount = m_allItem.Count;
     }

     public override void SetItemInfo(RectTransform rec, int index)
     {
         Item item = rec.GetComponent<Item>();
         if (index == -1 || index >= m_allItem.Count)
         {
             // 设置默认值
             item.SetItemInfo("None", null, new List<string>(), 0,false);
             return;
         }
         // 设置物品信息
         string objName = m_allItem[index];
         Sprite sprite = null;
         List<string> region = new List<string>();
         if (m_ItemMapping.ContainsKey(objName))
         {
             sprite = m_ItemMapping[objName]._sprite;
             region = m_ItemMapping[objName]._region;
         }

         bool isEqual = m_curNeedCount[objName] == 0;
         item.SetItemInfo(objName, sprite, region, m_curNeedCount[objName],isEqual); 
     }
     #region External Call
    
     /// <summary>
     /// remove from cart,call
     /// </summary>
     /// <param name="item"></param>
     public void AddNeed(string objectName,int count)
     {
         if (m_curNeedCount.ContainsKey(objectName))
         {
             m_curNeedCount[objectName] += count;
         }
         else
         {
             m_curNeedCount.Add(objectName, count);
             m_allItem.Add(objectName);
         }
     }
     /// <summary>
     /// add to cart,call
     /// </summary>
     public void RemoveNeed(string objectName,int count)
     {
         if (m_curNeedCount.ContainsKey(objectName))
         {
             m_curNeedCount[objectName] -= count;
         }
         else
         {
             m_curNeedCount.Add(objectName, -count);
             m_allItem.Add(objectName);
             Debug.LogWarning($"Item {objectName} overload");
         }
     }
     /*/// <summary>
           /// modify shopping list
           /// add item count
           /// </summary>
           /// <param name="objectName"></param>
           /// <param name="count"></param>
           public void AddItem(string objectName, int count)
           {
               if (m_curNeedCount.ContainsKey(objectName))
               {
                   m_curNeedCount[objectName] += count;
               }
               else
               {
                   m_curNeedCount.Add(objectName, count);
                   m_allItem.Add(objectName);
               }
           }*/
          
      /*/// <summary>
      /// modify shopping list
      /// if count > item count, set to 0
      /// </summary>
      /// <param name="objectTag"></param>
      /// <param name="count"></param>
           public void RemoveItem(string objectName, int count)
           {
               if (m_curNeedCount.ContainsKey(objectName))    
               {
                   m_curNeedCount[objectName] -= count;
                   if (m_curNeedCount[objectName] <= 0)
                   {
                       m_curNeedCount.Remove(objectName);
                       m_allItem.Remove(objectName);
                   }
               }
               else
               {
                   Debug.LogError($"Item {objectName} not found");
               }
           }*/
     #endregion 
     #region Debug
     public void Debug_AddNeed()
     {
        AddNeed(11.ToString(),1);
     }
     public void Debug_RemoveNeed()
     {
        RemoveNeed(11.ToString(),1);
     }
     #endregion
 }
}