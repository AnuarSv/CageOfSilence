using System.Collections;
using System.Collections.Generic;
using KeySystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;

public class OpenBoxScript : MonoBehaviour
{
    public Animator boxOB;
    public GameObject keyOBNeeded;
    public GameObject openText;
    public GameObject keyMissingText;
    public GameObject ObjectInside;
    public AudioSource openSound;

    [SerializeField] private Image crosshair = null;

    [SerializeField] private KeyCode Interact = KeyCode.E;
    public GameObject Player;
    bool use = false;
    bool ObjectInsideActive = false;

    public bool inReach;
    public bool isOpen;

    void Start()
    {
        inReach = false;
        openText.SetActive(false);
        keyMissingText.SetActive(false);
        ObjectInside.SetActive(false);

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
            openText.SetActive(true);
            CrosshairChange(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            openText.SetActive(false);
            keyMissingText.SetActive(false);
            CrosshairChange(false);
        }
    }

    IEnumerator MakeActive()
    {
        if (ObjectInsideActive)
        {
            yield return new WaitForSeconds(0.3f);
            ObjectInside.SetActive(true);
        }
    }
    void Update()
    {
        Use();

        if (keyOBNeeded.activeInHierarchy == true && inReach && use)
        {
            keyOBNeeded.SetActive(false);
            openSound.Play();
            boxOB.SetBool("open", true);
            openText.SetActive(false);
            keyMissingText.SetActive(false);
            isOpen = true;
        }

        else if (keyOBNeeded.activeInHierarchy == false && inReach && use)
        {
            openText.SetActive(false);
            keyMissingText.SetActive(true);
        }

        if(isOpen)
        {
            boxOB.GetComponent<BoxCollider>().enabled = false;
            boxOB.GetComponent<OpenBoxScript>().enabled = false;
            ObjectInsideActive = true;
            StartCoroutine(MakeActive());
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
