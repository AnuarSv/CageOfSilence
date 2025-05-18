using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using StarterAssets;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class FlashlightAdvanced : MonoBehaviour
{
    [SerializeField] private KeyCode flashlight = KeyCode.F;
    [SerializeField] private KeyCode reloadFlash = KeyCode.R;

    public Light light;
    public TMP_Text Lifetime;

    public GameObject Player;
    bool reload = false;
    bool flash = false;

    public TMP_Text batteryText;

    public float lifetime = 100;
    public int batteries_add = 50;
    public float batteries = 0;

    public AudioSource flashON;
    public AudioSource flashOFF;

    void Start()
    {
        light = GetComponent<Light>();

        flash = false;
        light.enabled = false;
        Player.GetComponent<StarterAssetsInputs>().flash = false;
        Player.GetComponent<StarterAssetsInputs>().reloadFlash = false;
    }

    private IEnumerator Flash()
    {
        light.enabled = true;
        flashON.Play();
        yield return new WaitForSeconds(0.2f);
        flash = true;
    }
    
    private IEnumerator notFlash()
    { 
        light.enabled = false;
        flashOFF.Play();
        yield return new WaitForSeconds(0.2f);
        flash = false;
    }

    private IEnumerator Reload()
    {
        reload = true;
        batteries -= 1;
        lifetime += batteries_add;
        yield return new WaitForSeconds(0.5f);
        reload = false;
    }

    void Update()
    {
        Lifetime.text = "CHARGE: " + lifetime.ToString("0") + "%";
        batteryText.text = "BATTERIES: " + batteries.ToString();

        if (Player.GetComponent<StarterAssetsInputs>().flash | Input.GetKey(flashlight) && !flash)
        {
            StartCoroutine(Flash());
        }
        else if (Player.GetComponent<StarterAssetsInputs>().flash | Input.GetKey(flashlight) && flash)
        {
            StartCoroutine(notFlash());
        }

        if (Player.GetComponent<StarterAssetsInputs>().reloadFlash | Input.GetKey(reloadFlash) && !reload)
        {
            StartCoroutine (Reload());
        }


            if (flash)
        {
            lifetime -= 1 * Time.deltaTime;
        }

        if(lifetime <= 0)
        {
            light.enabled = false;
            flash = false;
            lifetime = 0;
        }

        if (lifetime >= 100)
        {
            lifetime = 100;
        }

        if(batteries <= 0)
        {
            batteries = 0;
        }
    }
}
