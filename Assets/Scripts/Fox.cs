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
    private bool TakingDamage;
    private int Health = 3;

    private float KnockDur = 1;
    private float KnockPow = 2;

    // Start is called before the first frame update
    void Start()
    {
        Target = Player.instance.transform;
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!TakingDamage)
        {
            int sign = (int)Mathf.Sign(Target.position.x - transform.position.x);

            sprite.flipX = sign > 0;

            rigidbody2D.velocity = new Vector2(sign * Speed, rigidbody2D.velocity.y);
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
            Health--;

            if (Health > 0)
            {
                Vector3 knockDir = transform.position - other.transform.position;

                StartCoroutine(Knockback(KnockDur, KnockPow, knockDir));
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else if (other.CompareTag("Player"))
        {
            Player.instance.TakeDamage(2, other.transform.position - transform.position);
        }
    }

    public IEnumerator Knockback(float knockDur, float knockbackPwr, Vector3 knockbackDir)
    {
        TakingDamage = true;
        rigidbody2D.AddForce(new Vector2(Mathf.Sign(knockbackDir.x) * knockbackPwr, knockbackPwr), ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockDur);

        TakingDamage = false;
    }
}
