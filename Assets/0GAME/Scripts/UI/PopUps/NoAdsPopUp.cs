using System;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    public class NoAdsPopUp : PopUp<NoAdsPopUp>
    {
        [SerializeField] private Button closeButton;
        private Action callBack;

        private void Awake()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnCloseButtonClicked()
        {
            ClosePopUp();
        }

        public void SetCallBack(Action action)
        {
            callBack = action;
        }

        public void BuyNoAdsSuccess()
        {
            UseProfile.IsRemoveAds = true;
            GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp(Vector3.zero,
                Localization.Get("s_purchase_success"), GameConfig.Instance.blueColor);
            ClosePopUp(callBack);
        }

        public void BuyNoAdsFailed()
        {
            GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp(Vector3.zero,
                Localization.Get("s_purchase_failed"), Color.red);
        }

        public override void SetUp()
        {
            base.SetUp();
            GameController.Instance.admobAds.ShowMRec();
        }

        public override void ClosePopUp(Action action = null)
        {
            action += callBack;
            base.ClosePopUp(action);
            GameController.Instance.admobAds.HideMRec();
        }
    }
}