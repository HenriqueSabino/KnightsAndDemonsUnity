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
    public bool facingRight = true;
    public int Health { get; private set; } = 100;
    public int Points { get; private set; } = 0;
    public int Arrows { get; private set; } = 10;
    public int Lives { get; private set; } = 3;
    public float WingsCD { get; private set; } = 0;
    public Collider2D GroundCollider;
    public Transform ArrowSpawn;
    public GameObject ArrowPrefab;

    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer sprite;
    private Vector3 movement;
    private bool Invulnarable;
    private bool HasWings;
    private bool CanUseWings;


    void Awake()
    {
        if (instance == null)
            instance = this;

        if (PlayerPrefs.HasKey("Lives"))
        {
            Lives = PlayerPrefs.GetInt("Lives");
            PlayerPrefs.DeleteKey("Lives");
        }

        if (PlayerPrefs.HasKey("Health"))
        {
            Health = PlayerPrefs.GetInt("Health");
            PlayerPrefs.DeleteKey("Health");
        }

        if (PlayerPrefs.HasKey("Arrows"))
        {
            Arrows = PlayerPrefs.GetInt("Arrows");
            PlayerPrefs.DeleteKey("Arrows");
        }

        if (PlayerPrefs.HasKey("Points"))
        {
            Points = PlayerPrefs.GetInt("Points");
            PlayerPrefs.DeleteKey("Points");
        }

        HasWings = PlayerPrefs.HasKey("Wings");
        CanUseWings = HasWings;
        WingsCD = HasWings ? 1 : 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.UpdateWingsCooldown(WingsCD);
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

        if (isAlive)
        {
            if (!TakingDamage)
            {
                Move();
            }

            if (!TakingDamage || CanUseWings)
            {
                Jump();
            }

            Attack();
        }
        else
        {
            anim.SetBool("IsJumping", true);
        }

        if (transform.position.y < -10)
        {
            GameManager.instance.PlayerDeath(Lives);
        }
    }

    void Move()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, 0f);

        if (isAttacking)
            movement = Vector3.zero;

        rig.velocity = new Vector2(movement.x * Speed, rig.velocity.y);

        anim.SetBool("IsMoving", movement.x != 0 && !anim.GetBool("IsJumping"));

        if (movement.x != 0)
        {
            transform.rotation = movement.x < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
        }

        if (movement.x > 0)
        {
            facingRight = true;
        }
        else if (movement.x < 0)
        {
            facingRight = false;
        }
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (CanJump || CanUseWings)
            {

                if (!CanJump)
                {
                    WingsCD = 0;
                    GameManager.instance.UpdateWingsCooldown(WingsCD);
                    StartCoroutine(WingsCooldown());
                    TakingDamage = false;
                    rig.velocity = Vector2.zero;
                }

                Vector2 v = rig.velocity;
                v.y += JumpForce;
                rig.velocity = v;
                anim.SetBool("IsJumping", true);
                CanJump = false;
            }
        }
    }

    void Attack()
    {
        if (!isAttacking)
        {
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(0))
            {
                isAttacking = true;
                anim.SetTrigger("SwordAttack");
            }
            else if (Arrows > 0 && (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1)))
            {
                isAttacking = true;
                anim.SetTrigger("BowAttack");
            }
        }
    }

    public void SpawnArrow()
    {
        // TODO: Object pooling
        Instantiate(ArrowPrefab, ArrowSpawn.position, Quaternion.identity);
        Arrows--;
        GameManager.instance.UpdatePlayerArrows(Arrows);
    }

    public void TakeDamage(int damage, Vector3 knockDir)
    {
        if (!Invulnarable && isAlive)
        {
            Health -= damage;
            GameManager.instance.UpdatePlayerHealth(Health);

            TakingDamage = true;
            isAttacking = false;

            if (Health > 0)
            {
                rig.velocity = Vector2.right * knockDir.x * 3 + Vector2.up * 2;
                StartCoroutine(Invulnarability());
            }
            else
            {
                isAlive = false;
                Health = 0;
                GameManager.instance.UpdatePlayerHealth(Health);
                GroundCollider.enabled = false;
                rig.velocity = new Vector2(0, 5);
            }
        }

        if (damage == 100)
        {
            sprite.enabled = true;
            isAlive = false;
            Health = 0;
            GameManager.instance.UpdatePlayerHealth(Health);
            GroundCollider.enabled = false;
            rig.velocity = new Vector2(0, 5);
        }
    }

    public void Heal(int amount)
    {
        Health += amount;

        if (Health > 100)
            Health = 100;

        GameManager.instance.UpdatePlayerHealth(Health);
    }

    public void IncreaseArrows(int amount)
    {
        Arrows += amount;

        if (Arrows > 20)
            Arrows = 20;

        GameManager.instance.UpdatePlayerArrows(Arrows);
    }

    public void AddPoints(int amount)
    {
        Points += amount;

        Lives += Points / 100;

        Points = Points % 100;

        GameManager.instance.UpdatePlayerPoints(Points);
        GameManager.instance.UpdatePlayerLives(Lives);
    }

    private IEnumerator WingsCooldown()
    {
        CanUseWings = false;

        while (WingsCD < 1)
        {
            yield return new WaitForSeconds(1f);
            WingsCD += 0.1f;
            GameManager.instance.UpdateWingsCooldown(WingsCD);
        }

        CanUseWings = true;
    }

    private IEnumerator Invulnarability()
    {
        Invulnarable = true;
        for (float i = 0f; i < 1.5f; i += 0.1f)
        {
            if (isAlive)
                sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            if (isAlive)
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

        if (collision.gameObject.layer == 11 && rig.velocity.y == 0)
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

        if (collision.gameObject.layer == LayerMask.NameToLayer("SpikeActive"))
        {
            TakeDamage(5, (transform.position + Vector3.up * 0.5f - collision.transform.position).normalized);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Lava"))
        {
            TakeDamage(100, (transform.position + Vector3.up * 0.5f - collision.transform.position).normalized);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 11)
        {
            CanJump = false;
            anim.SetBool("IsJumping", true);
            anim.SetBool("IsMoving", false);
        }
    }
}
