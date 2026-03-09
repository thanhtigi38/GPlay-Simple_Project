using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EventDispatcher;
using MoreMountains.NiceVibrations;
using Newtonsoft.Json;
using ThanhND;

public class UseProfile : MonoBehaviour
{
    public static bool NewUser
    {
        get { return PlayerPrefs.GetInt(StringHelper.NEW_USER, 1) == 1; }
        set
        {
            PlayerPrefs.SetInt(StringHelper.NEW_USER, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static int CurrentLevel
    {
        get { return PlayerPrefs.GetInt(StringHelper.CURRENT_LEVEL, 1); }
        set
        {
            PlayerPrefs.SetInt(StringHelper.CURRENT_LEVEL, value);
            PlayerPrefs.Save();
        }
    }

    public static bool IsTrackedPremission
    {
        get { return PlayerPrefs.GetInt(StringHelper.IS_TRACKED_PREMISSION, 0) == 1; }
        set
        {
            PlayerPrefs.SetInt(StringHelper.IS_TRACKED_PREMISSION, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool IsAcceptTracker
    {
        get { return PlayerPrefs.GetInt(StringHelper.IS_ACCEPT_TRACKED_PREMISSION, 0) == 1; }
        set
        {
            PlayerPrefs.SetInt(StringHelper.IS_ACCEPT_TRACKED_PREMISSION, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool IsRemoveAds
    {
        get { return PlayerPrefs.GetInt(StringHelper.REMOVE_ADS, 0) == 1; }
        set
        {
            PlayerPrefs.SetInt(StringHelper.REMOVE_ADS, value ? 1 : 0);
            if (value)
            {
                GameController.Instance.admobAds.DestroyBanner();
                GameController.Instance.admobAds.HideMRec();
            }

            PlayerPrefs.Save();
        }
    }

    public static bool OnVibration
    {
        get { return PlayerPrefs.GetInt(StringHelper.ONOFF_VIBRATION, 1) == 1; }
        set
        {
            PlayerPrefs.SetInt(StringHelper.ONOFF_VIBRATION, value ? 1 : 0);
            MMVibrationManager.SetHapticsActive(value);
            PlayerPrefs.Save();
        }
    }

    public static bool OnSound
    {
        get { return PlayerPrefs.GetInt(StringHelper.ONOFF_SOUND, 1) == 1; }
        set
        {
            PlayerPrefs.SetInt(StringHelper.ONOFF_SOUND, value ? 1 : 0);
            GameController.Instance.musicManager.SetSoundVolume(value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool OnMusic
    {
        get { return PlayerPrefs.GetInt(StringHelper.ONOFF_MUSIC, 1) == 1; }
        set
        {
            PlayerPrefs.SetInt(StringHelper.ONOFF_MUSIC, value ? 1 : 0);
            GameController.Instance.musicManager.SetMusicVolume(value ? 0.2f : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool IsFirstTimeInstall
    {
        get { return PlayerPrefs.GetInt(StringHelper.FIRST_TIME_INSTALL, 1) == 1; }
        set
        {
            PlayerPrefs.SetInt(StringHelper.FIRST_TIME_INSTALL, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static int RetentionD
    {
        get { return PlayerPrefs.GetInt(StringHelper.RETENTION_D, 0); }
        set
        {
            if (value < 0)
                value = 0;

            PlayerPrefs.SetInt(StringHelper.RETENTION_D, value);
            PlayerPrefs.Save();
        }
    }

    public static int DaysPlayed
    {
        get { return PlayerPrefs.GetInt(StringHelper.DAYS_PLAYED, 1); }
        set
        {
            PlayerPrefs.SetInt(StringHelper.DAYS_PLAYED, value);
            PlayerPrefs.Save();
        }
    }

    public static int PayingType
    {
        get { return PlayerPrefs.GetInt(StringHelper.PAYING_TYPE, 0); }
        set
        {
            PlayerPrefs.SetInt(StringHelper.PAYING_TYPE, value);
            PlayerPrefs.Save();
        }
    }


    public static int NumberOfAdsInPlay;

    public static int NumberOfAdsInDay
    {
        get { return PlayerPrefs.GetInt(StringHelper.NUMBER_OF_ADS_IN_DAY, 0); }
        set
        {
            PlayerPrefs.SetInt(StringHelper.NUMBER_OF_ADS_IN_DAY, value);
            PlayerPrefs.Save();
        }
    }


    public static bool CanShowRate
    {
        get { return PlayerPrefs.GetInt(StringHelper.CAN_SHOW_RATE, 1) == 1; }
        set
        {
            PlayerPrefs.SetInt(StringHelper.CAN_SHOW_RATE, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool IsCheat
    {
        get => PlayerPrefs.GetInt(StringHelper.CHEAT_GAME, 0) == 1;
        set => PlayerPrefs.SetInt(StringHelper.CHEAT_GAME, value ? 1 : 0);
    }

    public static bool GetLogStartLevel(int levelId, PlayMode playMode)
    {
        return PlayerPrefs.GetInt(StringHelper.LOG_START_LEVEL + levelId + playMode, 0) == 1;
    }

    public static void SetLogStartLevel(int levelId, PlayMode playMode, bool value)
    {
        PlayerPrefs.SetInt(StringHelper.LOG_START_LEVEL + levelId + playMode, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool GetLogEndLevel(int levelId, PlayMode playMode)
    {
        return PlayerPrefs.GetInt(StringHelper.LOG_END_LEVEL + levelId + playMode, 0) == 1;
    }

    public static void SetLogEndLevel(int levelId, PlayMode playMode, bool value)
    {
        PlayerPrefs.SetInt(StringHelper.LOG_END_LEVEL + levelId + playMode, value ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    //my game
    
}