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
        [SerializeField] private GameObject loadingText;
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
                        canvas.DOFade(0f, 0.4f).OnComplete(() =>
                        {
                            action?.Invoke();
                            canvas.gameObject.SetActive(false);
                        });
                    }));
                }
                else
                {
                    AppOpenAdManager.Instance.ShowAdIfAvailable(() =>
                    {
                        isShowOpen = true;
                        canvas.DOFade(0f, 0.4f).OnComplete(() =>
                        {
                            action?.Invoke();
                            canvas.gameObject.SetActive(false);
                        });
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
                        canvas.DOFade(0f, 0.4f).OnComplete(() =>
                        {
                            action?.Invoke();
                            canvas.gameObject.SetActive(false);
                        });
                    }));
                }
                else
                {
                    canvas.DOFade(0f, 0.4f).OnComplete(() =>
                    {
                        action?.Invoke();
                        canvas.gameObject.SetActive(false);
                    });
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
            canvas.gameObject.SetActive(true);
            canvas.DOFade(1f, 0.4f);
            loadingText.gameObject.SetActive((false));
            yield return new WaitForSeconds(0.5f);

            AsyncOperation async = SceneManager.LoadSceneAsync(scene);
            async.allowSceneActivation = false;
            while (!async.isDone)
            {
                yield return null;
                if (async.progress >= 0.9f)
                {
                    async.allowSceneActivation = true;
                }
            }
            
            yield return new WaitForSeconds(0.5f);
            loadingText.SetActive(true);
            
            yield return new WaitForSeconds(2);
            float timerPro = 4;
            while (timerPro > 0)
            {
                timerPro -= Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            yield return new WaitForSeconds(0.2f);
            StartCoroutine(FadeInScene(action));
        }
    }
}