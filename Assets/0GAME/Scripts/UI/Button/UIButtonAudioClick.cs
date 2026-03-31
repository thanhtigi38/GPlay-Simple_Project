using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    public class UIButtonAudioClick : MonoBehaviour
    {
        [SerializeField] private AudioClickType _audioClickType = AudioClickType.click1;

        [SerializeField] private Button _button;

        private void Reset()
        {
            _button = gameObject.GetComponent<Button>();
        }

        private void Awake()
        {
            switch (_audioClickType)
            {
                case AudioClickType.click1:
                    _button.onClick.AddListener(() =>
                    {
                        if (GameController.Instance != null)
                        {
                            GameController.Instance.musicManager.PlayClickSound();
                        }
                    });
                    break;
            }
        }
    }

    public enum AudioClickType
    {
        click1,
        click2,
        normal,
        hard
    }
}