using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Firebase.Analytics;
using Firebase;
//using Facebook.Unity;
using System;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using System.Threading.Tasks;

//using com.adjust.sdk;

public class AnalyticsController : MonoBehaviour
{
    #region Init

    static UnityEvent onFinishFirebaseInit = new UnityEvent();
    private static bool m_firebaseInitialized = false;

    public static bool firebaseInitialized
    {
        get { return m_firebaseInitialized; }
        set
        {
            m_firebaseInitialized = value;
            if (value == true)
            {
                if (onFinishFirebaseInit != null)
                {
                    onFinishFirebaseInit.Invoke();
                    onFinishFirebaseInit.RemoveAllListeners();
                }

                //SetUserProperties();
            }
        }
    }

    #endregion

    public const string aj_inters_ad_eligible = "lvghbs";
    public const string aj_inters_api_called = "dacwpr";
    public const string aj_inters_displayed = "xwf3w4";
    public const string aj_level_complete = "dbltlf";
    public const string aj_purchase = "lizm6f";
    public const string aj_rewarded_ad_completed = "fzna67";
    public const string aj_rewarded_ad_eligible = "nkewwc";
    public const string aj_rewarded_api_called = "wwnq9m";
    public const string aj_rewarded_displayed = "yj9npp";
    public const string aj_tutorial_completion = "qksj8v";

    private static void LogBuyInappAdjust(string inappID, string trancstionID)
    {
    }

