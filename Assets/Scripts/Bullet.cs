// Bullet.cs
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f;

    void Awake()
    {
        // Запускаем таймер на самоуничтожение
        Destroy(gameObject, lifeTime);
    }

    // При столкновении пуля просто уничтожается. 
    // Логика урона будет на том, в кого попали.
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}