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
        [SerializeField] private string Open = "DoorOpen";
        [SerializeField] private string Close = "DoorClose";

        [SerializeField] private int timeToShowUI = 1;
        [SerializeField] private GameObject showDoorLockedUI = null;

        [SerializeField] private KeyInventory _keyInventory = null;
        [SerializeField] private KeyItemController _itemController = null;

        [SerializeField] private int waitTimer = 1;
        [SerializeField] private bool pauseInteraction = false;

        [Header("Optional Door Collider (will auto-assign if left empty)")]
        [SerializeField] private Collider doorCollider;

        private void Awake()
        {
            DoorAnim = gameObject.GetComponent<Animator>();
            if (doorCollider == null)
                doorCollider = GetComponent<Collider>();
        }

        private IEnumerator PauseDoorInteraction()
        {
            pauseInteraction = true;
            yield return new WaitForSeconds(waitTimer);
            pauseInteraction = false;
        }

        public void PlayAnimation()
        {
            if (_keyInventory.hasFirstKey && _itemController.FirstDoor)
            {
                StartCoroutine(OpenDoorCoroutine());
            }
            else if (_keyInventory.hasMainKey && _itemController.MainDoor)
            {
                StartCoroutine(OpenDoorCoroutine());
            }
            else
            {
                StartCoroutine(ShowDoorLocked());
            }
        }

        IEnumerator OpenDoorCoroutine()
        {
            if (pauseInteraction) yield break;

            pauseInteraction = true;
            doorCollider.enabled = false;

            if (!doorOpen)
            {
                DoorAnim.Play(Open, 0, 0.0f);
                doorSound.Play();
                doorOpen = true;
            }
            else
            {
                DoorAnim.Play(Close, 0, 0.0f);
                doorSound.Play();
                doorOpen = false;
            }

            yield return new WaitForSeconds(DoorAnim.GetCurrentAnimatorStateInfo(0).length);
            doorCollider.enabled = true;

            yield return new WaitForSeconds(waitTimer);
            pauseInteraction = false;
        }

        IEnumerator ShowDoorLocked()
        {
            showDoorLockedUI.SetActive(true);
            yield return new WaitForSeconds(timeToShowUI);
            showDoorLockedUI.SetActive(false);
        }
    }
}
