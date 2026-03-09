using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using EventDispatcher;
using UnityEngine.Events;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
//using com.adjust.sdk;
//using Facebook.Unity;
using Firebase.Analytics;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class AdsController : MonoBehaviour
{
    public static bool isAdmobInitDone;
    public static bool isMaxInitDone;
    
#if UNITY_ANDROID
    private const string MaxSdkKey =
        "eQt0q3679KmUyKeNcSzqC01eB-lILmfTnJoufGxpSn__n1NVhHLeMgxZOaICke451El4ZBfuZum9Qw4WxzpW52";

    private const string RewardedAdUnitId = "96cedcb4a6a42105";
    private const string InterstitialAdUnitId = "bf8a341057b42e03";
    private const string BanerAdUnitId = "8a2459e9b1211f3c";
#elif UNITY_IOS
    private const string MaxSdkKey =
 "eQt0q3679KmUyKeNcSzqC01eB-lILmfTnJoufGxpSn__n1NVhHLeMgxZOaICke451El4ZBfuZum9Qw4WxzpW52";
    private const string RewardedAdUnitId = "e1669818a271b94c";
    private const string InterstitialAdUnitId = "38182765ebb3997c";
    private const string BanerAdUnitId = "6a5313f661bcd1ef";
#endif
#if UNITY_ANDROID
    private string _adUnitIdHigh = "ca-app-pub-8467610367562059/9656627420";
    private string _adUnitIdMedium = "ca-app-pub-8467610367562059/2324932310";
    private string _adUnitIdLow = "ca-app-pub-8467610367562059/8893924685";
#elif UNITY_IPHONE
    private string _adUnitIdHigh = "ca-app-pub-3940256099942544/6978759866";
    private string _adUnitIdMedium = "ca-app-pub-3940256099942544/6978759866";
    private string _adUnitIdLow = "ca-app-pub-8467610367562059/4180939469";
#else
  private string _adUnitId = "unused";
#endif
    private RewardedInterstitialAd rewardedInterstitialAd;
    bool isShowingAds;
    private bool _isInited;
    private IEnumerator reloadBannerCoru;
    public UnityAction actionInterstitialClose;
    public UnityAction actionShortInterstitialClose;
    private bool _isLoading;
    private UnityAction _actionClose;
    private UnityAction _actionRewardVideo;
    private UnityAction _actionNotLoadedVideo;
    private string actionWatchVideo;
    public bool enableMrec;
    [SerializeField] private GameObject blackImage;

    //public NativeAdsManager adsManager;
    // public bool enableMrec;


    public void Init()
    {
        ResetCoolDownTime();
        countdownAdsclick = 0;
        countdownAdsInterImage = 0;

        #region Applovin Ads

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            Debug.Log("MAX SDK Initialized");
            InitInterstitial();
            InitRewardVideo();
            //InitializeBannerAds();
            isMaxInitDone = true;
            // MaxSdk.ShowMediationDebugger();
            InitializeBannerMax();
            MaxSdk.SetCreativeDebuggerEnabled(false);
            enableMrec = EnableMrec();
            if (enableMrec)
            {
                InitializeMRecAds();
            }
        };
        MaxSdk.SetHasUserConsent(UseProfile.IsTrackedPremission);
        MaxSdk.SetVerboseLogging(false);
        MaxSdk.SetSdkKey(MaxSdkKey);
        MaxSdk.InitializeSdk();

        #endregion

        _isInited = true;

        //#if !UNITY_EDITOR
        Debug.Log("===== Init Admob ====");
        MobileAds.Initialize((initStatus) =>
        {
            Debug.Log("===== Init Admob Done ====");
            LoadInterstitialAd();
            AppOpenAdManager.Instance.LoadAd();
            InitBannerAdmob();
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
            isAdmobInitDone = true;
            //adsManager.OnInitAdmobDone();
            LoadRewardedInterstitialAd();
        });
        //#endif
        
    }

    public bool EnableMrec()
    {
        return RemoteConfigController.GetBoolConfig("enable_mrec", true);
    }

    private bool IsAvaiableAdsMrec()
    {
        return isLoadedMrec;
    }

    public bool IsShowMrec()
    {
        return EnableMrec() && IsAvaiableAdsMrec();
    }
    
    #region MREC
#if TESTER 
    private const string MRecAdUnitId = "6fb959c7b9a69602";
#elif UNITY_ANDROID
    private const string MRecAdUnitId = "038cfaab8a9e95a1";
#elif UNITY_IOS
    private const string MRecAdUnitId = "5e77bf34cafd2da5";
