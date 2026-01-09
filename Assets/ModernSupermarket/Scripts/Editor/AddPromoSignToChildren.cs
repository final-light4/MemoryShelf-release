using UnityEditor;
using UnityEngine;

public class AddPromoSignToChildren : EditorWindow
{
    private GameObject promoPrefab;  // 要添加的促销预制件

    [MenuItem("Tools/Add Promo Sign to All Children")]
    private static void ShowWindow()
    {
        GetWindow<AddPromoSignToChildren>("Add Promo Sign");
    }

    private void OnGUI()
    {
        promoPrefab = (GameObject)EditorGUILayout.ObjectField("促销预制件", promoPrefab, typeof(GameObject), false);

        if (GUILayout.Button("给选中物体的所有子物体添加预制件"))
        {
            if (promoPrefab == null)
            {
                Debug.LogError("请先指定促销预制件！");
                return;
            }

            GameObject selectedParent = Selection.activeGameObject;
            if (selectedParent == null)
            {
                Debug.LogError("请先在 Hierarchy 中选中一个父物体！");
                return;
            }

            // GetComponentsInChildren<Transform>(true) 会递归拿到 
            // selectedParent 下面所有层级的子物体（包括嵌套的子物体）。
            foreach (Transform child in selectedParent.GetComponentsInChildren<Transform>(true))
            {
                if (child != selectedParent.transform)  // 不给父物体自己加
                {
                    // 方式 2：Instantiate 预制件并作为子物体挂载
                    GameObject newSign = Instantiate(promoPrefab, child);
                    newSign.name = promoPrefab.name;  // 可选：避免名字带 (Clone)
                }
            }

            Debug.Log("已给 " + selectedParent.name + " 下所有子物体添加促销预制件！");
        }
    }
}