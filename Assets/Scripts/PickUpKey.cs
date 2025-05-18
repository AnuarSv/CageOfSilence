using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class PickUpKey : MonoBehaviour
{
    public GameObject keyOB;
    public GameObject invOB;
    public GameObject pickUpText;
    public AudioSource keySound;
    public GameObject Player;
    bool use = false;
    public bool inReach;

    [SerializeField] private KeyCode Interact = KeyCode.E;
    [SerializeField] private Image crosshair = null;
    void Start()
    {
        inReach = false;
        pickUpText.SetActive(false);
        invOB.SetActive(false);
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

        if (inReach && use)
        {
            keyOB.SetActive(false);
            keySound.Play();
            invOB.SetActive(true);
            pickUpText.SetActive(false);
            CrosshairChange(false);
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
