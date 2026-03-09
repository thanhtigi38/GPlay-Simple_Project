using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StringHelper
{
    public const string NEW_USER = "NEW_USER";
    public const string ONOFF_SOUND = "ONOFF_SOUND";
    public const string ONOFF_MUSIC = "ONOFF_MUSIC";
    public const string ONOFF_VIBRATION = "ONOFF_VIBRATION";
    public const string FIRST_TIME_INSTALL = "FIRST_TIME_INSTALL";

    public const string VERSION_FIRST_INSTALL = "VERSION_FIRST_INSTALL";
    public const string REMOVE_ADS = "REMOVE_ADS";
    public const string CURRENT_LEVEL = "CURRENT_LEVEL";

    public const string SALE_IAP = "_sale";

    public const string RETENTION_D = "retent_type";
    public const string DAYS_PLAYED = "days_played";
    public const string PAYING_TYPE = "retent_type";
    public const string LEVEL = "level";

    public const string NUMBER_OF_ADS_IN_DAY = "NUMBER_OF_ADS_IN_DAY";
  
    public const string IS_TRACKED_PREMISSION = "is_tracked_premission";
    public const string IS_ACCEPT_TRACKED_PREMISSION = "is_accept_tracked_premission";
    public const string CAN_SHOW_RATE = "CAN_SHOW_RATE";
    public const string CHEAT_GAME = "CHEAT_GAME";
    public const string LEVEL_DATA = "LEVEL_DATA";
    public const string LEVEL_UNLOCK_STATUS = "LEVEL_UNLOCK_STATUS";
    public const string HINT_ITEM = "HINT_ITEM";
    public const string FREEZE_ITEM = "FREEZE_ITEM";
    public const string IS_VIP = "IS_VIP";
    public const string LAST_DAY_LOGIN = "LAST_DAY_LOGIN";
    public const string LOG_START_LEVEL = "LOG_START_LEVEL";
    public const string LOG_END_LEVEL = "LOG_END_LEVEL";
    
    public const string CURRENT_NORMAL_CONFIG = "current_normal_config";
    public const string CURRENT_BOOK_CONFIG = "current_book_config";
    public const string CURRENT_TWEEZER_ID = "CURRENT_TWEEZER_ID";
    public const string TWEEZER_REWARD_PERCENT = "TWEEZER_REWARD_PERCENT";
    public const string TWEEZER_STATUS = "TWEEZER_STATUS";
    public const string CURRENT_WIN_STREAK = "CURRENT_WIN_STREAK";
    public const string LEVEL_COMPLETE_TO_SHOW_INTER = "LEVEL_COMPLETE_TO_SHOW_INTER";
    public const string COINS = "COINS";
    public const string BOOK_UNLOCK_STATUS = "BOOK_UNLOCK_STATUS";
    public const string ADS_COUNT_TO_UNLOCK_5_LEVEL = "ADS_COUNT_TO_UNLOCK_BOOK";
    public const string LAST_DATE_TIME_LOGIN = "LAST_DATE_TIME_LOGIN";
    public const string DAILY_QUEST_PROGRESS = "DAILY_QUEST_PROGRESS";
    public const string IS_DAILY_QUEST_CLAIMED = "IS_DAILY_QUEST_CLAIMED";
    public const string CLAIMED_TOTAL_DAILY_QUEST_GIFT = "CLAIMED_TOTAL_DAILY_QUEST_GIFT";
    public const string CLAIMED_BOOK_GIFT = "CLAIMED_BOOK_GIFT";
    public const string CURRENT_LEVEL_INDEX = "CURRENT_LEVEL_INDEX";
    public const string HAVE_NEW_TWEEZERS = "HAVE_NEW_TWEEZERS";
    public const string NEXT_LEVEL_FREE = "NEXT_LEVEL_FREE";
    public const string IS_RECEIVED_NEXT_LEVEL_FREE = "IS_RECEIVED_NEXT_LEVEL_FREE";
    public const string MAX_LEVEL_INDEX_PASSED = "MAX_LEVEL_INDEX_PASSED";

}

