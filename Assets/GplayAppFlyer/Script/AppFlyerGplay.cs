// Copyright (c) 2024 GplayJSC (gplayjsc.com)
// 
// Author: Axolotl (lamanh.w@gmail.com)
// 
// Created: 04/06/2024
// 
// File: AppFlyerGplay.cs
// 
// Note:

using System;
using System.Collections.Generic;
using AppsFlyerSDK;
using GplayAppFlyer.Script;
using UnityEngine;

public class AppFlyerGplay : MonoBehaviour, IAppsFlyerConversionData
{
    private const string DevKey = "XM6HPCReBAqLH5uCaQHRDY";
    [SerializeField] private string appId;

    [SerializeField] private bool isDebug;
    [SerializeField] private bool getConversionData;

    private static AppFlyerGplay _instance;


    public void Awake()
    {
        //  Firebase.Messaging.FirebaseMessaging.TokenReceived += OnTokenReceived;
        _instance = this;
    }


    public void Start()
    {
#if UNITY_IOS
            appId = "6526475653";
#endif
        AppsFlyerAdRevenue.setIsDebug(isDebug);
        AppsFlyer.setIsDebug(isDebug);
        AppsFlyer.initSDK(DevKey, appId, getConversionData ? this : null);
#if UNITY_IOS && !UNITY_EDITOR
        AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
#endif
        AppsFlyerAdRevenue.start();
        AppsFlyer.startSDK();
        Debug.Log("Start AppFlyer");


        try
        {
            var attributionId = AppsFlyer.getAppsFlyerId();
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty("attribution_id", attributionId);
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    // Mark AppsFlyer CallBacks
    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("didReceiveConversionData", conversionData);
        if (isDebug) Debug.Log(conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        // add deferred deeplink logic here
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        Dictionary<string, object> attributionDataDictionary = AppsFlyer.CallbackStringToDictionary(attributionData);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }

    private static void OnTokenReceived(object sender, Firebase.Messaging.TokenReceivedEventArgs token)
    {
#if UNITY_ANDROID
        AppsFlyer.updateServerUninstallToken(token.Token);
#endif
    }


    public static void LogRevenue(string placement, MediationType mediationType, string unit,
        string format, double value, string currency)
    {
        Dictionary<string, string> addParams = new Dictionary<string, string>()
        {
            { AFAdRevenueEvent.AD_UNIT, unit },
            { AFAdRevenueEvent.AD_TYPE, format },
            { AFAdRevenueEvent.PLACEMENT, placement }
        };
        AppsFlyerAdRevenue.logAdRevenue(
            mediationType.ToString(),
            Parser(),
            value,
            currency,
            addParams
        );
        return;

        AppsFlyerAdRevenueMediationNetworkType Parser()
        {
            return mediationType switch
            {
                MediationType.Admob => AppsFlyerAdRevenueMediationNetworkType
                    .AppsFlyerAdRevenueMediationNetworkTypeGoogleAdMob,
                MediationType.Max => AppsFlyerAdRevenueMediationNetworkType
                    .AppsFlyerAdRevenueMediationNetworkTypeApplovinMax,
                _ => AppsFlyerAdRevenueMediationNetworkType.AppsFlyerAdRevenueMediationNetworkTypeCustomMediation
            };
        }
    }
}

public enum MediationType
{
    Admob = 0,
    Max = 1,
}