#endif
    
    bool isLoadedMrec;
    public bool isShowMrec;
    public void InitializeMRecAds()
    {
        if (UseProfile.IsRemoveAds ||  !enableMrec)
            return;
        // Attach Callbacks
        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent +=
            OnAdRevenuePaidEvent; // MRECs are automatically sized to 300x250. show bottom trên banner ads khoảng 50px vì banner ads có height là 50px
        var density = MaxSdkUtils.GetScreenDensity();
        float mrecWidth = 300f;
        float mrecHeight = 250f;
        float bannerHeight = Screen.width / 6.4f;
        float mrecPosX = (Screen.width / density - mrecWidth) / 2f;
        
        float mrecPosY = Screen.safeArea.height / density - bannerHeight / density - mrecHeight - 10f;
        MaxSdk.CreateMRec(MRecAdUnitId, mrecPosX,
            mrecPosY);
    }

    public void ShowMRec()
    {
#if THANH_TESTER
        if (UseProfile.IsCheat) return;
#endif
        Debug.Log("MREC=============call show mrec" + EnableMrec());
        if (UseProfile.IsRemoveAds || !EnableMrec())
            return;

        if (isLoadedMrec)
        {
            if (_isShowingCollapse) return;
            isShowMrec = true;
            MaxSdk.ShowMRec(MRecAdUnitId);
            this.PostEvent(EventID.SHOW_MREC);
            Debug.Log("MREC============= show mrec" + EnableMrec());
        }
        else
        {
            Debug.Log("MREC============= not loaded mrec");
        }
    }
    public void HideMRec()
    {
        if (!EnableMrec())
            return;
        //Debug.Log("MREC=============hide mrec");
        isShowMrec = false;
        MaxSdk.HideMRec(MRecAdUnitId);
    }

    private void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isLoadedMrec = true;
        // MRec ad is ready to be shown.
        // If you have already called MaxSdk.ShowMRec(MRecAdUnitId) it will automatically be shown on the next MRec refresh.
        Debug.Log("MREC=============MRec ad loaded");
        if (isShowMrec) ShowMRec();
    }

    private void OnMRecAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        isLoadedMrec = false;
        this.RemoveListener(EventID.SHOW_MREC, null);
        // MRec ad failed to load. MAX will automatically try loading a new ad internally.
        Debug.Log("MREC=============MRec ad failed to load with error code: " + errorInfo.Code);
    }

    private void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("MREC=============MRec ad clicked");
    }

    #endregion

    #region IdAdmobBanner

#if TESTER
        private const string BANNER_ADMOB_ID = "ca-app-pub-8467610367562059/3591940548";
        private const string BANNER_ADMOB_COLLAP = "ca-app-pub-3940256099942544/2014213617";
#elif UNITY_ANDROID
        private const string BANNER_ADMOB_ID = "ca-app-pub-8467610367562059/9428848203";
        private const string BANNER_ADMOB_COLLAP = "ca-app-pub-8467610367562059/8443069596";
#elif UNITY_IOS
        private const string BANNER_ADMOB_ID = "ca-app-pub-8467610367562059/1461238907";
        private const string BANNER_ADMOB_COLLAP = "ca-app-pub-8467610367562059/5651781262";
