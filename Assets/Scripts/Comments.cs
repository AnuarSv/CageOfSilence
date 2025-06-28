using UnityEngine;
using TMPro;
using System.Collections;

public class Comments : MonoBehaviour
{
    public GameObject textField;
    private TextMeshProUGUI _textField;
    [SerializeField] private bool Repeatable;
    [SerializeField] private string comment;
    public float SecBeforeSymbol = 0.05f;
    private bool commentOnce = false;
    private bool commenting = false;
    private char[] com;

    void Start()
    {
        _textField = textField.GetComponent<TextMeshProUGUI>();
        com = new char[comment.Length];
        com = comment.ToCharArray();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!commenting)
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

    private IEnumerator Comment()
    {
        commenting = true;
        _textField.text = "";
        textField.SetActive(true);
        for (int i = 0; i < com.Length; i += 1)
        {
            _textField.text += com[i];
            yield return new WaitForSeconds(SecBeforeSymbol);
        }
        yield return new WaitForSeconds(0.8f);
        textField.SetActive(false);
        commenting = false;
    }
}