    public static void LogEventFirebase(string eventName, Parameter[] parameters)
    {
        if (firebaseInitialized)
        {
            FirebaseAnalytics.LogEvent(eventName, parameters);
        }
        else
        {
            onFinishFirebaseInit.AddListener(() => { FirebaseAnalytics.LogEvent(eventName, parameters); });
        }
    }

//     public static void LogEventFacebook(string eventName, Dictionary<string, object> parameters)
//     {
//         if (FB.IsInitialized)
//         {
// #if !ENV_PROD
//             parameters["test"] = true;
// #endif
//             FB.LogAppEvent(eventName, null, parameters);
//         }
//     }
    public static void LogSelectArea(string idArea)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
                {
                    new Parameter("select_area", idArea.ToString())
                };
                FirebaseAnalytics.LogEvent("select_area", parameters);
            }
        }
        catch
        {
        }
    }

    public static void LogComplete(string id)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
                {
                    new Parameter("pic_id", id.ToString())
                };
                FirebaseAnalytics.LogEvent("complete_pic", parameters);
            }
        }
        catch
        {
        }
    }


    public static void LogStartLevel(int level, PlayMode playMode)
    {
        try
        {
            if (!firebaseInitialized) return;

            Parameter[] parameters = new Parameter[2]
            {
                new Parameter(FirebaseAnalytics.ParameterLevel, level.ToString()),
                new Parameter("level_mode", playMode.ToString()),
            };


            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, parameters);
        }
        catch
        {

        }
    }

    public static void LogLevelEnd(int level,PlayMode playMode, int playTime, bool success)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[4]
                {
                    new Parameter(FirebaseAnalytics.ParameterLevel, level.ToString()),
                    new Parameter("level_mode",playMode.ToString()),
                    new Parameter("play_time",playTime.ToString()),
                    new Parameter(FirebaseAnalytics.ParameterSuccess,success.ToString())

                };

                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, parameters);
            }
        }
        catch
        {

        }

    }


    public static void LogClickVIP(string id = "")
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
                {
                    new Parameter("select_iap", id.ToString())
                };
                FirebaseAnalytics.LogEvent("select_iap", parameters);
            }
        }
        catch
        {
        }
    }

    public static void LogAdsRevenue(string name_event, string ad_source, string ad_unit_name, string ad_format,
        double value, string currency)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] AdRevenueParameters =
                {
                    new Parameter(FirebaseAnalytics.ParameterLevel, UseProfile.CurrentLevel.ToString()),
                    new Parameter("level_mode", "Free"),
                    new Parameter("ad_source", ad_source),
                    new Parameter("ad_unit_name", ad_unit_name),
                    new Parameter("ad_format", ad_format ?? ""),
                    new Parameter(FirebaseAnalytics.ParameterValue, value),
                    new Parameter(FirebaseAnalytics.ParameterCurrency, currency)
                };
                FirebaseAnalytics.LogEvent(name_event, AdRevenueParameters);
                Debug.Log("ad_source| " + ad_source + "ad_unit_name| " + ad_unit_name + "ad_format| " + ad_format +
                          "value| " + value + "currency| " + currency);
            }
        }
        catch
        {
        }
    }

    public static void LogAdsRevenue(string adSource, string adUnitName, string adFormat,
        double value, string currency, MediationType mediationType, bool isSubRevenue)
    {
        try
        {
            if (firebaseInitialized)
            {
                List<Parameter> adRevenueParameters = new List<Parameter>
                {
                    new Parameter("ad_source", adSource),
                    new Parameter("ad_unit_name", adUnitName),
                    new Parameter("ad_format", adFormat ?? ""),
                    new Parameter(FirebaseAnalytics.ParameterValue, value),
                    new Parameter(FirebaseAnalytics.ParameterCurrency, currency)
                };

                FirebaseAnalytics.LogEvent(Keys.RealtimeAdRevenue, adRevenueParameters.ToArray());
                FirebaseAnalytics.LogEvent(Keys.RealtimeAdRevenueGoogle, adRevenueParameters.ToArray());
                if (isSubRevenue) FirebaseAnalytics.LogEvent(Keys.AdSubRevenue, adRevenueParameters.ToArray());
                Debug.Log("Log ads revenue | ad_source: " + adSource +
                          ", ad_unit_name: " + adUnitName + ", ad_format: " + adFormat +
                          ", value: " + value + ", currency: " + currency);
            }
            else
            {
                Debug.LogError("firebase is not initialized, cannot log ads revenue");
            }
        }
        catch
        {
            // ignored
        }

        try
        {
            //AF 
            AppFlyerGplay.LogRevenue(adSource, mediationType, adUnitName, adFormat, value, currency);
        }
        catch
        {
            // igonred
        }
    }

    public static void LogIAPSDK(double iapValue, string iapCurrency)
    {
        Parameter[] IAPRevenueParameters =
        {
            new Parameter(FirebaseAnalytics.ParameterValue, iapValue),
            new Parameter(FirebaseAnalytics.ParameterCurrency, iapCurrency)
        };
        FirebaseAnalytics.LogEvent("iap_sdk", IAPRevenueParameters);
    }

    public static void SetUserProperties()
    {
        if (!firebaseInitialized) return;

        FirebaseAnalytics.SetUserProperty(StringHelper.RETENTION_D, UseProfile.RetentionD.ToString());
        FirebaseAnalytics.SetUserProperty(StringHelper.DAYS_PLAYED, UseProfile.DaysPlayed.ToString());
        FirebaseAnalytics.SetUserProperty(StringHelper.PAYING_TYPE, UseProfile.PayingType.ToString());
        FirebaseAnalytics.SetUserProperty(StringHelper.LEVEL, UseProfile.CurrentLevel.ToString());
    }

    #region Event

    public void LogWatchVideo(string action, bool isHasVideo, bool isHasInternet, string level)
    {
        try
        {
            if (!firebaseInitialized) return;
            Parameter[] parameters = new Parameter[4]
            {
                new Parameter("actionWatch", action.ToString()),
                new Parameter("has_ads", isHasVideo.ToString()),
                new Parameter("has_internet", isHasInternet.ToString()),
                new Parameter("level", level)
            };

            FirebaseAnalytics.LogEvent("watch_video_game", parameters);
            LogVideoRewardShowDone(action.ToString());
        }
        catch
        {
        }
    }

    public static void LogUseItem(Item item, string level = "")
    {
        try
        {
            if (!firebaseInitialized) return;
            Parameter[] parameters = new Parameter[2]
            {
                new Parameter("item", item.ToString()),
                new Parameter("pic_id", level.ToString()),
            };

            FirebaseAnalytics.LogEvent("use_item", parameters);
            FirebaseAnalytics.LogEvent(item.ToString());
        }
        catch
        {
        }
    }

    public void LogWatchInter(string action, bool isHasVideo, bool isHasInternet, string level)
    {
        try
        {
            if (!firebaseInitialized) return;
            Parameter[] parameters = new Parameter[4]
            {
                new Parameter("actionWatch", action.ToString()),
                new Parameter("has_ads", isHasVideo.ToString()),
                new Parameter("has_internet", isHasInternet.ToString()),
                new Parameter("level", level)
            };

            FirebaseAnalytics.LogEvent("show_inter", parameters);
        }
        catch
        {
        }
    }

    public static void LogBuyInapp(string inappID, string trancstionID)
    {
        try
        {
            LogBuyInappAdjust(inappID, trancstionID);
        }
        catch
        {
        }

        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
                {
                    new Parameter("id", inappID),
                };
                LogEventFirebase("inapp_event", parameters);
            }
        }
        catch
        {
        }
    }

    public void LogStartLevel(string id, bool isRotate, int pieceCount)
    {
        try
        {
            if (!firebaseInitialized) return;

            string[] stringSplit = id.Split('/');
            string picName = stringSplit[stringSplit.Length - 1].Split('.')[0];
            var match = Regex.Match(picName, @"^([a-zA-Z]+)(\d+)$");
            string cateName = match.Groups[1].Value;
            string index = match.Groups[2].Value;
            // Debug.LogError("Logggg" + cateName +":::" + index);

            Parameter[] parameters = new Parameter[4]
            {
                new Parameter("topic", cateName.ToString()),
                new Parameter("ID", index.ToString()),
                new Parameter("is_rotate", isRotate.ToString()),
                new Parameter("piece_count", pieceCount.ToString()),
            };


            FirebaseAnalytics.LogEvent("level_start", parameters);
        }
        catch
        {
        }
    }

    public static void LogLevelEnd(int level, string levelMode, bool isSuccess, ReasonType reasonType, double playTime)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[5]
                {
                    new Parameter("level", level.ToString()),
                    new Parameter("level_mode", levelMode),
                    new Parameter("is_success", isSuccess.ToString()),
                    new Parameter("reason", reasonType.ToString()),
                    new Parameter("play_time", playTime.ToString())
                };

                FirebaseAnalytics.LogEvent("level_end", parameters);
