using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    [RequireComponent(typeof(Button))]
    public class BombRewardButton : MonoBehaviour
    {
        [SerializeField] private string rewardActionType = "bomb";
        [SerializeField] private string noVideoMessage = "No video available";
        [SerializeField] private string cannotBombMessage = "No matching pair for bomb";

        private Button _button;
        private bool _isRequestingAd;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnBombButtonPressed);
        }

        public void OnBombButtonPressed()
        {
            if (_isRequestingAd)
                return;

            if (GameplayController.Instance == null)
            {
                Debug.LogWarning("BombRewardButton: GameplayController.Instance is null.");
                return;
            }

            if (!GameplayController.Instance.CanBombPairFromPot())
            {
                ShowOverlayMessage(cannotBombMessage);
                return;
            }

            if (!GameController.Instance)
            {
                Debug.LogWarning("BombRewardButton: GameController.Instance is null.");
                return;
            }

            // Rewarded ad: only clear pair in OnReward.
            _isRequestingAd = true;
            bool started = GameController.Instance.admobAds.ShowVideoReward(
                actionReward: OnReward,
                actionNotLoadedVideo: OnVideoNotLoaded,
                actionClose: OnAdClosed,
                actionType: rewardActionType);

            if (!started)
                _isRequestingAd = false;
        }

        private void OnReward()
        {
            if (GameplayController.Instance != null &&
                !GameplayController.Instance.TryBombClearOnePotStackPair())
            {
                ShowOverlayMessage(cannotBombMessage);
            }
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
