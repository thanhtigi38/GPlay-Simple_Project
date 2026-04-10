using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ThanhND
{
    public class LossGamePopUp : PopUp<LossGamePopUp>
    {
        [SerializeField] private Button replayWithVideoButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button homeButton;
        
        private void Awake()
        {
            replayWithVideoButton.onClick.AddListener(OnReplayWithVideoButtonClicked);
            replayButton.onClick.AddListener(OnReplayButtonClicked);
            homeButton.onClick.AddListener(OnHomeButtonClicked);
        }
        
        private void OnReplayWithVideoButtonClicked()
        {
            if (GameController.Instance)
            {
                GameController.Instance.admobAds.ShowVideoReward(actionReward: OnReward,
                    actionNotLoadedVideo: () =>
                    {
                        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp_Overlay(
                            replayWithVideoButton.transform.position, "No video available", Color.red);
                    }, null, "replay_with_ads");
            }

            void OnReward()
            {
                ClosePopUp(GoToGameplaySafe);
            }
        }
        
        private void OnReplayButtonClicked()
        {
            ClosePopUp(GoToGameplaySafe);
        }
        
        private void OnHomeButtonClicked()
        {
            ClosePopUp(GoToHomeSafe);
        }

        private static void GoToGameplaySafe()
        {
            // GotoScene gọi GameController.Instance.admobAds — thiếu một trong hai thì fallback load thẳng scene.
            if (LoadingPanel.Instance != null && GameController.Instance != null)
            {
                LoadingPanel.Instance.GotoScene(SceneName.GAME_PLAY, true);
                return;
            }

            SceneManager.LoadScene(SceneName.GAME_PLAY);
        }

        private static void GoToHomeSafe()
        {
            if (LoadingPanel.Instance != null && GameController.Instance != null)
            {
                LoadingPanel.Instance.GotoScene(SceneName.HOME_SCENE, true);
                return;
            }

            SceneManager.LoadScene(SceneName.HOME_SCENE);
        }
        
        public override void ClosePopUp(Action action = null)
        {
            gameObject.SetActive(false);
            action?.Invoke();
        }

        public override void ShowPopUp()
        {
            panel.DOFade(1, 0.3f).From(0);
            
        }
    }
}