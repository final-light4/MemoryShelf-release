using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PromotionIdentifier : MonoBehaviour
{
    public string promoText = "SALE!";
    public Vector3 offsetPosition = new Vector3(0, 0, 0);
    public Vector3 rotationOffset = new Vector3(-90, 0, 0);
}

public class BatchAddPromotionIdentifier : EditorWindow
{
    private GameObject identifierPrefab;
    private Vector3 customOffsetPosition;
    private Vector3 customRotationOffset;

    [MenuItem("Tools/Batch Add Promotion Identifier")]
    static void ShowWindow()
    {
        GetWindow<BatchAddPromotionIdentifier>("批量添加促销标识");
    }

    private void OnGUI()
    {
        GUILayout.Label("批量添加促销标识", EditorStyles.boldLabel);

        // 选择预制件选项
        identifierPrefab = (GameObject)EditorGUILayout.ObjectField("促销标识预制件", identifierPrefab, typeof(GameObject), false);

        // 自定义偏移量选项
        customOffsetPosition = EditorGUILayout.Vector3Field("位置偏移", customOffsetPosition);
        customRotationOffset = EditorGUILayout.Vector3Field("旋转偏移", customRotationOffset);

        if (GUILayout.Button("给选中的物体添加促销标识"))
        {
            AddPromotionIdentifiers();
        }
    }

    private void AddPromotionIdentifiers()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("请先在Hierarchy中选中要添加标识的物体！");
            return;
        }

        int addedCount = 0;

        foreach (GameObject obj in selectedObjects)
        {
            // 添加促销标识组件
            PromotionIdentifier identifier = obj.GetComponent<PromotionIdentifier>();
            if (identifier == null)
            {
                identifier = obj.AddComponent<PromotionIdentifier>();

                // 设置默认值
                identifier.promoText = "SALE!";
                identifier.offsetPosition = customOffsetPosition;
                identifier.rotationOffset = customRotationOffset;

                addedCount++;
            }
            else
            {
                Debug.Log($"{obj.name} 已经有促销标识组件，跳过添加。");
            }

            // 添加预制件
            if (identifierPrefab != null)
            {
                AddPromotionPrefab(obj);
            }
        }

        Debug.Log($"已成功为 {addedCount} 个物体添加促销标识！");
    }

    private void AddPromotionPrefab(GameObject parent)
    {
        // 检查是否已经有相同的预制件
        Transform existingPrefab = parent.transform.Find("Promotion_Prefab");
        if (existingPrefab != null)
        {
            Debug.Log($"{parent.name} 已经有促销预制件，跳过添加。");
            return;
        }

        // 实例化预制件
        GameObject prefabInstance = Instantiate(identifierPrefab, parent.transform);
        prefabInstance.name = "Promotion_Prefab";

        // 设置位置和旋转
        PromotionIdentifier identifier = parent.GetComponent<PromotionIdentifier>();
        if (identifier != null)
        {
            prefabInstance.transform.localPosition = identifier.offsetPosition;
            prefabInstance.transform.localEulerAngles = identifier.rotationOffset;
        }
        else
        {
            // 使用默认值
            prefabInstance.transform.localPosition = new Vector3(0, 0, 0);
            prefabInstance.transform.localEulerAngles = new Vector3(-90, 0, 0);
        }
    }
}
