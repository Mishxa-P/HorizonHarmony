using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float initialSpeed = 8.0f;
    [SerializeField] private float maxGroundAcceleration = 14.0f;
    [SerializeField] private float maxFallingAcceleration = 8.0f;
    [SerializeField] private float maxFallingSpeed = 35.0f;

    [Header("AccelerationOnGround")]
    [Range(0.0f, 90.0f)]
    [SerializeField] private float maxDownAngle = 75.0f;
    [SerializeField] private float groundAccelerationIncreaseForMaxDownAngle = 0.08f;
    [Range(0.0f, 45.0f)]
    [SerializeField] private float maxUpAngle = 45.0f;
    [SerializeField] private float minGroundAccelerationFade = 0.04f;
    [SerializeField] private float groundAccelerationFadeForMaxUpAngle = 0.1f;
    [SerializeField] private float groundAccelerationFadeInFall = 0.01f;

    [Header("AccelerationInFall")]
    [SerializeField] private float fallingAccelerationIncrease = 0.04f;
    [SerializeField] private float fallingAccelerationFadeOnGround = 0.03f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 400.0f;

    [Header("Rotation")]
    [SerializeField] private float rotationRate = 2.65f;

    [Space(15)]
    [Header("Ground")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheckCircleCenter;
    [SerializeField] private float groundCheckCircleRadius = 0.11f;
    [SerializeField] private Transform destructionCheckPoint;

    [Space(15)]
    [Header("Development")]
    [SerializeField] private bool showGroundCheckCircle = false;
    [SerializeField] private float groundAcceleration = 0.0f;
    [SerializeField] private float fallingAcceleration = 0.0f;

    public float InitialSpeed { get; private set; }
    public float CurrentSpeed { get; private set; }
    public float MaxSpeedForCameraAdjustments { get; private set; }

    private Rigidbody2D rigidBody2D;
    private PlayerEvents playerEvents;
    private PlayerEffects playerEffects;
    private Animator playerMainAnimator;

    private bool rotate = false;
    private float currentAngle = 0.0f;
    private bool isGrounded = false;
    private bool canCheckIfIsGrounded = true;
    private bool isSliding = false;
    private bool isTricking = false;
    private bool isUpsideDown = false;
   
    private Vector2 direction;
    private Vector2 jumpDirection;
    private bool wasGrounded = false;
    private bool wasSliding = false;

    private float screenWidth;

    private void OnEnable()
    {
        PlayerInputManager.Singleton.InputActions.Player.Jump.performed += Jump;
        PlayerInputManager.Singleton.InputActions.Player.RotateByTouchscreen.performed += TouchRotateStart;
        PlayerInputManager.Singleton.InputActions.Player.RotateByTouchscreen.canceled += TouchRotateEnd;
    }

    private void OnDisable()
    {
        PlayerInputManager.Singleton.InputActions.Player.Jump.performed -= Jump;
        PlayerInputManager.Singleton.InputActions.Player.RotateByTouchscreen.performed -= TouchRotateStart;
        PlayerInputManager.Singleton.InputActions.Player.RotateByTouchscreen.canceled -= TouchRotateEnd;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") || collision.gameObject.layer == LayerMask.NameToLayer("Slide") && !playerEvents.IsPlayerDead)
        {
            Vector3 offset = transform.rotation * Vector2.up;
            RaycastHit2D hit = Physics2D.Raycast(groundCheckCircleCenter.position, -offset.normalized, groundCheckCircleRadius * 6.0f, whatIsGround);
            Debug.DrawLine(groundCheckCircleCenter.position, groundCheckCircleCenter.position - offset.normalized * groundCheckCircleRadius * 6.0f, Color.cyan);
            if (hit)
            {
                //Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red, 3.0f);
                if (!wasGrounded)
                {
                    rigidBody2D.AddForce(-hit.normal * jumpForce);
                }
            }
        }
    }
    private void Awake()
    {
        rigidBody2D = GetComponentInChildren<Rigidbody2D>();
        playerEffects = GetComponentInChildren<PlayerEffects>();
        playerEvents = GetComponent<PlayerEvents>();
        if (playerEffects == null)
        {
            Debug.LogError("Player effects switcher not found");
        }
        if (playerEvents == null)
        {
            Debug.LogError("Player events switcher not found");
        }
        playerEvents.Initialize(whatIsGround, groundCheckCircleCenter, destructionCheckPoint);
    }

    private void Start()
    {
        screenWidth = Screen.width;
        MaxSpeedForCameraAdjustments = initialSpeed + maxGroundAcceleration + maxFallingAcceleration * 0.25f;
        InitialSpeed = initialSpeed;
        CurrentSpeed = initialSpeed;
    }
    private void FixedUpdate()
    {
        Rotate();
        CheckIsUpsideDown();
        CheckIsGrounded();
        SlopeCheck();
    }
    public void SetPlayerMainAnimator(Animator animator)
    {
        playerMainAnimator = animator;
    }
    private void TouchRotateStart(InputAction.CallbackContext context)
    {
        PointerEventData eventData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();

        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            return;
        }
        rotate = true;
    }
    private void TouchRotateEnd(InputAction.CallbackContext context)
    {
        rotate = false;
    }
    private void CheckIsUpsideDown()
    {
        bool wasUpsideDown = isUpsideDown;
        isUpsideDown = false;
        if (groundCheckCircleCenter.position.y <= destructionCheckPoint.position.y)
        {
            isUpsideDown = true;
            if (!wasUpsideDown)
            {
                int slideLayer = LayerMask.NameToLayer("Slide");
                GetComponentInChildren<CapsuleCollider2D>().forceReceiveLayers &= ~(1 << slideLayer);
            }
        }
        else
        {
            isUpsideDown = false;
            if (wasUpsideDown)
            {
                int slideLayer = LayerMask.NameToLayer("Slide");
                GetComponentInChildren<CapsuleCollider2D>().forceReceiveLayers |= (1 << slideLayer);
            }
        }
    }
    private void CheckIsGrounded()
    {
        wasGrounded = isGrounded;
        wasSliding = isSliding;
        isSliding = false;
        isGrounded = false;

        if (!playerEvents.IsPlayerDead && !isUpsideDown && canCheckIfIsGrounded)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCircleCenter.position, groundCheckCircleRadius, whatIsGround);
            HandleCollisions(colliders, ref isGrounded, ref isSliding, ref wasSliding);
        }

        if (!isGrounded)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Clamp(rigidBody2D.velocity.y, -maxFallingSpeed, maxFallingSpeed));
            if (rigidBody2D.velocity.y < 0.0f)
            {
                if (fallingAcceleration < maxFallingAcceleration)
                {
                    fallingAcceleration += fallingAccelerationIncrease;
                }
                CurrentSpeed = groundAcceleration + fallingAcceleration + initialSpeed;
            }

            if (wasSliding)
            {
                playerMainAnimator.SetBool("IsSliding", false);
                playerEvents.EndSliding();
                AudioManager.Singleton.Stop("Slide");
                Debug.Log("Stoped sliding");
                isTricking = true;
                Debug.Log("The player is in the process of a trick");
            }

            if (wasGrounded)
            {
                playerEffects.DisableLandingEffect();
            }

            if (groundAcceleration > 0.0f)
            {
                groundAcceleration -= groundAccelerationFadeInFall;
            }
            else
            {
                groundAcceleration = 0.0f;
            }

            playerMainAnimator.SetBool("IsFalling", true);
        }
    }   
    private void HandleCollisions(Collider2D[] colliders, ref bool isGrounded, ref bool isSliding, ref bool wasSliding)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Slide") && rigidBody2D.velocity.y > MaxSpeedForCameraAdjustments)
                {
                    Debug.Log("Player passed slide");
                    break;
                }
                isGrounded = true;
                if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Slide"))
                {
                    isSliding = true;
                    if (!wasSliding)
                    {
                        playerMainAnimator.SetBool("IsSliding", true);
                        playerEvents.StartSliding();
                        AudioManager.Singleton.Play("Slide");
                        Debug.Log("Started sliding");
                    }
                }
                if (!wasGrounded)
                {
                    playerMainAnimator.SetBool("IsFalling", false);
                    playerMainAnimator.SetBool("IsJumping", false);

                    if (!isSliding)
                    {
                        playerEffects.PlayLandingEffect();
                        Debug.Log("Landed");
                    }
                    if (groundAcceleration > maxGroundAcceleration)
                    {
                        groundAcceleration = maxGroundAcceleration;
                    }
                }
                if (isTricking && !isSliding || playerEvents.DestroyedObstacle && !isSliding)
                {
                    Debug.Log("The player did the trick!");
                    isTricking = false;
                    playerEvents.DestroyedObstacle = false;
                    playerEvents.ActivateTrickEvent();    
                }
            }
        }
    }
    private void SlopeCheck()
    {
        if (isGrounded)
        {
            Vector3 offset = transform.rotation * Vector2.up;
            RaycastHit2D hit = Physics2D.Raycast(groundCheckCircleCenter.position, -offset.normalized, groundCheckCircleRadius * 1.5f, whatIsGround);
            //Debug.DrawLine(groundCheckCircleCenter.position, groundCheckCircleCenter.position - offset.normalized * groundCheckCircleRadius * 1.5f, Color.cyan);
            if (hit)
            {
                //Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red, 3.0f);
                if (!wasGrounded)
                {
                    rigidBody2D.AddForce(-hit.normal * 250.0f);
                }
            
                float angle = Vector2.Angle(Vector2.up, hit.normal);
                jumpDirection = hit.normal;
                if (Vector2.Angle(Vector2.right, hit.normal) <= 85.0f)
                {
                    direction = new Vector2(MathF.Cos(Mathf.Deg2Rad * angle), -Mathf.Sin(Mathf.Deg2Rad * angle));
                    if (groundAcceleration < maxGroundAcceleration)
                    {
                        groundAcceleration += (angle / maxDownAngle) * groundAccelerationIncreaseForMaxDownAngle;
                    }
                    else
                    {
                        groundAcceleration = maxGroundAcceleration;
                    }
                }
                else if (Vector2.Angle(Vector2.right, hit.normal) > 85.0f && Vector2.Angle(Vector2.right, hit.normal) <= 90.0f)
                {
                    direction = new Vector2(MathF.Cos(Mathf.Deg2Rad * angle), -Mathf.Sin(Mathf.Deg2Rad * angle));
                    if (groundAcceleration > 0.0f)
                    {
                        groundAcceleration -= minGroundAccelerationFade;
                    }
                    else
                    {
                        groundAcceleration = 0.0f;
                    }
                }
                else
                {
                    jumpDirection = Vector2.up;
                    direction = new Vector2(MathF.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
                    if (groundAcceleration > 0.0f)
                    {
                        groundAcceleration -= (angle / maxUpAngle) * groundAccelerationFadeForMaxUpAngle;
                    }
                    else
                    {
                        groundAcceleration = 0.0f;
                    }
                }

                if (fallingAcceleration > 0.0f)
                {
                    fallingAcceleration -= fallingAccelerationFadeOnGround;
                }

                if (!playerEvents.IsPlayerDead)
                {
                    CurrentSpeed = groundAcceleration + fallingAcceleration + initialSpeed;
                    if (playerEvents.IsWindEvent)
                    {
                        CurrentSpeed *= playerEvents.WindMultiplierForSpeed;
                    }
                    if (playerEvents.IsTrickEvent)
                    {
                        CurrentSpeed *= playerEvents.TrickMultiplierForSpeed;
                    }
                    rigidBody2D.velocity = direction * CurrentSpeed;
                }
                else
                {
                    if (CurrentSpeed > initialSpeed)
                    {
                        CurrentSpeed -= minGroundAccelerationFade;
                    }      
                }              
                playerMainAnimator.SetFloat("Speed", CurrentSpeed);

#if UNITY_EDITOR
                OnNextDrawGizmos += () =>
                {
                    GUI.color = Color.red;
                    GUIStyle headStyle = new GUIStyle();
                    headStyle.fontSize = 40;
                    Handles.Label(transform.position + new Vector3(0.0f, 2.0f, 0.0f), "Angle: " + angle.ToString(), headStyle);
                };
#endif
            }
        }
    }
    private void Jump(InputAction.CallbackContext context)
    {
        PointerEventData eventData = new(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new();

        EventSystem.current.RaycastAll(eventData, results);

        if (results.Count > 0)
        {
            return;
        }

        if (context.performed && isGrounded)
        {
            Debug.Log("Jumped");
			playerMainAnimator.SetBool("IsJumping", true);
            StartCoroutine(CantCheckIfIsGrounded());
            AddJumpForce();
        }
    }
    private void Rotate()
    {
        if (!isGrounded && !playerEvents.IsPlayerDead && rotate)
        {
            float rotateInput = 0.0f;
            if (PlayerInputManager.Singleton.InputActions.Player.TouchPosition.ReadValue<Vector2>().x != 0.0f)
            {
                if (PlayerInputManager.Singleton.InputActions.Player.TouchPosition.ReadValue<Vector2>().x < screenWidth / 2)
                {
                    rotateInput = -1.0f;
                }
                else
                {
                    rotateInput = 1.0f;
                }
            }
            if (rotateInput != 0.0f)
            {
                currentAngle = transform.eulerAngles.z;
                if (GameSettings.inversion)
                {
                    currentAngle += (rotateInput * rotationRate);
                }
                else
                {
                    currentAngle -= (rotateInput * rotationRate);
                }
                rigidBody2D.MoveRotation(currentAngle);
                Vector3 vector = groundCheckCircleCenter.position - destructionCheckPoint.position;
                vector.Normalize();
                if (vector.y <= -0.95f && !isTricking)
                {
                    isTricking = true;
                    Debug.Log("The player is in the process of a trick");
                }
            }
        }
    }
    private IEnumerator CantCheckIfIsGrounded()
    {
        canCheckIfIsGrounded = false;
        yield return new WaitForSeconds(0.5f);
        canCheckIfIsGrounded = true;
    }
    private void AddJumpForce()
    {
        rigidBody2D.freezeRotation = true;
        rigidBody2D.AddForce(jumpDirection * jumpForce);
        rigidBody2D.freezeRotation = false;
    }

    private Action OnNextDrawGizmos;
    private void OnDrawGizmos()
    {
        OnNextDrawGizmos?.Invoke();
        OnNextDrawGizmos = null;
        if (showGroundCheckCircle)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(groundCheckCircleCenter.position, groundCheckCircleRadius);
        }
    }
}