#endif
    #endregion
    
    #region  Collapse
    
    private bool       _isReloadBannerCollapse;
    private bool       _isLoadedCollapse;
    private bool       _isShowingCollapse;
    private bool       _isShowAdmobCollapse;
    private float      _timeReloadBannerCollapse;
    private float      _timerReloadBannerCollapse;
    private DateTime   _lastRefreshCollapseBanner;
    private AdRequest  _bannerCollapseLoadRequest;
    private BannerView _bannerViewCollapse;
    
    
     private void RefreshBannerCollapse()
    {
        Debug.Log("refresh collapse banner ==========");
        _bannerViewCollapse?.LoadAd(_bannerCollapseLoadRequest);
    }
    private void LoadBannerCollapse()
    {
        Debug.Log("load banner collap=========");
        AdSize adaptiveSize =
            AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerViewCollapse?.Destroy();
        _bannerViewCollapse = new BannerView(BANNER_ADMOB_COLLAP, adaptiveSize, AdPosition.Bottom);
        _bannerViewCollapse.Hide();
        _bannerCollapseLoadRequest = new AdRequest();
        //Admob request
        _bannerCollapseLoadRequest.Extras.Add("collapsible", "bottom");

        _bannerViewCollapse.LoadAd(_bannerCollapseLoadRequest);
        _bannerViewCollapse.OnBannerAdLoaded += OnBannerAdLoaded;
        _bannerViewCollapse.OnAdPaid += OnAdPaid;
        _bannerViewCollapse.OnBannerAdLoadFailed += OnBannerAdLoadFailed;
        _bannerViewCollapse.OnAdFullScreenContentClosed += () =>
        {
            // HideBannerCollap();
        };
        _timerReloadBannerCollapse = 0;
        _isReloadBannerCollapse = false;
        _timeReloadBannerCollapse = RemoteConfigController.GetFloatConfig(FirebaseConfig.RELOAD_BANNER_COLLAPSE_TIME, 75);
        _isShowAdmobCollapse = RemoteConfigController.GetBoolConfig(FirebaseConfig.ENABLE_ADMOB_BANNER_COLLAP, true);
        
        void OnBannerAdLoaded()
        {
            _isReloadBannerCollapse = false;
            _isLoadedCollapse = true;
            Debug.Log("load admob banner collap  success=====");
            // ShowBannerCollapse();
        }
        void OnAdPaid(AdValue adValue)
        {
            var revenue = adValue.Value / 1000000d;
            try
            {
                AnalyticsController.LogAdsRevenue("admob", "collapsible", "collapsible", revenue, adValue.CurrencyCode,
                    MediationType.Admob, true);
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        void OnBannerAdLoadFailed(LoadAdError error)
        {
            Debug.Log("load admob banner collapse fail=====");
            _isLoadedCollapse = false;
            _isReloadBannerCollapse = true;
            _timerReloadBannerCollapse = 30;
            _timeReloadBannerCollapse = RemoteConfigController.GetFloatConfig(FirebaseConfig.RELOAD_BANNER_COLLAPSE_TIME, 75);
            ShowBanner();
        }
    }


    public void ShowBannerCollapse()
    {
        if (UseProfile.IsRemoveAds || UseProfile.IsCheat/*|| UseProfile.IsVip|| UseProfile.IsTryRemoveAds*/)
            return;

        if (_isShowAdmobCollapse)
        {
            if (_isShowingCollapse) return;
            if (_isLoadedCollapse)
            {
                Debug.Log("show collapse");
                // _cooldownAdmobBannerTimer =
                // RemoteConfigController.GetFloatConfig(FirebaseConfig.COOLDOWN_ADMOB_REFRESH, 30);
                Debug.Log("show banner collapse===========");
                _bannerView?.Hide();
                MaxSdk.HideBanner(BanerAdUnitId);
                _bannerViewCollapse?.Show();
                _isShowingCollapse = true;
            }
        }
    }
    #endregion

    #region Common

    public bool IsCooldownAdmobBanner
    {
        get => _isCooldownAdmobBanner;
        set
        {
            _isCooldownAdmobBanner = value;
            var cooldownAdmobBannerTimer =
                RemoteConfigController.GetFloatConfig(FirebaseConfig.MINIMUM_TIME_SHOW_COLLAPSE, 5);
            if (_cooldownAdmobBannerTimer < cooldownAdmobBannerTimer)
            {
                _cooldownAdmobBannerTimer =
                    cooldownAdmobBannerTimer;
            }
        }
    }
    private bool _isShowAdmobBanner;
    private bool _isCooldownAdmobBanner;
    private bool _isTryLoadAdmobBanner;
    private double _cooldownAdmobBannerTimer;
    private BannerView _bannerView;
    private AdRequest _bannerHomeLoadRequest;

    
    private void InitBannerAdmob()
    {
        LoadBannerAdmob();
        LoadBannerCollapse();
    }
    
    private void RefreshAdmobBanner()
    {
        Debug.Log("refresh banner admob======");
        _isTryLoadAdmobBanner = true;
        _bannerView?.LoadAd(_bannerHomeLoadRequest);
    }
    public void LoadBannerAdmob()
    {
        AdSize adaptiveSize =
             AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView?.Destroy();
        _bannerView = new BannerView(BANNER_ADMOB_ID, adaptiveSize, AdPosition.Bottom);
        _bannerView.Hide();
        _bannerHomeLoadRequest = new AdRequest();
        _lastRefreshCollapseBanner = DateTime.Now;
        IsCooldownAdmobBanner = true;
        //Admob request
        //bannerHomeLoadRequest.Extras.Add("collapsible", "bottom");

        // bannerHomeLoadRequest.Extras.Add("collapsible_request_id", Guid.NewGuid().ToString());
        _isTryLoadAdmobBanner = true;
        _bannerView.LoadAd(_bannerHomeLoadRequest);

        _bannerView.OnBannerAdLoaded += OnBannerAdLoaded;

        _bannerView.OnAdPaid += OnAdPaid;

        _bannerView.OnBannerAdLoadFailed += OnBannerAdLoadFailed;

        void OnBannerAdLoaded()
        {
            Debug.Log("load admob banner success=====");
            _isTryLoadAdmobBanner = false;
            var isEnableAdmobBanner = RemoteConfigController.GetBoolConfig(FirebaseConfig.ENABLE_ADMOB_BANNER, true);
            _isShowAdmobBanner = isEnableAdmobBanner;
            _cooldownAdmobBannerTimer =
                RemoteConfigController.GetFloatConfig(FirebaseConfig.COOLDOWN_ADMOB_REFRESH, 30);
            isLoadedBannerAdmob = true;
            ShowBanner();
        }

        void OnAdPaid(AdValue adValue)
        {
            var revenue = adValue.Value / 1000000d;
            try
            {
                AnalyticsController.LogAdsRevenue("admob", "banner", "banner", revenue, adValue.CurrencyCode,
                    MediationType.Admob, true);
            }
            catch (Exception e)
            {
                // ignored
            }
        }

        void OnBannerAdLoadFailed(LoadAdError error)
        {
            Debug.Log("load admob banner fail=====");
            isLoadedBannerAdmob = false;
            _isTryLoadAdmobBanner = true;
            _cooldownAdmobBannerTimer = 15;
            ShowBanner();
        }
    }

    #endregion

    public void InitializeBannerMax()
    {
        MaxSdkCallbacks.OnBannerAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.OnBannerAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.OnBannerAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        MaxSdk.CreateBanner(BanerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerBackgroundColor(BanerAdUnitId, Color.clear);
        MaxSdk.SetBannerExtraParameter(BanerAdUnitId, "adaptive_banner", "true");
        ShowBanner();
    }

    private void OnBannerAdLoadedEvent(string obj)
    {
        Debug.Log("Request success");
        if (reloadBannerCoru != null)
        {
            StopCoroutine(reloadBannerCoru);
            reloadBannerCoru = null;
        }

        isLoadedBannerMax = true;
        Debug.Log("Loaded banner max");
        ShowBanner();
    }

    private void OnBannerAdClickedEvent(string obj)
    {
        //inter click
        Debug.Log("Click Baner !!!");
    }

    private void OnBannerAdLoadFailedEvent(string arg1, int arg2)
    {
        if (reloadBannerCoru != null)
        {
            StopCoroutine(reloadBannerCoru);
            reloadBannerCoru = null;
        }

        isLoadedBannerMax = false;
        Debug.Log("Load Failed Banner Max");
        reloadBannerCoru = Helper.StartAction(() => { ShowBanner(); }, 0.3f);
        StartCoroutine(reloadBannerCoru);
    }

    private void InitRewardVideo()
    {
        InitializeRewardedAds();
    }

    #region Interstitial

    private void OnInterstitialLoadedEvent(string adUnitId)
    {
        _isLoading = true;
        GameController.Instance.AnalyticsController.LogInterReady();
    }

    private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
    {
        _isLoading = false;
        actionInterstitialClose?.Invoke();
        actionInterstitialClose = null;
        Invoke("RequestInterstitial", 3);
    }
    void RefeshCloseAds()
    {
        isShowingAds = false;
    }

    private void RequestInterstitial()
    {
        if (_isLoading) return;

        MaxSdk.LoadInterstitial(InterstitialAdUnitId);
        GameController.Instance.AnalyticsController.LogInterLoad();
        _isLoading = true;
    }

    private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
    {
        _isLoading = false;
        actionInterstitialClose?.Invoke();
        actionInterstitialClose = null;
        RequestInterstitial();
    }

    private void OnInterstitialHiddenEvent(string adUnitId)
    {
        _isLoading = false;
        Debug.Log("InterstitialAdClosedEvent");
        Time.timeScale = 1;

        _actionRewardVideo?.Invoke();
        _actionRewardVideo = null;

        _actionClose?.Invoke();
        _actionClose = null;

        actionInterstitialClose?.Invoke();
        actionInterstitialClose = null;
        countdownAdsclick = 0;
        countdownAdsInterImage = 0;
        RequestInterstitial();
        ResetCoolDownTime();
        Invoke("RefeshCloseAds", 1);

        //if (GamePlayControl.Instance != null)
        //{
        //    GamePlayControl.Instance.timer = 0;
        //}
    }

    public void ResetCoolDownTime()
    {
        countdownAdsclick = 0;
        countdownAdsInterImage = 0;
        AppOpenAdManager.Instance.ResetCoolDownTime();
    }
    
    private void MaxSdkCallbacks_OnInterstitialDisplayedEvent(string adUnitId)
    {
        Debug.Log("InterstitialAdOpenedEvent");
        _isLoading = false;
        Time.timeScale = 0;
    }

    private void MaxSdkCallbacks_OnInterstitialClickedEvent(string adUnitId)
    {
        GameController.Instance.AnalyticsController.LogInterClick();
        _isLoading = false;
    }

    public bool ShowInterstitial(bool isShowImmediately = false, string actionWatchLog = "other",
        UnityAction actionIniterClose = null, UnityAction actionIniterShow = null, string level = null,
        bool isShowBreakAds = true)
    {
        blackImage.gameObject.SetActive(true);
        actionIniterClose += () => { blackImage.gameObject.SetActive(false); };
        if (UseProfile.IsRemoveAds /*|| UseProfile.IsVip*/ || UseProfile.IsCheat)
        {
            actionIniterClose?.Invoke();
            return false;
        }

        if (countdownAdsclick > RemoteConfigController.GetFloatConfig(FirebaseConfig.DELAY_SHOW_INITSTIALL, 30) && !isShowImmediately)
        {
            actionIniterClose?.Invoke();
            return false;
        }
        ShowInterstitialHandle(actionWatchLog, actionIniterClose);
        
        return true;
    }
    

    public bool ShowInterstitialClick(bool isShowImmediatly = false, string actionWatchLog = "other",
        UnityAction actionIniterClose = null, UnityAction actionIniterShow = null, string level = null,
        bool isInGame = false, bool showSuggestNoAds = true)
    {
        blackImage.gameObject.SetActive(true);
        actionIniterClose += () => { blackImage.gameObject.SetActive(false); };
        
        if (UseProfile.IsRemoveAds || UseProfile.IsCheat)
        {
            if (!isInGame)
                actionIniterClose?.Invoke();
            return false;
        }

        if ((countdownAdsclick > GameConfig.Instance.coolDownInterAds ||
            isShowImmediatly))
        {
            Debug.Log("show inter click ========");
                ShowInterstitialHandle(actionWatchLog, actionIniterClose, isInGame: isInGame);
        }
        else
        {
            if (!isInGame)
                if (actionIniterClose != null)
                    actionIniterClose();
        }
        
        return true;
    }

    public bool IsLoadedInterstitial()
    {
        return MaxSdk.IsInterstitialReady(InterstitialAdUnitId);
    }

    private void ShowInterstitialHandle(string actionWatchLog = "other",
        UnityAction actionIniterClose = null, string level = null, bool isInGame = false)
    {
        actionIniterClose += () => { IsCooldownAdmobBanner = true; };
        var close = actionIniterClose;
        actionIniterClose = () => { ShowInterstitialAd(true, close); };
        if (IsLoadedInterstitial())
        {
            isShowingAds = true;
            if (isInGame)
            {
                Debug.Log("show inter ======");
                AdsBreakPopUp.Setup().Show(() =>
                {
                    this.actionInterstitialClose = actionIniterClose;
                    IsCooldownAdmobBanner = false;
                    MaxSdk.ShowInterstitial(InterstitialAdUnitId, actionWatchLog);

                    countdownAdsclick = 0;
                    GameController.Instance.AnalyticsController.LogInterShow(actionWatchLog);

                    UseProfile.NumberOfAdsInDay = UseProfile.NumberOfAdsInDay + 1;
                    UseProfile.NumberOfAdsInPlay = UseProfile.NumberOfAdsInPlay + 1;
                });
            }
            else
            {
                this.actionInterstitialClose = actionIniterClose;
                IsCooldownAdmobBanner = false;
                MaxSdk.ShowInterstitial(InterstitialAdUnitId, actionWatchLog);

                countdownAdsclick = 0;
                GameController.Instance.AnalyticsController.LogInterShow(actionWatchLog);

                UseProfile.NumberOfAdsInDay = UseProfile.NumberOfAdsInDay + 1;
                UseProfile.NumberOfAdsInPlay = UseProfile.NumberOfAdsInPlay + 1;
            }
        }
        else
        {
            if (!isInGame)
                if (actionIniterClose != null)
                    actionIniterClose();
            RequestInterstitial();
        }
    }

    private void InitInterstitial()
    {
        MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
        MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.OnInterstitialClickedEvent += MaxSdkCallbacks_OnInterstitialClickedEvent;
        MaxSdkCallbacks.OnInterstitialDisplayedEvent += MaxSdkCallbacks_OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;

        RequestInterstitial();

        // MaxSdkCallbacks.
    }

    public float countdownAdsclick;
    public float countdownAdsInterImage;

    #endregion

    #region Inter admob

    // These ad units are configured to always serve test ads.

#if TESTER
    private string _adUnitIdInter = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_ANDROID
    private string _adUnitIdInter = "ca-app-pub-8467610367562059/2211992727";
#elif UNITY_IPHONE
  private string _adUnitIdInter = "ca-app-pub-8467610367562059/5655666608";
#endif

    private InterstitialAd _interstitialAd;

    /// <summary>
    /// Loads the interstitial ad.
    /// </summary>
    public void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");
        GameController.Instance.AnalyticsController.LogInterLoad();
        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitIdInter, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                _interstitialAd = ad;
                RegisterEventHandlers(_interstitialAd);
            });
    }

    public bool CheckCanShowShortInter()
    {
        if (_interstitialAd == null || !_interstitialAd.CanShowAd())
        {
            LoadInterstitialAd();
        }
        
#if UNITY_EDITOR || TESTER
        return (!UseProfile.IsRemoveAds && !UseProfile.IsCheat && _interstitialAd != null && _interstitialAd.CanShowAd()
                                        && RemoteConfigController.GetBoolConfig(FirebaseConfig.ENABLE_SHORT_INTER,
                                            true));
#else
        return (!UseProfile.IsRemoveAds && !UseProfile.IsCheat && _interstitialAd != null && _interstitialAd.CanShowAd()
                                        && RemoteConfigController.GetBoolConfig(FirebaseConfig.ENABLE_SHORT_INTER,
                                            false));
#endif
    }

    /// <summary>
    /// Shows the interstitial ad.
    /// </summary>
    public bool ShowInterstitialAd(bool isShowImmediatly, UnityAction actionIniterClose = null)
    {
        Debug.Log("Check Enable Short Inter:" +
                       RemoteConfigController.GetBoolConfig(FirebaseConfig.ENABLE_SHORT_INTER, false));
        if (UseProfile.IsRemoveAds)
        {
            actionIniterClose?.Invoke();
            return false;
        }

        if (isShowImmediatly || countdownAdsInterImage >
            RemoteConfigController.GetFloatConfig(FirebaseConfig.DELAY_SHOW_INTER_IMAGE, 15))
        {
            if (CheckCanShowShortInter())
            {
                Debug.Log("Showing interstitial ad.");
                UseProfile.NumberOfAdsInDay = UseProfile.NumberOfAdsInDay + 1;
                UseProfile.NumberOfAdsInPlay = UseProfile.NumberOfAdsInPlay + 1;
                GameController.Instance.AnalyticsController.LogInterShow("LoadingInter");
                actionShortInterstitialClose = actionIniterClose;
                _interstitialAd.Show();
            }
            else
            {
                Debug.LogError("Interstitial ad is not ready yet.");
                if (_interstitialAd == null || !_interstitialAd.CanShowAd())
                {
                    LoadInterstitialAd();
                }
                if (actionIniterClose != null)
                    actionIniterClose();
            }
        }
        else
        {
            if (actionIniterClose != null)
                actionIniterClose();
        }
        // if (actionIniterClose != null)
        //     actionIniterClose();

        return true;
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
            if (adValue == null) return;
            double value = adValue.Value * 0.000001f;
            AnalyticsController.LogAdsRevenue("admob", "inter", "inter", value, adValue.CurrencyCode,
                MediationType.Admob, true);
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () => { Debug.Log("Interstitial ad recorded an impression."); };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () => { Debug.Log("Interstitial ad was clicked."); };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            isShowingAds = true;
            Time.timeScale = 0;
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            StartCoroutine(OnAdFullScreenContentClosed());
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            StartCoroutine(OnAdFullScreenContentClosed());
            LoadInterstitialAd();
        };
    }

    private IEnumerator OnAdFullScreenContentClosed()
    {
        yield return null;
        try
        {
            this.actionShortInterstitialClose?.Invoke();
            actionShortInterstitialClose = null;
            isShowingAds = false;
            Time.timeScale = 1;
            countdownAdsInterImage = 0;
            Debug.Log("Interstitial ad full screen content closed.");
            LoadInterstitialAd();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    #endregion
    
    #region Video Reward

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(RewardedAdUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId)
    {
        GameController.Instance.AnalyticsController.LogVideoRewardReady();
    }

    private void OnRewardedAdFailedEvent(string adUnitId, int errorCode)
    {
        Debug.Log("Rewarded ad failed to load with error code: " + errorCode);
        Invoke("LoadRewardedAd", 15);
        if (actionWatchVideo != null) GameController.Instance.AnalyticsController.LogVideoRewardLoadFail(actionWatchVideo.ToString(),
            errorCode.ToString());
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
    {
        Debug.Log("Rewarded ad failed to display with error code: " + errorCode);
        isVideoDone = false;

        //if (IsLoadedInterstitial())
        //{
        //    ShowInterstitial(isShowImmediatly: true);
        //}
        //else
        //{
        //    //ConfirmBox.Setup().AddMessageYes(Localization.Get("s_noti"), Localization.Get("s_TryAgain"), () => { });
        //}
        LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId)
    {
        Debug.Log("Rewarded ad displayed " + isVideoDone);
        isVideoDone = false;
    }

    private void OnRewardedAdClickedEvent(string adUnitId)
    {
        Debug.Log("Rewarded ad clicked");
        isVideoDone = true;
        GameController.Instance.AnalyticsController.LogClickToVideoReward(actionWatchVideo.ToString());
    }

    private void OnRewardedAdDismissedEvent(string adUnitId)
    {
        //if (GamePlayControl.Instance != null)
        //{
        //    GamePlayControl.Instance.timer = 0;
        //}

        // Rewarded ad is hidden. Pre-load the next ad
        Debug.Log("Rewarded ad dismissed");
        _actionClose?.Invoke();
        _actionClose = null;
        _actionRewardVideo = null;
        LoadRewardedAd();
    }

    bool isVideoDone;

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
    {
        // Rewarded ad was displayed and user should receive the reward
        Debug.Log("Rewarded ad received reward");
        isShowingAds = false;
        isVideoDone = true;
        _actionRewardVideo?.Invoke();
        _actionRewardVideo = null;
        ResetCoolDownTime();
        countdownAdsclick = 0;
        GameController.Instance.AnalyticsController.LogVideoRewardShowDone(actionWatchVideo.ToString());
    }

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        // Load the first RewardedAd
        LoadRewardedAd();
    }

    public bool IsLoadedVideoReward()
    {
        var result = MaxSdk.IsRewardedAdReady(RewardedAdUnitId);
        if (!result)
        {
            RequestInterstitial();
        }

        return result;
    }

    /// <summary>
    /// Xử lý Show Video
    /// </summary>
    /// <param name="actionReward">Hành động khi xem xong Video và nhận thưởng </param>
    /// <param name="actionNotLoadedVideo"> Hành động báo lỗi không có video để xem </param>
    /// <param name="actionClose"> Hành động khi đóng video (Đóng lúc đang xem dở hoặc đã xem hết) </param>
    public bool ShowVideoReward(UnityAction actionReward, UnityAction actionNotLoadedVideo, UnityAction actionClose,
        string actionType, string level = "")
    {
        if (UseProfile.IsCheat)
        {
            actionReward?.Invoke();
            return true;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable && GameConfig.Instance.showNoInternet)
        {
            GameController.Instance.InitCheckInternet();
            actionNotLoadedVideo?.Invoke();
            GameController.Instance.AnalyticsController.LogWatchVideo(actionType, true, false, level);
            return false;
        }
        
        actionWatchVideo = actionType;
        GameController.Instance.AnalyticsController.LogRequestVideoReward(actionType.ToString());
        GameController.Instance.AnalyticsController.LogVideoRewardEligible();
        if (IsLoadedVideoReward())
        {
            isShowingAds = true;
            ResetCoolDownTime();
            
            countdownAdsclick = 0;
            this._actionNotLoadedVideo = actionNotLoadedVideo;
            this._actionClose = actionClose;
            this._actionRewardVideo = actionReward;

            MaxSdk.ShowRewardedAd(RewardedAdUnitId, actionType.ToString());
            GameController.Instance.AnalyticsController.LogWatchVideo(actionType, true, true, level);
            GameController.Instance.AnalyticsController.LogVideoRewardShow(actionWatchVideo.ToString());
        }
        else
        {
            if (IsLoadedInterstitial())
            {
                this._actionNotLoadedVideo = actionNotLoadedVideo;
                this._actionClose = actionClose;
                this._actionRewardVideo = actionReward;

                ShowInterstitial( false,"other", actionIniterClose: () =>
                {
                },null);
                GameController.Instance.AnalyticsController.LogWatchVideo(actionType, true, true, level);
                ResetCoolDownTime();
                countdownAdsclick = 0;
                return true;
            }
            else
            {
                //ConfirmBox.Setup().AddMessageYes(Localization.Get("s_noti"), Localization.Get("s_TryAgain"), () => { });
                actionNotLoadedVideo?.Invoke();
                GameController.Instance.AnalyticsController.LogWatchVideo(actionType, false, true, level);
                return false;
            }
        }

        return true;
    }

    #endregion

    #region Banner
    private bool isLoadedBannerAdmob;
    private bool isLoadedBannerMax;
    public void ShowBanner()
    {
        Debug.Log("Call Show banner");
        if (UseProfile.IsRemoveAds || _isShowingCollapse || UseProfile.IsCheat/*|| UseProfile.IsVip || UseProfile.IsTryRemoveAds*/)
            return;
        
        if (isMaxInitDone && isLoadedBannerMax)
        {
            ShowBannerMax();
        }
        else if (isAdmobInitDone && isLoadedBannerAdmob && _isShowAdmobBanner)
        {
            ShowBannerAdmob();
        }
    }

    public void ShowBannerAdmob()
    {
        Debug.Log("ShowBannerAdmob");
        MaxSdk.HideBanner(BanerAdUnitId);
        _bannerView?.Show();
        _bannerViewCollapse?.Hide();
    }

    public void ShowBannerMax()
    {
        Debug.Log("ShowBannerMax");
        MaxSdk.ShowBanner(BanerAdUnitId);
        _bannerView?.Hide();
        _bannerViewCollapse?.Hide();
    }
    float timerCooldownDestroyCollap = 0;
    public void DestroyBanner()
    {
        Debug.Log("destroy banner");
        MaxSdk.HideBanner(BanerAdUnitId);
        _bannerView?.Hide();
        _bannerViewCollapse?.Hide();
    }
    
    public void DestroyBannerCollapse()
    {
        Debug.Log("destroy banner admob collap========");
        _isShowingCollapse = false;
        _isLoadedCollapse = false;  
        timerCooldownDestroyCollap = 0;
        // _cooldownAdmobBannerTimer = RemoteConfigController.GetFloatConfig(FirebaseConfig.COOLDOWN_ADMOB_REFRESH, 30);
        _bannerViewCollapse.Hide();
        ShowBanner();
        IsCooldownAdmobBanner = true;
        RefreshBannerCollapse();
    }

    #endregion

    #region Open App Ads

    DateTime oldTime = DateTime.MinValue;

    public void OnAppStateChanged(AppState state)
    {
        if (isAdmobInitDone && SceneManager.GetActiveScene().name != "Loading")
            if (state == AppState.Foreground && TimeManager.CaculateTime(oldTime, DateTime.Now) > 30 && !isShowingAds)
            {
                // COMPLETE: Show an app open ad if available.
                AppOpenAdManager.Instance.ShowAdIfAvailable();
                oldTime = DateTime.Now;
            }
    }

    #endregion

    #region Reward Inter

    public void LoadRewardedInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedInterstitialAd != null)
        {
            rewardedInterstitialAd.Destroy();
            rewardedInterstitialAd = null;
        }

        Debug.Log("Loading the rewarded interstitial ad.");
        // create our request used to load the ad.
        var adRequest = new AdRequest();
        // send the request to load the ad.
        RewardedInterstitialAd.Load(_adUnitIdLow, adRequest, 
            (RewardedInterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("rewarded interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    //LoadMoreRewardInter(_adUnitIdMedium);
                    return;
                }

                Debug.Log("Rewarded interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedInterstitialAd = ad;
                InitRegister(ad);
            });
    }
    
    private UnityAction actionRewardInter;
    private UnityAction actionCloseRewardInter;
    private bool isRewardInter;


    private void HandleAdPaidEvent(AdValue adValue)
    {
        double revenue = adValue.Value / 1000000d;

        AnalyticsController.LogAdsRevenue("admob", "reward_inter",
            "reward_inter", revenue, "USD", MediationType.Admob, false);
    }

    void InitRegister(RewardedInterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += OnAdContentclose;
        ad.OnAdPaid += HandleAdPaidEvent;
    }

    private void OnAdContentclose()
    {
        Debug.Log("=======close");
        Invoke("CloseAdReward", 0.1f);
        if (actionCloseRewardInter != null)
            actionCloseRewardInter.Invoke();
        LoadRewardedInterstitialAd();
    }

    void CloseAdReward()
    {
        if (isRewardInter)
            if (actionRewardInter != null)
            {
                actionRewardInter.Invoke();
                actionRewardInter = null;
            }

        isRewardInter = false;
    }

    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        double revenue = impressionData.Revenue;
        AnalyticsController.LogAdsRevenue(impressionData.NetworkName, impressionData.AdUnitIdentifier,
            impressionData.AdFormat, revenue, "USD", MediationType.Max, false);
    }

    #endregion

    private void Update()
    {
        // if (_isShowAdmobBanner || _isTryLoadAdmobBanner)
        // {
        //     if (IsCooldownAdmobBanner)
        //     {
        //         _cooldownAdmobBannerTimer -= Time.unscaledDeltaTime;
        //         if (_cooldownAdmobBannerTimer <= 0)
        //         {
        //             RefreshAdmobBanner();
        //             _cooldownAdmobBannerTimer = 30;
        //         }
        //     }
        // }
        if (_isReloadBannerCollapse)
        {
            _timerReloadBannerCollapse += Time.deltaTime;
            if (_timerReloadBannerCollapse >= _timeReloadBannerCollapse)
            {
                RefreshBannerCollapse();
                _timerReloadBannerCollapse = 0;
            }
        }
        if (_isShowingCollapse)
        {
            IsCooldownAdmobBanner = false;
            timerCooldownDestroyCollap += Time.deltaTime;
            if (timerCooldownDestroyCollap > 30)
            {
                DestroyBannerCollapse();
            }
        }
        // if (GamePlayControl.Instance != null)
        countdownAdsclick += Time.deltaTime;
        countdownAdsInterImage += Time.deltaTime;
    }
}