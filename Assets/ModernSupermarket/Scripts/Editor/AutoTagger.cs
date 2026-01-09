using UnityEditor;
using UnityEngine;
using System.Linq;

public class AutoTagger : EditorWindow
{
    [MenuItem("Tools/Smart Tag Pickable Items (Improved)")]
    public static void TagPickableItems()
    {
        EnsureTagExists("Pickable");

        int taggedCount = 0;
        int colliderAdded = 0;

        var allObjects = FindObjectsOfType<GameObject>(true);

        foreach (var go in allObjects)
        {
            if (!go.activeInHierarchy)
                continue;

            string lowerName = go.name.ToLower();

            // 跳过明显不是货物的对象
            string[] ignoreKeywords = {
                "shelf", "rack", "floor", "wall", "roof", "light",
                "sign", "banner", "ceiling", "pillar", "beam", "door",
                "window", "lamp", "poster", "exit", "spotlight"
            };
            if (ignoreKeywords.Any(k => lowerName.Contains(k)))
                continue;

            // 如果物体或其子物体中含有 Renderer，则认为它是货物整体
            bool hasRenderer = go.GetComponentsInChildren<Renderer>(true).Any();
            if (!hasRenderer)
                continue;

            // 如果父级也有 Renderer，就说明这是一个局部子件，跳过（只标最外层）
            var parent = go.transform.parent;
            bool parentHasRenderer = parent != null && parent.GetComponentsInChildren<Renderer>(true).Any();
            if (parentHasRenderer)
                continue;

            // 打上 Pickable 标签
            if (!go.CompareTag("Pickable"))
            {
                Undo.RecordObject(go, "AutoTag Pickable");
                go.tag = "Pickable";
                taggedCount++;
            }

            // 添加 Collider（若整组都没有）
            if (go.GetComponentInChildren<Collider>(true) == null)
            {
                var meshFilter = go.GetComponentInChildren<MeshFilter>();
                if (meshFilter != null && meshFilter.sharedMesh != null)
                {
                    var meshCollider = go.AddComponent<MeshCollider>();
                    meshCollider.convex = true;
                }
                else
                {
                    go.AddComponent<BoxCollider>();
                }
                colliderAdded++;
            }
        }

        Debug.Log($"✅ AutoTagger (Improved): 已为 {taggedCount} 个最外层货物设置了 Pickable 标签，并为 {colliderAdded} 个对象添加碰撞体。");
    }

    private static void EnsureTagExists(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(
            AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");

        bool tagExists = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tagName))
            {
                tagExists = true;
                break;
            }
        }

        if (!tagExists)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            tagsProp.GetArrayElementAtIndex(0).stringValue = tagName;
            tagManager.ApplyModifiedProperties();
            Debug.Log($"🆕 已创建Tag: {tagName}");
        }
    }
}
