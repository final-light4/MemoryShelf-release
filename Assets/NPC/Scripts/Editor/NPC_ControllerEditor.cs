using UnityEditor;
using UnityEngine;
using NPC.Scripts; // 匹配你的命名空间

// 绑定到NPC_Controller类，启用多对象编辑
[CustomEditor(typeof(NPC_Controller))]
[CanEditMultipleObjects]
public class NPC_ControllerEditor : Editor
{
    // 缓存各个Context字段的Property对象，提升性能
    private SerializedProperty m_stateType;
    private SerializedProperty m_routeInfo;
    private SerializedProperty m_TaskAvaliableContext;
    private SerializedProperty m_wanderInfo;
    private SerializedProperty m_idleInfo;
    private SerializedProperty m_OnTaskContext;
    
    // 其他需要显示的基础字段
    private SerializedProperty m_name;

    // 初始化：获取所有需要操作的字段
    private void OnEnable()
    {
        // 获取StateType枚举字段
        m_stateType = serializedObject.FindProperty("m_stateType");
        
        // 获取所有Context字段
        m_routeInfo = serializedObject.FindProperty("m_routeInfo");
        m_TaskAvaliableContext = serializedObject.FindProperty("m_TaskAvaliableContext");
        m_wanderInfo = serializedObject.FindProperty("m_wanderInfo");
        m_idleInfo = serializedObject.FindProperty("m_idleInfo");
        
        // 获取Debug字段
        m_name = serializedObject.FindProperty("m_name");
    }

    // 重写Inspector绘制逻辑
    public override void OnInspectorGUI()
    {
        // 绑定序列化对象
        serializedObject.Update();

        // 1. 绘制"All Base"和"Settings: State Type"区域
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("All Base", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.PropertyField(m_stateType, new GUIContent("State Type"));
        EditorGUILayout.Space();
        

        // 2. 根据m_stateType的值，只绘制对应的Context字段
        EditorGUILayout.LabelField("Settings: State", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        StateType currentState = (StateType)m_stateType.enumValueIndex;
        switch (currentState)
        {
            case StateType.Idle:
                DrawContextProperty(m_idleInfo, "Idle Context");
                break;
            case StateType.Shopping:
                DrawContextProperty(m_wanderInfo, "Wander Context (Shopping)");
                DrawContextProperty(m_idleInfo, "Idle Context");
                break;
            case StateType.GoToAimedArea:
                DrawContextProperty(m_routeInfo, "Aim Context (GoToAimedArea)");
    
                // 核心：读取AimContext中的_isTask布尔值（SerializedProperty方式）
                SerializedProperty isTaskProp = m_routeInfo.FindPropertyRelative("_isTask");
                // 安全判断：防止字段不存在导致空引用
                bool isTask = isTaskProp != null ? isTaskProp.boolValue : true;
    
                // 动态绘制字段
                if (isTask == true)
                {
                    DrawContextProperty(m_TaskAvaliableContext, "Task Avaliable Context");
                }
                else
                {
                    DrawContextProperty(m_idleInfo, "Idle Context");
                }
                break;
            case StateType.TaskAvaliable:
                DrawContextProperty(m_TaskAvaliableContext, "Task Avaliable Context");
                break;
            case StateType.OnTask:
                break;
            default:
                EditorGUILayout.HelpBox("无效的状态类型！", MessageType.Error);
                break;
        }
        
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