using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    public float Speed;
    public float JumpForce;
    public bool CanJump;
    public bool isAttacking;
    public bool TakingDamage;
    public int Health { get; private set; } = 100;
    public int Lives { get; private set; } = 3;

    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer sprite;
    private Vector3 movement;
    private bool Invulnarable;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (TakingDamage && rig.velocity.y == 0)
        {
            TakingDamage = false;
        }

        if (!TakingDamage)
        {
            Move();
            Jump();
        }

        Attack();
        if (transform.position.y < -20)
        {
            print($"Player lives: {--Lives}");
            transform.position = new Vector3(4.21f, 10, 0);
        }
    }

    void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);

        if (isAttacking)
            movement = Vector3.zero;

        rig.velocity = new Vector2(movement.x * Speed, rig.velocity.y);

        anim.SetBool("IsMoving", movement.x != 0 && !anim.GetBool("IsJumping"));

        sprite.flipX = movement.x != 0 ? movement.x < 0 : sprite.flipX;
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (CanJump)
            {
                Vector2 v = rig.velocity;
                v.y += 5;
                rig.velocity = v;
                anim.SetBool("IsJumping", true);
                CanJump = false;
            }
        }
    }

    void Attack()
    {
        if (!isAttacking && (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(0)))
        {
            isAttacking = true;
            anim.SetTrigger("SwordAttack");
        }
    }

    public void TakeDamage(int damage, Vector3 knockDir)
    {
        if (!Invulnarable)
        {
            Health -= damage;
            print($"Player health: {Health}");

            TakingDamage = true;
            rig.velocity = Vector2.right * knockDir.x * 3 + Vector2.up * 2;
            StartCoroutine(Invulnarability());
        }
    }

    private IEnumerator Invulnarability()
    {
        Invulnarable = true;
        yield return new WaitForSeconds(3);
        Invulnarable = false;
    }

    public void ResetAttack()
    {
        isAttacking = false;
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            foreach (var contact in collision.contacts)
            {
                if (contact.normal.normalized == Vector2.up)
                {
                    CanJump = true;
                    anim.SetBool("IsJumping", false);
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            CanJump = false;
            anim.SetBool("IsJumping", true);
            anim.SetBool("IsMoving", false);
        }
    }
}
