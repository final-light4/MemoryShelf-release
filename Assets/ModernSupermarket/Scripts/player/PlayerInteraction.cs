using System;
using System.Collections;
using Global.Scripts;
using NPC.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModernSupermarket.Scripts.player
{
    [Serializable]
    public enum PlayerStatus
    {
        Idle,
        OnTask,
    }

    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction Settings")] public float interactDistance = 3f; // 射线检测距离
        public LayerMask interactLayerMask; // 可交互层（可以为空）

        [Header("Item Info UI References")] public GameObject interactionPanel; // UI面板
        public TMP_Text itemNameText;
        public TMP_Text itemDescriptionText;
        public TMP_Text itemPriceText;
        public TMP_Text promotionPriceText;

        private Camera cam;
        private PickableItem currentItem;
        private PickableItem heldItem;
        [Header("Warning Text")]
        [SerializeField] private GameObject warningTextGameObject;
        [SerializeField] private string warningTextContent;
        private TMP_Text warningText;
        [Header("Player Status")]
        [SerializeField] private PlayerStatus player = PlayerStatus.Idle;
        public PlayerStatus Player => player;
        [Header("Input Settings")]
        [SerializeField] private KeyCode pickKey = KeyCode.E;
        [SerializeField] private KeyCode placeKey = KeyCode.G;

        [Header("Event Settings")] 
        [SerializeField] private String_IntSO addToCartEvent;
        [SerializeField]  private String_IntSO removeFromCartEvent;
        
        [SerializeField] private VoidSO taskAcceptEvent;
        [SerializeField] private VoidSO onTaskEndEvent;
        private void OnEnable()
        {
            taskAcceptEvent.action += OnTaskAccept;
            onTaskEndEvent.action += OnTaskEnd;
        }

        private void OnDisable()
        {
            taskAcceptEvent.action -= OnTaskAccept;
            onTaskEndEvent.action -= OnTaskEnd;
        }
        private void OnTaskAccept()
        {
            player = PlayerStatus.OnTask;
        }
        private void OnTaskEnd()
        {
            player = PlayerStatus.Idle;
        }
        private void Awake()
        {
            warningText = warningTextGameObject.GetComponentInChildren<TMP_Text>();
        }
        private void Start()
        {
            cam = Camera.main;
            if (interactionPanel != null)
                interactionPanel.SetActive(false);
        }

        private void Update()
        {
            if (GameManager.Instance.IsGameOver || GameManager.Instance.IsGamePaused) return;
            HandleRaycast();
            HandleInput();
        }

        private void HandleRaycast()
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            RaycastHit hit;

            // 带层过滤的射线检测
            if (Physics.Raycast(ray, out hit, interactDistance, interactLayerMask == 0 ? ~0 : interactLayerMask))
            {
                var pickable = hit.collider.GetComponent<PickableItem>();

                if (pickable != null && pickable.CompareTag("Pickable"))
                {
                    if (currentItem != pickable)
                    {
                        currentItem = pickable;
                        Debug.Log("当前物品: " + currentItem.itemName);
                        ShowUI(currentItem);
                    }

                    else
                    {
                        currentItem = null; // 如果没有命中物体，清空 currentItem
                    }

                    Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.red);
                    if (hit.collider != null)
                        Debug.DrawLine(ray.origin, hit.point, Color.green);

                    return;
                }
            }

            // 未检测到可交互对象
            else
            {
                currentItem = null; // 如果没有命中物体，清空 currentItem
                HideUI();
            }

            HideUI();
        }

        private void HandleInput()
        {
            if (currentItem != null && Input.GetKeyDown(pickKey))
            {
                if (heldItem == null)
                {
                    // 拿取
                    heldItem = currentItem;
                    heldItem.gameObject.SetActive(false); // 隐藏物体
                    Debug.Log($"✅ 拿取：{heldItem.name}");
                    //raise event
                    addToCartEvent.RaiseEvent(heldItem.itemName, 1);
                }
            }
            else if (heldItem != null && Input.GetKeyDown(placeKey))
            {
                // 放置
                heldItem.gameObject.SetActive(true);
                heldItem.transform.position = cam.transform.position + cam.transform.forward * 1f;
                heldItem.transform.rotation = Quaternion.identity;
                Debug.Log($"🔄 放置：{heldItem.name}");
                //raise event
                removeFromCartEvent.RaiseEvent(heldItem.itemName, 1);
                heldItem = null;
            }

            if (currentItem != null)
            {
                ShowUI(currentItem); // 传递当前选中的物体
            }
            else
            {
                Debug.LogWarning("currentItem is null! Make sure raycast hits a PickableItem.");
            }
        }

        #region item info UI
        /// <summary>
        /// show item info in ui panel
        /// </summary>
        /// <param name="item"></param>
        private void ShowUI(PickableItem item)
        {
            if (item == null || itemPriceText == null)
            {
                Debug.LogError("PickableItem or itemPriceText is null! Cannot display UI.");
                return;
            }

            if (interactionPanel == null) return;
            interactionPanel.SetActive(true); // 显示 UI 面板

            itemNameText.text = item.itemName;
            itemDescriptionText.text = item.itemDescription;

            // 判断是否有促销价格
            if (item.isDiscounted)
            {
                // 显示划掉的原价
                itemPriceText.text = $"<s>￥{item.itemPrice:F2}</s>";
                // 显示促销价格
                promotionPriceText.text = $"促销价: ￥{item.promotionPrice:F2}";
            }
            else
            {
                // 显示正常价格
                itemPriceText.text = $"￥{item.itemPrice:F2}";
                promotionPriceText.text = ""; // 清空促销价格
            }
        }
        private void HideUI()
        {
            if (interactionPanel == null) return;
            interactionPanel.SetActive(false);
        }
        #endregion
        #region cannot accept warning
        /// <summary>
        /// tell player is on task, cannot accept task
        /// </summary>
        /// <param name="warning"></param>
        public void ShowWarning()
        {
            StartCoroutine(ShowWarningCoroutine());
        }
        /// <summary>
        /// show warning text for 2 seconds
        /// </summary>
        /// <param name="warning"></param>
        /// <returns></returns>
        IEnumerator ShowWarningCoroutine()
        {
            warningTextGameObject.SetActive(true);
            warningText.text = warningTextContent;
            yield return new WaitForSeconds(2f);
            warningTextGameObject.SetActive(false);
        }
        #endregion
    }

}
