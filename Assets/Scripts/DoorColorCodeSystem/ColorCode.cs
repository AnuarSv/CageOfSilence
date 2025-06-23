using System.Collections;
using StarterAssets;
using UnityEngine;
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
    private ColorButton redButton;
    private bool red;
    private bool rOnce = false;

    public GameObject Green;
    private ColorButton greenButton;
    private bool green;
    private bool gOnce = false;

    public GameObject Blue;
    private ColorButton blueButton;
    private bool blue;
    private bool bOnce = false;

    public GameObject Pink;
    private ColorButton pinkButton;
    private bool pink;
    private bool pOnce = false;

    public GameObject Yellow;
    private ColorButton yellowButton;
    private bool yellow;
    private bool yOnce = false;

    public GameObject Brown;
    private ColorButton brownButton;
    private bool brown;
    private bool brOnce = false;

    public GameObject Magenta;
    private ColorButton magentaButton;
    private bool magenta;
    private bool mOnce = false;

    public GameObject Cyan;
    private ColorButton cyanButton;
    private bool cyan;
    private bool cOnce = false;

    public GameObject DarkGreen;
    private ColorButton dgButton;
    private bool dg;
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

        redButton = Red.GetComponent<ColorButton>();
        greenButton = Green.GetComponent<ColorButton>();
        blueButton = Blue.GetComponent<ColorButton>();
        pinkButton = Pink.GetComponent<ColorButton>();
        yellowButton = Yellow.GetComponent<ColorButton>();
        brownButton = Brown.GetComponent<ColorButton>();
        magentaButton = Magenta.GetComponent<ColorButton>();
        cyanButton = Cyan.GetComponent<ColorButton>();
        dgButton = DarkGreen.GetComponent<ColorButton>();
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
        red = redButton.Pressed;
        green = greenButton.Pressed;
        blue = blueButton.Pressed;
        pink = pinkButton.Pressed;
        yellow = yellowButton.Pressed;
        brown = brownButton.Pressed;
        magenta = magentaButton.Pressed;
        cyan = cyanButton.Pressed;
        dg = dgButton.Pressed;

        Use();

        if (use && inReach && !doOnce && !isCorrect)
        {
            StartCoroutine(Open());
        }
        else if (use && doOnce)
        {
            StartCoroutine(Close());
        }

        if (red && !rOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "r/";
            Debug.Log(InputCode);
            rOnce = true;
            Red.GetComponent<Image>().color = Color.grey;
        }
        if (green && !gOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "g/";
            Debug.Log(InputCode);
            gOnce = true;
            Green.GetComponent<Image>().color = Color.grey;
        }
        if (blue && !bOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "b/";
            Debug.Log(InputCode);
            bOnce = true;
            Blue.GetComponent<Image>().color = Color.grey;
        }
        if (pink && !pOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "p/";
            Debug.Log(InputCode);
            pOnce = true;
            Pink.GetComponent<Image>().color = Color.grey;
        }
        if (yellow && !yOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "y/";
            Debug.Log(InputCode);
            yOnce = true;
            Yellow.GetComponent<Image>().color = Color.grey;
        }
        if (brown && !brOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "br/";
            Debug.Log(InputCode);
            brOnce = true;
            Brown.GetComponent<Image>().color = Color.grey;
        }
        if (magenta && !mOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "m/";
            Debug.Log(InputCode);
            mOnce = true;
            Magenta.GetComponent<Image>().color = Color.grey;
        }
        if (cyan && !cOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "c/";
            Debug.Log(InputCode);
            cOnce = true;
            Cyan.GetComponent<Image>().color = Color.grey;
        }
        if (dg && !dgOnce)
        {
            StartCoroutine(PressSound());
            InputCode += "dg/";
            Debug.Log(InputCode);
            dgOnce = true;
            DarkGreen.GetComponent<Image>().color = Color.grey;
        }

        if (red && green && blue && pink && yellow && brown && magenta && cyan && dg)
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
        Red.GetComponent<ColorButton>().Pressed = false;
        Green.GetComponent<ColorButton>().Pressed = false;
        Blue.GetComponent<ColorButton>().Pressed = false;
        Pink.GetComponent<ColorButton>().Pressed = false;
        Yellow.GetComponent<ColorButton>().Pressed = false;
        Brown.GetComponent<ColorButton>().Pressed = false;
        Magenta.GetComponent<ColorButton>().Pressed = false;
        Cyan.GetComponent<ColorButton>().Pressed = false;
        DarkGreen.GetComponent<ColorButton>().Pressed = false;

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
