using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class ReadNotes : MonoBehaviour
{
    [SerializeField] private KeyCode Interact = KeyCode.E;
    public GameObject Player;
    bool use = false;

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

    private IEnumerator Open()
    {
        noteUI.SetActive(true);
        pickUpSound.Play();
        hud.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        doOnce = true;
    }

    private IEnumerator Close()
    {
        noteUI.SetActive(false);
        hud.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        doOnce = false;
    }

    void Update()
    {
        Use();

        if(use && inReach && !doOnce)
        {
            StartCoroutine(Open());
        }
        else if(use && doOnce)
        {
            StartCoroutine(Close());
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
