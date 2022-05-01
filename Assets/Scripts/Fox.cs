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
        if (other.gameObject.layer == 8)
        {
            TakingDamage = true;
            int sign = (int)Mathf.Sign(Target.position.x - transform.position.x);
            rigidbody2D.AddForce(new Vector2(sign * 2, 2));
            if(rigidbody2D.transform.position.x > Target.position.x)
                StartCoroutine(Knockback(KnockDur,KnockPow,0.5f,rigidbody2D.transform.position));
            else
                StartCoroutine(Knockback(KnockDur,KnockPow,-0.5f,rigidbody2D.transform.position));
        }

        if(other.gameObject.tag == "Player")
        {
            if(rigidbody2D.transform.position.x > Target.position.x)
                StartCoroutine(Player.instance.Knockback(KnockDur,KnockPow,-3,Target.position));
            else
                StartCoroutine(Player.instance.Knockback(KnockDur,KnockPow,3,Target.position));
        }
    }

    public IEnumerator Knockback(float knockDur, float knockbackPwrY,float knobackPwrX, Vector3 knockbackDir){
 
        float timer = 0;
 
        while( knockDur > timer){
 
            timer+=Time.deltaTime;
 
            rigidbody2D.AddForce(new Vector2(knockbackDir.x * knobackPwrX, knockbackDir.y * knockbackPwrY));
 
        }
 
        yield return 0;
 
    }
}
