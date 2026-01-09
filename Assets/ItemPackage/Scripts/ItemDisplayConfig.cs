using System;
using UnityEngine;

namespace ItemPackage.Scripts
{
    [Serializable]
    public class ItemDisplayConfig
    {
        public string ItemID;                  // 物品ID
        public string ItemCategory;            // 物品分类
        public string DisplayNameKey;          // 显示名称Key
        public string DescriptionKey;          // 描述Key
        public string IconPath;                // 图标Addressables资源名（不再是路径）
        public string BigIconPath;             // 大图标Addressables资源名
        public string ModelPreviewPath;        // 模型预览Addressables资源名
        public string Rarity;                  // 物品品质
        public string RarityColor;             // 品质颜色（十六进制）
        public string StackDisplay;        // 堆叠显示规则
        public string StackTextColor;          // 堆叠数字颜色
        public string BindTagKey;              // 绑定标识Key
        public string BindTagPosition;         // 绑定标识位置
        public string SpecialBadgePath;        // 特殊角标Addressables资源名
        public int DisplayWeight;              // 显示权重
        public bool IsShowInInventory;         // 是否显示在背包
        public string DetailShowFields;        // 详情页显示字段
        public float HoverTipDelay;            // 悬浮提示延迟
        public float DisabledGrayScale;        // 禁用灰度值

        // 辅助方法：解析品质颜色为Color对象（增加容错）
        public Color GetRarityColor()
        {
            if (string.IsNullOrEmpty(RarityColor) || !ColorUtility.TryParseHtmlString(RarityColor, out Color color))
            {
                return Color.white; // 解析失败返回默认白色
            }
            return color;
        }

        // 辅助方法：解析堆叠显示规则（增加容错）
        public StackDisplayRule GetStackDisplayRule()
        {
            if (string.IsNullOrEmpty(StackDisplay) || 
                !Enum.TryParse(StackDisplay, out StackDisplayRule rule))
            {
                return StackDisplayRule.Hide; // 解析失败默认隐藏
            }
            return rule;
        }

        // 新增：解析绑定位置（增加容错）
        public BindTagPositionRule GetBindTagPosition()
        {
            if (string.IsNullOrEmpty(BindTagPosition) || 
                !Enum.TryParse(BindTagPosition, out BindTagPositionRule pos))
            {
                return BindTagPositionRule.TopLeft; // 解析失败默认左上角
            }
            return pos;
        }

        // 新增：解析堆叠文本颜色
        public Color GetStackTextColor()
        {
            if (string.IsNullOrEmpty(StackTextColor) || !ColorUtility.TryParseHtmlString(StackTextColor, out Color color))
            {
                return Color.white;
            }
            return color;
        }
    }

    public enum StackDisplayRule
    {
        Hide,          // 隐藏
        TopRight,      // 右上角
        BottomRight    // 右下角
    }

    public enum BindTagPositionRule
    {
        TopLeft,       // 左上角
        BottomLeft,    // 左下角
        TopRight,      // 右上角
        BottomRight    // 右下角
    }
}