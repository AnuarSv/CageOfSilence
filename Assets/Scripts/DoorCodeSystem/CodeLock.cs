using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
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

    public GameObject Button1;
    public GameObject Button2;
    public GameObject Button3;
    public GameObject Button4;
    public GameObject Button5;
    public GameObject Button6;
    public GameObject Button7;
    public GameObject Button8;
    public GameObject Button9;
    public GameObject Button0;
    public GameObject ButtonReset;
    public GameObject ButtonEnter;

    [HideInInspector] public string InputCode;
    [HideInInspector] public bool isCorrect = false;
    public int CodeLength = 4;
    public string CorrectCode = "2811";

    public TextMeshProUGUI CodeTextField;

    void Start()
    {
        codeUI.SetActive(false);
        hud.SetActive(true);
        TextE.SetActive(false);

        inReach = false;

        Player.GetComponent<StarterAssetsInputs>().use = false;

        Button1.GetComponent<Button>().PressedButton1 = false;
        Button2.GetComponent<Button>().PressedButton2 = false;
        Button3.GetComponent<Button>().PressedButton3 = false;
        Button4.GetComponent<Button>().PressedButton4 = false;
        Button5.GetComponent<Button>().PressedButton5 = false;
        Button6.GetComponent<Button>().PressedButton6 = false;
        Button7.GetComponent<Button>().PressedButton7 = false;
        Button8.GetComponent<Button>().PressedButton8 = false;
        Button9.GetComponent<Button>().PressedButton9 = false;
        Button0.GetComponent<Button>().PressedButton0 = false;
        ButtonReset.GetComponent<Button>().PressedButtonReset = false;
        ButtonEnter.GetComponent<Button>().PressedButtonEnter = false;
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
                if (Button1.GetComponent<Button>().PressedButton1)
                {
                    Debug.Log("1");
                    InputCode += 1;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (Button2.GetComponent<Button>().PressedButton2)
                {
                    Debug.Log("2");
                    InputCode += 2;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (Button3.GetComponent<Button>().PressedButton3)
                {
                    Debug.Log("3");
                    InputCode += 3;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (Button4.GetComponent<Button>().PressedButton4)
                {
                    Debug.Log("4");
                    InputCode += 4;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (Button5.GetComponent<Button>().PressedButton5)
                {
                    Debug.Log("5");
                    InputCode += 5;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (Button6.GetComponent<Button>().PressedButton6)
                {
                    Debug.Log("6");
                    InputCode += 6;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (Button7.GetComponent<Button>().PressedButton7)
                {
                    Debug.Log("7");
                    InputCode += 7;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (Button8.GetComponent<Button>().PressedButton8)
                {
                    Debug.Log("8");
                    InputCode += 8;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (Button9.GetComponent<Button>().PressedButton9)
                {
                    Debug.Log("9");
                    InputCode += 9;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
                else if (Button0.GetComponent<Button>().PressedButton0)
                {
                    Debug.Log("0");
                    InputCode += 0;
                    Debug.Log(InputCode);
                    StartCoroutine(PressSound());
                }
            }

            if (ButtonReset.GetComponent<Button>().PressedButtonReset)
            {
                Debug.Log("Reset");
                InputCode = "";
                Debug.Log(InputCode);
                CodeTextField.text = "0000";
                StartCoroutine(PressSound());
            }
            else if (ButtonEnter.GetComponent<Button>().PressedButtonEnter)
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


        if (Button0.GetComponent<Button>().PressedButton0 | Button1.GetComponent<Button>().PressedButton1 | Button2.GetComponent<Button>().PressedButton2 | Button3.GetComponent<Button>().PressedButton3 | Button4.GetComponent<Button>().PressedButton4 | Button5.GetComponent<Button>().PressedButton5 | Button6.GetComponent<Button>().PressedButton6 | Button7.GetComponent<Button>().PressedButton7 | Button8.GetComponent<Button>().PressedButton8 | Button9.GetComponent<Button>().PressedButton9 | ButtonReset.GetComponent<Button>().PressedButtonReset | ButtonEnter.GetComponent<Button>().PressedButtonEnter)
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
