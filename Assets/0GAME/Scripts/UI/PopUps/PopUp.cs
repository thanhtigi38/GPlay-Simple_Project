using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ThanhND
{
    public class PopUp<T> : MonoBehaviour where T : PopUp<T>
    {
        public static T Instance;
        [Space]
        [SerializeField]
        protected float appearDuration = 0.4f;

        [SerializeField]
        protected float disappearDuration = 0.3f;

        [Space]
        [SerializeField]
        protected float startScale = 1f;
        
        [Space]
        [SerializeField]
        protected float endScale = 0f;

        [Space]
        [SerializeField]
        protected CanvasGroup panel;

        public static T Init()
        {
            if (Instance == null)
            {
                Instance = Instantiate(Resources.Load<T>(PathPrefabs.POP_UP_PATH + typeof(T).Name));
            }
            Instance.gameObject.SetActive(true);
            return Instance;
        }
        
        protected virtual void OnEnable()
        {
            SetUp();
            ShowPopUp();
        }

        public virtual void SetUp()
        {}
        

        public virtual void ShowPopUp()
        { 
            panel.interactable = true;
            panel.transform.DOKill();
            panel.transform.localScale = Vector3.zero;
            panel.transform.DOScale(startScale, 0.45f).SetEase(Ease.OutBack);
        }

        public virtual void ClosePopUp(Action action = null)
        {
            panel.interactable = false;
            panel.transform.DOKill();
            panel.transform.DOScale(endScale, 0.45f).SetEase(Ease.InBack).OnComplete((() =>
            {
                action?.Invoke();
                gameObject.SetActive(false);
            }));
        }
    }
}
