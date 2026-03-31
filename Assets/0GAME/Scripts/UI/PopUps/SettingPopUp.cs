using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    public class SettingPopUp : PopUp<SettingPopUp>
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
        [SerializeField] private Button privacyButton;
        [SerializeField] private Button termsButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private Button cheatButton;


        private void Awake()
        {
            soundButton.onClick.AddListener(OnSoundButtonClick);
            musicButton.onClick.AddListener(OnMusicButtonClick);
            vibrationButton.onClick.AddListener(OnVibrationButtonClick);
            privacyButton.onClick.AddListener(OpenPrivacy);
            termsButton.onClick.AddListener(OpenTerm);
            closeButton.onClick.AddListener(OnCloseButtonClick);
            cheatButton.gameObject.SetActive(false);

#if THANH_TESTER || UNITY_EDITOR
            SetCheatButtonColor();
            cheatButton.gameObject.SetActive(true);
            cheatButton.onClick.AddListener((() =>
            {
                UseProfile.IsCheat = !UseProfile.IsCheat;
                SetCheatButtonColor();

                UseProfile.Coins = UseProfile.IsCheat ? 100000 : 100;
                
            }));
#endif
        }

        private void SetCheatButtonColor()
        {
            cheatButton.GetComponent<Image>().color = UseProfile.IsCheat ? Color.green : Color.gray;
        }



        public override void SetUp()
        {
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

            GameController.Instance.admobAds.ShowMRec();
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


        public void OpenPrivacy()
        {
            Application.OpenURL(Config.LinkPolicy);
        }

        public void OpenTerm()
        {
            Application.OpenURL(Config.LinkTerm);
        }


        void OnCloseButtonClick()
        {
            ClosePopUp((() =>
            {
                GameController.Instance.admobAds.HideMRec();
            }));
        }
    }
}