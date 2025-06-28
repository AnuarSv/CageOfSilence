using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem
{
    public class KeyItemController : MonoBehaviour
    {
        public bool FirstDoor = false;
        public bool MainDoor = false;
        [SerializeField] private bool FirstKey = false;
        [SerializeField] private bool MainKey = false;

        [SerializeField] private KeyInventory _keyInventory = null;

        [SerializeField] private AudioSource keyPickupSound = null;

        private KeyDoorController doorObject;
        private MainDoorController doorMain;

        private void Start()
        {
            if (FirstDoor)
            {
                doorObject = GetComponent<KeyDoorController>();
            }
            else if (MainDoor)
            {
                doorMain = GetComponent<MainDoorController>();
            }

            if ((FirstKey || MainKey) && keyPickupSound == null)
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
            else if (MainDoor)
            {
                doorMain.PlayAnimation();
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
            else if (MainKey)
            {
                _keyInventory.hasMainKey = true;

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
