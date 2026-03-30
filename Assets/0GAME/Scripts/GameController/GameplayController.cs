using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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

        private List<Vector2> spawnedPositions = new List<Vector2>();
        private float screenWidthLimit;
        private List<ObjectDrop> objectDrops = new List<ObjectDrop>();

        private void Start()
        {
            Init().Forget();
        }

        private void CalculateScreenWidth()
        {
            // Tính toán chiều rộng màn hình dựa trên Camera chính ở tọa độ Z = 0
            Camera cam = Camera.main;
            float height = 2f * cam.orthographicSize;
            float width = height * cam.aspect;

            // Giới hạn X để object không bị tràn ra ngoài mép
            screenWidthLimit = (width / 2f) - paddingX;
        }

        [Button]
        private async UniTaskVoid Init()
        {
            if (levelDatabase != null && levelDatabase.levels.Count > 0)
            {
                CalculateScreenWidth();
                await SpawnObjects();
                gameState = GameState.Playing;
            }
        }

        public async UniTask SpawnObjects()
        {
            LevelData levelData = levelDatabase.levels[0];
            foreach (var objectDrop in objectDrops)
            {
                if (objectDrop != null) Destroy(objectDrop.gameObject);
            }

            List<string> objectIDsToSpawn = PrepareObjectList(levelData);
            Shuffle(objectIDsToSpawn);

            spawnedPositions.Clear();
            objectDrops.Clear();

            // Tính toán chiều cao ước tính của "đống" đồ vật
            // Giả sử mỗi hàng chứa được khoảng (Width / spacing) objects
            float objectsPerRow = (screenWidthLimit * 2) / spacing;
            float estimatedRows = levelData.totalObjects / objectsPerRow;
            float maxY = startY + (estimatedRows * spacing * 1.5f); // Cho phép dôi dư không gian để lộn xộn

            foreach (string id in objectIDsToSpawn)
            {
                Vector2 pos = GetRandomOrganicPosition(startY, maxY);
                spawnedPositions.Add(pos);

                var handle = Addressables.LoadAssetAsync<GameObject>(id);
                await handle;

                // Random rotation rộng (0-360) và Scale nhẹ để tăng độ lộn xộn
                GameObject go = Instantiate(handle.Result, pos, Quaternion.Euler(0, 0, Random.Range(0, 360)));
                float randomScale = Random.Range(0.9f, 1.1f);
                go.transform.localScale = Vector3.one * randomScale;

                objectDrops.Add(go.GetComponent<ObjectDrop>());
            }
        }

        private Vector2 GetRandomOrganicPosition(float minY, float maxY)
        {
            Vector2 candidate = Vector2.zero;
            int maxAttempts = 200; // Tăng số lần thử để tìm chỗ khít
            int attempts = 0;

            // Bán kính để kiểm tra va chạm (nhỏ hơn spacing một chút để chúng có thể đè nhẹ lên nhau)
            float checkRadius = spacing * 0.85f;

            while (attempts < maxAttempts)
            {
                attempts++;

                // Random X toàn màn hình
                float x = Random.Range(-screenWidthLimit, screenWidthLimit);

                // Quan trọng: Random Y theo kiểu "nặng ở dưới" 
                // Chúng ta muốn object ưu tiên nằm ở dưới trước khi tràn lên trên
                // Dùng AnimationCurve hoặc đơn giản là lấy mẫu ngẫu nhiên tăng dần
                float y = Random.Range(minY, maxY);

                candidate = new Vector2(x, y);

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

                // Nếu thử nhiều lần không được ở vùng dưới, ta hơi nhích nhẹ maxY lên để nới lỏng không gian
                maxY += 0.01f;
            }

            return candidate; // Trả về vị trí cuối cùng nếu quá khó tìm
        }

        private List<string> PrepareObjectList(LevelData levelData)
        {
            List<string> list = new List<string>();
            // Đảm bảo đủ m loại
            for (int i = 1; i <= levelData.totalTypes; i++)
            {
                list.Add($"Objects/Object_{i}.prefab");
                list.Add($"Objects/Object_{i}.prefab");
            }

            // Thêm các cặp còn lại
            int remainingPairs = (levelData.totalObjects - list.Count) / 2;
            for (int i = 0; i < remainingPairs; i++)
            {
                int randomType = Random.Range(1, levelData.totalTypes + 1);
                list.Add($"Objects/Object_{randomType}.prefab");
                list.Add($"Objects/Object_{randomType}.prefab");
            }

            return list;
        }

        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T temp = list[i];
                int randomIndex = Random.Range(i, list.Count);
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
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