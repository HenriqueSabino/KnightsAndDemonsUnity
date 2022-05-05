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

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
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
                rig.AddForce(new Vector2(0f, JumpForce), ForceMode2D.Impulse);
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
        float knobackPwr = 5;
        Health -= damage;
        print($"Player health: {Health}");

        StartCoroutine(Knockback(1, knobackPwr, knobackPwr, knockDir));
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
                if (contact.normal == Vector2.up)
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

    public IEnumerator Knockback(float knockDur, float knockbackPwrY, float knobackPwrX, Vector3 knockbackDir)
    {
        TakingDamage = true;

        rig.AddForce(new Vector2(Mathf.Sign(knockbackDir.x) * knobackPwrX, knockbackPwrY), ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockDur);

        TakingDamage = false;
    }
}
