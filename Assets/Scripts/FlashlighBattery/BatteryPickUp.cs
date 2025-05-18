using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class BatteryPickUp : MonoBehaviour
{
    private bool inReach;

    [SerializeField] private KeyCode Interact = KeyCode.E;
    [SerializeField] private Image crosshair = null;

    public GameObject pickUpText;
    private GameObject flashlight;
    public GameObject Player;
    bool use = false;

    public AudioSource pickUpSound;

    void Start()
    {
        inReach = false;
        pickUpText.SetActive(false);
        flashlight = GameObject.Find("FlashLight");
        Player.GetComponent<StarterAssetsInputs>().use = false;
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
        Use();

        if(use && inReach)
        {
            flashlight.GetComponent<FlashlightAdvanced>().batteries += 1;
            pickUpSound.Play();
            inReach = false;
            pickUpText.SetActive(false);
            Destroy(gameObject);
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
