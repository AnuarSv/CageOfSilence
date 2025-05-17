using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;


public class ReadNotes : MonoBehaviour
{
    [SerializeField] private KeyCode Interact = KeyCode.E;
    [SerializeField] private Image crosshair = null;

    public GameObject noteUI;
    public GameObject hud;

    public GameObject pickUpText;

    public AudioSource pickUpSound;

    public bool inReach;

    private bool doOnce = false;

    void Start()
    {
        noteUI.SetActive(false);
        hud.SetActive(true);
        pickUpText.SetActive(false);

        inReach = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = true;
            pickUpText.SetActive(true);
            CrosshairChange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            pickUpText.SetActive(false);
            CrosshairChange(false);
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(Interact) && inReach && !doOnce)
        {
            noteUI.SetActive(true);
            pickUpSound.Play();
            hud.SetActive(false);
            doOnce = true;
        }
        else if(Input.GetKeyDown(Interact) && doOnce)
        {
            noteUI.SetActive(false);
            hud.SetActive(true);
            doOnce= false;
        } 
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
