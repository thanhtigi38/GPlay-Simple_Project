using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ThanhND
{
    public abstract class UIBaseButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] protected Button button;

        protected virtual void OnValidate()
        {
            button = gameObject.GetComponent<Button>();
        }


        protected virtual void OnEnable()
        {
            button.onClick.AddListener(Action);
        }

        protected virtual void OnDisable()
        {
            button.onClick.RemoveListener(Action);
        }

        public abstract void Action();

        public virtual void OnHoldEvent()
        {
        }

        public virtual void OnReleaseEvent()
        {
        }

        public virtual void OnUpdate(float dt)
        {
        }

        private void Update()
        {
            OnUpdate(Time.deltaTime);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (button.interactable)
                OnReleaseEvent();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (button.interactable)
                OnHoldEvent();
        }
    }
}