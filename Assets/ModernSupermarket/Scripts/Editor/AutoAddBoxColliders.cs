using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class AutoBoxColliderTool : EditorWindow
{
    private bool includeInactive = true;
    private string keyword = "";
    private bool onlyStatic = false;

    [MenuItem("Tools/Auto BoxCollider Tool")]
    public static void ShowWindow()
    {
        GetWindow<AutoBoxColliderTool>("Auto BoxCollider Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("自动为环境物体添加 BoxCollider", EditorStyles.boldLabel);
        includeInactive = EditorGUILayout.Toggle("包含未激活对象", includeInactive);
        onlyStatic = EditorGUILayout.Toggle("仅限静态对象", onlyStatic);
        keyword = EditorGUILayout.TextField("检测关键字（匹配名称）", keyword);

        if (GUILayout.Button("开始批量添加 BoxCollider", GUILayout.Height(30)))
        {
            AddColliders();
        }
    }

    private void AddColliders()
    {
        int addedCount = 0;
        var scene = SceneManager.GetActiveScene();
        var roots = scene.GetRootGameObjects();

        foreach (var root in roots)
        {
            foreach (var renderer in root.GetComponentsInChildren<MeshRenderer>(includeInactive))
            {
                var go = renderer.gameObject;

                // 过滤关键字
                if (!string.IsNullOrEmpty(keyword) && !go.name.ToLower().Contains(keyword.ToLower()))
                    continue;

                // 跳过不符合的静态性
                if (onlyStatic && !go.isStatic)
                    continue;

                // 跳过已有 Collider 的
                if (go.GetComponent<Collider>() != null)
                    continue;

                // 确保 renderer bounds 有效
                if (renderer.bounds.size.magnitude < 0.001f)
                    continue;

                // 添加 BoxCollider
                var collider = Undo.AddComponent<BoxCollider>(go);

                // 使用世界坐标 Bounds 拟合
                Bounds b = renderer.bounds;
                collider.center = go.transform.InverseTransformPoint(b.center);
                collider.size = go.transform.InverseTransformVector(b.size);

                addedCount++;
            }
        }

        Debug.Log($"成功为 {addedCount} 个对象添加 BoxCollider。");
    }
}