#if THANH_TESTER
                Debug.LogError("LOG EVENT LEVEL END=====");
#endif
            }
            else
            {
#if THANH_TESTER
                Debug.LogError("NOT LOG EVENT LEVEL END BECAUSE NOT INITED=====");
#endif
            }
        }
        catch
        {
            Debug.LogError("NOT LOG EVENT LEVEL END BECAUSE ERROR=====");
        }
    }

    public void LogLevelFail(int level)
    {
        if (!firebaseInitialized) return;
        Parameter[] parameters = new Parameter[1]
        {
            new Parameter("level", level.ToString())
        };


        FirebaseAnalytics.LogEvent("level_fail", parameters);
    }

    public void LogRequestVideoReward(string placement)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
                {
                    new Parameter("placement", placement.ToString())
                };


                FirebaseAnalytics.LogEvent("ads_reward_offer", parameters);
            }
        }
        catch
        {
        }
    }

    public void LogVideoRewardEligible()
    {
        try
        {
            //AdjustEvent adjustEvent = new AdjustEvent(aj_rewarded_ad_eligible);
            //Adjust.trackEvent(adjustEvent);
        }
        catch
        {
        }
    }

    public void LogClickToVideoReward(string placement)
    {
        try
        {
            if (!firebaseInitialized) return;
            Parameter[] parameters = new Parameter[1]
            {
                new Parameter("placement", placement.ToString())
            };


            FirebaseAnalytics.LogEvent("ads_reward_click", parameters);
        }
        catch
        {
        }
    }

    public void LogVideoRewardShow(string placement)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
                {
                    new Parameter("placement", placement.ToString())
                };


                FirebaseAnalytics.LogEvent("ads_reward_show", parameters);
            }
        }
        catch
        {
        }
    }

    public void LogVideoRewardLoadFail(string placement, string errormsg)
    {
        try
        {
            if (!firebaseInitialized) return;
            Parameter[] parameters = new Parameter[2]
            {
                new Parameter("placement", placement.ToString()),
                new Parameter("errormsg", errormsg.ToString())
            };


            FirebaseAnalytics.LogEvent("ads_reward_fail", parameters);
        }
        catch
        {
        }
    }

    public void LogVideoRewardShowDone(string placement)
    {
        try
        {
            if (firebaseInitialized)
            {
                Parameter[] parameters = new Parameter[1]
                {
                    new Parameter("placement", placement.ToString()),
                };


                FirebaseAnalytics.LogEvent(placement, parameters);
            }
        }
        catch
        {
        }
    }

    public void LogInterLoadFail(string errormsg)
    {
        if (!firebaseInitialized) return;
        Parameter[] parameters = new Parameter[1]
        {
            new Parameter("errormsg", errormsg.ToString())
        };


        FirebaseAnalytics.LogEvent("ad_inter_fail", parameters);
    }

    public void LogInterLoad()
    {
        try
        {
            if (firebaseInitialized)
                FirebaseAnalytics.LogEvent("ad_inter_load");
        }
        catch
        {
        }
    }

    public void LoadInterEligible()
    {
        try
        {
            //AdjustEvent adjustEvent = new AdjustEvent(aj_inters_ad_eligible);
            //Adjust.trackEvent(adjustEvent);
        }
        catch
        {
        }
    }

    public void LogInterShow(string actionWatchLog)
    {
        try
        {
            if (firebaseInitialized)
            {
                bool isHasInternet = Application.internetReachability != NetworkReachability.NotReachable;
                Parameter[] parameters = new Parameter[2]
                {
                    new Parameter("action_watch", actionWatchLog.ToString()),
                    new Parameter("has_internet", isHasInternet.ToString())
                };
                FirebaseAnalytics.LogEvent("ad_inter_show", parameters);
                FirebaseAnalytics.LogEvent(actionWatchLog);
            }
        }
        catch
        {
        }

        try
        {
            //AdjustEvent adjustEvent = new AdjustEvent(aj_inters_displayed);
            //Adjust.trackEvent(adjustEvent);
        }
        catch
        {
        }
    }

    public void LogInterClick()
    {
        try
        {
            if (!firebaseInitialized) return;
            FirebaseAnalytics.LogEvent("ad_inter_click");
        }
        catch
        {
        }
    }

    public void LogInterReady()
    {
        try
        {
            //AdjustEvent adjustEvent = new AdjustEvent(aj_inters_api_called);
            //Adjust.trackEvent(adjustEvent);
        }
        catch
        {
        }
    }

    public void LogVideoRewardReady()
    {
        try
        {
            //AdjustEvent adjustEvent = new AdjustEvent(aj_rewarded_api_called);
            //Adjust.trackEvent(adjustEvent);
        }
        catch
        {
        }
    }

    public void LogTutLevelStart(int level)
    {
        try
        {
            if (firebaseInitialized)
                FirebaseAnalytics.LogEvent(string.Format("tutorial_start_{0}", level));
        }
        catch
        {
        }
    }

    public void LogTutLevelEnd(int level)
    {
        try
        {
            if (firebaseInitialized)
                FirebaseAnalytics.LogEvent(string.Format("tutorial_end_{0}", level));
        }
        catch
        {
        }

        try
        {
            //AdjustEvent adjustEvent = new AdjustEvent(aj_tutorial_completion);
            //adjustEvent.addCallbackParameter("level", level.ToString());
            //adjustEvent.addCallbackParameter("tutorial_id", level.ToString());
            //Adjust.trackEvent(adjustEvent);
        }
        catch
        {
        }
    }

    public static void LogIAP(int level, string productID, string price, string currency)
    {
        try
        {
            //AdjustEvent adjustEvent = new AdjustEvent(aj_purchase);
            //adjustEvent.addCallbackParameter("level", level.ToString());
            //adjustEvent.addCallbackParameter("productID", productID.ToString());
            //adjustEvent.addCallbackParameter("price", price.ToString());
            //adjustEvent.addCallbackParameter("currency", currency.ToString());
            //Adjust.trackEvent(adjustEvent);
        }
        catch
        {
        }
    }

    public static void LogCompletePiece(int levelShowId, int layerTargetId, int objectTargetId, PlayMode playMode)
    {
        try
        {
            if (!m_firebaseInitialized) return;

            Parameter[] parameters = new Parameter[4]
            {
                new Parameter("level", levelShowId.ToString()),
                new Parameter("step_id", layerTargetId.ToString()),
                new Parameter("piece_id", objectTargetId.ToString()),
                new Parameter("play_mode", playMode.ToString())
            };
            FirebaseAnalytics.LogEvent("complete_piece", parameters);
            Debug.Log("Log event complete piece: " + levelShowId + " - " + layerTargetId + " - " + objectTargetId);
        }
        catch
        {
        }
    }

    #endregion

    private void OnApplicationQuit()
    {
        SetUserProperties();
    }
}

