using UnityEngine;
using StarterAssets;

public class FootstepScript : MonoBehaviour
{
    public GameObject footsteps;
    StarterAssetsInputs _input;
    private FirstPersonController controller;

    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        controller = GetComponent<FirstPersonController>();
        footsteps.SetActive(false);
        controller._isCrouching = false;
    }

    void Update()
    {
        if (_input.move == Vector2.zero | controller._isCrouching)
        {
            StopFootsteps();
        }

        if (_input.move != Vector2.zero && !controller._isCrouching)
        {
            Footsteps();
        }

    }

    void Footsteps()
    {
       footsteps.SetActive(true);
    }

    void StopFootsteps()
    {
        footsteps.SetActive(false);
    }

}