public class PathPrefabs
{
    public const string CHECK_INTERNET_POP_UP = "UI/Popups/CheckInternetPopUp";
    public const string POPUP_REWARD_BASE = "UI/Popups/PopupRewardBase";
    public const string CONFIRM_POPUP = "UI/Popups/ConfirmBox";
    public const string WAITING_BOX = "UI/Popups/WaitingBox";
    public const string REWARD_IAP_BOX = "UI/Popups/RewardIAPBox";
    public const string SHOP_BOX = "UI/ShopBox";

    public const string REWARD_CONGRATULATION_BOX = "UI/Popups/RewardCongratulationBox";
  
    public const string TRACKING_BOX = "UI/TrackingBox";


    public const string ADS_BREAK_POP_UP = "ADS_BREAK_POP_UP";
    public const string RATE_GAME_BOX = "UI/Popups/RateGameBox";
    
    //my game
    public const string POP_UP_PATH = "UI/PopUps/";
    public const string STAR_CHEST = "UI/PopUps/StarChest";
    
    // AnnoyTok
    public const string POPUP_HINT = "UI/AnnoyMe/HintPopup";

}

public class SceneName
{
    public const string LOADING_SCENE = "LoadingScene";
    public const string HOME_SCENE = "HomeScene";
    public const string GAME_PLAY = "GameplayScene";
}

public class AudioName
{
    //public const string bgMainHome = "Music_BG_MainHome";
}

public class KeyPref
{
    //public const string SERVER_INDEX = "SERVER_INDEX";

}
public class PackIAP
{
    //public const string REMOVE_ADS = "remove_ads";
}
public static class CategoryConst
{
    //public const int POPULAR = 10002;
}
public class FirebaseConfig
{
    public const string interstitial_cool_down = "interstitial_cool_down";
    public const string open_ads_cool_down = "open_ads_cool_down";
    public const string level_show_ads_break = "level_show_ads_break";
    public const string APP_OPEN_ADS_ENABLE = "appOpenAdsEnable";
    public const string show_no_internet = "show_no_internet";
    public const string LEVEL_NORMAL_CONFIG = "level_normal_config";
    public const string LEVEL_NORMAL_CONFIG_TEST = "level_normal_config_test";

    public const string ENABLE_ADMOB_BANNER = "enable_admob_banner";
    public const string MINIMUM_TIME_SHOW_COLLAPSE = "minimum_time_show_collapse";
    public const string COOLDOWN_ADMOB_REFRESH = "cooldown_admob_refresh";
    
    public const string RELOAD_BANNER_COLLAPSE_TIME = "reload_banner_collapse_time";
    public const string ENABLE_ADMOB_BANNER_COLLAP = "enable_admob_banner_collap";
    public const string ENABLE_SHORT_INTER = "enable_short_inter";
    public const string ENABLE_INTER_START = "enable_inter_start";
    public const string ENABLE_INTER_START_GAMEPLAY = "enable_inter_start_gameplay";
    public const string DELAY_SHOW_INITSTIALL = "delay_show_initi_ads_click";//Thời gian giữa 2 lần show inital 30
    public const string DELAY_SHOW_INTER_IMAGE = "delay_show_inter_image";//Thời gian giữa 2 lần show inital 30
    
    public const string LEVEL_COUNT_TO_SHOW_NO_INTERNET = "level_count_to_show_no_internet";
    public const string LEVEL_COMPLETE_TO_SHOW_INTER = "level_complete_to_show_inter";
}

public class LevelGameData
{
    public const string LEVEL_SAVE = "Datas/DataLevelSave";
}

public class ChapterGameData
{
    public const string CHAPTER_SAVE = "Datas/ChapterDataSave";
}

