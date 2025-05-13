using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    public GameObject flashlight;

    public AudioSource turnOn;
    public AudioSource turnOff;

    private bool isOn = false;

    void Start()
    {
        flashlight.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isOn = !isOn;

            flashlight.SetActive(isOn);

            if (isOn)
            {
                turnOn.Play();
            }
            else
            {
                turnOff.Play();
            }
        }
    }
}

