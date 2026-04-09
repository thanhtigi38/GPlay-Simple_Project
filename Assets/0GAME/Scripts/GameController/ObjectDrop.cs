using UnityEngine;

namespace ThanhND
{
    public class ObjectDrop : MonoBehaviour
    {
        public int id;
        [SerializeField] private Rigidbody2D rigidbody2D;
        public ObjectState objectState = ObjectState.Idle;

        private void OnMouseUpAsButton()
        {
            if (GameplayController.Instance.gameState != GameState.Playing || objectState != ObjectState.Idle) return;
            DropObject();
        }

        private void DropObject()
        {
            objectState = ObjectState.Dropped;
            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
            GameplayController.Instance.OnDropObject(this);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (GameplayController.Instance.gameState == GameState.Ended) return;
            if (other.CompareTag("Pot"))
            {
                if (objectState == ObjectState.Dropped)
                {
                    objectState = ObjectState.InPot;
                    GameplayController.Instance.OnObjectIsInPot(this);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (GameplayController.Instance.gameState == GameState.Ended)
                return;

            if (!other.gameObject.CompareTag("ObjectDrop"))
                return;

            var otherDrop = other.gameObject.GetComponent<ObjectDrop>();
            if (otherDrop == null || otherDrop.id != id)
                return;

            if (objectState != ObjectState.InPot && otherDrop.objectState != ObjectState.InPot)
                return;

            // Unity gọi OnCollisionEnter2D trên cả hai phía; chỉ xử lý một lần cho mỗi cặp va chạm.
            if (GetInstanceID() > otherDrop.GetInstanceID())
                return;

            GameplayController.Instance.CheckObjectsInPot(this, otherDrop);
        }

    }

    public enum ObjectState
    {
        Idle,
        Dropped,
        InPot,
    }
}