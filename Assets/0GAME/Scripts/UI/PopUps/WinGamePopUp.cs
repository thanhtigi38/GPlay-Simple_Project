using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    public class WinGamePopUp : PopUp<WinGamePopUp>
    {
        [SerializeField] private Button claimX2Button;
        [SerializeField] private Button claimButton;
        
        private void Awake()
        {
            claimX2Button.onClick.AddListener(OnClaimX2ButtonClicked);
            claimButton.onClick.AddListener(OnClaimButtonClicked);
        }
        
        private void OnClaimX2ButtonClicked()
        {
            if (GameController.Instance)
            {
                GameController.Instance.admobAds.ShowVideoReward(actionReward: OnReward,
                    actionNotLoadedVideo: () =>
                    {
                        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp_Overlay(
                            claimX2Button.transform.position, "No video available", Color.red);
                    }, null, "claim_x2");
            }

            void OnReward()
            {
                ClosePopUp(() =>
                {
                    LoadingPanel.Instance.GotoScene(SceneName.GAME_PLAY,true);
                });
            }
        }
        
        private void OnClaimButtonClicked()
        {
            ClosePopUp(() =>
            {
                LoadingPanel.Instance.GotoScene(SceneName.GAME_PLAY,true);
            });
        }
        
        public override void ClosePopUp(Action action = null)
        {
            gameObject.SetActive(false);
            action?.Invoke();
        }

        public override void ShowPopUp()
        {
            claimButton.gameObject.SetActive(false);
            panel.DOFade(1, 0.3f).From(0);
            
            DOVirtual.DelayedCall(3f, () =>
            {
                claimButton.gameObject.SetActive(true);
            });
        }
    }
}