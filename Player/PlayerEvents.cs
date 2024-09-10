using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
public class PlayerEvents : MonoBehaviour
{
    [Header("Wind Event")]
    [SerializeField] private float windEventDuration = 3.0f;
    [SerializeField] private float windEventMultiplierForSpeed = 1.5f;
    [Header("Trick Event")]
    [SerializeField] private float trickEventMultiplierForSpeed = 1.5f;
    [SerializeField] private float trickEventDurationForAcceleration = 3.0f;
    [SerializeField] private float trickEventDurationForInvulnerability = 4.5f;
    [Header("Start")]
    [SerializeField] private float startAccelerationDuration = 5.0f;

    [Header("Obstacle")]
    [SerializeField] private float maxAngleForObstacleDestruction = 70.0f;
    [SerializeField] private float jumpForceFromObstacle = 350.0f;

    [Header("Goal")]
    [SerializeField] private float minMetersToNextGoalFromStartOfNewLocation = 300.0f;
    [SerializeField] private float maxMetersToNextGoalFromStartOfNewLocation = 500.0f;

    [Space(15)]
    [Header("Skins")]
    [SerializeField] private GameObject desertSkin;
    [SerializeField] private GameObject oceanSkin;
    [SerializeField] private GameObject snowMountainsSkin;

    public float WindMultiplierForSpeed { get; private set; }

    public float TrickMultiplierForSpeed { get; private set; }
    public bool IsWindEvent { get; private set; } = false;
    public bool IsTrickEvent { get; private set; } = false;
    public bool IsPlayerDead { get; private set; } = false;
    public bool IsInvulnerable { get; private set; } = false;

    [HideInInspector]
    public bool DestroyedObstacle = false;

    private Rigidbody2D rigidBody2D;
    private PlayerEffects playerEffects;
    private Animator playerMainAnimator;

    private LayerMask whatIsGround;
    private Transform groundCheckCircleCenter;
    private Transform destructionCheckPoint;

    private Vector3 startPosition;
    private Vector3 startSlidingPosition;
    private Vector3 lastGoalPosition;
    private float goalMetersForNewLocation;
    private float goalMetersForNewEvent;
    private bool canActivateBackgroundEvent = false;

    private MapGenerationManager.LocationState currentLocationState;

    private void OnEnable()
    {
        MapGenerationManager.onPlayerEquipmentChanged += ChangeEquipment;
        PlayerDeathZone.onPlayerDied += Death;
    }
    private void OnDisable()
    {
        MapGenerationManager.onPlayerEquipmentChanged -= ChangeEquipment;
        PlayerDeathZone.onPlayerDied -= Death;
    }
    public void Initialize(LayerMask ground, Transform groundCheckCircleCenter, Transform destructionCheckPoint)
    {
        whatIsGround = ground;
        this.groundCheckCircleCenter = groundCheckCircleCenter;
        this.destructionCheckPoint = destructionCheckPoint;
    }
    private void Awake()
    {
        rigidBody2D = GetComponentInChildren<Rigidbody2D>();
        playerEffects = GetComponentInChildren<PlayerEffects>();
        if (playerEffects == null)
        {
            Debug.LogError("Player effects switcher not found");
        }
        WindMultiplierForSpeed = windEventMultiplierForSpeed;
        TrickMultiplierForSpeed = trickEventMultiplierForSpeed;
    } 
    private void Start()
    {
        startPosition = transform.position;
        lastGoalPosition = startPosition;
        goalMetersForNewLocation = Random.Range(minMetersToNextGoalFromStartOfNewLocation,
         maxMetersToNextGoalFromStartOfNewLocation);
        Debug.Log("Next goal is: " + goalMetersForNewLocation);
        canActivateBackgroundEvent = true;
        goalMetersForNewEvent = BackgroundEventManager.Singleton.GetGoalMetersForNewEvent(goalMetersForNewLocation);
        Debug.Log("Next event will appear at: " + ((startPosition - transform.position).magnitude + goalMetersForNewEvent));
        ActivateAccelerationEvent(startAccelerationDuration);
        AudioManager.Singleton.Play("Start_ride");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Obstacle obstacle = collision.GetComponent<Obstacle>();
            if (!IsInvulnerable && !IsPlayerDead)
            {
                float angle = Vector2.Angle(destructionCheckPoint.position - obstacle.pointForDestruction.position, obstacle.gameObject.transform.up);
                Debug.DrawLine(destructionCheckPoint.position, obstacle.pointForDestruction.position, Color.green, 3.0f);
                Debug.DrawLine(obstacle.pointForDestruction.position, new Vector3(obstacle.pointForDestruction.position.x, obstacle.pointForDestruction.position.y + 1.0f, obstacle.pointForDestruction.position.z), Color.green, 3.0f);
                Debug.Log($"Collided with obstacle at angle of {angle} degrees");
                Vector3 isUpsideDownVector = groundCheckCircleCenter.position - destructionCheckPoint.position;
                isUpsideDownVector.Normalize();
                if (isUpsideDownVector.y > 0.45f && angle <= maxAngleForObstacleDestruction)
                {
                    obstacle.PlayDestroyAnimation();
                    GameData.localObstaclesDestroyed++;
                    Vector2 newJumpDirection = new Vector2(MathF.Cos(90.0f - Mathf.Deg2Rad * angle), Mathf.Sin(90.0f - Mathf.Deg2Rad * angle));
                    Debug.DrawLine(transform.position, newJumpDirection, Color.cyan, 3.0f);
                    rigidBody2D.AddForce(newJumpDirection * jumpForceFromObstacle);
                    DestroyedObstacle = true;
                    Debug.Log("The player is in the process of a trick");
                }
                else
                {
                    PlayerDeathZone.onPlayerDied?.Invoke(); 
                }
            }
            else
            {
                obstacle.PlayDestroyAnimation();
            }
        }

