using UnityEngine;
using StarterAssets;
using Unity.VisualScripting;

public class FootstepScript : MonoBehaviour
{
    public GameObject footstep;
    StarterAssetsInputs _input;
    KeyCode crouchKey = KeyCode.LeftControl;
    public bool crouch = false;

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        footstep.SetActive(false);
    }

    void Update()
    {
        if (_input.move == Vector2.zero | crouch)
        {
            StopFootsteps();
        }

        if (_input.move != Vector2.zero && !crouch)
        {
            Footsteps();
        }

        if (Input.GetKeyDown(crouchKey))
        {
            crouch = true;
        }

        if (Input.GetKeyUp(crouchKey))
        {
            crouch = false;
        }

    }

    void Footsteps()
    {
       footstep.SetActive(true);
    }

    void StopFootsteps()
    {
        footstep.SetActive(false);
    }

}