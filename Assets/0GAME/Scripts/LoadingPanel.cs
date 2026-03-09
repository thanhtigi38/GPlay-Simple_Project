using System;
using System.Collections;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ThanhND
{
    public class LoadingPanel : MonoBehaviour
    {
        public static LoadingPanel Instance;
        [SerializeField] private CanvasGroup canvas = default;
        [SerializeField] private Image slider = default;
        [SerializeField] private Text loadingText;
        private bool isLoad = false;
        [HideInInspector] public bool isShowOpen = false;

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            isShowOpen = false;
        }

        public void ActiveScene(Action action = null, bool showInter = true)
        {
            StartCoroutine(IEActiveScene(showInter, action));
        }

        private IEnumerator IEActiveScene(bool showInter, Action action = null)
        {
            yield return new WaitUntil(() => isLoad);
            GameController.Instance.admobAds.HideMRec();
            if (!isShowOpen)
            {
#if UNITY_EDITOR || TESTER
                if (RemoteConfigController.GetBoolConfig(FirebaseConfig.ENABLE_INTER_START, true)
                    && GameController.Instance.admobAds.CheckCanShowShortInter())
#else
                if (RemoteConfigController.GetBoolConfig(FirebaseConfig.ENABLE_INTER_START, true)
                    && GameController.Instance.admobAds.CheckCanShowShortInter())
#endif
                {
                    GameController.Instance.admobAds.ShowInterstitialAd(true, (() =>
                    {
                        isShowOpen = true;
                        action?.Invoke();
                        canvas.DOFade(0f, 0.4f).OnComplete(() => { canvas.gameObject.SetActive(false); });
                    }));
                }
                else
                {
                    AppOpenAdManager.Instance.ShowAdIfAvailable(() =>
                    {
                        isShowOpen = true;
                        action?.Invoke();
                        canvas.DOFade(0f, 0.4f).OnComplete(() => { canvas.gameObject.SetActive(false); });
                    });
                }
            }
            else
            {
                Debug.Log("Check show inter start gameplay: " +
                          RemoteConfigController.GetBoolConfig(FirebaseConfig.ENABLE_INTER_START_GAMEPLAY, true)
                          + " - showInter: " + showInter);
                if (RemoteConfigController.GetBoolConfig(FirebaseConfig.ENABLE_INTER_START_GAMEPLAY, true) || showInter)
                {
                    GameController.Instance.admobAds.ShowInterstitialClick(actionIniterClose: (() =>
                    {
                        action?.Invoke();
                        canvas.DOFade(0f, 0.4f).OnComplete(() => { canvas.gameObject.SetActive(false); });
                    }));
                }
                else
                {
                    action?.Invoke();
                    canvas.DOFade(0f, 0.4f).OnComplete(() => { canvas.gameObject.SetActive(false); });
                }
            }
        }

        public void GotoScene(string scene, bool showMrec, Action action = null)
        {
            isLoad = false;
            if (showMrec) GameController.Instance.admobAds.ShowMRec();
            StartCoroutine(FadeOutScene(scene, action));
        }

        IEnumerator FadeInScene(Action action = null)
        {
            yield return new WaitForFixedUpdate();
            action?.Invoke();
            if (!isLoad)
            {
                isLoad = true;
            }
        }

        IEnumerator FadeOutScene(string scene, Action action = null)
        {
            slider.fillAmount = 0;
            loadingText.text = "0%";
            canvas.alpha = isShowOpen ? 0 : 1;
            canvas.DOFade(1, 0.4f);
            canvas.gameObject.SetActive(true);
            float timerPro = 3;
            while (timerPro > 0)
            {
                timerPro -= Time.fixedDeltaTime;
                slider.fillAmount += Time.fixedDeltaTime / 6;
                loadingText.text = ((int)Mathf.Round(slider.fillAmount * 100)) + "%";
                yield return new WaitForFixedUpdate();
            }

            string currentScene = SceneManager.GetActiveScene().name;

            float sliderValue = slider.fillAmount;
            AsyncOperation async = SceneManager.LoadSceneAsync(scene);
            async.allowSceneActivation = false;
            while (!async.isDone)
            {
                slider.fillAmount = sliderValue + async.progress / 4;
                loadingText.text = ((int)Mathf.Round(slider.fillAmount * 100)) + "%";
                yield return null;
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }
            }

            timerPro = 1;
            while (timerPro > 0)
            {
                timerPro -= Time.fixedDeltaTime;
                slider.fillAmount += Time.fixedDeltaTime / 2;
                loadingText.text = ((int)Mathf.Round(slider.fillAmount * 100)) + "%";
                yield return new WaitForFixedUpdate();
            }

            slider.fillAmount = 1;
            loadingText.text = ((int)Mathf.Round(slider.fillAmount * 100)) + "%";
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(FadeInScene(action));
            if (!currentScene.Equals(scene))
            {
                if (scene.Equals(SceneName.GAME_PLAY))
                {
                    GameController.Instance.musicManager.PlayGameplaySceneBackgroundMusic();
                }
                else
                {
                    GameController.Instance.musicManager.PlayHomeSceneBackgroundMusic();
                }
            }
        }
    }
}