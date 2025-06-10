using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.UI;

public class CodeDoor : MonoBehaviour
{
    public Animator door;
    [SerializeField] private Image crosshair = null;
    public GameObject Locked;
    public GameObject CodeLockObject;
    public GameObject Player;
    public AudioSource doorSound;
    public Collider doorCollider;
    [SerializeField] private int timeToShowUI = 1;

    public bool inReach;
    private bool isOpen = false;
    private bool isMoving = false;

    [SerializeField] private KeyCode Interact = KeyCode.E;
    bool use = false;

    void Start()
    {
        Player.GetComponent<StarterAssetsInputs>().use = false;
        CodeLockObject.GetComponent<CodeLock>().isCorrect = false;

        inReach = false;
        isOpen = false;

        if (doorCollider == null)
            doorCollider = GetComponent<Collider>();
    }
    private void Use()
    {
        if (Player.GetComponent<StarterAssetsInputs>().use | Input.GetKey(Interact))
        {
            use = true;
        }
        else
        {
            use = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            CrosshairChange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            CrosshairChange(false);
        }
    }

    void Update()
    {
        Use();

        if (inReach && use && !isMoving)
        {
            if (!isOpen && CodeLockObject.GetComponent<CodeLock>().isCorrect)
            {
                StartCoroutine(OpenDoor());
            }
            else if (!isOpen && !CodeLockObject.GetComponent<CodeLock>().isCorrect)
            {
                StartCoroutine(ShowDoorLocked());
            }
            else if (isOpen)
            {
                StartCoroutine(CloseDoor());
            }
        }
    }

    IEnumerator OpenDoor()
    {
        isMoving = true;
        doorCollider.enabled = false;

        door.SetBool("Open", true);
        door.SetBool("Close", false);
        doorSound.Play();
        isOpen = true;

        yield return new WaitForSeconds(door.GetCurrentAnimatorStateInfo(0).length);
        isMoving = false;
        doorCollider.enabled = true;
    }

    IEnumerator CloseDoor()
    {
        isMoving = true;
        doorCollider.enabled = false;

        door.SetBool("Open", false);
        door.SetBool("Close", true);
        doorSound.Play();
        isOpen = false;

        yield return new WaitForSeconds(door.GetCurrentAnimatorStateInfo(0).length);
        isMoving = false;
        doorCollider.enabled = true;
    }

    IEnumerator ShowDoorLocked()
    {
        Locked.SetActive(true);
        yield return new WaitForSeconds(timeToShowUI);
        Locked.SetActive(false);
    }

    void CrosshairChange(bool on)
    {
        if (on)
        {
            crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.white;
        }
    }

}
