using UnityEngine;
using UnityEngine.AI;
using System.Collections;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{

    [Header("�������� ���������")]
    public Transform firePoint;          // �����, ������ �������� ����
    public GameObject bulletPrefab;      // ������ ����
    public float bulletSpeed = 20f;      // �������� ������ ����
    public float attackRange = 15f;      // ��������� ��� �����
    public float fireRate = 1f;          // ���������������� (��������� � �������)
    public float fleeDistance = 20f;     // �� ����� ���������� ������� ��� �����������

    [Header("�����")]
    public AudioSource walkAudio;        // ���� ������
    public AudioSource shootAudio;       // ���� ��������
    public AudioSource reloadAudio;      // ���� �����������

    [Header("��������")]
    public float wanderSpeed = 2f;       // �������� � �������
    public float chaseSpeed = 4f;        // �������� � ������
    public float fleeSpeed = 5f;         // �������� ��� �������

    [Header("�������������� (���������)")]
    public Transform[] patrolPoints;     // ������ ����� ��� ��������������
    public float wanderPauseTime = 4f;   // ����� �������� �� ����� �������
    public bool randomPatrolOrder = false; // ��������� �� ������ ��������?

    [Header("�����������")]
    public float sightRange = 25f;       // ��������� ������ (��� �������� ������)
    public float crouchSightMultiplier = 0.5f; // ��������� ��������� ������, ���� ����� �����
    [Range(0, 360)] public float fieldOfViewAngle = 90f;   // ���� ��������� ������
    [Range(0, 360)] public float peripheralViewAngle = 160f; // ���� ������������� (��������) ������
    public float peripheralSightMultiplier = 0.4f; // ��������� ��������� ��� �������� ������
    public float behindDetectionRadius = 2.5f; // ������ ����������� ������ ����� (����� �����)

    [Header("�������")]
    public float flashlightDetectionRange = 30f; // ���������, �� ������� ����� �������
    public Light flashlight;             // ������ �� ��������� Light �������� ������

    [Header("�����")]
    public float searchRadius = 10f;     // ������ ������ ������ ��������� ��������� �������
    public float searchDuration = 10f;   // ��� ����� ������, ������ ��� ��������� � �������

    // === ��������� ���������� ===

    // ��������� ��
    private enum AIState
    {
        Patrol,       // ��������������
        Chase,        // ������
        Attack,       // �����
        Search,       // ����� ������
        Investigate,  // �������� ��������������� ����� (��������, ���� ������)
        Flee          // ������� (�� ����� �����������)
    }
    private AIState currentState;

    // ���������� � ������
    private NavMeshAgent agent;
    private Transform player;
    private StarterAssets.EditedPersonController playerController; // ���������� ������������ ���������� ��� �������� �������

    // ������ ���
    private bool isReloading = false;
    private float nextFireTime = 0f;
    private int bulletsLeft = 8;
    private int magazineSize = 8;

    // ������ ���������
    private Vector3 lastKnownPosition;   // ��������� �������, ��� ��� ������� �����
    private float stateTimer;            // ������ ��� ��������� (�����, ��������)
    private int patrolIndex = -1;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // ����� ������ � ��� �����������
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerController = playerObj.GetComponent<StarterAssets.EditedPersonController>();
        }
        else
        {
            Debug.LogError("����� � ����� 'Player' �� ������! �� �� ����� ��������.");
            this.enabled = false; // ��������� ������, ���� ��� ������
            return;
        }

        // ��������� ��������, ���� �� ������
        if (flashlight == null)
        {
            var foundLight = FindObjectOfType<Light>(); // ���������� �����, ����� ������� �������
            if (foundLight != null && foundLight.type == LightType.Spot)
            {
                flashlight = foundLight;
                Debug.Log("������� ������ ������ �������������.");
            }
        }

        // ��������� ���������
        currentState = AIState.Patrol;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (player == null) return; // �������� �� ������, ���� ����� ��� ���������

        // ������� ������������� ���������. ��� ������ ��������� �� ��������.
        switch (currentState)
        {
            case AIState.Patrol:
                UpdatePatrolState();
                break;
            case AIState.Chase:
                UpdateChaseState();
                break;
            case AIState.Attack:
                UpdateAttackState();
                break;
            case AIState.Search:
                UpdateSearchState();
                break;
            case AIState.Investigate:
                UpdateInvestigateState();
                break;
            case AIState.Flee:
                UpdateFleeState();
                break;
        }
    }

    // --- ������ ��������� ---

    private void UpdatePatrolState()
    {
        // ������������� �������� ��� ��������������
        agent.speed = wanderSpeed;
        if (walkAudio != null && !walkAudio.isPlaying && agent.velocity.magnitude > 0.1f)
        {
            walkAudio.Play();
        }

        // ���������, �� ����� �� �����
        if (IsPlayerVisible())
        {
            TransitionToState(AIState.Chase);
            return;
        }

        // ���������, �� ����� �� ���� ��������
        if (IsFlashlightVisible())
        {
            TransitionToState(AIState.Investigate);
            return;
        }

        // ���� ����� �� ����� �������, ���� � ���� � ���������
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            stateTimer -= Time.deltaTime;
            if (stateTimer <= 0)
            {
                GoToNextPatrolPoint();
            }
        }
    }

    private void UpdateChaseState()
    {
        agent.speed = chaseSpeed;
        if (walkAudio != null && !walkAudio.isPlaying) walkAudio.Play();

        // ������ ��������� ���� � ��������� ��������� ������� ������
        agent.SetDestination(lastKnownPosition);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ���� ����� � ���� ����� � �� ��� �����, �������
        if (IsPlayerVisible() && distanceToPlayer <= attackRange)
        {
            TransitionToState(AIState.Attack);
        }
        // ���� �������� ������ �� ����, �������� �����
        else if (!IsPlayerVisible())
        {
            TransitionToState(AIState.Search);
        }
    }

    private void UpdateAttackState()
    {
        agent.isStopped = true; // ������������� ��������
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // ���� ����� ������ �� ���� �����, �� �� ��� ��� ����� - � ������
        if (Vector3.Distance(transform.position, player.position) > attackRange && IsPlayerVisible())
        {
            TransitionToState(AIState.Chase);
            return;
        }

        // ���� �������� ������ (��������� �� �����) - ����
        if (!IsPlayerVisible())
        {
            TransitionToState(AIState.Search);
            return;
        }

        // ������ ��������
        if (Time.time >= nextFireTime)
        {
            if (bulletsLeft > 0)
            {
                Shoot();
            }
            else
            {
                StartCoroutine(ReloadCoroutine());
            }
        }
    }

    private void UpdateSearchState()
    {
        agent.speed = wanderSpeed;
        stateTimer -= Time.deltaTime;

        // ���� ����� ������ �����, ������������ � ��������������
        if (stateTimer <= 0)
        {
            TransitionToState(AIState.Patrol);
            return;
        }

        // ���� �� ����� ������ ����� ������� ������ - � ������
        if (IsPlayerVisible())
        {
            TransitionToState(AIState.Chase);
            return;
        }

        // ���� ����� �� ����� ������, ���� �����
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SearchForPlayer();
        }
    }

    private void UpdateInvestigateState()
    {
        agent.speed = wanderSpeed;
        agent.SetDestination(lastKnownPosition); // ���� � �����, ��� ��� ������� ����

        // ���� ������� ������ �� ���� - � ������
        if (IsPlayerVisible())
        {
            TransitionToState(AIState.Chase);
            return;
        }

        // ���� ����� �� �����, � ��� ������, �������� �����
        if (!agent.pathPending && agent.remainingDistance < 1.0f)
        {
            TransitionToState(AIState.Search);
        }
    }

    private void UpdateFleeState()
    {
        // ������ ������� ����������� ������ �������� ReloadCoroutine
        // ��� ��������� �������, ���� isReloading = true
    }


    // --- ��������������� ������� ---

    private void TransitionToState(AIState newState)
    {
        // ����� �� ����������� ���������
        OnStateExit(currentState);

        currentState = newState;

        // ���� � ����� ���������
        OnStateEnter(newState);
    }

    // ��� ������ ��� ����� � ���������
    private void OnStateEnter(AIState state)
    {
        switch (state)
        {
            case AIState.Patrol:
                GoToNextPatrolPoint();
                break;
            case AIState.Chase:
                agent.isStopped = false;
                break;
            case AIState.Attack:
                agent.isStopped = true;
                if (walkAudio != null) walkAudio.Stop();
                break;
            case AIState.Search:
                agent.isStopped = false;
                lastKnownPosition = player.position; // ���������� ������ �����, ��� ��������
                stateTimer = searchDuration;
                SearchForPlayer(); // �������� ������ �����
                break;
            case AIState.Investigate:
                agent.isStopped = false;
                // lastKnownPosition ��� ���������� � IsFlashlightVisible()
                break;
            case AIState.Flee:
                agent.isStopped = false;
                agent.speed = fleeSpeed;
                Vector3 fleeDir = (transform.position - player.position).normalized;
                Vector3 fleeTarget = transform.position + fleeDir * fleeDistance;

                NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, fleeDistance, NavMesh.AllAreas);
                agent.SetDestination(hit.position);
                break;
        }
    }

    // ��� ������ ��� ������ �� ���������
    private void OnStateExit(AIState state)
    {
        // ����� �������� ��� ������ �� ������ ���������, ���� �����
        // ��������, ���������, ��� ����� ����� ����� ���������, ���� ��� � �����
        if (state == AIState.Attack)
        {
            agent.isStopped = false;
        }
    }

    private void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        if (randomPatrolOrder)
        {
            patrolIndex = Random.Range(0, patrolPoints.Length);
        }
        else
        {
            patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
        }

        agent.SetDestination(patrolPoints[patrolIndex].position);
        stateTimer = wanderPauseTime; // ������������� ������ �������� �� �����
    }

    private void SearchForPlayer()
    {
        Vector3 randomPoint = lastKnownPosition + Random.insideUnitSphere * searchRadius;
        NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, searchRadius, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }

    // --- ������ ����������� ---

    private bool IsPlayerVisible()
    {
        // �������� �� ����� ������� ���������� (����������� �����)
        float dist = Vector3.Distance(transform.position, player.position);
        bool isCrouching = playerController.Grounded && Input.GetKey(KeyCode.LeftControl); // ������ �������� �������

        if (!isCrouching && dist <= behindDetectionRadius)
        {
            // ��������� ������ ���������, ���� ���� ������
            if (HasLineOfSight())
            {
                lastKnownPosition = player.position;
                return true;
            }
        }

        // �������� � ���� ������
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

        // ���������� ������� ��������� ��������� � ����������� �� ��������� ������
        float currentSightRange = isCrouching ? sightRange * crouchSightMultiplier : sightRange;

        // �������� � �������� ���� ������ (����� � �������)
        if (angleToPlayer < fieldOfViewAngle * 0.5f)
        {
            if (dist <= currentSightRange && HasLineOfSight())
            {
                lastKnownPosition = player.position;
                return true;
            }
        }

        // �������� � ������������ ���� ������ (�������, �� �������)
        if (angleToPlayer < peripheralViewAngle * 0.5f)
        {
            if (dist <= currentSightRange * peripheralSightMultiplier && HasLineOfSight())
            {
                lastKnownPosition = player.position;
                return true;
            }
        }

        return false;
    }

    private bool IsFlashlightVisible()
    {
        if (flashlight == null || !flashlight.enabled) return false;

        float distToFlashlight = Vector3.Distance(transform.position, flashlight.transform.position);
        if (distToFlashlight > flashlightDetectionRange) return false;

        Vector3 dirToFlashlight = (flashlight.transform.position - transform.position).normalized;

        // ������� ����� �������� � ����� ������� ����
        if (Vector3.Angle(transform.forward, dirToFlashlight) < fieldOfViewAngle)
        {
            // ���������, ��� �� ����� ����� �� � ���������� �����
            if (!Physics.Linecast(transform.position + Vector3.up, flashlight.transform.position, out RaycastHit hit))
            {
                lastKnownPosition = flashlight.transform.position;
                return true;
            }
        }

        return false;
    }

    private bool HasLineOfSight()
    {
        // ������� ��� �� "����" �� � "����" ������
        Vector3 rayStart = transform.position + Vector3.up * 1.5f; // ������ ���� ��
        Vector3 playerTarget = player.position + Vector3.up; // ����� ���� ������

        if (Physics.Linecast(rayStart, playerTarget, out RaycastHit hit))
        {
            // ���� ��� ����� � ������ - ������, �� � ������ ���������
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }


    // --- ������ ��� ---

    void Shoot()
    {
        if (player == null) return;

        // ������� ����
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // ���������� �� � ������
        Vector3 shootDir = (player.position + Vector3.up * 1.0f - firePoint.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(shootDir);

        // ������� ��������
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = shootDir * bulletSpeed;

        if (shootAudio != null) shootAudio.Play();

        bulletsLeft--;
        nextFireTime = Time.time + 1f / fireRate;
    }

    IEnumerator ReloadCoroutine()
    {
        if (isReloading) yield break; // ���� ��� ��������������, �������

        isReloading = true;

        // ��������� � ��������� �������
        TransitionToState(AIState.Flee);

        if (reloadAudio != null) reloadAudio.Play();

        yield return new WaitForSeconds(3.0f); // ����� �����������

        bulletsLeft = magazineSize;
        isReloading = false;

        // ����� ����������� �� ������������ � ��� �����, � �������� ������ ������
        TransitionToState(AIState.Search);
    }

    // --- ����� ��� ������� � ��������� ---

    void OnDrawGizmosSelected()
    {
        // ��������� �����
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // �������� ���� ������
        Gizmos.color = Color.yellow;
        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfViewAngle * 0.5f, transform.up) * transform.forward * sightRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfViewAngle * 0.5f, transform.up) * transform.forward * sightRange;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        // ������������ ���� ������
        Gizmos.color = new Color(1, 1, 0, 0.2f); // ���������� ������
        Vector3 periLine1 = Quaternion.AngleAxis(peripheralViewAngle * 0.5f, transform.up) * transform.forward * (sightRange * peripheralSightMultiplier);
        Vector3 periLine2 = Quaternion.AngleAxis(-peripheralViewAngle * 0.5f, transform.up) * transform.forward * (sightRange * peripheralSightMultiplier);
        Gizmos.DrawRay(transform.position, periLine1);
        Gizmos.DrawRay(transform.position, periLine2);

        // ����������� �����
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, behindDetectionRadius);

        // ��������� ����������� ��������
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, flashlightDetectionRange);

        // ������ ������
        if (currentState == AIState.Search || currentState == AIState.Investigate)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lastKnownPosition, searchRadius);
        }
    }
}