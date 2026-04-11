using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThanhND
{
    /// <summary>
    /// Box gần nồi: chỉ box “đang mở” (trên cùng theo trục Y) mới gọi rewarded ad; sau reward box ẩn và tăng max đồ trong nồi.
    /// Click: OverlapPoint + ScreenToWorld — không dùng IsPointerOverGameObject() trước khi test collider (tránh Canvas phủ màn chặn hết).
    /// Ads: cùng pattern <see cref="ShuffleRewardButton"/>.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class BoxUnlockController : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private SpriteRenderer boxRenderer;
        [SerializeField] private SpriteRenderer rewardIconRenderer;
        [SerializeField] private Sprite rewardIconSprite;
        [SerializeField] private Vector3 rewardIconLocalPosition = Vector3.zero;
        [SerializeField] private Vector3 rewardIconLocalScale = new Vector3(1.5f, 1.5f, 1.5f);
        [SerializeField] private Color lockedColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        [SerializeField] private Color normalColor = Color.white;

        [Header("Reward ads")]
        [SerializeField] private string rewardPlacement = "box_unlock";
        [SerializeField] private string noVideoMessage = "No video available";
        [SerializeField] private int potCapacityIncrease = 1;

        private static readonly List<BoxUnlockController> Instances = new List<BoxUnlockController>(0);

        private BoxCollider2D _boxCollider;
        private bool _consumed;
        private bool _isRequestingAd;

        private void Reset()
        {
            boxRenderer = GetComponent<SpriteRenderer>();
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            if (boxRenderer == null)
                boxRenderer = GetComponent<SpriteRenderer>();

            if (rewardIconRenderer == null)
                rewardIconRenderer = CreateAdsIconIfNeeded();

            if (rewardIconRenderer != null)
            {
                rewardIconRenderer.transform.SetParent(transform);
                rewardIconRenderer.transform.localPosition = rewardIconLocalPosition;
                rewardIconRenderer.transform.localScale = rewardIconLocalScale;
            }
        }

        private void OnEnable()
        {
            _consumed = false;
            if (!Instances.Contains(this))
                Instances.Add(this);
        }

        private void OnDisable()
        {
            Instances.Remove(this);
        }

        private void Start()
        {
            RefreshVisuals();
        }

        private void Update()
        {
            if (!TryGetPrimaryPointerDown(out Vector2 screenPos))
                return;

            if (_boxCollider == null || !_boxCollider.enabled || !_boxCollider.gameObject.activeInHierarchy)
                return;

            if (!TryGetPointerWorld2D(screenPos, out Vector2 world))
                return;

            if (!_boxCollider.OverlapPoint(world))
                return;

            HandlePointerDown();
        }

        private static bool TryGetPrimaryPointerDown(out Vector2 screenPos)
        {
            screenPos = default;
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase != TouchPhase.Began)
                    return false;
                screenPos = t.position;
                return true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                screenPos = Input.mousePosition;
                return true;
            }

            return false;
        }

        private bool TryGetPointerWorld2D(Vector2 screenPos, out Vector2 world)
        {
            world = default;
            Camera cam = Camera.main;
            if (cam == null)
                cam = FindFirstObjectByType<Camera>();
            if (cam == null)
                return false;

            float zPlane = transform.position.z;
            float camZ = cam.transform.position.z;
            Vector3 screen = new Vector3(screenPos.x, screenPos.y, Mathf.Abs(camZ - zPlane));
            Vector3 w = cam.ScreenToWorldPoint(screen);
            world = new Vector2(w.x, w.y);
            return true;
        }

        private void HandlePointerDown()
        {
            if (_consumed || _isRequestingAd)
                return;

            if (GetCurrentUnlockable() != this)
                return;

            if (GameController.Instance == null || GameController.Instance.admobAds == null)
                return;

            _isRequestingAd = true;
            bool started = GameController.Instance.admobAds.ShowVideoReward(
                actionReward: OnReward,
                actionNotLoadedVideo: OnVideoNotLoaded,
                actionClose: OnAdClosed,
                actionType: rewardPlacement);

            if (!started)
                _isRequestingAd = false;
        }

        private void OnReward()
        {
            UnlockCurrentBox();
        }

        private void OnVideoNotLoaded()
        {
            ShowOverlayMessage(noVideoMessage);
            _isRequestingAd = false;
        }

        private void OnAdClosed()
        {
            _isRequestingAd = false;
        }

        private void ShowOverlayMessage(string message)
        {
            if (GameController.Instance == null || GameController.Instance.moneyEffectController == null)
                return;

            GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp_Overlay(
                transform.position,
                message,
                Color.red);
        }

        private void UnlockCurrentBox()
        {
            if (_consumed)
                return;

            _consumed = true;
            gameObject.SetActive(false);

            if (GameplayController.Instance != null)
                GameplayController.Instance.IncreaseMaxObjectsInPot(potCapacityIncrease);

            RefreshVisuals();
        }

        private static void RefreshVisuals()
        {
            RebuildInstancesFromScene();
            BoxUnlockController current = GetCurrentUnlockable();

            foreach (BoxUnlockController box in Instances)
            {
                if (box == null || !box.gameObject.activeSelf)
                    continue;

                box.ApplyVisualState(box == current);
            }
        }

        private void ApplyVisualState(bool isActiveUnlock)
        {
            if (boxRenderer != null)
                boxRenderer.color = isActiveUnlock ? lockedColor : normalColor;

            if (rewardIconRenderer != null)
                rewardIconRenderer.gameObject.SetActive(isActiveUnlock);
        }

        private static BoxUnlockController GetCurrentUnlockable()
        {
            SortInstancesTopToBottom();

            foreach (BoxUnlockController box in Instances)
            {
                if (box != null && box.gameObject.activeSelf && !box._consumed)
                    return box;
            }

            return null;
        }

        private static void SortInstancesTopToBottom()
        {
            Instances.Sort((a, b) => b.transform.position.y.CompareTo(a.transform.position.y));
        }

        private static void RebuildInstancesFromScene()
        {
            Instances.Clear();
            var all = FindObjectsByType<BoxUnlockController>(FindObjectsSortMode.None);
            for (int i = 0; i < all.Length; i++)
            {
                BoxUnlockController box = all[i];
                if (box != null)
                    Instances.Add(box);
            }
        }

        private SpriteRenderer CreateAdsIconIfNeeded()
        {
            if (rewardIconSprite == null)
                return null;

            var go = new GameObject("AdsIcon");
            go.transform.SetParent(transform);
            go.transform.localPosition = rewardIconLocalPosition;
            go.transform.localScale = rewardIconLocalScale;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = rewardIconSprite;
            sr.color = Color.white;
            sr.sortingOrder = boxRenderer != null ? boxRenderer.sortingOrder + 1 : 2;
            return sr;
        }

        /// <summary>
        /// Gắn script lên object tên BoxUnlock* thiếu component (scene cũ / copy tay). Prefab chuẩn nên gán sẵn trên prefab.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureControllersOnSceneBoxes()
        {
            Instances.Clear();

            var sprites = FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < sprites.Length; i++)
            {
                SpriteRenderer sr = sprites[i];
                if (sr == null)
                    continue;
                GameObject obj = sr.gameObject;
                if (!obj.name.StartsWith("BoxUnlock", StringComparison.Ordinal))
                    continue;
                if (obj.GetComponent<BoxCollider2D>() == null)
                    continue;
                if (obj.GetComponent<BoxUnlockController>() == null)
                    obj.AddComponent<BoxUnlockController>();
            }

            RebuildInstancesFromScene();
        }
    }
}
