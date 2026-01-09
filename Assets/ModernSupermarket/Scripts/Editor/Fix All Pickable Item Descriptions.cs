using UnityEditor;
using UnityEngine;

public class FixPickableItemDescription : EditorWindow
{
    [MenuItem("Tools/Fix All Pickable Item Descriptions")]
    static void ShowWindow()
    {
        GetWindow<FixPickableItemDescription>("Fix Item Descriptions");
    }

    void OnGUI()
    {
        GUILayout.Label("批量修改 PickableItem 的描述", EditorStyles.boldLabel);

        if (GUILayout.Button("将所有 PickableItem 的描述改为 \"按E拿取\""))
        {
            // 查找场景中所有带有 PickableItem 组件的对象
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            int count = 0;

            foreach (GameObject go in allObjects)
            {
                PickableItem pickableItem = go.GetComponent<PickableItem>();
                if (pickableItem != null)
                {
                    pickableItem.itemDescription = "按E拿取";
                    EditorUtility.SetDirty(go); // 标记为已修改，保存时会更新
                    count++;
                }
            }

            Debug.Log($"成功修改了 {count} 个 PickableItem 的描述。");
        }
    }
}