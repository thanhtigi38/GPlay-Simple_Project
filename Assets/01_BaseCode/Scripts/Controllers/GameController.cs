using System;
using GPlay.GplayGoogleUMP;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EventDispatcher;
using Sirenix.OdinInspector;
using ThanhND;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public MoneyEffectController moneyEffectController;
    public UseProfile useProfile;
    public DataContain dataContain;
    public MusicManager musicManager;
    public AdsController admobAds;
    public AnalyticsController AnalyticsController;
    public IapController iapController;
    [HideInInspector] public SceneType currentScene;
    public bool isTest;
    [SerializeField] TextAsset firstGame;
    [SerializeField] Slider progressBar;
    [SerializeField] Text txtProgress;
    public bool canShowAppOpen = true;
    Coroutine coroutineStartGame;
    public GiftDatabase giftDatabase;
    

    protected void Awake()
    {
        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(this);
    }
    
    private void Start()
    {
        UseProfile.NumberOfAdsInPlay = 0;
        Application.targetFrameRate = 60;
        //adsManager.Init();
        musicManager.Init();
        //coroutineStartGame = StartCoroutine(LoadingScene());
        if (UseProfile.NewUser)
        {
            UseProfile.NewUser = false;
            LoadingPanel.Instance.GotoScene(SceneName.GAME_PLAY,true, () =>
            {
            });
        }
        else
        {
            LoadingPanel.Instance.GotoScene(SceneName.HOME_SCENE, true,() =>
            {
            });
        }
#if THANH_TESTER
        SRDebug.Init();
#endif
        
        GplayUMP.ShowConsentForm((consert) =>
        {
            UseProfile.IsTrackedPremission = consert;
#if UNITY_IOS
                if(ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == 
                ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {

                    ATTrackingStatusBinding.RequestAuthorizationTracking();
                }
#endif
            Init();
        });
    }

    public void Init()
    {
        admobAds.Init();
        
        //LoadFirstGame();
        //iapController.Init();
        //MMVibrationManager.SetHapticsActive(useProfile.OnVibration);
        //UseProfile.IsVip = true;
#if UNITY_EDITOR || THANH_TESTER
        Debug.unityLogger.logEnabled = true;
#else
  Debug.unityLogger.logEnabled = false;
#endif
    }

    public void LoadScene(string sceneName)
    {
        Initiate.Fade(sceneName.ToString(), Color.black, 1.2f);
    }

    void LoadFirstGame()
    {
        StartCoroutine(LoadLevelAsync(SceneName.HOME_SCENE));
    }

    private IEnumerator LoadLevelAsync(string scene)
    {
        yield return new WaitForSeconds(1);
        StopCoroutine(coroutineStartGame);
        AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
        progressBar.value = 1;
        txtProgress.text = 100 + "%";
        yield break;
        Time.timeScale = 1;
    }

    public static void SetUserProperties()
    {
    }

    public void ShowPanelPermissionSetting(int typePermission)
    {
    }

    public void ShowTotalPumpkin()
    {
        //RecivePumpkinController recive = Instantiate(pumpkinController);
        //recive.InitPanel();
    }
    public void InitCheckInternet(bool setContinueGame = false)
    {
            // var checkInternetPopUp = Instantiate(Resources.Load<CheckInternetPopUp>(PathPrefabs.CHECK_INTERNET_POP_UP));
            // if (setContinueGame) checkInternetPopUp.SetContinueGame();
    }
}

public enum SceneType
{
    StartLoading = 0,
    MainHome = 1,
    GamePlay = 2
}