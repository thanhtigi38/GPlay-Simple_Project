using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    public class UIButtonAudioClick2 : MonoBehaviour
    {
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private Button _button;
        [SerializeField] private bool checkIsPlayingGame = false;

        private void Reset()
        {
            _button = gameObject.GetComponent<Button>();
        }

        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                if (GameController.Instance != null)
                {
                    GameController.Instance.musicManager.PlaySingle(audioClip);
                }
            });
        }
    }
}