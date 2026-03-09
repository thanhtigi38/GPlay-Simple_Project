using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

public class GameConfig : MonoBehaviour
{
    public static GameConfig Instance;

    [NonSerialized] public int coolDownInterAds = 30;
    [NonSerialized] public int coolDownOpenAds = 30;
    [NonSerialized] public int levelShowAdsBreak = 2;
    [NonSerialized] public bool appOpenAdsEnable = true;
    [NonSerialized] public bool showNoInternet = true;

    public Color blueColor;
    public Color playingColor;
    public Color grayColor;

    [NonSerialized] public int levelCountToShowNoInternet;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        levelShowAdsBreak = 2;
        coolDownInterAds = 30;
        coolDownOpenAds = 30;
        appOpenAdsEnable = true;
        showNoInternet = true;
        levelCountToShowNoInternet = 1;
    }


    public static void ReimportMaterialInGameObject(GameObject reimportGameObject)
    {
        // Thu thập tất cả Material trong Level
        var materials = new HashSet<Material>();

        // 1. Renderer thường (MeshRenderer, SpriteRenderer, ParticleSystemRenderer...)
        var renderers = reimportGameObject.GetComponentsInChildren<Renderer>(true);
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat != null)
                    materials.Add(mat);
            }
        }

        // 2. Spine SkeletonAnimation
        var skeletonAnimations = reimportGameObject.GetComponentsInChildren<SkeletonAnimation>(true);
        foreach (var skeletonAnimation in skeletonAnimations)
        {
            var meshRenderer = skeletonAnimation.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                foreach (var mat in meshRenderer.sharedMaterials)
                {
                    if (mat != null)
                        materials.Add(mat);
                }
            }

            // Fix luôn materials từ atlas asset (Spine đặc biệt)
            if (skeletonAnimation.skeletonDataAsset != null &&
                skeletonAnimation.skeletonDataAsset.atlasAssets != null)
            {
                foreach (var atlas in skeletonAnimation.skeletonDataAsset.atlasAssets)
                {
                    if (atlas == null || atlas.Materials == null) continue;
                    foreach (var mat in atlas.Materials)
                    {
                        if (mat != null)
                            materials.Add(mat);
                    }
                }
            }
        }

        // 3. Spine SkeletonGraphic (UGUI)
        var skeletonGraphics = reimportGameObject.GetComponentsInChildren<SkeletonGraphic>(true);
        foreach (var skeletonGraphic in skeletonGraphics)
        {
            if (skeletonGraphic.material != null)
                materials.Add(skeletonGraphic.material);
        }

        // ✅ Cuối cùng: Re-assign lại Shader cho tất cả materials
        foreach (var mat in materials)
        {
            if (mat == null || mat.shader == null) continue;

            var fixedShader = Shader.Find(mat.shader.name);
            if (fixedShader != null)
            {
                mat.shader = fixedShader;
                // Debug.Log("ReAssign Shader: " + fixedShader.name);
            }
            else
            {
                Debug.LogWarning("Shader not found: " + mat.shader.name);
            }
        }
    }
}