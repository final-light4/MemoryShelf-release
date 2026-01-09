using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace ItemPackage.Scripts
{
    public class ItemPackageUI:MonoBehaviour
    {
        public static ItemPackageUI Instance { get; private set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        public Image iconImage;
        private AsyncOperationHandle<Sprite> _iconHandle; // 缓存句柄用于释放

        // 加载并显示物品图标
        public void SetItemIcon(string iconAssetName)
        {
            // 先释放旧资源
            if (_iconHandle.IsValid())
            {
                ItemPackageData.Instance.ReleaseAsset(_iconHandle);
            }

            if (string.IsNullOrEmpty(iconAssetName))
            {
                iconImage.sprite = null;
                return;
            }

            // 异步加载图标
            _iconHandle = ItemPackageData.Instance.LoadItemIconAsync(iconAssetName);
            _iconHandle.Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    iconImage.sprite = handle.Result;
                }
                else
                {
                    Debug.LogWarning($"加载图标失败：{iconAssetName}");
                    iconImage.sprite = null; // 可替换为默认占位图
                }
            };
        }
        public void CreateItemSlot(ItemDisplayConfig config, int count)
        {
            // TODO: 为物品格子设置数据（包括异步加载图标）
            SetItemIcon(config.IconPath);
        }

        // 销毁时释放资源
        private void OnDestroy()
        {
            if (_iconHandle.IsValid())
            {
                ItemPackageData.Instance.ReleaseAsset(_iconHandle);
            }
        }
    }
}