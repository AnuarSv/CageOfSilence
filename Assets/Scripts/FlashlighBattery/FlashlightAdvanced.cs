using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FlashlightAdvanced : MonoBehaviour
{
    [SerializeField] private KeyCode flashlight = KeyCode.F;
    [SerializeField] private KeyCode reload = KeyCode.R;

    public new Light light;
    public TMP_Text Lifetime;

    public TMP_Text batteryText;

    public float lifetime = 100;
    public int batteries_add = 50;
    public float batteries = 0;

    public AudioSource flashON;
    public AudioSource flashOFF;

    private bool on;
    private bool off;

    void Start()
    {
        light = GetComponent<Light>();

        off = true;
        light.enabled = false;

    }

    void Update()
    {
        Lifetime.text = "CHARGE: " + lifetime.ToString("0") + "%";
        batteryText.text = "BATTERIES: " + batteries.ToString();

        if(Input.GetKeyDown(flashlight) && off)
        {
            flashON.Play();
            light.enabled = true;
            on = true;
            off = false;
        }

        else if (Input.GetKeyDown(flashlight) && on)
        {
            flashOFF.Play();
            light.enabled = false;
            on = false;
            off = true;
        }

        if (on)
        {
            lifetime -= 1 * Time.deltaTime;
        }

        if(lifetime <= 0)
        {
            light.enabled = false;
            on = false;
            off = true;
            lifetime = 0;
        }

        if (lifetime >= 100)
        {
            lifetime = 100;
        }

        if (Input.GetKeyDown(reload) && batteries >= 1)
        {
            batteries -= 1;
            lifetime += batteries_add;
        }

        if (Input.GetKeyDown(reload) && batteries == 0)
        {
            return;
        }

        if(batteries <= 0)
        {
            batteries = 0;
        }
    }
}
