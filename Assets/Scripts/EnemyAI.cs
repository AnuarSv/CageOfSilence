using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Основное")]
    public Transform[] patrolPoints;
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float sightRange = 15f;
    public float attackRange = 10f;
    public float fireRate = 1f;
    public float fleeDistance = 10f;
    public AudioSource walkAudio;
    public AudioSource shootAudio;
    public AudioSource reloadAudio;

    [Header("Скорости")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3.5f;
    public float fleeSpeed = 5f;

    private NavMeshAgent agent;
    private Transform player;
    private int patrolIndex;
    private bool reloading = false;
    private float nextFireTime = 0f;
    private int bulletsLeft = 8;
    private int missedShots = 0;
    private Vector3 lastSeenPosition;
    private bool playerVisible;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        PatrolToNextPoint();
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 dirToPlayer = player.position - transform.position;
        Ray ray = new Ray(transform.position + Vector3.up * 1.5f, dirToPlayer.normalized);
        playerVisible = false;

        if (Physics.Raycast(ray, out RaycastHit hit, sightRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                lastSeenPosition = player.position;
                playerVisible = true;
            }
        }

        if (reloading)
        {
            Flee();
            return;
        }

        if (playerVisible && distanceToPlayer <= attackRange)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(transform.position); // остановиться
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

            if (Time.time >= nextFireTime && bulletsLeft > 0)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }

            if (bulletsLeft <= 0)
            {
                StartCoroutine(Reload());
            }
        }
        else if (playerVisible)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            if (!walkAudio.isPlaying) walkAudio.Play();
        }
        else if (lastSeenPosition != Vector3.zero)
        {
            agent.speed = patrolSpeed;
            agent.SetDestination(lastSeenPosition);
            if (Vector3.Distance(transform.position, lastSeenPosition) < 1f)
                lastSeenPosition = Vector3.zero;
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
            PatrolToNextPoint();
        }
    }

    void PatrolToNextPoint()
    {
        if (patrolPoints.Length == 0) return;
        agent.SetDestination(patrolPoints[patrolIndex].position);
        if (!walkAudio.isPlaying) walkAudio.Play();
    }

    void Shoot()
    {
        if (player == null) return;

        Vector3 shootDir;

        if (missedShots < 3)
        {
            shootDir = (player.position - firePoint.position + Random.insideUnitSphere * 2f).normalized;
            missedShots++;
        }
        else
        {
            shootDir = (player.position - firePoint.position).normalized;
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(shootDir));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = shootDir * bulletSpeed;

        bulletsLeft--;
        if (shootAudio != null) shootAudio.Play();
    }

    void Flee()
    {
        agent.speed = fleeSpeed;
        Vector3 dirAway = (transform.position - player.position).normalized;
        Vector3 fleeTarget = transform.position + dirAway * fleeDistance;
        agent.SetDestination(fleeTarget);
    }

    System.Collections.IEnumerator Reload()
    {
        reloading = true;
        if (reloadAudio != null) reloadAudio.Play();
        yield return new WaitForSeconds(3f);
        bulletsLeft = 8;
        missedShots = 0;
        reloading = false;
    }
}
