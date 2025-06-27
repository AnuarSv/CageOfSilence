using UnityEngine;
using StarterAssets;
using UnityEngine.UI;
using System.Collections;

public class Picklock : MonoBehaviour
{
    public GameObject Pointer;
    public GameObject PicklockUI;

    public AudioSource ButtonPress;
    public AudioSource Correct;
    public AudioSource Incorrect;

    private RectTransform position;
    private Canvas canvas;

    public GameObject Player;
    public GameObject Hud;
    public GameObject ImagePrefab;

    public GameObject ButtonUI;
    private PicklockButton ButtonUIScript;
    private bool bUIraw;
    private bool bUI;

    private GameObject image0;
    [HideInInspector] public bool Pressed0 = false;
    private GameObject image1;
    [HideInInspector] public bool Pressed1 = false;
    private GameObject image2;
    [HideInInspector] public bool Pressed2 = false;
    private GameObject image3;
    [HideInInspector] public bool Pressed3 = false;

    private float speed;
    private float targetspeed;
    private int ObjectsToSpawn = 4;
    private int Spawned;
    private float point = -100f;

    [SerializeField] private Image crosshair = null;
    [SerializeField] private KeyCode Interact = KeyCode.E;
    bool use = false;

    [Header("Pointer Speed")]
    public float minspeed;
    public float maxspeed;
    public float totalincrement;
    public float lerpincrement;

    private bool bound = false;
    private bool inReach;
    private bool doOnce = false;
    private bool pressOnce = false;
    [HideInInspector] public bool isCorrect = false;

    void OnEnable()
    {
        position = Pointer.GetComponent<RectTransform>();
        canvas = PicklockUI.GetComponent<Canvas>();
        Player.GetComponent<StarterAssetsInputs>().use = false;
        ButtonUIScript = ButtonUI.GetComponent<PicklockButton>();
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
            point = -100f;
            
        }

        if (PicklockUI.activeSelf)
        {
            bUIraw = ButtonUIScript.Pressed;
            if (ObjectsToSpawn > Spawned)
            {
                Spawn();
            }

            Move();
            DoTouch();
        }

