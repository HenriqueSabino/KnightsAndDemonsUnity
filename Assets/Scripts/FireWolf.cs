using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWolf : MonoBehaviour
{
    public float Speed;

    [SerializeField]
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer sprite;
    private Animator anim;
    private bool IsAlive = true;
    public int Health = 50;
    public int Damage = 12;
    public float dashSpeed = 8;
    public Collider2D GroundCollider;
    public GameObject Fireball;
    public Transform IdlePos;
    public Transform ArenaEnd;
    public Transform FireballSpawn;

    private State CurrentState = 0;

    [SerializeField]
    private float TimeBetweenAttacks;
    private float LastAttackTime;

    private enum State
    {
        IDLE,
        DASH,
        DASH_START,
        DASHING_FORWARDS,
        DASHING_BACKWARDS,
        SPIT,
        SPITTING,
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
                        if (Random.Range(0f, 1f) < 0.5f)
                            CurrentState = State.SPIT;
                        else
                            CurrentState = State.DASH;
                    }
                    break;
                case State.DASH:
                    anim.SetTrigger("Dash");
                    CurrentState = State.DASH_START;
                    break;
                case State.DASHING_FORWARDS:
                    Dash(true);
                    break;
                case State.DASHING_BACKWARDS:
                    Dash(false);
                    break;
                case State.SPIT:
                    anim.SetTrigger("Spit");
                    CurrentState = State.SPITTING;
                    break;
            }
        }
    }


    private void ResetAttack()
    {
        LastAttackTime = Time.timeSinceLevelLoad;
        CurrentState = State.IDLE;
    }

    private void StartDash()
    {
        CurrentState = State.DASHING_FORWARDS;
    }

    private void Dash(bool forwards)
    {
        Vector3 target;

        if (forwards)
            target = ArenaEnd.position;
        else
            target = IdlePos.position;

        sprite.flipX = !forwards;

        if (Vector3.Distance(target, transform.position) <= 0.1f)
        {
            transform.position = target;
            if (forwards)
                CurrentState = State.DASHING_BACKWARDS;
            else
            {
                ResetAttack();
                anim.SetTrigger("Idle");
            }
        }

        float dir = (target - transform.position).normalized.x;

        rigidbody2D.velocity = Vector2.right * dir * dashSpeed;
    }


    // Called by animation
    private void Spit()
    {
        Fireball fb = Instantiate(Fireball, FireballSpawn.position, Quaternion.Euler(0, 0, 180)).GetComponent<Fireball>();

        fb.damage = Damage;

        ResetAttack();
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
                GroundCollider.enabled = false;
                rigidbody2D.velocity = new Vector2(0, 5);
                if (transform.position.y < -20)
                {
                    Destroy(gameObject);
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
}
