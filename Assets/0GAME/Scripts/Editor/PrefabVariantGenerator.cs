using UnityEngine;
using UnityEditor;
using System.IO;

namespace ThanhND.Editor
{
    public class PrefabVariantGenerator : EditorWindow
    {
        private GameObject basePrefab;
        private DefaultAsset sourceFolder;
        private DefaultAsset destinationFolder;

        [MenuItem("Tools/ThanhND/Prefab Variant Generator")]
        public static void ShowWindow()
        {
            GetWindow<PrefabVariantGenerator>("Prefab Variant Gen");
        }

        private void OnGUI()
        {
            GUILayout.Label("Tạo Prefab Variants Hàng Loạt", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            basePrefab = (GameObject)EditorGUILayout.ObjectField("Base Prefab (Template)", basePrefab, typeof(GameObject), false);
            sourceFolder = (DefaultAsset)EditorGUILayout.ObjectField("Thư mục chứa Sprite", sourceFolder, typeof(DefaultAsset), false);
            destinationFolder = (DefaultAsset)EditorGUILayout.ObjectField("Thư mục Đích (Save)", destinationFolder, typeof(DefaultAsset), false);

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate Prefab Variants", GUILayout.Height(40)))
            {
                GenerateVariants();
            }
        }

        private void GenerateVariants()
        {
            if (basePrefab == null || sourceFolder == null || destinationFolder == null)
            {
                EditorUtility.DisplayDialog("Lỗi", "Vui lòng điền đầy đủ các thông tin!", "OK");
                return;
            }

            string sourcePath = AssetDatabase.GetAssetPath(sourceFolder);
            string destPath = AssetDatabase.GetAssetPath(destinationFolder);

            // Tìm tất cả Sprite
            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { sourcePath });
            
            int count = 0;
            foreach (string guid in guids)
            {
                string spritePath = AssetDatabase.GUIDToAssetPath(guid);
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if (sprite == null) continue;

                // 1. Tạo Instance từ Base Prefab trên Scene
                GameObject variantSource = (GameObject)PrefabUtility.InstantiatePrefab(basePrefab);
                
                // 2. Thay đổi Sprite
                SpriteRenderer sr = variantSource.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = sprite;
                
                ObjectDrop objectDrop = variantSource.GetComponent<ObjectDrop>();
                objectDrop.id = count + 1;

                // 4. SỬA LỖI ĐƯỜNG DẪN: Kết hợp destPath và tên file
                // Kết quả mong muốn: "Assets/YourFolder/Object_1.prefab"
                string variantFullRelativePath = Path.Combine(destPath, $"Object_{count + 1}.prefab");
                
                // 5. Lưu thành Prefab Variant
                // SaveAsPrefabAsset sẽ tự hiểu là Variant nếu source được sinh ra từ một Prefab gốc
                PrefabUtility.SaveAsPrefabAsset(variantSource, variantFullRelativePath);

                // 6. Dọn dẹp Scene
                DestroyImmediate(variantSource);
                count++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Thành công", $"Đã tạo xong {count} Prefab Variants tại {destPath}!", "OK");
        }
    }
}