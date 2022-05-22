using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

public class JumperBoss : MonoBehaviour
{
    public float Speed;

    [SerializeField]
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer sprite;
    private Animator anim;
    private bool IsAlive = true;
    public int Health = 50;
    private int MaxHealth;
    public int Damage = 12;
    public float stompSpeed = 8;
    public Transform FireballSpawn;
    public GameObject Fireball;
    public GameObject DemonBat;
    public GameObject Ghost;
    public bool TakingDamage;
    public bool Invulnarable;
    private float KnockPow = 5;

    private State CurrentState = 0;
    private List<GameObject> spawnedEnemies;

    [SerializeField]
    private float TimeBetweenAttacks;
    private float LastAttackTime;

    private enum State
    {
        IDLE,
        READY_STOMP,
        STOMPING_UP,
        STOMP_WAITING,
        STOMPING_DOWN,
        FIREBALLS,
        DYING
    }

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        LastAttackTime = Time.timeSinceLevelLoad;
        MaxHealth = Health;
        spawnedEnemies = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAlive)
        {
            int sign = (int)Mathf.Sign(Player.instance.transform.position.x - transform.position.x);

            // Rotates Fireball spawn as well
            transform.rotation = sign < 0 ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;

            switch (CurrentState)
            {
                case State.IDLE:
                    // wait choose the next attack
                    if (Time.timeSinceLevelLoad - LastAttackTime >= TimeBetweenAttacks)
                    {
                        SelectMove(sign);
                    }
                    else
                    {
                        Idle(sign);
                    }
                    break;
                case State.READY_STOMP:
                    rigidbody2D.velocity = Vector2.zero;
                    break;
                case State.STOMPING_UP:
                    StompUp();
                    break;
                case State.STOMPING_DOWN:
                    StompDown();
                    break;
            }

            if (spawnedEnemies.Count == 0 && Health <= MaxHealth / 2)
            {
                spawnedEnemies.Add(Instantiate(Ghost, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity));
                spawnedEnemies.Add(Instantiate(Ghost, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity));
                spawnedEnemies.Add(Instantiate(DemonBat, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity));
                spawnedEnemies.Add(Instantiate(DemonBat, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity));
            }
        }
    }

    private void Idle(int sign)
    {
        if (!TakingDamage)
        {
            rigidbody2D.velocity = new Vector2(sign * Speed, rigidbody2D.velocity.y);
        }
        else if (rigidbody2D.velocity.y == 0)
        {
            TakingDamage = false;
        }
    }

    private void SelectMove(int sign)
    {
        if (!TakingDamage)
        {
            int max = Health > MaxHealth / 2 ? 1 : 2;
            int move = Random.Range(0, max);

            switch (move)
            {
                case 0:
                    CurrentState = State.FIREBALLS;

                    rigidbody2D.velocity = Vector2.zero;

                    anim.SetTrigger("Fireball");
                    Invulnarable = true;
                    break;
                case 1:
                    CurrentState = State.READY_STOMP;
                    anim.SetTrigger("Stomp");
                    Invulnarable = true;
                    break;
            }
        }
        else
        {
            Idle(sign);
        }
    }

    private void ResetAttack()
    {
        LastAttackTime = Time.timeSinceLevelLoad;
        CurrentState = State.IDLE;
        Invulnarable = false;
    }

    // Called by animation
    private void Die()
    {
        Destroy(gameObject);
    }

    // Called by animation
    private void StartStomp()
    {
        CurrentState = State.STOMPING_UP;
    }

    // Called by animation
    private void FireSpit()
    {
        Fireball fb = Instantiate(Fireball, FireballSpawn.position, Quaternion.identity).GetComponent<Fireball>();

        fb.damage = Damage;
        fb.direction = Vector2.left;

        ResetAttack();
    }

    private void StompUp()
    {
        if (sprite.isVisible)
        {
            rigidbody2D.velocity = Vector2.up * stompSpeed / 2;
        }
        else
        {
            CurrentState = State.STOMP_WAITING;
            StartCoroutine(StompWait());
        }
    }

    private void StompDown()
    {
        rigidbody2D.velocity = Vector2.down * stompSpeed;
    }

    private IEnumerator BlinkSprite()
    {
        while (TakingDamage)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator StompWait()
    {
        rigidbody2D.gravityScale = 0;
        rigidbody2D.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        rigidbody2D.gravityScale = 1;
        CurrentState = State.STOMPING_DOWN;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!Invulnarable && !TakingDamage && other.gameObject.layer == LayerMask.NameToLayer("Attack") && IsAlive)
        {
            if (other.CompareTag("Arrow"))
                Health--;
            else
                Health -= 2;

            rigidbody2D.velocity = Player.instance.transform.position.x > transform.position.x ? new Vector2(-1, 0.5f) * KnockPow : new Vector2(1, 0.5f) * KnockPow;
            if (Health > 0)
            {
                TakingDamage = true;
                StartCoroutine(BlinkSprite());
            }
            else
            {
                rigidbody2D.gravityScale = 0;
                CurrentState = State.DYING;
                anim.SetTrigger("Die");

                foreach (var enemy in spawnedEnemies)
                {
                    Destroy(enemy);
                }
            }

            if (other.CompareTag("Arrow"))
                Destroy(other.gameObject);
        }
        else if (other.CompareTag("Player"))
        {
            if (IsAlive)
                Player.instance.TakeDamage(Damage, (other.transform.position + Vector3.up * 0.5f - transform.position).normalized);
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (CurrentState == State.STOMPING_DOWN)
            {
                foreach (var contact in other.contacts)
                {
                    if (contact.normal == Vector2.up)
                    {
                        anim.SetTrigger("Idle");
                        ResetAttack();
                        rigidbody2D.velocity = Vector2.zero;

                        if (Player.instance.CanJump)
                        {
                            Player.instance.TakeDamage(Damage, (other.transform.position + Vector3.up * 0.5f - transform.position).normalized);
                        }
                        break;
                    }
                }
            }
        }
    }

}
