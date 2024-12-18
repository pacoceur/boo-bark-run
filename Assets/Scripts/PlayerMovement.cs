using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform GFX;
    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform feetPos;
    [SerializeField] private float groundDistance = 0.25f;
    [SerializeField] private float jumpTime = 0.3f;
    [SerializeField] private float maxCrouchTime = 2f;
    [SerializeField] private float barkRadius = 5f;
    [SerializeField] private AudioSource barkSound;

    private float colliderSize;
    private float colliderOffset;
    private float crouchedColliderSize;
    private float crouchedColliderOffset;
    private bool isGrounded = false;
    private bool isJumping = false;
    private float jumpTimer = 0f;
    private float crouchTimer = 0f;

    private PlayerControls inputActions;

    private void Awake()
    {
        inputActions = new PlayerControls();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Jump.started += OnJumpStarted;
        inputActions.Player.Jump.canceled += OnJumpCanceled;
        inputActions.Player.Crouch.started += OnCrouchStarted;
        //inputActions.Player.Crouch.canceled += OnCrouchCanceled;
        inputActions.Player.Bark.started += OnBark;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.started -= OnJumpStarted;
        inputActions.Player.Jump.canceled -= OnJumpCanceled;
        inputActions.Player.Crouch.started -= OnCrouchStarted;
        //inputActions.Player.Crouch.canceled -= OnCrouchCanceled;
        inputActions.Player.Bark.started -= OnBark;

        inputActions.Player.Disable();
    }

    private void Start()
    {
        colliderSize = boxCollider.size.y;
        colliderOffset = boxCollider.offset.y;
        crouchedColliderSize = colliderSize / 2;
        crouchedColliderOffset = colliderOffset * 1.4f;
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, groundDistance, groundLayer);

        if (isJumping && jumpTimer < jumpTime)
        {
            rb.velocity = Vector2.up * jumpForce;
            jumpTimer += Time.deltaTime;
        }
        else
        {
            isJumping = false;
        }

        if (animator.GetBool("Crouch"))
        {
            crouchTimer += Time.deltaTime;
            if (crouchTimer >= maxCrouchTime)
            {
                Crouch(false);
            }
        }

        animator.SetBool("Jump", !isGrounded);
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    private void OnBark(InputAction.CallbackContext context)
    {
        animator.SetTrigger("Bark");
        barkSound.Play();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, barkRadius);

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                Destroy(collider.transform.parent.gameObject);
            }
        }
    }

    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        if (animator.GetBool("Crouch"))
        {
            Crouch(false);
        }

        if (isGrounded)
        {
            isJumping = true;
            rb.velocity = Vector2.up * jumpForce;
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        isJumping = false;
        jumpTimer = 0;
    }

    private void OnCrouchStarted(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            crouchTimer = 0f;
            Crouch(true);
        }
    }

    private void OnCrouchCanceled(InputAction.CallbackContext context)
    {
        Crouch(false);
        crouchTimer = 0f;
    }

    private void Crouch(bool active)
    {
        if (active)
        {
            boxCollider.size = new Vector2(boxCollider.size.x, crouchedColliderSize);
            boxCollider.offset = new Vector2(boxCollider.offset.x, crouchedColliderOffset);
            animator.SetBool("Crouch", true);
        }
        else
        {
            boxCollider.size = new Vector2(boxCollider.size.x, colliderSize);
            boxCollider.offset = new Vector2(boxCollider.offset.x, colliderOffset);
            animator.SetBool("Crouch", false);
            crouchTimer = 0f;
        }
    }
}