using System.Collections;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeLock: MonoBehaviour
{
    [SerializeField] private KeyCode Interact = KeyCode.E;
    public GameObject Player;
    public AudioSource Press;
    public AudioSource Correct;
    public AudioSource Incorrect;
    bool use = false;

    [SerializeField] private Image crosshair = null;
    public GameObject codeUI;
    public GameObject hud;
    public GameObject TextE;
    [HideInInspector] public bool inReach;
    private bool doOnce = false;
    private bool pressOnce = false;

    public GameObject B1;
    private Button b1Script;
    private bool b1;

    public GameObject B2;
    private Button b2Script;
    private bool b2;
    
    public GameObject B3;
    private Button b3Script;
    private bool b3;

    public GameObject B4;
    private Button b4Script;
    private bool b4;

    public GameObject B5;
    private Button b5Script;
    private bool b5;

    public GameObject B6;
    private Button b6Script;
    private bool b6;

    public GameObject B7;
    private Button b7Script;
    private bool b7;

    public GameObject B8;
    private Button b8Script;
    private bool b8;

    public GameObject B9;
    private Button b9Script;
    private bool b9;

    public GameObject B0;
    private Button b0Script;
    private bool b0;

    public GameObject BRes;
    private Button bResScript;
    private bool bRes;

    public GameObject BEnt;
    private Button bEntScript;
    private bool bEnt;


    [HideInInspector] public string InputCode = null;
    [HideInInspector] public bool isCorrect = false;
    public int CodeLength = 4;
    [SerializeField] private string CorrectCode;

    public TextMeshProUGUI CodeTextField;

    void Start()
    {
        codeUI.SetActive(false);
        hud.SetActive(true);
        TextE.SetActive(false);

        inReach = false;

        Player.GetComponent<StarterAssetsInputs>().use = false;

        b1Script = B1.GetComponent<Button>();
        b2Script = B2.GetComponent<Button>();
        b3Script = B3.GetComponent<Button>();
        b4Script = B4.GetComponent<Button>();
        b5Script = B5.GetComponent<Button>();
        b6Script = B6.GetComponent<Button>();
        b7Script = B7.GetComponent<Button>();
        b8Script = B8.GetComponent<Button>();
        b9Script = B9.GetComponent<Button>();
        b0Script = B0.GetComponent<Button>();
        bResScript = BRes.GetComponent<Button>();
        bEntScript = BEnt.GetComponent<Button>();
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
        if (other.gameObject.CompareTag("Reach"))
        {
            inReach = true;
            TextE.SetActive(true);
            CrosshairChange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Reach"))
        {
            inReach = false;
            TextE.SetActive(false);
            CrosshairChange(false);
        }
    }

    private IEnumerator Open()
    {
        codeUI.SetActive(true);
        hud.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        doOnce = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    private IEnumerator Close()
    {
        codeUI.SetActive(false);
        hud.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        doOnce = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        b1 = b1Script.Pressed;
        b2 = b2Script.Pressed;
        b3 = b3Script.Pressed;
        b4 = b4Script.Pressed;
        b5 = b5Script.Pressed;
        b6 = b6Script.Pressed;
        b7 = b7Script.Pressed;
        b8 = b8Script.Pressed;
        b9 = b9Script.Pressed;
        b0 = b0Script.Pressed;
        bRes = bResScript.Pressed;
        bEnt = bEntScript.Pressed;

        Use();

        if (use && inReach && !doOnce && !isCorrect)
        {
            StartCoroutine(Open());
        }
        else if (use && doOnce)
        {
            StartCoroutine(Close());
        }

        if (!pressOnce)
        {
            if (!(InputCode.Length >= CodeLength))
            {
                if (b1)
                {
                    Debug.Log("1");
                    InputCode += 1;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (b2)
                {
                    Debug.Log("2");
                    InputCode += 2;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (b3)
                {
                    Debug.Log("3");
                    InputCode += 3;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (b4)
                {
                    Debug.Log("4");
                    InputCode += 4;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (b5)
                {
                    Debug.Log("5");
                    InputCode += 5;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (b6)
                {
                    Debug.Log("6");
                    InputCode += 6;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (b7)
                {
                    Debug.Log("7");
                    InputCode += 7;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (b8)
                {
                    Debug.Log("8");
                    InputCode += 8;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (b9)
                {
                    Debug.Log("9");
                    InputCode += 9;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (b0)
                {
                    Debug.Log("0");
                    InputCode += 0;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
            }

            if (bRes)
            {
                Debug.Log("Reset");
                InputCode = "";
                Debug.Log(InputCode);
                CodeTextField.text = "0000";
                StartCoroutine(PressSound());
            }
            else if (bEnt)
            {
                Debug.Log("Enter");
                if (InputCode == CorrectCode)
                {
                    Correct.Play();
                    isCorrect = true;
                    Debug.Log("Correct!");
                    StartCoroutine(Close());
                }
                else
                {
                    Incorrect.Play();
                    InputCode = "";
                    CodeTextField.text = "0000";
                    Debug.Log("Incorrect!");
                    StartCoroutine(PressSound());
                }
            }
        }


        if (b1 | b2 | b3 | b4 | b5 | b6 | b7 | b8 | b9 | b0 | bRes | bEnt)
        {
            pressOnce = true;
            CodeTextField.text = InputCode;
        }
        else
        {
            pressOnce = false;
        }
    }

    private IEnumerator PressSound()
    {
        Press.Play();
        Debug.Log("Start");
        yield return new WaitForSeconds(Press.clip.length);
        Press.Stop();
        Debug.Log("Stop");
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
