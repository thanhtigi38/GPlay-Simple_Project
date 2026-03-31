using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ThanhND
{
    public class UIButtonScaleUp : UIBaseButton
    {
        public bool isOverrideButton = false;
        [SerializeField] bool _isIdle = false;

        protected override void OnValidate()
        {
            if (isOverrideButton) return;
            base.OnValidate();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_isIdle)

                ScalableLoop();
        }

        public override void OnHoldEvent()
        {
            DOTween.Kill(gameObject);
            base.OnHoldEvent();
            Sequence sequence = DOTween.Sequence(gameObject);

            sequence.Append(button.transform.DOScale(Vector3.one * 1.2f, 0.05f))
                .Append(button.transform.DOScale(Vector3.one * 1f, 0.1f))
                .AppendCallback(ScalableLoop);
        }

        public override void OnReleaseEvent()
        {
            base.OnReleaseEvent();
        }

        private void ScalableLoop()
        {
            if (_isIdle)
            {
                button.transform.DOScale(Vector3.one * 1.1f, 0.2f).From(Vector3.one).SetLoops(-1, LoopType.Yoyo)
                    .SetId(gameObject);
            }
        }

        public override void Action()
        {
        }
    }
}