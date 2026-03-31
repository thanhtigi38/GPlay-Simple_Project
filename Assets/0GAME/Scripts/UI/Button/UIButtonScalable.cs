using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

//button loop va khi an vao thi scale be di
namespace ThanhND
{
    public class UIButtonScalable : UIBaseButton
    {
        [SerializeField] private float scaleHold = 0.9f;
        [SerializeField] private float scaleRelease = 1.1f;
        [SerializeField] private bool isOverrideButton = false;
        [SerializeField] private bool isIdle = false;
        [SerializeField] private float idleLoopScale = 1.2f;
        [SerializeField] private float idleLoopTime = 0.6f;

        protected override void OnValidate()
        {
            if (isOverrideButton) return;
            base.OnValidate();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (isIdle)
                ScalableLoop();
        }

        public override void OnHoldEvent()
        {
            DOTween.Kill(gameObject);
            base.OnHoldEvent();
            button.transform.DOScale(Vector3.one * scaleHold, 0.1f).SetUpdate(true).SetId(gameObject);
        }

        public override void OnReleaseEvent()
        {
            base.OnReleaseEvent();
            Sequence sequence = DOTween.Sequence(gameObject);

            sequence.Append(button.transform.DOScale(Vector3.one * scaleRelease, 0.1f))
                .AppendCallback(ScalableLoop);
        }

        public override void Action()
        {
        }

        private void ScalableLoop()
        {
            if (isIdle)
            {
                button.transform.DOScale(Vector3.one * idleLoopScale, idleLoopTime).From(Vector3.one * scaleRelease)
                    .SetLoops(-1, LoopType.Yoyo).SetId(gameObject);
            }
        }
    }
}