using UnityEngine;
using UnityEngine.AI;
using System.Collections;


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{

    [Header("Основные параметры")]
    public Transform firePoint;          // Точка, откуда вылетают пули
    public GameObject bulletPrefab;      // Префаб пули
    public float bulletSpeed = 20f;      // Скорость полета пули
    public float attackRange = 15f;      // Дистанция для атаки
    public float fireRate = 1f;          // Скорострельность (выстрелов в секунду)
    public float fleeDistance = 20f;     // На какое расстояние убегать при перезарядке

    [Header("Аудио")]
    public AudioSource walkAudio;        // Звук ходьбы
    public AudioSource shootAudio;       // Звук выстрела
    public AudioSource reloadAudio;      // Звук перезарядки

    [Header("Скорости")]
    public float wanderSpeed = 2f;       // Скорость в патруле
    public float chaseSpeed = 4f;        // Скорость в погоне
    public float fleeSpeed = 5f;         // Скорость при бегстве

    [Header("Патрулирование (Блуждание)")]
    public Transform[] patrolPoints;     // Массив точек для патрулирования
    public float wanderPauseTime = 4f;   // Время ожидания на точке патруля
    public bool randomPatrolOrder = false; // Двигаться по точкам случайно?

    [Header("Обнаружение")]
    public float sightRange = 25f;       // Дальность обзора (для стоящего игрока)
    public float crouchSightMultiplier = 0.5f; // Множитель дальности обзора, если игрок сидит
    [Range(0, 360)] public float fieldOfViewAngle = 90f;   // Угол основного обзора
    [Range(0, 360)] public float peripheralViewAngle = 160f; // Угол периферийного (бокового) зрения
    public float peripheralSightMultiplier = 0.4f; // Множитель дальности для бокового зрения
    public float behindDetectionRadius = 2.5f; // Радиус обнаружения игрока сзади (когда стоит)

    [Header("Фонарик")]
    public float flashlightDetectionRange = 30f; // Дистанция, на которой виден фонарик
    public Light flashlight;             // Ссылка на компонент Light фонарика игрока

    [Header("Поиск")]
    public float searchRadius = 10f;     // Радиус поиска вокруг последней известной позиции
    public float searchDuration = 10f;   // Как долго искать, прежде чем вернуться к патрулю

    // === ПРИВАТНЫЕ ПЕРЕМЕННЫЕ ===

    // Состояния ИИ
    private enum AIState
    {
        Patrol,       // Патрулирование
        Chase,        // Погоня
        Attack,       // Атака
        Search,       // Поиск игрока
        Investigate,  // Проверка подозрительного места (например, свет фонаря)
        Flee          // Бегство (во время перезарядки)
    }
    private AIState currentState;

    // Компоненты и ссылки
    private NavMeshAgent agent;
    private Transform player;
    private StarterAssets.EditedPersonController playerController; // Используем оригинальный контроллер для проверки приседа

    // Логика боя
    private bool isReloading = false;
    private float nextFireTime = 0f;
    private int bulletsLeft = 8;
    private int magazineSize = 8;

    // Логика состояний
    private Vector3 lastKnownPosition;   // Последняя позиция, где был замечен игрок
    private float stateTimer;            // Таймер для состояний (поиск, ожидание)
    private int patrolIndex = -1;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Поиск игрока и его компонентов
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerController = playerObj.GetComponent<StarterAssets.EditedPersonController>();
        }
        else
        {
            Debug.LogError("Игрок с тегом 'Player' не найден! ИИ не будет работать.");
            this.enabled = false; // Отключаем скрипт, если нет игрока
            return;
        }

        // Автопоиск фонарика, если не указан
        if (flashlight == null)
        {
            var foundLight = FindObjectOfType<Light>(); // Упрощенный поиск, лучше указать вручную
            if (foundLight != null && foundLight.type == LightType.Spot)
            {
                flashlight = foundLight;
                Debug.Log("Фонарик игрока найден автоматически.");
            }
        }

        // Начальное состояние
        currentState = AIState.Patrol;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        if (player == null) return; // Проверка на случай, если игрок был уничтожен

        // Главный переключатель состояний. Вся логика разделена по функциям.
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

    // --- ЛОГИКА СОСТОЯНИЙ ---

    private void UpdatePatrolState()
    {
        // Устанавливаем скорость для патрулирования
        agent.speed = wanderSpeed;
        if (walkAudio != null && !walkAudio.isPlaying && agent.velocity.magnitude > 0.1f)
        {
            walkAudio.Play();
        }

        // Проверяем, не виден ли игрок
        if (IsPlayerVisible())
        {
            TransitionToState(AIState.Chase);
            return;
        }

        // Проверяем, не виден ли свет фонарика
        if (IsFlashlightVisible())
        {
            TransitionToState(AIState.Investigate);
            return;
        }

        // Если дошли до точки патруля, ждем и идем к следующей
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

        // Всегда обновляем путь к последней известной позиции игрока
        agent.SetDestination(lastKnownPosition);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Если игрок в зоне атаки и мы его видим, атакуем
        if (IsPlayerVisible() && distanceToPlayer <= attackRange)
        {
            TransitionToState(AIState.Attack);
        }
        // Если потеряли игрока из виду, начинаем поиск
        else if (!IsPlayerVisible())
        {
            TransitionToState(AIState.Search);
        }
    }

    private void UpdateAttackState()
    {
        agent.isStopped = true; // Останавливаем движение
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

        // Если игрок убежал из зоны атаки, но мы его еще видим - в погоню
        if (Vector3.Distance(transform.position, player.position) > attackRange && IsPlayerVisible())
        {
            TransitionToState(AIState.Chase);
            return;
        }

        // Если потеряли игрока (спрятался за углом) - ищем
        if (!IsPlayerVisible())
        {
            TransitionToState(AIState.Search);
            return;
        }

        // Логика стрельбы
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

        // Если время поиска вышло, возвращаемся к патрулированию
        if (stateTimer <= 0)
        {
            TransitionToState(AIState.Patrol);
            return;
        }

        // Если во время поиска снова увидели игрока - в погоню
        if (IsPlayerVisible())
        {
            TransitionToState(AIState.Chase);
            return;
        }

        // Если дошли до точки поиска, ищем новую
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            SearchForPlayer();
        }
    }

    private void UpdateInvestigateState()
    {
        agent.speed = wanderSpeed;
        agent.SetDestination(lastKnownPosition); // Идем к месту, где был замечен свет

        // Если увидели игрока по пути - в погоню
        if (IsPlayerVisible())
        {
            TransitionToState(AIState.Chase);
            return;
        }

        // Если дошли до места, а там никого, начинаем поиск
        if (!agent.pathPending && agent.remainingDistance < 1.0f)
        {
            TransitionToState(AIState.Search);
        }
    }

    private void UpdateFleeState()
    {
        // Логика бегства реализована внутри корутины ReloadCoroutine
        // Это состояние активно, пока isReloading = true
    }


    // --- ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ ---

    private void TransitionToState(AIState newState)
    {
        // Выход из предыдущего состояния
        OnStateExit(currentState);

        currentState = newState;

        // Вход в новое состояние
        OnStateEnter(newState);
    }

    // Что делать при ВХОДЕ в состояние
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
                lastKnownPosition = player.position; // Запоминаем точное место, где потеряли
                stateTimer = searchDuration;
                SearchForPlayer(); // Начинаем искать сразу
                break;
            case AIState.Investigate:
                agent.isStopped = false;
                // lastKnownPosition уже установлен в IsFlashlightVisible()
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

    // Что делать при ВЫХОДЕ из состояния
    private void OnStateExit(AIState state)
    {
        // Общие действия при выходе из любого состояния, если нужно
        // Например, убедиться, что агент снова может двигаться, если был в атаке
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
        stateTimer = wanderPauseTime; // Устанавливаем таймер ожидания на точке
    }

    private void SearchForPlayer()
    {
        Vector3 randomPoint = lastKnownPosition + Random.insideUnitSphere * searchRadius;
        NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, searchRadius, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
    }

    // --- ЛОГИКА ОБНАРУЖЕНИЯ ---

    private bool IsPlayerVisible()
    {
        // Проверка на очень близкое расстояние (обнаружение сзади)
        float dist = Vector3.Distance(transform.position, player.position);
        bool isCrouching = playerController.Grounded && Input.GetKey(KeyCode.LeftControl); // Пример проверки приседа

        if (!isCrouching && dist <= behindDetectionRadius)
        {
            // Проверяем прямую видимость, даже если близко
            if (HasLineOfSight())
            {
                lastKnownPosition = player.position;
                return true;
            }
        }

        // Проверка в поле зрения
        Vector3 dirToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);

        // Определяем текущую дальность видимости в зависимости от состояния игрока
        float currentSightRange = isCrouching ? sightRange * crouchSightMultiplier : sightRange;

        // Проверка в основном поле зрения (узком и дальнем)
        if (angleToPlayer < fieldOfViewAngle * 0.5f)
        {
            if (dist <= currentSightRange && HasLineOfSight())
            {
                lastKnownPosition = player.position;
                return true;
            }
        }

        // Проверка в периферийном поле зрения (широком, но близком)
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

        // Фонарик можно заметить в более широком угле
        if (Vector3.Angle(transform.forward, dirToFlashlight) < fieldOfViewAngle)
        {
            // Проверяем, нет ли стены между ИИ и источником света
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
        // Пускаем луч из "глаз" ИИ в "тело" игрока
        Vector3 rayStart = transform.position + Vector3.up * 1.5f; // Высота глаз ИИ
        Vector3 playerTarget = player.position + Vector3.up; // Центр тела игрока

        if (Physics.Linecast(rayStart, playerTarget, out RaycastHit hit))
        {
            // Если луч попал в игрока - значит, он в прямой видимости
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }


    // --- ЛОГИКА БОЯ ---

    void Shoot()
    {
        if (player == null) return;

        // Создаем пулю
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // Направляем ее в игрока
        Vector3 shootDir = (player.position + Vector3.up * 1.0f - firePoint.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(shootDir);

        // Придаем скорость
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = shootDir * bulletSpeed;

        if (shootAudio != null) shootAudio.Play();

        bulletsLeft--;
        nextFireTime = Time.time + 1f / fireRate;
    }

    IEnumerator ReloadCoroutine()
    {
        if (isReloading) yield break; // Если уже перезаряжаемся, выходим

        isReloading = true;

        // Переходим в состояние бегства
        TransitionToState(AIState.Flee);

        if (reloadAudio != null) reloadAudio.Play();

        yield return new WaitForSeconds(3.0f); // Время перезарядки

        bulletsLeft = magazineSize;
        isReloading = false;

        // После перезарядки не возвращаемся в бой сразу, а начинаем искать игрока
        TransitionToState(AIState.Search);
    }

    // --- ГИЗМО ДЛЯ ОТЛАДКИ В РЕДАКТОРЕ ---

    void OnDrawGizmosSelected()
    {
        // Дальность атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Основное поле зрения
        Gizmos.color = Color.yellow;
        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfViewAngle * 0.5f, transform.up) * transform.forward * sightRange;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfViewAngle * 0.5f, transform.up) * transform.forward * sightRange;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        // Периферийное поле зрения
        Gizmos.color = new Color(1, 1, 0, 0.2f); // Прозрачный желтый
        Vector3 periLine1 = Quaternion.AngleAxis(peripheralViewAngle * 0.5f, transform.up) * transform.forward * (sightRange * peripheralSightMultiplier);
        Vector3 periLine2 = Quaternion.AngleAxis(-peripheralViewAngle * 0.5f, transform.up) * transform.forward * (sightRange * peripheralSightMultiplier);
        Gizmos.DrawRay(transform.position, periLine1);
        Gizmos.DrawRay(transform.position, periLine2);

        // Обнаружение сзади
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, behindDetectionRadius);

        // Дальность обнаружения фонарика
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, flashlightDetectionRange);

        // Радиус поиска
        if (currentState == AIState.Search || currentState == AIState.Investigate)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(lastKnownPosition, searchRadius);
        }
    }
}