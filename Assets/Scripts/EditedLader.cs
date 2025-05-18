using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.UI;
using UnityEngine.Rendering;


public class EditedLader : MonoBehaviour
{
    bool inside = false;
    public float speed = 4f;
    public EditedPersonController player;
    public StarterAssetsInputs inputs;
    public AudioSource sound;
    [SerializeField] private KeyCode Interact = KeyCode.E;
    [SerializeField] private Image crosshair = null;
    public GameObject UseText;
    bool use = false;

    private void Start()
    {
        player = GetComponent<EditedPersonController>();
        inside = false;
        player._verticalVelocity = 0f;
        inputs = GetComponent<StarterAssetsInputs>();
        inputs.use = false;
    }

    private void Use()
    {
        if (inputs.use | Input.GetKey(Interact))
        {
            use = true;
        }
        else
        {
            use = false; 
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            Debug.Log("TouchLadderTrue");
            inside = !inside;
            UseText.SetActive(true);
            CrosshairChange(true);
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            Debug.Log("TouchLadderFalse");
            inside = !inside;
            UseText.SetActive(false);
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
    void Update()
    {
        Use();

        if (inside == true && use)
        {

            player._verticalVelocity = speed;
            player.targetSpeed = 0f;

            sound.enabled = true;
            sound.loop = true;
        }
        else
        {
            sound.enabled = false;
            sound.loop = false;
        }
    }
}
