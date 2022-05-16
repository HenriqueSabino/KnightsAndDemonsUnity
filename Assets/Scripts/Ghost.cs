using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public Transform Target;
    private Vector3 TargetOffset;
    public float Speed;

    [SerializeField]
    private new Rigidbody2D rigidbody2D;
    private SpriteRenderer sprite;
    private Animator anim;
    public int Health = 3;
    public int Damage = 2;

    private Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        Target = Player.instance.transform;
        TargetOffset = new Vector3(2, 0.5f);
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (sprite.isVisible)
        {
            if (rigidbody2D.position.x > Target.position.x)
            {
                if(Player.instance.facingRight == false) 
                {
                    rigidbody2D.velocity = (Target.position - transform.position).normalized * Speed;
                    anim.SetBool("IsMoving", true);
                    sprite.color = new Color(1f,1f,1f,1f);
                }
                else 
                {
                    rigidbody2D.velocity = Vector2.zero;
                    anim.SetBool("IsMoving", false);
                    sprite.color = new Color(1f,1f,1f,.2f);
                }
            }
            else
            {
                if(Player.instance.facingRight == true) 
                {
                    rigidbody2D.velocity = (Target.position - transform.position).normalized * Speed;
                    anim.SetBool("IsMoving", true);
                    sprite.color = new Color(1f,1f,1f,1f);
                }
                else 
                {
                    rigidbody2D.velocity = Vector2.zero;
                    anim.SetBool("IsMoving", false);
                    sprite.color = new Color(1f,1f,1f,.2f);
                }
            }

            sprite.flipX = (Target.position - transform.position).x > 0;
        }
    }

    // Update is called once per frame
     private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.TakeDamage(Damage, (other.transform.position + Vector3.up * 0.5f - transform.position).normalized);
        }
    }
}
