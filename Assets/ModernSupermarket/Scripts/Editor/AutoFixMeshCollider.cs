using UnityEditor;
using UnityEngine;

public class AutoFixMeshCollider : EditorWindow
{
    [MenuItem("Tools/Auto Fix MeshColliders")]
    public static void ShowWindow()
    {
        GetWindow<AutoFixMeshCollider>("Auto Fix MeshColliders");
    }

    private void OnGUI()
    {
        GUILayout.Label("自动修复 Convex MeshCollider 超限物体", EditorStyles.boldLabel);
        GUILayout.Space(5);
        GUILayout.Label("功能：扫描场景中所有 MeshCollider，如果 Convex 打开且多边形数过多，则自动处理。");

        if (GUILayout.Button("开始扫描并修复"))
        {
            FixMeshColliders();
        }
    }

    private void FixMeshColliders()
    {
        int convexRemoved = 0;
        int replacedWithBox = 0;

        foreach (var collider in FindObjectsOfType<MeshCollider>())
        {
            if (collider.sharedMesh == null) continue;

            var mesh = collider.sharedMesh;
            int triangleCount = mesh.triangles.Length / 3;

            // 发现 Convex 且超过 256 面的物体
            if (collider.convex && triangleCount > 256)
            {
                // 判断是否静态物体
                Rigidbody rb = collider.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    // 静态物体 → 去掉 convex
                    collider.convex = false;
                    convexRemoved++;
                }
                else
                {
                    // 动态物体 → 改用 BoxCollider 替换
                    GameObject go = collider.gameObject;
                    Undo.RecordObject(go, "Replace Collider");
                    DestroyImmediate(collider);
                    go.AddComponent<BoxCollider>();
                    replacedWithBox++;
                }
            }
        }

        Debug.Log($"✅ 自动修复完成：去掉 Convex {convexRemoved} 个，替换为 BoxCollider {replacedWithBox} 个。");
    }
}
