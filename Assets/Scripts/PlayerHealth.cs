// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("���������")]
    [Tooltip("��� �������, ������� ������� ���� ������ (��������, ��������� ����)")]
    public string damageSourceTag = "EnemyBullet";

    [Header("UI")]
    [Tooltip("UI Image, ������� ����� ����������� ������� ��� ������")]
    public Image deathScreenImage;

    [Tooltip("����� ������� ������ ����� ������ ������������� ����")]
    public float restartDelay = 2f;

    private bool isDead = false;

    // ���� ����� ���������� ������������� ��� ������������
    void OnCollisionEnter(Collision collision)
    {
        // ���� ����� ��� �����, �������
        if (isDead)
        {
            return;
        }

        // ���������, ����� �� ������, � ������� �� �����������, ������ ���
        if (collision.gameObject.CompareTag(damageSourceTag))
        {
            // ���������� ����, ����� ��� �� ��������� �������� (�� ������ ������)
            Destroy(collision.gameObject);

            // ��������� ������� ������
            Die();
        }
    }

    // ��������� �����, ����� ����� ���� ������� ������ � �� ������ ��������, ���� �����������
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("����� ����!");
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        if (deathScreenImage != null)
        {
            // ������� ��������� �������� ������
            float fadeDuration = 0.5f;
            float timer = 0f;
            Color startColor = deathScreenImage.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // ������ ������������

            // ��������, ��� ���� �������
            deathScreenImage.color = new Color(1f, 0f, 0f, 0f);
            startColor = deathScreenImage.color;

            while (timer < fadeDuration)
            {
                deathScreenImage.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
                timer += Time.deltaTime;
                yield return null;
            }
            deathScreenImage.color = endColor;
        }

        yield return new WaitForSeconds(restartDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}