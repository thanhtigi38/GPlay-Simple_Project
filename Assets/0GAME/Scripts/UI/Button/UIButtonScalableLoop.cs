using UnityEngine;
using DG.Tweening;

//button chi loop
namespace ThanhND
{
    public class UIButtonScalableLoop : MonoBehaviour
    {
        public float scaleAmount = 1.1f;
        public float duration = 1f;

        void Start()
        {
            StartScaleAnimation();
        }

        void StartScaleAnimation()
        {
            transform.DOScale(scaleAmount, duration)
                .SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo);
        }
    }
}