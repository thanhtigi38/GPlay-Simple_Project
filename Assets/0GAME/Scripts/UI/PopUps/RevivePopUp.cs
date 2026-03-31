using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    public class RevivePopUp : PopUp<RevivePopUp>
    {
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button noThanksButton;

        private void Awake()
        {
            reviveButton.onClick.AddListener(OnReviveButtonClicked);
            noThanksButton.onClick.AddListener(OnNoThanksButtonClicked);
        }

        private void OnReviveButtonClicked()
        {
            if (GameController.Instance)
            {
                GameController.Instance.admobAds.ShowVideoReward(actionReward: OnReward,
                    actionNotLoadedVideo: () =>
                    {
                        GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp_Overlay(
                            reviveButton.transform.position, "No video available", Color.red);
                    }, null, "revive");
            }

            void OnReward()
            {
            }
        }

        private void OnNoThanksButtonClicked()
        {
            ClosePopUp(() => { LossGamePopUp.Init(); });
        }

        public override void ClosePopUp(Action action = null)
        {
            gameObject.SetActive(false);
            action?.Invoke();
        }

        public override void ShowPopUp()
        {
            noThanksButton.gameObject.SetActive(false);
            panel.DOFade(1, 0.3f).From(0);
            
            DOVirtual.DelayedCall(3f, () =>
            {
                noThanksButton.gameObject.SetActive(true);
            });
        }
    }
}