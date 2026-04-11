using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace ThanhND
{
    /// <summary>
    /// Điều phối luồng gameplay: spawn cặp đồ vật, thả vào nồi, ghép cặp, thắng/thua và cập nhật HUD tiến độ.
    /// </summary>
    public class GameplayController : Singleton<GameplayController>
    {
        #region Constants

        private const float ProgressSliderMin = 0f;
        private const float ProgressSliderMax = 1f;
        private const float StackVerticalNudge = 0.3f;
        private const float ObjectAppearDuration = 0.3f;
        private const float StackScrollDuration = 0.5f;
        private const float PairSpawnHeightStepFactor = 0.3f;
        private const int SpawnYieldInterval = 15;
        private const int MaxOrganicSpawnAttempts = 250;
        private const int MaxNearSpawnAttempts = 150;
        private const int OrganicSpawnRelaxThreshold = 100;
        private const float OrganicSpawnRelaxStep = 0.05f;
        private const float OverlapRadiusFactor = 0.82f;

        #endregion

        #region Serialized fields

        [ReadOnly] public GameState gameState = GameState.Loading;

        [Title("Bố cục spawn")]
        [SerializeField] private float startY = -3f;
        [SerializeField] private float paddingX = 0.5f;
        [SerializeField] private float spacing = 1.1f;
        [SerializeField] private LevelDatabase levelDatabase;
        [SerializeField] private GameObject allObjectsParent;
        [SerializeField] private int maxAvailableTypes = 27;

        [Title("Sinh level (procedural)")]
        [Tooltip("Bật để dùng sinh level thay vì LevelDatabase.")]
        [SerializeField] private bool useProceduralLevels;

        [Tooltip("Nếu tắt procedural, nhưng level vượt quá LevelDatabase thì vẫn sinh level runtime.")]
        [SerializeField] private bool proceduralWhenOutOfDatabase = true;

        [Min(2)]
        [SerializeField] private int baseTotalObjects = 30;
        [Min(0)]
        [SerializeField] private int totalObjectsIncreasePerLevel = 2;

        [Min(1)]
        [SerializeField] private int baseTotalTypes = 10;
        [Min(0)]
        [SerializeField] private int totalTypesIncreasePerLevel = 0;

        [Min(0f)]
        [SerializeField] private float baseMaxPairDistance = 4.0f;
        [Min(0f)]
        [SerializeField] private float maxPairDistanceIncreasePerLevel = 0.2f;

        [Title("Luật nồi")]
        [SerializeField] private int maxObjectsInPot = 3;

        [Title("HUD tiến độ level")]
        [Tooltip("Khuyến nghị Min = 0, Max = 1, Interactable = false.")]
        [SerializeField] private Slider levelProgressSlider;

        [Tooltip("Tùy chọn: Image kiểu Filled nếu không dùng Slider.")]
        [SerializeField] private Image levelProgressFill;

        [SerializeField] private Text levelProgressPercentText;
        [SerializeField] private Text levelRemainingItemsText;

        [SerializeField] private string progressPercentFormat = "{0}%";
        [SerializeField] private string remainingItemsFormat = "Còn lại: {0}";

        #endregion

        #region Runtime state

        private readonly SortedSet<ObjectDrop> _objectsOnStack =
            new SortedSet<ObjectDrop>(new ObjectYComparer());

        private readonly List<ObjectDrop> _objectsInPot = new List<ObjectDrop>();
        private bool _progressSliderConfigured;
        private int _totalObjectsAtLevelStart;
        private int _pairsCleared;

        #endregion

        #region Public read-only (HUD / analytics)

        /// <summary>Tiến độ level [0, 1]: đồng bộ với số vật còn lại (0 vật = 100%).</summary>
        public float LevelProgressNormalized { get; private set; }

        /// <summary>Số vật phẩm chưa bị loại; mỗi cặp ghép xong trừ 2.</summary>
        public int RemainingItemsCount =>
            _totalObjectsAtLevelStart > 0
                ? Mathf.Max(0, _totalObjectsAtLevelStart - 2 * _pairsCleared)
                : 0;

        public float screenWidthLimitPosX { get; private set; }

        #endregion

        #region Unity lifecycle

        private void Start()
        {
            Init().Forget();
        }

        #endregion

        #region Init & game flow

        [Button]
        private async UniTaskVoid Init()
        {
            if (levelDatabase == null || levelDatabase.levels.Count == 0)
                return;

            await SpawnObjects();

            if (LoadingPanel.Instance != null)
                LoadingPanel.Instance.ActiveScene(StartGame);
            else
                StartGame();
        }

        private void StartGame()
        {
            gameState = GameState.Playing;

            foreach (ObjectDrop drop in _objectsOnStack)
                drop.transform.DOScale(1f, ObjectAppearDuration);
        }

        private void WinGame()
        {
            WinGamePopUp.Init();
        }

        private void LossGame()
        {
            gameState = GameState.Ended;
            RevivePopUp.Init();
        }

        #endregion

        #region Object interaction (gọi từ ObjectDrop)

        public void OnDropObject(ObjectDrop objectDrop)
        {
            _objectsOnStack.Remove(objectDrop);
            allObjectsParent.transform.DOKill();

            if (_objectsOnStack.Count == 0)
                return;

            float targetY = allObjectsParent.transform.position.y -
                            _objectsOnStack.Min.transform.position.y - StackVerticalNudge;

            allObjectsParent.transform.DOMoveY(targetY, StackScrollDuration);
            objectDrop.transform.SetParent(transform);
        }

        public void OnObjectIsInPot(ObjectDrop objectDrop)
        {
            _objectsInPot.Add(objectDrop);
            EvaluateLossCondition();
        }

        public void CheckObjectsInPot(ObjectDrop a, ObjectDrop b)
        {
            if (a == null || b == null || !a.gameObject.activeInHierarchy || !b.gameObject.activeInHierarchy)
                return;

            if (!_objectsInPot.Contains(a) || !_objectsInPot.Contains(b))
                return;

            _pairsCleared++;
            RefreshProgressHud();

            a.gameObject.SetActive(false);
            b.gameObject.SetActive(false);
            _objectsInPot.Remove(a);
            _objectsInPot.Remove(b);

            if (_objectsOnStack.Count == 0 && _objectsInPot.Count == 0)
                WinGame();
        }

        public bool CanShuffleObjectsOnStack()
        {
            // Chỉ cho phép shuffle khi đang chơi và còn ít nhất 2 object trên stack.
            return gameState == GameState.Playing && _objectsOnStack.Count > 1;
        }

        public bool ShuffleObjectsOnStack()
        {
            if (!CanShuffleObjectsOnStack())
                return false;

            var activeDrops = new List<ObjectDrop>(_objectsOnStack.Count);
            foreach (ObjectDrop drop in _objectsOnStack)
            {
                if (drop != null && drop.gameObject.activeInHierarchy)
                    activeDrops.Add(drop);
            }

            if (activeDrops.Count <= 1)
                return false;

            // Shuffle vị trí giữa các object còn active trên stack, không đổi id/type.
            var positions = new List<Vector3>(activeDrops.Count);
            for (int i = 0; i < activeDrops.Count; i++)
                positions.Add(activeDrops[i].transform.position);

            ShuffleList(positions);

            _objectsOnStack.Clear();
            for (int i = 0; i < activeDrops.Count; i++)
            {
                activeDrops[i].transform.position = positions[i];
                _objectsOnStack.Add(activeDrops[i]);
            }

            return true;
        }

        #endregion

        #region Win / loss rules

        public void IncreaseMaxObjectsInPot(int amount)
        {
            if (amount <= 0)
                return;

            maxObjectsInPot += amount;
        }

        private void EvaluateLossCondition()
        {
            if (_objectsInPot.Count <= maxObjectsInPot)
                return;

            if (!PotContainsAdjacentMatch())
                LossGame();
        }

        /// <summary>True nếu trong nồi có hai vật liền kề cùng loại (còn cơ hội ghép).</summary>
        private bool PotContainsAdjacentMatch()
        {
            for (int i = 1; i < _objectsInPot.Count; i++)
            {
                if (_objectsInPot[i].id == _objectsInPot[i - 1].id)
                    return true;
            }

            return false;
        }

        #endregion

        #region HUD — tiến độ level

        private void BeginLevelProgress(int spawnedObjectCount)
        {
            _totalObjectsAtLevelStart = spawnedObjectCount;
            _pairsCleared = 0;
            RefreshProgressHud();
        }

        private void RefreshProgressHud()
        {
            // Một nguồn sự thật: % = phần đã loại khỏi level; còn 0 vật => 100%.
            if (_totalObjectsAtLevelStart <= 0)
                LevelProgressNormalized = 1f;
            else
            {
                float cleared = _totalObjectsAtLevelStart - RemainingItemsCount;
                LevelProgressNormalized = Mathf.Clamp01(cleared / _totalObjectsAtLevelStart);
            }

            EnsureProgressSliderRange();

            if (levelProgressSlider != null)
                levelProgressSlider.SetValueWithoutNotify(LevelProgressNormalized);

            if (levelProgressFill != null)
                levelProgressFill.fillAmount = LevelProgressNormalized;

            if (levelProgressPercentText != null)
            {
                int percent = Mathf.RoundToInt(LevelProgressNormalized * 100f);
                levelProgressPercentText.text = string.Format(progressPercentFormat, percent);
            }

            if (levelRemainingItemsText != null)
                levelRemainingItemsText.text = string.Format(remainingItemsFormat, RemainingItemsCount);
        }

        private void EnsureProgressSliderRange()
        {
            if (levelProgressSlider == null || _progressSliderConfigured)
                return;

            levelProgressSlider.minValue = ProgressSliderMin;
            levelProgressSlider.maxValue = ProgressSliderMax;
            _progressSliderConfigured = true;
        }

        #endregion

        #region Spawning

        public async UniTask SpawnObjects()
        {
            float halfPlayWidth = ComputeHalfPlayWidth();
            LevelData level = GetLevelDataForCurrentProfile();

            foreach (ObjectDrop drop in FindObjectsByType<ObjectDrop>(FindObjectsSortMode.None))
            {
                if (drop != null)
                    Destroy(drop.gameObject);
            }

            _objectsInPot.Clear();

            List<string> spawnQueue = BuildSpawnQueue(level);
            BeginLevelProgress(spawnQueue.Count);

            var spawnedPositions = new List<Vector3>();
            _objectsOnStack.Clear();

            float rowMaxY = startY + spacing;
            float rowStepY = spacing * PairSpawnHeightStepFactor;

            for (int i = 0; i < spawnQueue.Count; i += 2)
            {
                string address = spawnQueue[i];

                Vector2 first = GetRandomOrganicPosition(startY, rowMaxY, halfPlayWidth, spawnedPositions);
                spawnedPositions.Add(first);
                await SpawnOne(address, first);

                Vector2 second = GetNearPosition(first, startY, rowMaxY, halfPlayWidth, spawnedPositions, level);
                spawnedPositions.Add(second);
                await SpawnOne(address, second);

                rowMaxY += rowStepY;

                if (i % SpawnYieldInterval == 0)
                    await UniTask.Yield();
            }
        }

        private LevelData GetLevelDataForCurrentProfile()
        {
            int levelNumber = Mathf.Max(1, UseProfile.CurrentLevel);

            if (useProceduralLevels)
                return GenerateLevelData(levelNumber);

            if (levelDatabase != null && levelDatabase.levels != null && levelDatabase.levels.Count > 0)
            {
                if (levelNumber - 1 < levelDatabase.levels.Count)
                    return levelDatabase.levels[levelNumber - 1];

                if (proceduralWhenOutOfDatabase)
                    return GenerateLevelData(levelNumber);

                return levelDatabase.levels[levelDatabase.levels.Count - 1];
            }

            return GenerateLevelData(levelNumber);
        }

        private LevelData GenerateLevelData(int levelNumber)
        {
            // levelNumber bắt đầu từ 1.
            int step = Mathf.Max(0, levelNumber - 1);

            int totalObjects = baseTotalObjects + step * totalObjectsIncreasePerLevel;
            if (totalObjects < 2) totalObjects = 2;
            if ((totalObjects & 1) == 1) totalObjects += 1; // đảm bảo chẵn để luôn spawn theo cặp

            int totalTypes = baseTotalTypes + step * totalTypesIncreasePerLevel;
            totalTypes = Mathf.Clamp(totalTypes, 1, maxAvailableTypes);

            // Invariant của BuildSpawnQueue:
            // - luôn spawn 2 object mỗi type trước => cần totalObjects >= 2 * totalTypes.
            // - cũng không thể có types > totalObjects/2.
            totalTypes = Mathf.Min(totalTypes, totalObjects / 2);
            totalObjects = Mathf.Max(totalObjects, totalTypes * 2);

            float maxPairDistance = baseMaxPairDistance + step * maxPairDistanceIncreasePerLevel;
            if (maxPairDistance < 0f) maxPairDistance = 0f;

            return new LevelData
            {
                id = levelNumber,
                totalObjects = totalObjects,
                totalTypes = totalTypes,
                maxPairDistance = maxPairDistance
            };
        }

        private float ComputeHalfPlayWidth()
        {
            Camera cam = Camera.main;
            float halfHeight = cam.orthographicSize;
            screenWidthLimitPosX = halfHeight * cam.aspect;
            float usableHalfWidth = screenWidthLimitPosX - paddingX;
            screenWidthLimitPosX -= spacing * 0.5f;
            return usableHalfWidth;
        }

        /// <summary>
        /// Danh sách địa chỉ Addressables: mỗi cặp liên tiếp cùng prefab (spawn 2 lần cùng id).
        /// </summary>
        private List<string> BuildSpawnQueue(LevelData levelData)
        {
            var queue = new List<string>();
            var typeIds = new List<int>(maxAvailableTypes);
            for (int i = 1; i <= maxAvailableTypes; i++)
                typeIds.Add(i);

            Shuffle(typeIds);

            var selectedTypes = new List<int>(levelData.totalTypes);
            for (int i = 0; i < levelData.totalTypes; i++)
                selectedTypes.Add(typeIds[i]);

            foreach (int typeId in selectedTypes)
            {
                string path = $"Objects/Object_{typeId}.prefab";
                queue.Add(path);
                queue.Add(path);
            }

            int extraPairs = (levelData.totalObjects - queue.Count) / 2;
            for (int p = 0; p < extraPairs; p++)
            {
                int id = selectedTypes[Random.Range(0, selectedTypes.Count)];
                string path = $"Objects/Object_{id}.prefab";
                queue.Add(path);
                queue.Add(path);
            }

            return queue;
        }

        private static void Shuffle(IList<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = Random.Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private static void ShuffleList<T>(IList<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int j = Random.Range(i, list.Count);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }

        private async UniTask SpawnOne(string address, Vector2 position)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(address);
            await handle;

            if (handle.Result == null)
                return;

            GameObject instance = Instantiate(
                handle.Result,
                position,
                Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));

            instance.transform.SetParent(allObjectsParent.transform);
            instance.transform.localScale = Vector3.zero;
            _objectsOnStack.Add(instance.GetComponent<ObjectDrop>());
        }

        private Vector2 GetRandomOrganicPosition(
            float minY, float maxY, float halfWidth, List<Vector3> occupied)
        {
            float radius = spacing * OverlapRadiusFactor;
            float relaxMaxY = maxY;

            for (int attempt = 0; attempt < MaxOrganicSpawnAttempts; attempt++)
            {
                var candidate = new Vector2(
                    Random.Range(-halfWidth, halfWidth),
                    Random.Range(minY, relaxMaxY));

                if (!OverlapsAny(candidate, occupied, radius))
                    return candidate;

                if (attempt >= OrganicSpawnRelaxThreshold)
                    relaxMaxY += OrganicSpawnRelaxStep;
            }

            return new Vector2(Random.Range(-halfWidth, halfWidth), relaxMaxY);
        }

        private Vector2 GetNearPosition(
            Vector2 origin,
            float minY,
            float maxY,
            float halfWidth,
            List<Vector3> occupied,
            LevelData level)
        {
            float radius = spacing * OverlapRadiusFactor;
            float maxDist = level.maxPairDistance;

            for (int attempt = 0; attempt < MaxNearSpawnAttempts; attempt++)
            {
                Vector2 candidate = origin + Random.insideUnitCircle * maxDist;
                candidate.x = Mathf.Clamp(candidate.x, -halfWidth, halfWidth);
                candidate.y = Mathf.Clamp(candidate.y, minY, maxY + spacing);

                if (!OverlapsAny(candidate, occupied, radius))
                    return candidate;
            }

            return GetRandomOrganicPosition(minY, maxY, halfWidth, occupied);
        }

        private static bool OverlapsAny(Vector2 point, List<Vector3> occupied, float minDistance)
        {
            for (int i = 0; i < occupied.Count; i++)
            {
                if (Vector2.Distance(point, occupied[i]) < minDistance)
                    return true;
            }

            return false;
        }

        #endregion
    }

    /// <summary>Sắp xếp ObjectDrop theo trục Y (và InstanceID khi trùng Y) cho tập hợp stack.</summary>
    public sealed class ObjectYComparer : IComparer<ObjectDrop>
    {
        public int Compare(ObjectDrop x, ObjectDrop y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            int byY = x.transform.position.y.CompareTo(y.transform.position.y);
            return byY != 0 ? byY : x.GetInstanceID().CompareTo(y.GetInstanceID());
        }
    }

    public enum GameState
    {
        Loading,
        Playing,
        Paused,
        Ended
    }
}
