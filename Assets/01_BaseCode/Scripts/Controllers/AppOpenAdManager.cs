// Copyright 2021 Google LLC
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

public class AppOpenAdManager
{
    public static bool IsShowFirstTime = false;
#if TESTER
    private const string AD_UNIT_ID = "ca-app-pub-3940256099942544/9257395921";
#elif UNITY_ANDROID
    private const string AD_UNIT_ID = "ca-app-pub-8467610367562059/5673436025";
#elif UNITY_IOS
    private const string AD_UNIT_ID = "ca-app-pub-8467610367562059/9096186342";
#else
    private const string AD_UNIT_ID = "unexpected_platform";
#endif

    private static AppOpenAdManager instance;

    private AppOpenAd ad;

    public bool isShowingAd = false;

    // COMPLETE: Add loadTime field
    private DateTime loadTime;

    private DateTime cooldownTime = DateTime.Now.AddSeconds(-300);


    public static AppOpenAdManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AppOpenAdManager();
            }

            return instance;
        }
    }

    private bool IsAdAvailable
    {
        get
        {
            // COMPLETE: Consider ad expiration
            return ad != null && (System.DateTime.UtcNow - loadTime).TotalHours < 4;
        }
    }


    public void LoadAd(UnityAction actionLoadDone = null)
    {
        var request = new AdRequest();

        // Load an app open ad for portrait orientation
        AppOpenAd.Load(AD_UNIT_ID, request, ((appOpenAd, error) =>
        {
            if (error != null)
            {
                // Handle the error.
                Debug.LogFormat("Failed to load the ad. (reason: {0})", error.GetMessage());
                return;
            }

            // App open ad is loaded
            ad = appOpenAd;
            Debug.Log("App open ad loaded");

            // COMPLETE: Keep track of time when the ad is loaded.
            loadTime = DateTime.UtcNow;

            actionLoadDone?.Invoke();
        }));
    }

    public void ShowAdIfAvailable(Action action = null)
    {
        Debug.Log("Pre Open ads");
        if (UseProfile.IsRemoveAds || !GameConfig.Instance.appOpenAdsEnable || UseProfile.IsCheat)
        {
            action?.Invoke();
            return;
        }

        if (!IsAdAvailable || isShowingAd || (cooldownTime > DateTime.Now && action == null))
        {
            Debug.Log("Not open ads: " + !IsAdAvailable + ":::" + isShowingAd + ":::" + ((cooldownTime > DateTime.Now && action == null)));
            LoadAd();
            action?.Invoke();
            return;
        }
        Debug.Log("Show open ads");

        ad.OnAdFullScreenContentClosed += HandleAdDidDismissFullScreenContent;
        ad.OnAdFullScreenContentClosed += action;
        ad.OnAdFullScreenContentClosed += () => { GameController.Instance.admobAds.ResetCoolDownTime(); };
        ad.OnAdFullScreenContentFailed += HandleAdFailedToPresentFullScreenContent;
        ad.OnAdFullScreenContentOpened += HandleAdDidPresentFullScreenContent;
        ad.OnAdImpressionRecorded += HandleAdDidRecordImpression;
        ad.OnAdPaid += HandlePaidEvent;
        ad.Show();
    }

    public void ResetCoolDownTime()
    {
        cooldownTime = DateTime.Now.AddSeconds(GameConfig.Instance.coolDownOpenAds);
    }

    private void HandleAdDidDismissFullScreenContent()
    {
        Debug.Log("Closed app open ad");
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        isShowingAd = false;
        IsShowFirstTime = true;
        GameController.Instance.admobAds.IsCooldownAdmobBanner = true;
        LoadAd();
        //GameController.Instance.admobAds.ShowBanner();
    }

    private void HandleAdFailedToPresentFullScreenContent(AdError adError)
    {
        // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
        ad = null;
        LoadAd();
    }

    private void HandleAdDidPresentFullScreenContent()
    {
        Debug.Log("Displayed app open ad");
        isShowingAd = true;
        GameController.Instance.admobAds.IsCooldownAdmobBanner = false;
        // GameController.Instance.admobAds.DestroyBanner();
    }

    private void HandleAdDidRecordImpression()
    {
        Debug.Log("Recorded ad impression");
    }

    private void HandlePaidEvent(AdValue adValue)
    {
        AnalyticsController.LogAdsRevenue("admob", "app_open", "app_open", adValue.Value / 1000000f,
            adValue.CurrencyCode, MediationType.Admob, true);
    }
}