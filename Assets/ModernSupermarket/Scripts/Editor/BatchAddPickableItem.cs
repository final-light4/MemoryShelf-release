using UnityEditor;
using UnityEngine;

public class BatchAddPickableItem : EditorWindow
{
    public GameObject parentObject;
    public float minPrice = 1f;
    public float maxPrice = 100f;
    public string defaultDescription = "A nice item you can pick up";

    [MenuItem("Tools/Batch Add Pickable Items")]
    static void Init()
    {
        BatchAddPickableItem window = (BatchAddPickableItem)GetWindow(typeof(BatchAddPickableItem));
        window.titleContent = new GUIContent("Batch Add Pickable Items");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Batch Add PickableItem", EditorStyles.boldLabel);
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);
        minPrice = EditorGUILayout.FloatField("Min Price", minPrice);
        maxPrice = EditorGUILayout.FloatField("Max Price", maxPrice);
        defaultDescription = EditorGUILayout.TextField("Default Description", defaultDescription);

        if (GUILayout.Button("Add PickableItem Components"))
        {
            if (parentObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign a parent object (e.g., Products).", "OK");
                return;
            }

            int addedCount = 0;
            AddPickableItemRecursive(parentObject.transform, ref addedCount);

            EditorUtility.DisplayDialog("Done", $"✅ Added PickableItem to {addedCount} objects.", "OK");
        }
    }

    /// <summary>
    /// 递归添加 PickableItem 组件
    /// </summary>
    void AddPickableItemRecursive(Transform parent, ref int count)
    {
        foreach (Transform child in parent)
        {
            // 如果该物体有 MeshRenderer 或 Collider，说明它是一个可视的物体
            bool isRenderable = child.GetComponent<MeshRenderer>() || child.GetComponent<Collider>();

            if (isRenderable && child.GetComponent<PickableItem>() == null)
            {
                var item = Undo.AddComponent<PickableItem>(child.gameObject);
                item.itemName = child.name;
                item.itemPrice = Random.Range(minPrice, maxPrice);
                item.itemDescription = defaultDescription;
                count++;
            }

            // 递归子物体
            AddPickableItemRecursive(child, ref count);
        }
    }
}
