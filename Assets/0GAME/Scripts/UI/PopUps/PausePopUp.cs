using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    public class PausePopUp : PopUp<PausePopUp>
    {
        [SerializeField] private Button musicButton;
        [SerializeField] private GameObject musicOn;
        [SerializeField] private GameObject musicOff;
        [Space(20)] [SerializeField] private Button soundButton;
        [SerializeField] private GameObject soundOn;
        [SerializeField] private GameObject soundOff;
        [Space(20)] [SerializeField] private Button vibrationButton;
        [SerializeField] private GameObject vibrationOn;
        [SerializeField] private GameObject vibrationOff;
        [Space(20)] [SerializeField] private Button homeButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button closeButton;
        [SerializeField] private Button cheatButton;


        private void Awake()
        {
            soundButton.onClick.AddListener(OnSoundButtonClick);
            musicButton.onClick.AddListener(OnMusicButtonClick);
            vibrationButton.onClick.AddListener(OnVibrationButtonClick);
            closeButton.onClick.AddListener(OnCloseButtonClick);
            homeButton.onClick.AddListener(OnHomeButtonClick);
            replayButton.onClick.AddListener(OnReplayButtonClick);
            cheatButton.gameObject.SetActive(false);

#if THANH_TESTER || UNITY_EDITOR
            SetCheatButtonColor();
            cheatButton.gameObject.SetActive(true);
            cheatButton.onClick.AddListener((() =>
            {
                UseProfile.IsCheat = !UseProfile.IsCheat;
                SetCheatButtonColor();
            }));
#endif
        }
        
        private void SetCheatButtonColor()
        {
            cheatButton.GetComponent<Image>().color = UseProfile.IsCheat ? Color.green : Color.gray;
        }

        private void OnReplayButtonClick()
        {
            // UseProfile.SetLevelData(GameConfig.Instance.currentLevelData.id, new List<int>());
            ClosePopUp(() =>
            {
                GameController.Instance.admobAds.HideMRec();
            });
        }

        private void OnHomeButtonClick()
        {
        }


        public override void SetUp()
        {
            GameController.Instance.admobAds.ShowMRec();
            GameplayController.Instance.gameState = GameState.Paused;
            base.SetUp();
            if (UseProfile.OnSound)
            {
                soundOn.SetActive(true);
                soundOff.SetActive(false);
            }
            else
            {
                soundOn.SetActive(false);
                soundOff.SetActive(true);
            }

            if (UseProfile.OnMusic)
            {
                musicOn.SetActive(true);
                musicOff.SetActive(false);
            }
            else
            {
                musicOn.SetActive(false);
                musicOff.SetActive(true);
            }

            if (UseProfile.OnVibration)
            {
                vibrationOn.SetActive(true);
                vibrationOff.SetActive(false);
            }
            else
            {
                vibrationOn.SetActive(false);
                vibrationOff.SetActive(true);
            }
        }

        void OnSoundButtonClick()
        {
            if (UseProfile.OnSound)
            {
                UseProfile.OnSound = false;
                soundOn.SetActive(false);
                soundOff.SetActive(true);
            }
            else
            {
                UseProfile.OnSound = true;
                soundOn.SetActive(true);
                soundOff.SetActive(false);
            }
        }

        void OnMusicButtonClick()
        {
            if (UseProfile.OnMusic)
            {
                UseProfile.OnMusic = false;
                musicOn.SetActive(false);
                musicOff.SetActive(true);
            }
            else
            {
                UseProfile.OnMusic = true;
                musicOn.SetActive(true);
                musicOff.SetActive(false);
            }
        }

        void OnVibrationButtonClick()
        {
            if (UseProfile.OnVibration)
            {
                UseProfile.OnVibration = false;
                vibrationOn.SetActive(false);
                vibrationOff.SetActive(true);
            }
            else
            {
                UseProfile.OnVibration = true;
                vibrationOn.SetActive(true);
                vibrationOff.SetActive(false);
            }
        }

        void OnCloseButtonClick()
        {
            ClosePopUp((() =>
            {
                GameplayController.Instance.gameState = GameState.Playing;
                GameController.Instance.admobAds.HideMRec();
            }));
        }
    }
}