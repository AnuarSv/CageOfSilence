// PlayerHealth.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Настройки")]
    [Tooltip("Тег объекта, который наносит урон игроку (например, вражеская пуля)")]
    public string damageSourceTag = "EnemyBullet";

    [Header("UI")]
    [Tooltip("UI Image, который будет становиться красным при смерти")]
    public Image deathScreenImage;

    [Tooltip("Через сколько секунд после смерти перезапустить игру")]
    public float restartDelay = 2f;

    private bool isDead = false;

    // Этот метод вызывается автоматически при столкновении
    void OnCollisionEnter(Collision collision)
    {
        // Если игрок уже мертв, выходим
        if (isDead)
        {
            return;
        }

        // Проверяем, имеет ли объект, с которым мы столкнулись, нужный тег
        if (collision.gameObject.CompareTag(damageSourceTag))
        {
            // Уничтожаем пулю, чтобы она не пролетела насквозь (на всякий случай)
            Destroy(collision.gameObject);

            // Запускаем процесс смерти
            Die();
        }
    }

    // Публичный метод, чтобы можно было вызвать смерть и из других скриптов, если понадобится
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Игрок умер!");
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        if (deathScreenImage != null)
        {
            // Плавное появление красного экрана
            float fadeDuration = 0.5f;
            float timer = 0f;
            Color startColor = deathScreenImage.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f); // Делаем непрозрачным

            // Убедимся, что цвет красный
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