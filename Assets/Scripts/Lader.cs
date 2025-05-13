using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;


public class Lader : MonoBehaviour
{
    public Transform playerController;
    bool inside = false;
    public float speed = 4f;
    public FirstPersonController player;
    public AudioSource sound;

    private void Start()
    {
        player = GetComponent<FirstPersonController>();
        inside = false;
    }
    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            Debug.Log("TouchLadderTrue");
            player.enabled = false;
            inside = !inside;
        }

    }
    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Ladder")
        {
            Debug.Log("TouchLadderFalse");
            player.enabled = true;
            inside = !inside;
        }

    }

    void Update()
    {
        if (inside == true && Input.GetKey("w"))
        {
            player.transform.position += Vector3.up /
                speed * Time.deltaTime;
        }
        if (inside == true && Input.GetKey("s"))
        {
            player.transform.position += Vector3.down /
                speed * Time.deltaTime;
        }
        if (inside == true && (Input.GetKey("w") || Input.GetKey("s")))
        {
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
