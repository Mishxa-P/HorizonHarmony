using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Slide")]
    [SerializeField] private float initialSpeed = 2.0f;
    [SerializeField] private float maxMultipierForSpeed = 2.5f;
    [Range(0.0f, 90.0f)]
    [SerializeField] private float maxDownAngle = 75.0f;
    [SerializeField] private float accelerationForMaxDownAngle = 0.5f;
    [Range(0.0f, 45.0f)]
    [SerializeField] private float maxUpAngle = 30.0f;
    [SerializeField] private float slowDownForMaxUpAngle = 0.5f;
    [SerializeField] private float minSlowDown = 0.1f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 250.0f;

    [Header("Fall")]
    [SerializeField] private float maxFallingSpeed = 35.0f;
    [SerializeField] private float accelerationForFalling = 0.02f;

    [Header("Rotation")]
    [SerializeField] private float rotationRate = 2.5f;
    private float currentAngle = 0.0f;

    [Space(15)]
    [Header("Ground")]
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheckCircleCenter;
    [SerializeField] private float groundCheckCircleRadius = 0.5f;
    [SerializeField] private Transform destructionCheckPoint;

    [Space(15)]
    [Header("Development")]
    [SerializeField] private bool showGroundCheckCircle = false;
    [SerializeField] private float acceleration = 0.0f;

    public float InitialSpeed { get; private set; }
    public float CurrentSpeed { get; private set; }
    public float MaxSpeedWithoutEvents { get; private set; }

    private Rigidbody2D rigidBody2D;
    private PlayerEvents playerEvents;
    private PlayerEffects playerEffects;
    private Animator playerMainAnimator;

    private float rotateInput = 0.0f;
    private bool isGrounded = false;
    private bool canCheckIfIsGrounded = true;
    private bool isSliding = false;
    private bool isTricking = false;
    private bool isUpsideDown = false;
   
    private Vector2 direction;
    private Vector2 jumpDirection;

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
        MaxSpeedWithoutEvents = initialSpeed * maxMultipierForSpeed;
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
    }
    private void TouchRotateEnd(InputAction.CallbackContext context)
    {
       rotateInput = 0.0f;
    }
    private void CheckIsUpsideDown()
    {
        if (groundCheckCircleCenter.position.y <= destructionCheckPoint.position.y)
        {
            isUpsideDown = true;
            GetComponentInChildren<CapsuleCollider2D>().enabled = false;
        }
        else
        {
            isUpsideDown = false;
            GetComponentInChildren<CapsuleCollider2D>().enabled = true;
        }
    }
    private void CheckIsGrounded()
    {
        bool wasGrounded = isGrounded;
        bool wasSliding = isSliding;
        isSliding = false;
        isGrounded = false;

        if (!playerEvents.IsPlayerDead && !isUpsideDown && canCheckIfIsGrounded)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCircleCenter.position, groundCheckCircleRadius, whatIsGround);
            HandleCollisions(colliders, ref isGrounded, ref isSliding, ref wasGrounded, ref wasSliding);
        }
        
        if (!isGrounded)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Clamp(rigidBody2D.velocity.y, -maxFallingSpeed, maxFallingSpeed));
            if (rigidBody2D.velocity.y < 0.0f)
            {
                if (acceleration < MaxSpeedWithoutEvents - initialSpeed)
                {
                    acceleration += accelerationForFalling;
                }
                else
                {
                    acceleration = MaxSpeedWithoutEvents - initialSpeed;
                }
                CurrentSpeed = acceleration + initialSpeed;
            }
           
            if (wasSliding)
            {
                playerMainAnimator.SetBool("IsSliding", false);
                playerEvents.EndSliding();
                Debug.Log("Stoped sliding");
                isTricking = true;
                Debug.Log("The player is in the process of a trick");
            }

            if (wasGrounded)
            {
                playerEffects.DisableLandingEffect();
            }

            playerMainAnimator.SetBool("IsFalling", true);
        }
    }   
    private void HandleCollisions(Collider2D[] colliders, ref bool isGrounded, ref bool isSliding, ref bool wasGrounded, ref bool wasSliding)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Slide") && rigidBody2D.velocity.y >= jumpForce * 0.1f)
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
                    if (acceleration > MaxSpeedWithoutEvents - initialSpeed)
                    {
                        acceleration = MaxSpeedWithoutEvents - initialSpeed;
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
        if(isGrounded)
        {
            Vector3 offset = transform.rotation * Vector2.up;
            RaycastHit2D hit = Physics2D.Raycast(groundCheckCircleCenter.position, -offset.normalized, groundCheckCircleRadius * 1.5f, whatIsGround);
            Debug.DrawLine(groundCheckCircleCenter.position, groundCheckCircleCenter.position - offset.normalized * groundCheckCircleRadius * 1.5f, Color.cyan);
            if (hit)
            {
                Debug.DrawLine(hit.point, hit.point + hit.normal, Color.red, 3.0f);
                float angle = Vector2.Angle(Vector2.up, hit.normal);
                jumpDirection = hit.normal;
                if (Vector2.Angle(Vector2.right, hit.normal) <= 85.0f)
                {
                    direction = new Vector2(MathF.Cos(Mathf.Deg2Rad * angle), -Mathf.Sin(Mathf.Deg2Rad * angle));
                    if (acceleration + initialSpeed < MaxSpeedWithoutEvents)
                    {
                        acceleration += (angle / maxDownAngle) * accelerationForMaxDownAngle;
                    }
                    else
                    {
                        acceleration = MaxSpeedWithoutEvents - initialSpeed;
                    }
                }
                else if (Vector2.Angle(Vector2.right, hit.normal) > 85.0f && Vector2.Angle(Vector2.right, hit.normal) <= 90.0f)
                {
                    direction = new Vector2(MathF.Cos(Mathf.Deg2Rad * angle), -Mathf.Sin(Mathf.Deg2Rad * angle));
                    if (acceleration > 0.0f)
                    {
                        acceleration -= minSlowDown;
                    }
                    else
                    {
                        acceleration = 0.0f;
                    }
                }
                else
                {
                    jumpDirection = Vector2.up;
                    direction = new Vector2(MathF.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle));
                    if (acceleration > 0.0f)
                    {
                        acceleration -= (angle / maxUpAngle) * slowDownForMaxUpAngle;
                    }
                    else
                    {
                        acceleration = 0.0f;
                    }
                }

                CurrentSpeed = acceleration + initialSpeed;

                if (playerEvents.IsWindEvent)
                {
                    CurrentSpeed *= playerEvents.WindMultiplierForSpeed;
                }

                if (playerEvents.IsTrickEvent)
                {
                    CurrentSpeed *= playerEvents.TrickMultiplierForSpeed;
                }

                if (CurrentSpeed < InitialSpeed)
                {
                    CurrentSpeed = initialSpeed;
                }

                rigidBody2D.velocity = direction * CurrentSpeed;

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
        if (!isGrounded && !playerEvents.IsPlayerDead)
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
            if (rotateInput != 0.0f)
            {
                Vector3 vector = groundCheckCircleCenter.position - destructionCheckPoint.position;
                vector.Normalize();
                Debug.Log(vector.y);
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
