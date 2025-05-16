using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KeySystem
{
    public class KeyDoorController : MonoBehaviour
    {
        private Animator DoorAnim;
        private bool doorOpen = false;
        public AudioSource doorSound;

        [Header("Animation Names")]
        [SerializeField] private string Open = "Open";
        [SerializeField] private string Close = "Close";

        [SerializeField] private int timeToShowUI = 1;
        [SerializeField] private GameObject showDoorLockedUI = null;

        [SerializeField] private KeyInventory _keyInventory = null;

        [SerializeField] private int waitTimer = 1;
        [SerializeField] private bool pauseInteraction = false;

        private void Awake()
        {
            DoorAnim = gameObject.GetComponent<Animator>();
        }

        private IEnumerator PauseDoorInteraction()
        {
            pauseInteraction = true;
            yield return new WaitForSeconds(waitTimer);
            pauseInteraction = false;
        }

        public void PlayAnimation()
        {
            if (_keyInventory.hasFirstKey)
            {
                OpenDoor();
            }
            else
            {
                StartCoroutine(ShowDoorLocked());
            }
        }

        void OpenDoor()
        {
            if (!doorOpen && !pauseInteraction)
            {
                DoorAnim.Play(Open, 0, 0.0f);
                doorOpen = true;
                doorSound.Play();
                StartCoroutine(PauseDoorInteraction());
            }

            else if (doorOpen && !pauseInteraction)
            {
                DoorAnim.Play(Close, 0, 0.0f);
                doorOpen = false;
                doorSound.Play();
                StartCoroutine(PauseDoorInteraction());
            }
        }

        IEnumerator ShowDoorLocked()
        {
            showDoorLockedUI.SetActive(true);
            yield return new WaitForSeconds(timeToShowUI);
            showDoorLockedUI.SetActive(false);
        }
    }
}