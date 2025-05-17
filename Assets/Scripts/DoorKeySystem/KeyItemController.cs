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

        [SerializeField] private AudioSource keyPickupSound = null;

        private KeyDoorController doorObject;

        private void Start()
        {
            if (FirstDoor)
            {
                doorObject = GetComponent<KeyDoorController>();
            }

            if (FirstKey && keyPickupSound == null)
            {
                keyPickupSound = GetComponent<AudioSource>();
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

                if (keyPickupSound != null)
                {
                    keyPickupSound.Play();
                    StartCoroutine(DisableKeyAfterSound());
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private IEnumerator DisableKeyAfterSound()
        {
            gameObject.SetActive(false);
            yield return new WaitForSeconds(keyPickupSound.clip.length);
        }
    }
}
