using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ThanhND
{
    public class GameplayController : Singleton<GameplayController>
    {
        [ReadOnly] public GameState gameState = GameState.Loading;

        [Title("Cấu hình vị trí")] [SerializeField]
        private float startY = -3f;

        [SerializeField] private float paddingX = 0.5f;
        [SerializeField] private float spacing = 1.1f;

        [SerializeField] private LevelDatabase levelDatabase;
        private SortedSet<ObjectDrop> totalObjectDrops = new SortedSet<ObjectDrop>(new ObjectYComparer());
        private List<ObjectDrop> objectDropped = new List<ObjectDrop>();
        private int maxObjectsInPot = 3;

        [SerializeField] GameObject allObjectsParent;
        [SerializeField] int maxAvailableTypes = 27; // Thay bằng con số thực tế bạn có
        public float screenWidthLimitPosX { get; private set; }

        private void Start()
        {
            Init().Forget();
        }

        private float CalculateScreenWidth()
        {
            Camera cam = Camera.main;
            float height = 2f * cam.orthographicSize;
            screenWidthLimitPosX = height * cam.aspect / 2f;
            Debug.LogError(screenWidthLimitPosX);

            var screenWidthLimit = screenWidthLimitPosX - paddingX;
            screenWidthLimitPosX -= spacing / 2;
            return screenWidthLimit;
        }

        [Button]
        private async UniTaskVoid Init()
        {
            if (levelDatabase != null && levelDatabase.levels.Count > 0)
            {
                await SpawnObjects();
                if (LoadingPanel.Instance)
                {
                    LoadingPanel.Instance.ActiveScene(StartGame);
                }
                else
                {
                    StartGame();
                }
            }
        }

        private void StartGame()
        {
            gameState = GameState.Playing;

            foreach (var objectDrop in totalObjectDrops)
            {
                objectDrop.transform.DOScale(1, 0.3f);
            }
        }

        public void OnDropObject(ObjectDrop objectDrop)
        {
            totalObjectDrops.Remove(objectDrop);
            allObjectsParent.transform.DOKill();
            if (totalObjectDrops.Count == 0) return;
            float targetY = allObjectsParent.transform.position.y - totalObjectDrops.Min.transform.position.y - 0.3f;
            allObjectsParent.transform.DOMoveY(targetY, 0.5f);
            objectDrop.transform.SetParent(this.transform);
        }

        public void OnObjectIsInPot(ObjectDrop objectDrop)
        {
            Debug.LogError("zo pot " + objectDrop.id);
            objectDropped.Add(objectDrop);
            
            CheckLossGame();
        }

        public void CheckObjectsInPot(ObjectDrop objectDrop, ObjectDrop otherObjectDrop)
        {
            Debug.LogError("Xoa 2 Object :" + objectDrop.id);

            objectDrop.gameObject.SetActive(false);
            otherObjectDrop.gameObject.SetActive(false);
            objectDropped.Remove(objectDrop);
            objectDropped.Remove(otherObjectDrop);
            if (totalObjectDrops.Count == 0 && objectDropped.Count == 0)
            {
                WinGame();
            }
        }

        private void CheckLossGame()
        {
            if (objectDropped.Count > maxObjectsInPot)
            {
                bool lossGame = true;
                for (int i = 1; i < objectDropped.Count; i++)
                {
                    if (objectDropped[i].id == objectDropped[i - 1].id)
                    {
                        lossGame = false;
                        break;
                    }
                }
                
                if (lossGame) LossGame();
            }
        }

        private void LossGame()
        {
            gameState = GameState.Ended;
            RevivePopUp.Init();
        }

        private void WinGame()
        {
            WinGamePopUp.Init();
        }

        #region SpawnObjects

        public async UniTask SpawnObjects()
        {
            float screenWidthLimit = CalculateScreenWidth();
            LevelData levelData = levelDatabase.levels[UseProfile.CurrentLevel - 1];

            foreach (var objectDrop in FindObjectsOfType<ObjectDrop>())
            {
                if (objectDrop != null) Destroy(objectDrop.gameObject);
            }

            objectDropped.Clear();

            List<string> objectIDsToSpawn = PrepareObjectList(levelData);

            List<Vector3> spawnedPositions = new List<Vector3>();
            totalObjectDrops.Clear();

            float currentMaxY = startY + spacing * 2f;
            float stepY = spacing * 0.5f;

            for (int i = 0; i < objectIDsToSpawn.Count; i += 2)
            {
                string id = objectIDsToSpawn[i];

                Vector2 pos1 = GetRandomOrganicPosition(startY, currentMaxY, screenWidthLimit, spawnedPositions);
                spawnedPositions.Add(pos1);
                await CreateObject(id, pos1);

                Vector2 pos2 = GetNearPosition(pos1, startY, currentMaxY, screenWidthLimit, spawnedPositions);
                spawnedPositions.Add(pos2);
                await CreateObject(id, pos2);

                currentMaxY += stepY;
            }
        }

        private List<string> PrepareObjectList(LevelData levelData)

        {
            List<string> list = new List<string>();


            List<int> selectedTypes = new List<int>();

            List<int> allAvailableIds = new List<int>();


            for (int i = 1; i <= maxAvailableTypes; i++) allAvailableIds.Add(i);


            for (int i = 0; i < allAvailableIds.Count; i++)

            {
                int temp = allAvailableIds[i];

                int randomIndex = Random.Range(i, allAvailableIds.Count);

                allAvailableIds[i] = allAvailableIds[randomIndex];

                allAvailableIds[randomIndex] = temp;
            }


            for (int i = 0; i < levelData.totalTypes; i++)

            {
                selectedTypes.Add(allAvailableIds[i]);
            }


            foreach (int typeId in selectedTypes)

            {
                string path = $"Objects/Object_{typeId}.prefab";

                list.Add(path);

                list.Add(path);
            }


            int remainingPairs = (levelData.totalObjects - list.Count) / 2;

            for (int i = 0; i < remainingPairs; i++)

            {
                int randomIdFromSelected = selectedTypes[Random.Range(0, selectedTypes.Count)];

                string path = $"Objects/Object_{randomIdFromSelected}.prefab";

                list.Add(path);

                list.Add(path);
            }


            return list;
        }

        private async UniTask CreateObject(string id, Vector2 pos)
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(id);
            await handle;
            if (handle.Result == null) return;

            GameObject go = Instantiate(handle.Result, pos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
            go.transform.SetParent(allObjectsParent.transform);
            go.transform.localScale = Vector3.zero;
            totalObjectDrops.Add(go.GetComponent<ObjectDrop>());
        }

        private Vector2 GetNearPosition(Vector2 center, float minY, float maxY, float screenWidthLimit,
            List<Vector3> spawnedPositions)
        {
            int attempts = 0;
            float checkRadius = spacing * 0.85f;

            while (attempts < 150)
            {
                attempts++;
                Vector2 randomDir = Random.insideUnitCircle * levelDatabase.levels[UseProfile.CurrentLevel - 1].maxPairDistance;
                Vector2 candidate = center + randomDir;

                candidate.x = Mathf.Clamp(candidate.x, -screenWidthLimit, screenWidthLimit);
                candidate.y = Mathf.Clamp(candidate.y, minY, maxY);

                bool isOverlapping = false;
                foreach (var p in spawnedPositions)
                {
                    if (Vector2.Distance(candidate, p) < checkRadius)
                    {
                        isOverlapping = true;
                        break;
                    }
                }

                if (!isOverlapping) return candidate;
            }

            return GetRandomOrganicPosition(minY, maxY, screenWidthLimit, spawnedPositions);
        }

        private Vector2 GetRandomOrganicPosition(float minY, float maxY, float screenWidthLimit,
            List<Vector3> spawnedPositions)
        {
            int attempts = 0;
            float checkRadius = spacing * 0.85f;

            while (attempts < 200)
            {
                attempts++;
                // Ưu tiên tìm ở vùng dưới trước (minY) để lấp đầy từ dưới lên
                float x = Random.Range(-screenWidthLimit, screenWidthLimit);
                float y = Random.Range(minY, maxY);
                Vector2 candidate = new Vector2(x, y);

                bool isOverlapping = false;
                foreach (var p in spawnedPositions)
                {
                    if (Vector2.Distance(candidate, p) < checkRadius)
                    {
                        isOverlapping = true;
                        break;
                    }
                }

                if (!isOverlapping) return candidate;
            }

            return new Vector2(Random.Range(-screenWidthLimit, screenWidthLimit), maxY); // Fallback
        }

        #endregion
    }

    public class ObjectYComparer : IComparer<ObjectDrop>
    {
        public int Compare(ObjectDrop x, ObjectDrop y)
        {
            if (x == y) return 0;
            int compare = x.transform.position.y.CompareTo(y.transform.position.y);
            // Nếu Y bằng nhau, so sánh ID hoặc InstanceID để tránh bị mất phần tử trong Set
            return compare == 0 ? x.GetInstanceID().CompareTo(y.GetInstanceID()) : compare;
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