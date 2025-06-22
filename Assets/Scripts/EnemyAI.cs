using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform firePoint;
    public Transform reloadPoint;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;

    [Header("Vision")]
    public float viewDistanceStanding = 20f;
    public float viewDistanceCrouching = 8f;

    [Header("Shooting")]
    public float shootCooldown = 1f;
    public int maxBullets = 8;

    [Header("Movement Speeds")]
    public float walkSpeed = 1.5f;
    public float runSpeed = 4f;

    [Header("Audio")]
    public AudioSource walkAudio;
    public AudioSource runAudio;
    public AudioSource shootAudio;
    public AudioSource reloadAudio;

    private NavMeshAgent agent;
    private int bulletsLeft;
    private int missedShots;
    private float lastShotTime;
    private bool isReloading = false;
    private Vector3 lastSeenPosition;
    private bool playerInSight = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        bulletsLeft = maxBullets;
    }

    void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float viewDistance = GetPlayerCrouchState() ? viewDistanceCrouching : viewDistanceStanding;

        if (isReloading)
        {
            RunToReload();
            return;
        }

        if (distToPlayer < viewDistance && CanSeePlayer())
        {
            lastSeenPosition = player.position;
            playerInSight = true;

            FacePlayer();
            agent.SetDestination(transform.position); // stop
            agent.speed = runSpeed;
            PlaySound(runAudio);

            if (Time.time - lastShotTime > shootCooldown && bulletsLeft > 0)
            {
                Shoot();
                lastShotTime = Time.time;
            }

            if (bulletsLeft <= 0)
            {
                isReloading = true;
            }
        }
        else if (playerInSight)
        {
            // Go to last seen position
            agent.speed = walkSpeed;
            PlaySound(walkAudio);
            agent.SetDestination(lastSeenPosition);
        }
    }

    void RunToReload()
    {
        agent.speed = runSpeed;
        PlaySound(runAudio);
        agent.SetDestination(reloadPoint.position);

        if (Vector3.Distance(transform.position, reloadPoint.position) < 1f)
        {
            bulletsLeft = maxBullets;
            missedShots = 0;
            isReloading = false;
            PlaySound(reloadAudio);
        }
    }

    void Shoot()
    {
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

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = shootDir * bulletSpeed;
        }

        bulletsLeft--;
        PlaySound(shootAudio);
    }

    bool CanSeePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        Ray ray = new Ray(transform.position + Vector3.up * 1.5f, dir);

        if (Physics.Raycast(ray, out RaycastHit hit, 50f))
        {
            return hit.collider.gameObject == player.gameObject;
        }

        return false;
    }

    void FacePlayer()
    {
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        if (lookPos != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 5f);
        }
    }

    bool GetPlayerCrouchState()
    {
        var playerController = player.GetComponent<StarterAssets.EditedPersonController>();
        return playerController != null && playerController._isCrouching;
    }

    void PlaySound(AudioSource source)
    {
        if (source != null && !source.isPlaying)
        {
            source.Play();
        }
    }
}
