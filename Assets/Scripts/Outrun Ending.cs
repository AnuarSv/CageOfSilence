using UnityEngine;
using UnityEngine.SceneManagement;

public class OutrunEnding : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(2);
        }
    }
}
