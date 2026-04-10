using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    [RequireComponent(typeof(Button))]
    public class ShuffleRewardButton : MonoBehaviour
    {
        [SerializeField] private string rewardPlacement = "shuffle";
        [SerializeField] private string noVideoMessage = "No video available";
        [SerializeField] private string cannotShuffleMessage = "Not enough items to shuffle";

        private Button _button;
        private bool _isRequestingAd;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnShuffleButtonPressed);
        }

        public void OnShuffleButtonPressed()
        {
            if (_isRequestingAd)
                return;

            if (GameplayController.Instance == null)
            {
                Debug.LogWarning("ShuffleRewardButton: GameplayController.Instance is null.");
                return;
            }

            if (!GameplayController.Instance.CanShuffleObjectsOnStack())
            {
                ShowOverlayMessage(cannotShuffleMessage);
                return;
            }

            if (!GameController.Instance)
            {
                Debug.LogWarning("ShuffleRewardButton: GameController.Instance is null.");
                return;
            }

            // Gọi rewarded ads: chỉ shuffle khi callback reward trả về.
            _isRequestingAd = true;
            bool started = GameController.Instance.admobAds.ShowVideoReward(
                actionReward: OnReward,
                actionNotLoadedVideo: OnVideoNotLoaded,
                actionClose: OnAdClosed,
                actionType: rewardPlacement);

            if (!started)
                _isRequestingAd = false;
        }

        private void OnReward()
        {
            // Reward thành công -> thực thi shuffle.
            GameplayController.Instance?.ShuffleObjectsOnStack();
        }

        private void OnVideoNotLoaded()
        {
            ShowOverlayMessage(noVideoMessage);
            _isRequestingAd = false;
        }

        private void OnAdClosed()
        {
            _isRequestingAd = false;
        }

        private void ShowOverlayMessage(string message)
        {
            if (!GameController.Instance)
                return;

            GameController.Instance.moneyEffectController.SpawnEffectText_FlyUp_Overlay(
                _button != null ? _button.transform.position : transform.position,
                message,
                Color.red);
        }
    }
}
