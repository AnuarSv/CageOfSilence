using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ColorCode : MonoBehaviour
{
    [SerializeField] private KeyCode Interact = KeyCode.E;
    public GameObject Player;
    public AudioSource Press;
    public AudioSource Correct;
    public AudioSource Incorrect;
    bool use = false;

    [SerializeField] private Image crosshair = null;
    public GameObject colorcodeUI;
    public GameObject hud;
    public GameObject TextE;
    [HideInInspector] public bool inReach;
    private bool doOnce = false;

    public GameObject Red;
    private bool rOnce = false;
    public GameObject Green;
    private bool gOnce = false;
    public GameObject Blue;
    private bool bOnce = false;
    public GameObject Pink;
    private bool pOnce = false;
    public GameObject Yellow;
    private bool yOnce = false;
    public GameObject Brown;
    private bool brOnce = false;
    public GameObject Magenta;
    private bool mOnce = false;
    public GameObject Cyan;
    private bool cOnce = false;
    public GameObject DarkGreen;
    private bool dgOnce = false;

    public string CorrectCode = "r/g/b/c/m/y/br/p/dg/";
    [HideInInspector] public string InputCode;
    [HideInInspector] public bool isCorrect = false;
    void Start()
    {
        colorcodeUI.SetActive(false);
        hud.SetActive(true);
        TextE.SetActive(false);

        inReach = false;

        Player.GetComponent<StarterAssetsInputs>().use = false;

        Red.GetComponent<ColorButton>().PressedRed = false;
        Green.GetComponent<ColorButton>().PressedGreen = false;
        Blue.GetComponent<ColorButton>().PressedBlue = false;
        Pink.GetComponent<ColorButton>().PressedPink = false;
        Yellow.GetComponent<ColorButton>().PressedYellow = false;
        Brown.GetComponent<ColorButton>().PressedBrown = false;
        Magenta.GetComponent<ColorButton>().PressedMagenta = false;
        Cyan.GetComponent<ColorButton>().PressedCyan = false;
        DarkGreen.GetComponent<ColorButton>().PressedDarkGreen = false;
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
            TextE.SetActive(true);
            CrosshairChange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            TextE.SetActive(false);
            CrosshairChange(false);
        }
    }

    private IEnumerator Open()
    {
        colorcodeUI.SetActive(true);
        hud.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        doOnce = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    private IEnumerator Close()
    {
        colorcodeUI.SetActive(false);
        hud.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        doOnce = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Use();

        if (use && inReach && !doOnce && !isCorrect)
        {
            StartCoroutine(Open());
        }
        else if (use && doOnce)
        {
            StartCoroutine(Close());
        }

        if (Red.GetComponent<ColorButton>().PressedRed && !rOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "r/";
            Debug.Log(InputCode);
            rOnce = true;
            Red.GetComponent<Image>().color = Color.grey;
        }
        if (Green.GetComponent<ColorButton>().PressedGreen && !gOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "g/";
            Debug.Log(InputCode);
            gOnce = true;
            Green.GetComponent<Image>().color = Color.grey;
        }
        if (Blue.GetComponent<ColorButton>().PressedBlue && !bOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "b/";
            Debug.Log(InputCode);
            bOnce = true;
            Blue.GetComponent<Image>().color = Color.grey;
        }
        if (Pink.GetComponent<ColorButton>().PressedPink && !pOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "p/";
            Debug.Log(InputCode);
            pOnce = true;
            Pink.GetComponent<Image>().color = Color.grey;
        }
        if (Yellow.GetComponent<ColorButton>().PressedYellow && !yOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "y/";
            Debug.Log(InputCode);
            yOnce = true;
            Yellow.GetComponent<Image>().color = Color.grey;
        }
        if (Brown.GetComponent<ColorButton>().PressedBrown && !brOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "br/";
            Debug.Log(InputCode);
            brOnce = true;
            Brown.GetComponent<Image>().color = Color.grey;
        }
        if (Magenta.GetComponent<ColorButton>().PressedMagenta && !mOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "m/";
            Debug.Log(InputCode);
            mOnce = true;
            Magenta.GetComponent<Image>().color = Color.grey;
        }
        if (Cyan.GetComponent<ColorButton>().PressedCyan && !cOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "c/";
            Debug.Log(InputCode);
            cOnce = true;
            Cyan.GetComponent<Image>().color = Color.grey;
        }
        if (DarkGreen.GetComponent<ColorButton>().PressedDarkGreen && !dgOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "dg/";
            Debug.Log(InputCode);
            dgOnce = true;
            DarkGreen.GetComponent<Image>().color = Color.grey;
        }

        if (Red.GetComponent<ColorButton>().PressedRed && Green.GetComponent<ColorButton>().PressedGreen && Blue.GetComponent<ColorButton>().PressedBlue && Pink.GetComponent<ColorButton>().PressedPink && Yellow.GetComponent<ColorButton>().PressedYellow && Brown.GetComponent<ColorButton>().PressedBrown && Magenta.GetComponent<ColorButton>().PressedMagenta && Cyan.GetComponent<ColorButton>().PressedCyan && DarkGreen.GetComponent<ColorButton>().PressedDarkGreen)
        {
            if (InputCode == CorrectCode)
            {
                Correct.Play();
                isCorrect = true;
                Debug.Log("Correct!");
                StartCoroutine(Close());
                Reset();
            }
            else
            {
                Incorrect.Play();
                Reset();
            }
        }
    }

    private IEnumerator PressSound()
    {
        Press.Play();
        yield return new WaitForSeconds(Press.clip.length);
        Press.Stop();
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

    void Reset()
    {
        Red.GetComponent<ColorButton>().PressedRed = false;
        Green.GetComponent<ColorButton>().PressedGreen = false;
        Blue.GetComponent<ColorButton>().PressedBlue = false;
        Pink.GetComponent<ColorButton>().PressedPink = false;
        Yellow.GetComponent<ColorButton>().PressedYellow = false;
        Brown.GetComponent<ColorButton>().PressedBrown = false;
        Magenta.GetComponent<ColorButton>().PressedMagenta = false;
        Cyan.GetComponent<ColorButton>().PressedCyan = false;
        DarkGreen.GetComponent<ColorButton>().PressedDarkGreen = false;

        Red.GetComponent<Image>().color = Color.white;
        Green.GetComponent<Image>().color = Color.white;
        Blue.GetComponent<Image>().color = Color.white;
        Pink.GetComponent<Image>().color = Color.white;
        Yellow.GetComponent<Image>().color = Color.white;
        Brown.GetComponent<Image>().color = Color.white;
        Magenta.GetComponent<Image>().color = Color.white;
        Cyan.GetComponent<Image>().color = Color.white;
        DarkGreen.GetComponent<Image>().color = Color.white;

        rOnce = false;
        gOnce = false;
        bOnce = false;
        pOnce = false;
        yOnce = false;
        brOnce = false;
        mOnce = false;
        cOnce = false;
        dgOnce = false;

        InputCode = "";
    }
}