        if (Pressed0 && Pressed1 && Pressed2 && Pressed3)
        {
            StartCoroutine(Close());
            if (!isCorrect)
            {
                StartCoroutine(Sound());
            }
            isCorrect = true;
            StopAllCoroutines();
        }
    }

    void Move()
    {
        Pointer.transform.Translate(Vector3.right * speed * Time.smoothDeltaTime);

        if (!bound)
        {
            speed = Mathf.Lerp(speed, targetspeed, lerpincrement);
        }
        else if (bound)
        {
            speed = Mathf.Lerp(speed, -targetspeed, lerpincrement);
        }

        if (position.anchoredPosition.x >= 100f)
        {
            if (!bound)
            {
                targetspeed = Mathf.Clamp(targetspeed + totalincrement, minspeed, maxspeed);
            }
            bound = true;
        }
        else if (position.anchoredPosition.x <= -100f)
        {
            if (bound)
            {
                targetspeed = Mathf.Clamp(targetspeed + totalincrement, minspeed, maxspeed);
            }
            bound = false;
        }
    }

    private void Use()
    {
        if (Player.GetComponent<StarterAssetsInputs>().use | Input.GetKey(Interact))
        {
            use = true;
            Reset();
        }
        else
        {
            use = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            CrosshairChange(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            CrosshairChange(false);
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

    private IEnumerator Open()
    {
        targetspeed = minspeed;
        PicklockUI.SetActive(true);
        Hud.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        doOnce = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private IEnumerator Close()
    {
        PicklockUI.SetActive(false);
        Hud.SetActive(true);
        StopCoroutine(Press());
        Destroy();
        yield return new WaitForSeconds(0.3f);
        doOnce = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Spawn()
    {
        if (Spawned == 0)
        {
            image0 = Instantiate(ImagePrefab);

            image0.transform.SetParent(canvas.transform, false);
            image0.transform.localPosition = new Vector2(point, 0);
            image0.transform.localScale = new Vector2(0.35f, 1f);
            image0.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }
        else if (Spawned == 1)
        {
            image1 = Instantiate(ImagePrefab);

            image1.transform.SetParent(canvas.transform, false);
            image1.transform.localPosition = new Vector2(point, 0);
            image1.transform.localScale = new Vector2(0.35f, 1f);
            image1.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }
        else if (Spawned == 2)
        {
            image2 = Instantiate(ImagePrefab);

            image2.transform.SetParent(canvas.transform, false);
            image2.transform.localPosition = new Vector2(point, 0);
            image2.transform.localScale = new Vector2(0.35f, 1f);
            image2.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }
        else if (Spawned == 3)
        {
            image3 = Instantiate(ImagePrefab);

            image3.transform.SetParent(canvas.transform, false);
            image3.transform.localPosition = new Vector2(point, 0);
            image3.transform.localScale = new Vector2(0.35f, 1f);
            image3.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        }

        Pointer.transform.SetAsLastSibling();
        point += Random.Range(39f, 69f);
        Spawned++;
    }

    void DoTouch()
    {
        if (image3 != null)
        {
            if (bUIraw && !pressOnce)
            {
                StartCoroutine(Press());
                pressOnce = true;
            }
            else if (!bUIraw)
            {
                pressOnce = false;
            }

            if (position.localPosition.x + position.localScale.x * position.rect.width / 2 >= image0.GetComponent<RectTransform>().localPosition.x - image0.GetComponent<RectTransform>().localScale.x * image0.GetComponent<RectTransform>().rect.width / 2 && position.localPosition.x - position.localScale.x * position.rect.width / 2 <= image0.GetComponent<RectTransform>().localPosition.x + image0.GetComponent<RectTransform>().localScale.x * image0.GetComponent<RectTransform>().rect.width / 2)
            {
                if (bUI)
                {
                    image0.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
                    Pressed0 = true;
                    ButtonPress.Play();
                }
            }
            else if (position.localPosition.x + position.localScale.x * position.rect.width / 2 >= image1.GetComponent<RectTransform>().localPosition.x - image1.GetComponent<RectTransform>().localScale.x * image1.GetComponent<RectTransform>().rect.width / 2 && position.localPosition.x - position.localScale.x * position.rect.width / 2 <= image1.GetComponent<RectTransform>().localPosition.x + image1.GetComponent<RectTransform>().localScale.x * image1.GetComponent<RectTransform>().rect.width / 2)
            {
                if (bUI)
                {
                    image1.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
                    Pressed1 = true;
                    ButtonPress.Play();
                }
            }
            else if (position.localPosition.x + position.localScale.x * position.rect.width / 2 >= image2.GetComponent<RectTransform>().localPosition.x - image2.GetComponent<RectTransform>().localScale.x * image2.GetComponent<RectTransform>().rect.width / 2 && position.localPosition.x - position.localScale.x * position.rect.width / 2 <= image2.GetComponent<RectTransform>().localPosition.x + image2.GetComponent<RectTransform>().localScale.x * image2.GetComponent<RectTransform>().rect.width / 2)
            {
                if (bUI)
                {
                    image2.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
                    Pressed2 = true;
                    ButtonPress.Play();
                }
            }
            else if (position.localPosition.x + position.localScale.x * position.rect.width / 2 >= image3.GetComponent<RectTransform>().localPosition.x - image3.GetComponent<RectTransform>().localScale.x * image3.GetComponent<RectTransform>().rect.width / 2 && position.localPosition.x - position.localScale.x * position.rect.width / 2 <= image3.GetComponent<RectTransform>().localPosition.x + image3.GetComponent<RectTransform>().localScale.x * image3.GetComponent<RectTransform>().rect.width / 2)
            {
                if (bUI)
                {
                    image3.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);
                    Pressed3 = true;
                    ButtonPress.Play();
                }
            }
            else
            {
                if (bUI)
                {
                    Reset();
                    Incorrect.Play();
                }
            }
        }
    }

    IEnumerator Press()
    {
        bUI = true;
        yield return new WaitForEndOfFrame();
        bUI = false;
    }

    IEnumerator Sound()
    {
        Correct.Play();
        yield return new WaitForSeconds(Correct.clip.length);
        Correct.Stop();
    }

    void Reset()
    {
        if (image0 != null)
            image0.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        Pressed0 = false;
        if (image1 != null)
            image1.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        Pressed1 = false;
        if (image2 != null)
            image2.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        Pressed2 = false;
        if (image3 != null)
            image3.GetComponent<Image>().color = new Color(1, 0, 0, 1);
        Pressed3 = false;
    }

    void Destroy()
    {
        Destroy(image0);
        Destroy(image1);
        Destroy(image2);
        Destroy(image3);
        Spawned = 0;
    }
}
