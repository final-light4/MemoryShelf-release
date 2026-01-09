using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

// 标记该类为编辑器工具，不参与打包
[CustomEditor(typeof(ListEnumFiller))]
public class ListEnumFillerEditor : Editor
{
    // 目标脚本的引用
    private ListEnumFiller targetScript;
    // 存储枚举类型的全名（用于序列化）
    private SerializedProperty enumTypeNameProp;
    private SerializedProperty fillModeProp;
    private SerializedProperty targetListProp;

    private void OnEnable()
    {
        // 获取挂载该编辑器的目标脚本
        targetScript = (ListEnumFiller)target;
        
        // 初始化序列化属性（避免直接修改字段导致的序列化问题）
        enumTypeNameProp = serializedObject.FindProperty("enumTypeName");
        fillModeProp = serializedObject.FindProperty("fillMode");
        targetListProp = serializedObject.FindProperty("targetList");
    }

    // 重写Inspector绘制
    public override void OnInspectorGUI()
    {
        // 更新序列化对象
        serializedObject.Update();

        // 绘制默认的Inspector属性（仅显示targetList）
        EditorGUILayout.PropertyField(targetListProp, true);

        // 绘制分隔线
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("枚举自动填充工具", EditorStyles.boldLabel);

        // ========== 修复核心问题：枚举类型选择逻辑 ==========
        // 1. 获取所有可用的枚举类型
        var allEnumTypes = GetAllEnumTypesInProject();
        
        // 2. 提取枚举类型名称列表
        var enumTypeNames = allEnumTypes.Select(t => t.FullName).ToList();
        enumTypeNames.Insert(0, "请选择枚举类型"); // 添加默认选项

        // 3. 找到当前选中的枚举类型索引
        int selectedIndex = string.IsNullOrEmpty(enumTypeNameProp.stringValue) 
            ? 0 
            : enumTypeNames.IndexOf(enumTypeNameProp.stringValue);
        
        // 4. 下拉选择框
        selectedIndex = EditorGUILayout.Popup("目标枚举类型", selectedIndex, enumTypeNames.ToArray());
        
        // 5. 更新选中的枚举类型名称
        if (selectedIndex > 0 && selectedIndex < enumTypeNames.Count)
        {
            enumTypeNameProp.stringValue = enumTypeNames[selectedIndex];
        }
        else
        {
            enumTypeNameProp.stringValue = string.Empty;
        }

        // 获取当前选中的枚举类型
        Type selectedEnumType = string.IsNullOrEmpty(enumTypeNameProp.stringValue)
            ? null
            : Type.GetType(enumTypeNameProp.stringValue);

        // 校验枚举类型是否有效
        if (selectedEnumType == null || !selectedEnumType.IsEnum)
        {
            EditorGUILayout.HelpBox("请选择有效的枚举类型！", MessageType.Warning);
        }
        else
        {
            // 2. 选择填充模式（覆盖/追加）
            EditorGUILayout.PropertyField(fillModeProp);

            // 3. 填充按钮
            if (GUILayout.Button("一键填充List", GUILayout.Height(30)))
            {
                FillListWithEnumValues(selectedEnumType);
                // 应用序列化修改
                serializedObject.ApplyModifiedProperties();
                // 标记场景有修改，提示保存
                EditorUtility.SetDirty(targetScript);
                AssetDatabase.SaveAssets();
                EditorUtility.DisplayDialog("成功", $"已将枚举【{selectedEnumType.Name}】的所有值填充到List！", "确定");
            }
        }

        // 应用所有序列化修改
        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 获取项目中所有的枚举类型
    /// </summary>
    private List<Type> GetAllEnumTypesInProject()
    {
        List<Type> enumTypes = new List<Type>();
        
        // 遍历所有程序集
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                // 获取程序集中所有的枚举类型
                Type[] types = assembly.GetTypes();
                enumTypes.AddRange(types.Where(t => t.IsEnum && !t.IsNestedPrivate));
            }
            catch (ReflectionTypeLoadException)
            {
                // 忽略加载失败的程序集
                continue;
            }
            catch (Exception)
            {
                continue;
            }
        }

        // 按名称排序
        enumTypes = enumTypes.OrderBy(t => t.Name).ToList();
        return enumTypes;
    }

    /// <summary>
    /// 核心逻辑：将枚举所有值填充到List
    /// </summary>
    private void FillListWithEnumValues(Type enumType)
    {
        // 获取枚举的所有值
        Array enumValues = Enum.GetValues(enumType);
        List<string> enumValueNames = enumValues.Cast<object>()
                                               .Select(v => v.ToString())
                                               .ToList();

        // 根据填充模式处理List
        switch ((FillMode)fillModeProp.enumValueIndex)
        {
            case FillMode.Override:
                // 覆盖模式：清空原有数据
                targetListProp.ClearArray();
                for (int i = 0; i < enumValueNames.Count; i++)
                {
                    targetListProp.InsertArrayElementAtIndex(i);
                    targetListProp.GetArrayElementAtIndex(i).stringValue = enumValueNames[i];
                }
                break;

            case FillMode.Append:
                // 追加模式：只添加不存在的值
                // 先获取现有List中的值
                List<string> existingValues = new List<string>();
                for (int i = 0; i < targetListProp.arraySize; i++)
                {
                    existingValues.Add(targetListProp.GetArrayElementAtIndex(i).stringValue);
                }

                // 添加不存在的值
                foreach (string enumName in enumValueNames)
                {
                    if (!existingValues.Contains(enumName))
                    {
                        int newIndex = targetListProp.arraySize;
                        targetListProp.InsertArrayElementAtIndex(newIndex);
                        targetListProp.GetArrayElementAtIndex(newIndex).stringValue = enumName;
                    }
                }
                break;
        }
    }
}

/// <summary>
/// 挂载到任意GameObject上的运行时脚本（存储List和配置）
/// </summary>
[Serializable]
public class ListEnumFiller : MonoBehaviour
{
    // 要填充的目标List（存储枚举值的字符串形式）
    [Tooltip("需要自动填充的目标List")]
    public List<string> targetList = new List<string>();

    // 编辑器用：存储选中的枚举类型全名（序列化用）
    [HideInInspector] public string enumTypeName = string.Empty;

    // 填充模式
    [HideInInspector] public FillMode fillMode = FillMode.Override;
}

/// <summary>
/// 填充模式枚举
/// </summary>
[Serializable]
public enum FillMode
{
    Override,  // 覆盖原有数据
    Append     // 追加新数据（去重）
}