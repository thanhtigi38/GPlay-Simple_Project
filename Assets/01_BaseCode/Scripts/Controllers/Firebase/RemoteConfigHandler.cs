using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Firebase.RemoteConfig;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

public class RemoteConfigHandler : MonoBehaviour
{
    public static RemoteConfigHandler Instance;
    public bool isReady = false;

    private void Awake()
    {
        Instance = this;
    }

    public async void GetDataAndActive()
    {
        try
        {
            await Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        //lấy dữ liệu từ remote
        FetchComplete();
    }

    private async void FetchComplete()
    {
        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        Dictionary<string, string> configDictionary = new Dictionary<string, string>();
        string[] configKeys = {FirebaseConfig.interstitial_cool_down, FirebaseConfig.open_ads_cool_down,
            FirebaseConfig.APP_OPEN_ADS_ENABLE,FirebaseConfig.show_no_internet, FirebaseConfig.level_show_ads_break,
            FirebaseConfig.LEVEL_COUNT_TO_SHOW_NO_INTERNET,
        };
        //fetching configs from server
        try
        {
            Task<bool> activeTask = FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
            await activeTask;
            //get data
            foreach (string key in configKeys)
            {
                configDictionary.Add(key, FirebaseRemoteConfig.DefaultInstance.GetValue(key).StringValue);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
        }

        foreach (var data in configDictionary)
        {
            Debug.Log(data.Key + " => " + data.Value);
        }
        
        if (configDictionary.ContainsKey(FirebaseConfig.interstitial_cool_down))
        {
            try
            {
                GameConfig.Instance.coolDownInterAds = int.Parse(configDictionary[FirebaseConfig.interstitial_cool_down]);
                Debug.LogError("interstitial_cool_down = " + GameConfig.Instance.coolDownInterAds);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
        
        if (configDictionary.ContainsKey(FirebaseConfig.open_ads_cool_down))
        {
            try
            {
                GameConfig.Instance.coolDownOpenAds = int.Parse(configDictionary[FirebaseConfig.open_ads_cool_down]);
                Debug.LogError("open_ads_cool_down = " + GameConfig.Instance.coolDownOpenAds);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            
        }
        
        if (configDictionary.ContainsKey(FirebaseConfig.level_show_ads_break))
        {
            try
            {
                GameConfig.Instance.levelShowAdsBreak = int.Parse(configDictionary[FirebaseConfig.level_show_ads_break]);
                Debug.LogError("level_show_ads_inter = " + GameConfig.Instance.levelShowAdsBreak);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            
        }
        
        if (configDictionary.ContainsKey(FirebaseConfig.APP_OPEN_ADS_ENABLE))
        {
            try
            {
                GameConfig.Instance.appOpenAdsEnable = bool.Parse(configDictionary[FirebaseConfig.APP_OPEN_ADS_ENABLE]);
                Debug.LogError("appOpenAdsEnable = " + GameConfig.Instance.appOpenAdsEnable);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            
        }
        
        if (configDictionary.ContainsKey(FirebaseConfig.show_no_internet))
        {
            try
            {
                GameConfig.Instance.showNoInternet = bool.Parse(configDictionary[FirebaseConfig.show_no_internet]);
                Debug.LogError("show_no_internet = " + GameConfig.Instance.showNoInternet);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            
        }
        
        
        if (configDictionary.ContainsKey(FirebaseConfig.LEVEL_COUNT_TO_SHOW_NO_INTERNET))
        {
            try
            {
                GameConfig.Instance.levelCountToShowNoInternet = int.Parse(configDictionary[FirebaseConfig.LEVEL_COUNT_TO_SHOW_NO_INTERNET]);
                Debug.LogError("levelCountToShowNoInternet = " + GameConfig.Instance.levelCountToShowNoInternet);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            
        }

        //done,remoteconfig is ready
        // FirebaseInitialize.onInit -= GetDataAndActive;
        isReady = true;
    }

    // private void OnDestroy()
    // {
    //     FirebaseInitialize.onInit -= GetDataAndActive;
    // }
}