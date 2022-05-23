using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Random = UnityEngine.Random;

public class DemonicBat : MonoBehaviour
{
    [SerializeField]
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer sprite;
    private Animator anim;
    private bool IsAlive = true;
    public int Health = 50;
    public int Damage = 12;
    public float dashSpeed = 8;
    public float repositionSpeed = 3;
    public Transform IdlePos;
    private Vector3 dashDir;
    public GameObject Fireball;
    public GameObject CaveBat;

    private State CurrentState = 0;

    [SerializeField]
    private float TimeBetweenAttacks;
    private float LastAttackTime;

    private enum State
    {
        IDLE,
        READY_DASH,
        DASHING,
        REPOSITIONING,
        SCREAMING,
        FIREBALLS,
    }

    void Awake()
    {
        if (PlayerPrefs.HasKey("DefeatedDemonicBat"))
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        LastAttackTime = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsAlive)
        {
            switch (CurrentState)
            {
                case State.IDLE:
                    sprite.flipX = false;

                    // wait choose the next attack
                    if (Time.timeSinceLevelLoad - LastAttackTime >= TimeBetweenAttacks)
                    {
                        SelectMove();
                    }
                    break;
                case State.DASHING:
                    Dash();
                    break;
                case State.REPOSITIONING:
                    Reposition();
                    break;
            }
        }
        else
        {
            if (transform.position.y < -10)
            {
                Destroy(gameObject);
            }
        }
    }

    private void SelectMove()
    {
        int move = Random.Range(0, 3);

        switch (move)
        {
            case 0:
                CurrentState = State.READY_DASH;
                anim.SetTrigger("Dash");
                break;
            case 1:
                CurrentState = State.SCREAMING;

                anim.SetTrigger("Scream");

                StartCoroutine(Shake(1, () =>
                {
                    ResetAttack();
                    anim.SetTrigger("Idle");
                }));

                Instantiate(CaveBat, transform.position, Quaternion.identity);
                break;
            case 2:
                CurrentState = State.FIREBALLS;

                StartCoroutine(Shake(1, ResetAttack));

                for (int i = 0; i < 3; i++)
                {
                    FireballFollow fb = Instantiate(Fireball, transform.position, Quaternion.identity).GetComponent<FireballFollow>();
                    fb.damage = Damage;
                }
                break;
        }
    }

    private void ResetAttack()
    {
        LastAttackTime = Time.timeSinceLevelLoad;
        CurrentState = State.IDLE;
    }

    // Called by animation
    private void StartDash()
    {
        CurrentState = State.DASHING;
        dashDir = (Player.instance.transform.position - transform.position).normalized;
    }

    private void Dash()
    {
        if (sprite.isVisible)
        {
            rigidbody2D.velocity = dashDir * dashSpeed;

            float angle = Mathf.Atan2(rigidbody2D.velocity.y, rigidbody2D.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, -angle);
        }
        else
        {
            transform.position = IdlePos.position + Vector3.up * 3;
            transform.rotation = Quaternion.identity;
            CurrentState = State.REPOSITIONING;
            anim.SetTrigger("Idle");
        }
    }

    private void Reposition()
    {
        if (Vector3.Distance(transform.position, IdlePos.position) >= 0.1f)
        {
            rigidbody2D.velocity = Vector2.down * repositionSpeed;
        }
        else
        {
            rigidbody2D.velocity = Vector2.zero;
            transform.position = IdlePos.position;
            ResetAttack();
        }
    }

    private IEnumerator BlinkSprite()
    {
        for (int i = 0; i < 3; i++)
        {
            sprite.enabled = false;
            yield return new WaitForSeconds(0.1f);
            sprite.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Shake(float seconds, Action afterShack = null)
    {
        while (seconds >= 0)
        {
            transform.position += (Vector3)Random.insideUnitCircle * 0.2f;
            yield return new WaitForSeconds(0.05f);
            transform.position = IdlePos.position;

            seconds -= 0.05f;
        }

        afterShack();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attack") && IsAlive)
        {
            if (other.CompareTag("Arrow"))
                Health--;
            else
                Health -= 2;

            if (Health > 0)
            {
                StartCoroutine(BlinkSprite());
            }
            else
            {
                IsAlive = false;
                rigidbody2D.velocity = new Vector2(0, 5);

                PlayerPrefs.SetInt("DefeatedDemonicBat", 1);

                Player.instance.AddPoints(30);
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
}
