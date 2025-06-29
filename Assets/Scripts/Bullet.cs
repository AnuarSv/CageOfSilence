// Bullet.cs
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 5f;

    void Awake()
    {
        // ��������� ������ �� ���������������
        Destroy(gameObject, lifeTime);
    }

    // ��� ������������ ���� ������ ������������. 
    // ������ ����� ����� �� ���, � ���� ������.
    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}