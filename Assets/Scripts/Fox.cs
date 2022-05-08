using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    public Transform Target;
    public float Speed;

    [SerializeField]
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer sprite;
    private Animator anim;
    private Vector3 TargetOffset;
    private bool TakingDamage;
    private bool IsAlive = true;
    public int Health = 3;
    public int Damage = 2;
    private float KnockPow = 5;
    public Collider2D GroundCollider;

    // Start is called before the first frame update
    void Start()
    {
        Target = Player.instance.transform;
        TargetOffset = new Vector3(1.5f, 0);
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TakingDamage && rigidbody2D.velocity.y == 0)
        {
            TakingDamage = false;
        }

        if (sprite.isVisible && !TakingDamage && IsAlive)
        {
            float dist = Target.position.x + TargetOffset.x - transform.position.x;

            if (!TakingDamage && Mathf.Abs(dist) <= 0.3f)
            {
                TargetOffset *= -1;
            }

            int sign = (int)Mathf.Sign(dist);

            sprite.flipX = sign != 0 ? sign > 0 : sprite.flipX;

            rigidbody2D.velocity = new Vector2(sign * Speed, rigidbody2D.velocity.y);
        }

        if (!IsAlive)
        {
            anim.SetBool("IsDeath", true);
        }
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!TakingDamage && other.gameObject.layer == LayerMask.NameToLayer("Attack") && IsAlive)
        {
            if (other.CompareTag("Arrow"))
                Health--;
            else
                Health -= 2;

            if (Health > 0)
            {
                TakingDamage = true;
                rigidbody2D.velocity = Target.position.x > transform.position.x ? new Vector2(-1, 0.5f) * KnockPow : new Vector2(1, 0.5f) * KnockPow;
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
