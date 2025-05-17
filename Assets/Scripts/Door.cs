using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public Animator door;
    public GameObject openText;
    public AudioSource doorSound;
    public Collider doorCollider;

    public bool inReach;
    private bool isOpen = false;
    private bool isMoving = false;

    void Start()
    {
        inReach = false;
        isOpen = false;

        if (doorCollider == null)
            doorCollider = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            openText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            openText.SetActive(false);
        }
    }

    void Update()
    {
        if (inReach && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
                StartCoroutine(OpenDoor());
            else
                StartCoroutine(CloseDoor());
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
}
