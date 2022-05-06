using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    public Transform Target;
    private Vector3 TargetOffset;
    public float Speed;

    [SerializeField]
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer sprite;
    private bool TakingDamage;
    private bool Attacking;
    public int Health = 3;
    public int Damage = 2;

    private float KnockPow = 5;

    private Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        Target = Player.instance.transform;
        TargetOffset = new Vector3(2, 0.5f);
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TakingDamage)
        {
            rigidbody2D.velocity = Target.position.x > transform.position.x ? Vector2.left * KnockPow : Vector2.right * KnockPow;

            if (Mathf.Abs(Target.position.x - transform.position.x) >= 3)
            {
                TakingDamage = false;
                rigidbody2D.velocity = Vector2.zero;
            }
        }

        if (Attacking)
        {
            float dist = Mathf.Abs(Target.position.x - transform.position.x);
            if (dist <= 0.1f)
            {
                Attacking = false;
                rigidbody2D.velocity = Vector2.zero;
            }
            else if (dist >= 2.5f)
            {
                TargetOffset.x *= -1;
                Attacking = false;
                rigidbody2D.velocity = Vector2.zero;
            }
        }

        if (sprite.isVisible && !TakingDamage && !Attacking)
        {

            if (Vector3.Distance(Target.position + TargetOffset, transform.position) <= 0.1f)
            {
                TargetOffset.x *= -1;
                rigidbody2D.velocity = Vector2.zero;
                rigidbody2D.AddForce((Target.position - transform.position + Vector3.up * 0.5f).normalized * 6, ForceMode2D.Impulse);
                Attacking = true;
            }

            if (!Attacking)
            {
                rigidbody2D.velocity = (Target.position + TargetOffset - transform.position).normalized * Speed;
            }
            sprite.flipX = (Target.position - transform.position).x > 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 6)
        {
            TakingDamage = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!TakingDamage && other.gameObject.layer == LayerMask.NameToLayer("Attack"))
        {
            if (other.CompareTag("Arrow"))
                Health--;
            else
                Health -= 2;

            if (Health > 0)
            {
                TakingDamage = true;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Player"))
        {
            Player.instance.TakeDamage(Damage, (other.transform.position - transform.position).normalized);
        }
    }
}
