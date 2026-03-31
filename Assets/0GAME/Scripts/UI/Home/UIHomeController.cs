using System;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    public class UIHomeController : MonoBehaviour
    {
        [SerializeField] private Button settingButton;
        [SerializeField] private Button noAdsButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button playButton;
        [SerializeField] private Text levelText;

        private void Awake()
        {
            settingButton.onClick.AddListener(OnSettingButtonClicked);
            noAdsButton.onClick.AddListener(OnNoAdsButtonClicked);
            shopButton.onClick.AddListener(OnShopButtonClicked);
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }


        private void Start()
        {
            Init();
        }

        private void Init()
        {
            levelText.text = $"Level {UseProfile.CurrentLevel}";
            
            LoadingPanel.Instance.ActiveScene();
        }
        
        private void OnSettingButtonClicked()
        {
            SettingPopUp.Init();
        }
        
        private void OnNoAdsButtonClicked()
        {
            NoAdsPopUp.Init();
        }
        
        private void OnShopButtonClicked()
        {
            Debug.Log("Shop button clicked");
            // Open shop panel
        }
        
        private void OnPlayButtonClicked()
        {
            LoadingPanel.Instance.GotoScene(SceneName.GAME_PLAY,true);
        }
    }
}