        if (collision.gameObject.tag == "Wind")
        {
            StartCoroutine(WindEvent());
        }
    }
    private void Update()
    {
        CheckGoalMeters();
    }

    public void StartSliding()
    {
        startSlidingPosition = transform.position;
    }
    public void EndSliding()
    {
        GameData.localSlideDistance+= (uint)(startSlidingPosition - transform.position).magnitude;
    }
    public void ActivateTrickEvent()
    {
        GameData.localTricksCount++;
        playerEffects.PlayAccelerationEffect(trickEventDurationForAcceleration);
        playerEffects.PlayInvulnerabilityEffect(trickEventDurationForInvulnerability);
        StartCoroutine(TrickEventForAcceleration(trickEventDurationForAcceleration));
        StartCoroutine(TrickEventForInvulnerability(trickEventDurationForInvulnerability));
    }
    public void ActivateAccelerationEvent(float duration)
    {
        playerEffects.PlayAccelerationEffect(duration);
        StartCoroutine(TrickEventForAcceleration(duration));
    }

    public void ActivateInvulnerabilityEvent(float duration)
    {
        playerEffects.PlayInvulnerabilityEffect(duration);
        StartCoroutine(TrickEventForInvulnerability(duration));
    }
    private void CheckGoalMeters()
    {
        GameData.localDistance = (uint)(startPosition - transform.position).magnitude;
        if (goalMetersForNewLocation < (lastGoalPosition - transform.position).magnitude)
        {
            Vector3 newLocationStart = MapGenerationManager.Singleton.SpawnNewLocation();
            lastGoalPosition = transform.position;
            goalMetersForNewLocation = Random.Range(minMetersToNextGoalFromStartOfNewLocation, maxMetersToNextGoalFromStartOfNewLocation)
                + (newLocationStart - transform.position).magnitude;
            Debug.Log("Next goal is: " + ((startPosition - transform.position).magnitude + goalMetersForNewLocation));
            canActivateBackgroundEvent = true;
            goalMetersForNewEvent = BackgroundEventManager.Singleton.GetGoalMetersForNewEvent(goalMetersForNewLocation);
            Debug.Log("Next event will appear at: " + ((startPosition - transform.position).magnitude + goalMetersForNewEvent));
        }
        if (canActivateBackgroundEvent && goalMetersForNewEvent < (lastGoalPosition - transform.position).magnitude)
        {
            canActivateBackgroundEvent = false;
            BackgroundEventManager.Singleton.ActivateRandomEvent(currentLocationState);
        }
    }
    private void ChangeEquipment(MapGenerationManager.LocationState locationState)
    {
        currentLocationState = locationState;
        switch (currentLocationState)
        {
            case MapGenerationManager.LocationState.Desert:
                desertSkin.SetActive(true);
                oceanSkin.SetActive(false);
                snowMountainsSkin.SetActive(false);
                playerMainAnimator = desertSkin.GetComponent<Animator>();
                break;
            case MapGenerationManager.LocationState.Ocean:
                desertSkin.SetActive(false);
                oceanSkin.SetActive(true);
                snowMountainsSkin.SetActive(false);
                playerMainAnimator = oceanSkin.GetComponent<Animator>();
                break;
            case MapGenerationManager.LocationState.SnowMountains:
                desertSkin.SetActive(false);
                oceanSkin.SetActive(false);
                snowMountainsSkin.SetActive(true);
                playerMainAnimator = snowMountainsSkin.GetComponent<Animator>();
                break;
        }
        GetComponent<PlayerMovement>().SetPlayerMainAnimator(playerMainAnimator);
    }
    private IEnumerator WindEvent()
    {
        IsWindEvent = true;
        Debug.Log("Wind event is started");
        yield return new WaitForSeconds(windEventDuration);
        IsWindEvent = false;
        Debug.Log("Wind event is ended");
    }
    private IEnumerator TrickEventForAcceleration(float duration)
    {
        IsTrickEvent = true;
        Debug.Log("Trick event for speed is started");
        yield return new WaitForSeconds(duration);
        IsTrickEvent = false;
        Debug.Log("Trick event for speed is ended");
    }
    private IEnumerator TrickEventForInvulnerability(float duration)
    {
        IsInvulnerable = true;
        Debug.Log("Trick event for invulnerability is started");
        yield return new WaitForSeconds(duration);
        IsInvulnerable = false;
        Debug.Log("Trick event for invulnerability is ended");
    }
    private void Death()
    {
        if (!IsPlayerDead)
        {
            IsPlayerDead = true;
            AudioManager.Singleton.StopAll();
            AudioManager.Singleton.Play("Death");
            PlayerInputManager.Singleton.DisableInput();
            rigidBody2D.simulated = false;
            playerEffects.DisasbleAllEffects();

            Vector3 direction = new Vector3(groundCheckCircleCenter.position.x - destructionCheckPoint.position.x,
                groundCheckCircleCenter.position.y - destructionCheckPoint.position.y).normalized;
            RaycastHit2D hit = Physics2D.Raycast(groundCheckCircleCenter.position, direction, 5.0f, whatIsGround);
            Debug.DrawLine(groundCheckCircleCenter.position, groundCheckCircleCenter.position + direction * 5.0f, Color.cyan, 30.0f);
            Debug.DrawLine(hit.point, hit.point + hit.normal * 5.0f, Color.green, 30.0f);

            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if ((hit.point + hit.normal * 5.0f).x > (hit.point + Vector2.up).x)
            {
                transform.eulerAngles = new Vector3(0.0f, 0.0f, -angle);
            }
            else
            {
                transform.eulerAngles = new Vector3(0.0f, 0.0f, angle);
            }
    
            playerMainAnimator.SetTrigger("Crashed");
            if (MapGenerationManager.Singleton.CurrentLocationState == MapGenerationManager.LocationState.Ocean)
            {
                AudioManager.Singleton.Play("Waterdrop");
            }
            else if (MapGenerationManager.Singleton.CurrentLocationState == MapGenerationManager.LocationState.SnowMountains)
            {
                int num = Random.Range(1, 4);
                switch (num)
                {
                    case 1:
                        AudioManager.Singleton.Play("Snow_crackle_1");
                        break;
                    case 2:
                        AudioManager.Singleton.Play("Snow_crackle_2");
                        break;
                    case 3:
                        AudioManager.Singleton.Play("Snow_crackle_3");
                        break;
                    default:
                        break;
                }
            }

            StartCoroutine(DeathCoroutine());
        }
    }
    private IEnumerator DeathCoroutine()
    {
        rigidBody2D.velocity = Vector2.zero;
        GetComponentInChildren<CapsuleCollider2D>().offset += new Vector2(1.45f, 0.0f);
        rigidBody2D.simulated = true;

        yield return new WaitForSeconds(1.0f);
        GameObject[] boards = GameObject.FindGameObjectsWithTag("Board");
        foreach (var board in boards)
        {
            board.SetActive(false);
        }
        rigidBody2D.simulated = false; 
    }
}