public enum ActionClick
{
    None = 0,
    Play = 1,
    Rate = 2,
    Share = 3,
    Policy = 4,
    Feedback = 5,
    Term = 6,
    NoAds = 10,
    Settings = 11,
    ReplayLevel = 12,
    SkipLevel = 13,
    Return = 14,
    BuyStand = 15
}

public enum ActionWatchVideo
{
    None = 0,
    Skip_level = 1,
    Return = 2,
    BuyStand = 3,
    BuyExtral = 4,
    ClaimSkin = 5,
    Hint = 6,
    Daily = 7,
    FreeCoin = 8,
    UnlockPic = 9,
    ClaimX2DailyQuest = 10,
    SaveImage = 11,
    Shuffe = 12,
    UnlockJigsawLevel = 13,
    SeePic = 14,
}

public enum ActionShowInter
{
    None = 0,
    Skip_level = 1,
    Return = 2,
    BuyStand = 3,

    EndGame = 4,
    Click_Setting = 5,
    Click_Replay = 6,
    SelectPic = 7
}

public static class Keys
{
    public const string RealtimeAdRevenueGoogle = "ad_impression";
    public const string RealtimeAdRevenue = "ad_revenue_sdk";
    public const string AdSubRevenue = "sub_revenue";
}

public enum ReasonType
{
    QuitApp = 0,
    Bomb = 1,
    LoseTimeOutGame = 2,
    LoseOutOfSlot = 3,
    LoseFullSpace = 4,
    Win = 5,
    BackHome = 6,
    Restart = 7,
}