using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 100;
    [SerializeField] float jumpForce = 400;
    [SerializeField] float jumpSpeedModifier = 0.6f;

    [SerializeField] float jumpCutModifier = 0.1f;
    [SerializeField] float fallModifier = 2;
    [SerializeField] float acceleration = 2f, deceleration = 3f;
    [SerializeField] float velPower = 0.8f;

    [Header("Sounds")]
    [SerializeField] AudioClip death;
    AudioSource deathSource;


    Rigidbody2D rb;
    Collider2D col;
    Animator anim;

    bool isDead;
    bool isJumpPressed;
    Vector2 inputDir;
    float coyoteTimer;
    float coyoteTimerThreshold = 0.1f;

    [Header("Collision")]
    public float collisionRadius = 0.25f;
    public LayerMask groundLayer;

    bool onGround, onWall;
    float groundCheckDistance = 0.05f;
    float wallCheckDistance = 0.02f;

    bool canJump
    {
        get
        {
            return onGround || coyoteTimer > 0;
        }
    }

    RaycastHit2D[] groundHit = new RaycastHit2D[5];
    RaycastHit2D[] wallHit = new RaycastHit2D[5];
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();
        deathSource = gameObject.AddComponent<AudioSource>();
        deathSource.clip = death;
    }

    void Update()
    {

        var x = Input.GetAxis("Horizontal");
        Debug.Log(x);
        if (!onGround)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = coyoteTimerThreshold;
        }
    }
    void FixedUpdate()
    {
        checkCollisions();
        doMove(inputDir);
        doGravity();
        anim.SetBool("isRunning", inputDir.x != 0);
        anim.SetBool("isGround", onGround);
        anim.SetBool("isJumping", rb.velocity.y > 0);
    }

    void checkCollisions()
    {
        ContactFilter2D f = new ContactFilter2D();
        f.layerMask = groundLayer;
        onGround = col.Cast(Vector2.down, f, groundHit, groundCheckDistance) > 0;
        onWall = col.Cast(transform.localScale.x > 0 ? Vector2.right : Vector2.left, f, wallHit, wallCheckDistance) > 0;
    }

    private void doGravity()
    {
        if (!onGround)
        {
            rb.AddForce(Vector2.down * fallModifier, ForceMode2D.Impulse);
        }
        if (!isJumpPressed && rb.velocity.y > 0)
        {
            rb.AddForce(Vector2.down * jumpCutModifier, ForceMode2D.Impulse);
        }
    }

    private void doMove(Vector2 dir)
    {
        if (!onWall)
        {
            float targetSpeed = dir.x * (onGround ? speed : speed * jumpSpeedModifier);
            float speedDif = targetSpeed - rb.velocity.x;
            float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
            float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
            rb.AddForce(movement * Vector2.right);
        }
    }

    public void Die()
    {
        isDead = true;
        deathSource.Play();
        rb.simulated = false;
        StartCoroutine(doDie());
    }

    IEnumerator doDie()
    {
        EventBus.Instance.CameraShake(0.3f);
        yield return new WaitForSeconds(death.length);
        EventBus.Instance.PlayerDied.Invoke();
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (isDead) return;
        inputDir = ctx.ReadValue<Vector2>();
        if (inputDir != Vector2.zero)
        {
            transform.localScale = new Vector2(inputDir.x > 0 ? 1 : -1, 1);
        }
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (isDead) return;
        isJumpPressed = ctx.ReadValueAsButton();
        if (isJumpPressed && canJump)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
