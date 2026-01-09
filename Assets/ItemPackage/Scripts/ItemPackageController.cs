using UnityEngine;

namespace ItemPackage.Scripts
{
    public class ItemPackageController : MonoBehaviour
    {
        private void Start()
        {
            // 监听配置加载完成后再渲染UI
            if (ItemPackageData.Instance.IsConfigLoaded)
            {
                RenderInventory();
            }
            else
            {
                ItemPackageData.Instance.OnConfigLoaded += RenderInventory;
            }
        }

        // 渲染背包UI
        private void RenderInventory()
        {
            // 遍历m_playerItems，为每个物品格子赋值
            foreach (var kvp in ItemPackageData.Instance.m_playerItems)
            {
                string itemID = kvp.Key;
                int count = kvp.Value;
                ItemDisplayConfig config = ItemPackageData.Instance.GetItemConfig(itemID);
            
                if (config != null)
                {
                    ItemPackageUI.Instance.CreateItemSlot(config, count);
                }
            }
        }

        private void OnDestroy()
        {
            // 取消回调监听，避免内存泄漏
            if (ItemPackageData.Instance != null)
            {
                ItemPackageData.Instance.OnConfigLoaded -= RenderInventory;
            }
        }
    }
}