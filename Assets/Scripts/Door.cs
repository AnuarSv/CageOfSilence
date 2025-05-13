using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    public Animator door;
    public GameObject openText;
    public AudioSource doorSound;

    public bool inReach;
    private bool isOpen = false; // Track door state

    void Start()
    {
        inReach = false;
        isOpen = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            openText.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
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
            {
                DoorOpens();
            }
            else
            {
                DoorCloses();
            }
        }
    }

    void DoorOpens()
    {
        Debug.Log("It Opens");
        door.SetBool("Open", true);
        door.SetBool("Close", false);
        doorSound.Play();
        isOpen = true;
    }

    void DoorCloses()
    {
        Debug.Log("It Closes");
        door.SetBool("Open", false);
        door.SetBool("Close", true);
        doorSound.Play();
        isOpen = false;
    }
}
