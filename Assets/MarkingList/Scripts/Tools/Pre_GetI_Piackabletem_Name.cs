using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarkingList.Scripts
{
    public class Pre_GetI_Piackabletem_Name:MonoBehaviour
    {
        private int cnt = 0;
        void Start()
        {
            // 存储唯一的物体名称
            List<string> uniqueObjectNames = new List<string>();
        
            // 获取场景中所有激活和未激活的物体
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        
            foreach (GameObject obj in allObjects)
            {
                // 排除资源文件（只保留场景中的物体）
                if (!obj.scene.IsValid() || obj.hideFlags != HideFlags.None)
                {
                    continue;
                }
            
                if (obj.GetComponent<PickableItem>() != null)
                {
                    cnt++;
                    uniqueObjectNames.Add(obj.GetComponent<PickableItem>().itemName);
                }
            }
        
            // 将去重后的名称转换为列表并排序（可选，便于查看）
            List<string> sortedNames = new List<string>(uniqueObjectNames);
            sortedNames.Sort();
        
            // 打印结果（两种格式：分行和逗号分隔，方便复制）
            Debug.Log("=== 场景中所有唯一物体名称（分行格式）===");
            foreach (string name in sortedNames)
            {
                Debug.Log(name);
            }
        
            Debug.Log("\n=== 场景中所有唯一物体名称（逗号分隔，可直接复制）===");
            string combinedNames = string.Join(", ", sortedNames);
            Debug.Log(combinedNames);
        
            // 额外：将结果写入剪贴板（Unity 2021+ 推荐使用GUIUtility，旧版本可使用TextEditor）
#if UNITY_2021_1_OR_NEWER
            GUIUtility.systemCopyBuffer = combinedNames;
            Debug.Log("已自动将逗号分隔的名称复制到剪贴板！");
#else
        TextEditor te = new TextEditor();
        te.text = combinedNames;
        te.SelectAll();
        te.Copy();
        Debug.Log("已自动将逗号分隔的名称复制到剪贴板！");
#endif
        }

        private void OnDestroy()
        {
            Debug.Log($"已获取 {cnt} 个可拾取物品名称");
        }
    }
    
}