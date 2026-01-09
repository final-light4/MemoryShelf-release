using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ItemPackage.Scripts
{
    public class ItemPackageData : MonoBehaviour
    {
        public static ItemPackageData Instance { get; private set; }
        
        // 物品配置字典（从JSON加载）
        private Dictionary<string, ItemDisplayConfig> m_itemConfigDict;
        // 玩家背包物品（ItemID + 数量）
        public Dictionary<string, int> m_playerItems = new Dictionary<string, int>();

        // 新增：配置加载完成回调（供UI层监听）
        public System.Action OnConfigLoaded;
        // 新增：标记配置是否加载完成
        public bool IsConfigLoaded { get; private set; } = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitTestData(); // 初始化测试数据
                LoadItemConfigFromAddressables(); // 替换原本地文件加载
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // 改造：从Addressables加载JSON配置（核心改动）
        private void LoadItemConfigFromAddressables()
        {
            // 注意：需在Addressables中把item_display.json标记为TextAsset，资源名保持"item_display"
            Addressables.LoadAssetAsync<TextAsset>("item_display")
                .Completed += OnConfigLoadCompleted;
        }

        // 配置加载完成回调
        private void OnConfigLoadCompleted(AsyncOperationHandle<TextAsset> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                try
                {
                    string jsonContent = handle.Result.text;
                    m_itemConfigDict = JsonConvert.DeserializeObject<Dictionary<string, ItemDisplayConfig>>(jsonContent);
                    IsConfigLoaded = true;
                    Debug.Log($"[ItemPackage] 加载{m_itemConfigDict.Count}条物品配置（Addressables）");
                    
                    // 通知UI层配置加载完成
                    OnConfigLoaded?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError($"[ItemPackage] JSON配置解析失败：{e.Message}");
                    m_itemConfigDict = new Dictionary<string, ItemDisplayConfig>();
                }
            }
            else
            {
                Debug.LogError($"[ItemPackage] 配置加载失败：{handle.OperationException?.Message ?? "未知错误"}");
                m_itemConfigDict = new Dictionary<string, ItemDisplayConfig>();
            }

            // 释放资源句柄（避免内存泄漏）
            Addressables.Release(handle);
        }

        // 保留：获取物品配置（增加空值判断）
        public ItemDisplayConfig GetItemConfig(string itemID)
        {
            if (!IsConfigLoaded || string.IsNullOrEmpty(itemID) || m_itemConfigDict == null)
            {
                Debug.LogWarning($"[ItemPackage] 配置未加载或ItemID无效：{itemID}");
                return null;
            }
            
            m_itemConfigDict.TryGetValue(itemID, out var config);
            return config;
        }

        // 新增：异步加载物品图标（供UI层调用）
        public AsyncOperationHandle<Sprite> LoadItemIconAsync(string iconAssetName)
        {
            if (string.IsNullOrEmpty(iconAssetName))
            {
                Debug.LogWarning("[ItemPackage] 图标资源名为空");
                return default;
            }
            
            return Addressables.LoadAssetAsync<Sprite>(iconAssetName);
        }

        // 新增：异步加载大图标
        public AsyncOperationHandle<Sprite> LoadItemBigIconAsync(string bigIconAssetName)
        {
            if (string.IsNullOrEmpty(bigIconAssetName))
            {
                Debug.LogWarning("[ItemPackage] 大图标资源名为空");
                return default;
            }
            
            return Addressables.LoadAssetAsync<Sprite>(bigIconAssetName);
        }

        // 新增：异步加载特殊角标
        public AsyncOperationHandle<Sprite> LoadSpecialBadgeAsync(string badgeAssetName)
        {
            if (string.IsNullOrEmpty(badgeAssetName))
            {
                Debug.LogWarning("[ItemPackage] 角标资源名为空");
                return default;
            }
            
            return Addressables.LoadAssetAsync<Sprite>(badgeAssetName);
        }

        // 新增：释放Addressables资源句柄（供UI层调用）
        public void ReleaseAsset(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        // 保留：初始化测试背包数据
        private void InitTestData()
        {
            m_playerItems.Clear();
            m_playerItems.Add("item_sword_001", 1);
            m_playerItems.Add("item_potion_001", 99);
            m_playerItems.Add("item_shield_001", 1);
        }

        // 新增：防止内存泄漏，清空回调
        private void OnDestroy()
        {
            OnConfigLoaded = null;
        }

        // 兼容：保留原本地加载方法（可选，用于编辑器调试）
        [ContextMenu("编辑器调试-本地加载配置")]
        private void LoadItemConfigFromLocal()
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, "Config/HotUpdate/item_display.json");
            if (File.Exists(jsonPath))
            {
                string jsonContent = File.ReadAllText(jsonPath, new UTF8Encoding(false));
                m_itemConfigDict = JsonConvert.DeserializeObject<Dictionary<string, ItemDisplayConfig>>(jsonContent);
                IsConfigLoaded = true;
                OnConfigLoaded?.Invoke();
                Debug.Log($"[ItemPackage] 本地加载{m_itemConfigDict.Count}条物品配置");
            }
            else
            {
                Debug.LogError($"[ItemPackage] 本地配置文件不存在：{jsonPath}");
            }
        }
    }
}