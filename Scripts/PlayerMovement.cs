using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private SpriteRenderer sprite;

    private Animator anim;

    [SerializeField] private LayerMask jumpableGround;

    private bool isGrounded;


    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private int maxJumps = 2;

    private int remainingJumps = 0;

    private enum MovementState { idel, running, jumping, falling }

    [SerializeField] private AudioSource jumpSoundEffect;
    [SerializeField] private LayerMask groundLayer;
    //[SerializeField] private Joystick _joistick;

    // Start is called before the first frame update 
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        remainingJumps = maxJumps; // Сколько осталось прыжков
    }

    public void Jump()
    {
        if (isGrounded || remainingJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            remainingJumps--;
        }
    }

    // Update is called once per frame
    private void Update()
    {

        dirX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        isGrounded = IsGrounded();

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        UpdateAnimationState();

    }


    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            state = MovementState.running;
            sprite.flipX = true;
        }
        else
        {
            state = MovementState.idel;
        }

        if (rb.velocity.y > .1f)
        {
            state = MovementState.jumping;
        }
        else if (rb.velocity.y < -.1f)
        {
            state = MovementState.falling;
        }

        anim.SetInteger("state", (int)state);

    }

    bool IsGrounded()
    {
        float extraHeight = 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, extraHeight, groundLayer);
        return hit.collider != null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            remainingJumps = maxJumps;
        }
    }
}
