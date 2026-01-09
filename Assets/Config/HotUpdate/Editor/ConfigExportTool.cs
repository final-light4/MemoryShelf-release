using System;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using System.IO;
using System.Text;
using ExcelDataReader;
using ItemPackage.Scripts;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

// 引用配置命名空间

// 放在Editor文件夹，Unity编辑器菜单
public class ConfigExportTool
{
    [MenuItem("策划工具/导出背包配置JSON")]
    public static void ExportItemConfig()
    {
        // Excel文件路径（策划放在指定目录，如Assets/Config/Excel）
        string excelPath = Path.Combine(Application.dataPath, "Config/Excel/ItemDisPlayConfig.xlsx");
        // JSON输出路径（热更目录）
        string jsonOutputPath = Path.Combine(Application.dataPath, "Config/HotUpdate/item_display.json");

        // 检查Excel文件是否存在
        if (!File.Exists(excelPath))
        {
            Debug.LogError($"Excel文件不存在：{excelPath}");
            EditorUtility.DisplayDialog("错误", "Excel配置文件不存在，请检查路径！", "确定");
            return;
        }

        // 设置ExcelDataReader编码（解决中文乱码）
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        // 读取Excel
        using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true // 第一行作为表头
                    }
                });

                // 转换为字典（Key=ItemID，Value=配置对象）
                var configDict = new Dictionary<string, ItemDisplayConfig>();
                DataTable table = result.Tables[0];

                // 遍历行（跳过表头）
                foreach (DataRow row in table.Rows)
                {
                    // 跳过空行或无效行
                    string itemID = row["ItemID"]?.ToString()?.Trim();
                    if (string.IsNullOrEmpty(itemID) || itemID == "ItemID")
                        continue;

                    try
                    {
                        var config = new ItemDisplayConfig
                        {
                            // 基础字段
                            ItemID = itemID,
                            ItemCategory = row["ItemCategory"]?.ToString() ?? "",
                            DisplayNameKey = row["DisplayNameKey"]?.ToString() ?? "",
                            DescriptionKey = row["DescriptionKey"]?.ToString() ?? "",
                            IconPath = row["IconPath"]?.ToString() ?? "",
                            BigIconPath = row["BigIconPath"]?.ToString() ?? "",
                            ModelPreviewPath = row["ModelPreviewPath"]?.ToString() ?? "",
                            Rarity = row["Rarity"]?.ToString() ?? "",
                            RarityColor = row["RarityColor"]?.ToString() ?? "#FFFFFF",
                            StackDisplay = row["StackDisplayRule"]?.ToString() ?? "Hide",
                            StackTextColor = row["StackTextColor"]?.ToString() ?? "#FFFFFF",
                            BindTagKey = row["BindTagKey"]?.ToString() ?? "",
                            BindTagPosition = row["BindTagPosition"]?.ToString() ?? "TopLeft",
                            SpecialBadgePath = row["SpecialBadgePath"]?.ToString() ?? "",
                            DetailShowFields = row["DetailShowFields"]?.ToString() ?? "",
                            
                            // 数值类型字段（带默认值，防止解析失败）
                            DisplayWeight = int.TryParse(row["DisplayWeight"]?.ToString(), out int weight) ? weight : 0,
                            IsShowInInventory = bool.TryParse(row["IsShowInInventory"]?.ToString(), out bool isShow) ? isShow : true,
                            HoverTipDelay = float.TryParse(row["HoverTipDelay"]?.ToString(), out float tipDelay) ? tipDelay : 0.5f,
                            DisabledGrayScale = float.TryParse(row["DisabledGrayScale"]?.ToString(), out float grayScale) ? grayScale : 0.5f
                        };

                        // 防止重复ItemID
                        if (!configDict.ContainsKey(itemID))
                        {
                            configDict.Add(itemID, config);
                        }
                        else
                        {
                            Debug.LogWarning($"重复的ItemID：{itemID}，已跳过");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"解析行失败（ItemID:{itemID}）：{e.Message}");
                        continue;
                    }
                }

                // 确保输出目录存在
                Directory.CreateDirectory(Path.GetDirectoryName(jsonOutputPath));
                
                // 写入JSON（格式化，UTF8无BOM，方便热更加载）
                var jsonSettings = new JsonSerializerSettings
                {
                    Formatting = Formatting.Indented,
                    NullValueHandling = NullValueHandling.Ignore // 忽略空值字段
                };
                string jsonContent = JsonConvert.SerializeObject(configDict, jsonSettings);
                File.WriteAllText(jsonOutputPath, jsonContent, new UTF8Encoding(false));

                // 刷新Unity资源
                AssetDatabase.Refresh();
                Debug.Log($"配置导出成功！共导出 {configDict.Count} 条配置\n路径：{jsonOutputPath}");
                EditorUtility.DisplayDialog("成功", $"导出{configDict.Count}条背包配置", "确定");
            }
        }
    }

    // 快速打开配置目录菜单
    [MenuItem("策划工具/打开配置目录/Excel目录")]
    public static void OpenExcelDir()
    {
        string excelDir = Path.Combine(Application.dataPath, "Config/Excel");
        Directory.CreateDirectory(excelDir);
        Application.OpenURL($"file:///{excelDir}");
    }

    [MenuItem("策划工具/打开配置目录/JSON输出目录")]
    public static void OpenJsonDir()
    {
        string jsonDir = Path.Combine(Application.dataPath, "Config/HotUpdate");
        Directory.CreateDirectory(jsonDir);
        Application.OpenURL($"file:///{jsonDir}");
    }
}