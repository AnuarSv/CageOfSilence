using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using StarterAssets;
using TMPro;


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

    // Comments
    public GameObject textField;
    private TextMeshProUGUI _textField;
    [SerializeField] private bool Repeatable;
    [SerializeField] private string comment;
    public float SecBeforeSymbol = 0.05f;
    private bool callOnce = false;
    private char[] com;
    private bool commentOnce = false;

    void Start()
    {
        inReach = false;
        openText.SetActive(false);
        keyMissingText.SetActive(false);
        ObjectInside.SetActive(false);

        Player.GetComponent<StarterAssetsInputs>().use = false;

        // Comments
        _textField = textField.GetComponent<TextMeshProUGUI>();
        com = new char[comment.Length];
        com = comment.ToCharArray();
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

    // Comments
    private IEnumerator Comment()
    {
        _textField.text = "";
        textField.SetActive(true);
        for (int i = 0; i < com.Length; i += 1)
        {
            _textField.text += com[i];
            yield return new WaitForSeconds(SecBeforeSymbol);
        }
        yield return new WaitForSeconds(0.8f);
        textField.SetActive(false);
        callOnce = false;
    }

    // Comments
    private void CallComment()
    {
        if (!callOnce)
        {
            if (!Repeatable)
            {
                if (!commentOnce)
                {
                    StartCoroutine(Comment());
                    commentOnce = true;
                }
            }
            else
            {
                StartCoroutine(Comment());
            }
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

            // Comments
            CallComment();
            callOnce = true;
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
