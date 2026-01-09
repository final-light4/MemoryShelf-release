using UnityEditor;
using UnityEngine;
using NPC.Scripts; // 匹配你的命名空间

// 绑定到NPC_Controller类，启用多对象编辑
[CustomEditor(typeof(NPC_Grama))]
[CanEditMultipleObjects]
public class NPC_GramaEditor : Editor
{
    // 缓存各个Context字段的Property对象，提升性能
    private SerializedProperty m_stateType;
    private  SerializedProperty m_routeInfo;
    private SerializedProperty m_TaskAvaliableContext;
    private SerializedProperty m_wanderInfo;
    private SerializedProperty m_idleInfo;
    private SerializedProperty c_OnTaskContext;
    
    // 其他需要显示的基础字段
    private SerializedProperty m_name;

    // 初始化：获取所有需要操作的字段
    private void OnEnable()
    {
        // 获取所有Context字段
        m_stateType = serializedObject.FindProperty("m_stateType");
        m_routeInfo = serializedObject.FindProperty("m_routeInfo");
        
        m_TaskAvaliableContext = serializedObject.FindProperty("m_TaskAvaliableContext");
        m_wanderInfo = serializedObject.FindProperty("m_wanderInfo");
        m_idleInfo = serializedObject.FindProperty("m_idleInfo");
        c_OnTaskContext = serializedObject.FindProperty("c_OnTaskContext");
        
        // 获取Debug字段
        m_name = serializedObject.FindProperty("m_name");
    }

    // 重写Inspector绘制逻辑
    public override void OnInspectorGUI()
    {
        // 绑定序列化对象
        serializedObject.Update();

        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(m_stateType, new GUIContent("State Type"));
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("Settings: State", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        DrawContextProperty(m_routeInfo, "Aim Context (GoToAimedArea)");
        DrawContextProperty(m_idleInfo, "Idle Context");
        DrawContextProperty(m_wanderInfo, "Wander Context (Shopping)");
        DrawContextProperty(m_TaskAvaliableContext, "Task Avaliable Context");
        DrawContextProperty(c_OnTaskContext, "On Task Context");
        
        EditorGUILayout.Space();
        EditorGUILayout.Separator(); // 分隔线

        // 3. 绘制Debug区域
        EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(m_name);

        // 4. 应用所有序列化修改（必须加，否则修改不生效）
        serializedObject.ApplyModifiedProperties();
    }

    // 封装Context字段绘制逻辑（适配可序列化自定义类）
    private void DrawContextProperty(SerializedProperty property, string label)
    {
        // 1. 绘制Context类字段（支持展开查看内部SO字段）
        EditorGUILayout.PropertyField(property, new GUIContent(label), true);
    
        // 2. 仅在Context类展开时，遍历内部字段检查SO引用是否为空（核心修复）
        if (property.isExpanded) // 只有展开Context时才检查内部字段
        {
            SerializedProperty childProp = property.Copy(); // 复制属性用于遍历
            SerializedProperty endProp = property.GetEndProperty();
        
            // 遍历Context内部所有可见字段
            while (childProp.NextVisible(true) && !SerializedProperty.EqualContents(childProp, endProp))
            {
                // 仅对内部的ObjectReference类型（SO/AudioClip等）做空值提示
                if (childProp.propertyType == SerializedPropertyType.ObjectReference && childProp.objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox(
                        $"⚠️ {label} → {childProp.displayName} 未赋值！", 
                        MessageType.Warning);
                }
            }
        }
    }
}