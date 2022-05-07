using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player instance;
    public float Speed;
    public float JumpForce;
    public bool CanJump;
    public bool isAttacking;
    public bool TakingDamage;
    public bool isAlive = true;
    public int Health { get; private set; } = 100;
    public int Lives { get; private set; } = 3;
    public Collider2D GroundCollider;

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

        if(isAlive)
        {
            if (!TakingDamage)
            {
                Move();
                Jump();
            }

            Attack();
        }
        else
        {
            anim.SetBool("IsJumping", true);
        }

        if (transform.position.y < -20)
        {
            GameManager.instance.PlayerDeath();
            print($"Player lives: {Lives}");
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

            if(Health > 0)
            {
                rig.velocity = Vector2.right * knockDir.x * 3 + Vector2.up * 2;
                StartCoroutine(Invulnarability());
            }
            else
            {
                isAlive = false;
                GroundCollider.enabled = false;
                rig.velocity = new Vector2 (0,5);
            }
        }
    }

    private IEnumerator Invulnarability()
    {
        Invulnarable = true;
        for (float i = 0f; i < 1.5f; i += 0.1f)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
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

        if(collision.gameObject.layer == LayerMask.NameToLayer("SpikeActive"))
        {
            TakeDamage(5, (transform.position + Vector3.up * 0.5f - collision.transform.position).normalized);
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

     void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("SpikeActive") && isAlive)
        {
            TakeDamage(50, (transform.position + Vector3.up * 0.5f - other.transform.position).normalized);
        }
    }
}
