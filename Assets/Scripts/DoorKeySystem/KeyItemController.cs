using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem
{
    public class KeyItemController : MonoBehaviour
    {
        [SerializeField] private bool FirstDoor = false;
        [SerializeField] private bool FirstKey = false;

        [SerializeField] private KeyInventory _keyInventory = null;

        private KeyDoorController doorObject;

        private void Start()
        {
            if (FirstDoor)
            {
                doorObject = GetComponent<KeyDoorController>();
            }
        }

        public void ObjectInteraction()
        {
            if (FirstDoor)
            {
                doorObject.PlayAnimation();
            }

            else if (FirstKey)
            {
                _keyInventory.hasFirstKey = true;
                gameObject.SetActive(false);
            }
        }
    }
}