using System;
using UnityEngine;

namespace ThanhND
{
    public class ObjectDrop : MonoBehaviour
    {
        public int id;
        [SerializeField] private Rigidbody2D rigidbody2D;
        private bool isDropped = false;

        private void OnMouseUpAsButton()
        {
            if (GameplayController.Instance.gameState != GameState.Playing || isDropped) return;
            DropObject();
        }
        
        private void DropObject()
        {
            isDropped = true;
            rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}