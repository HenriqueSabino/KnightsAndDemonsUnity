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

    // Start is called before the first frame update
    void Start()
    {
        Target = Player.instance.transform;
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int sign = (int)Mathf.Sign(Target.position.x - transform.position.x);

        sprite.flipX = sign > 0;

        rigidbody2D.velocity = new Vector2(sign * Speed, rigidbody2D.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 8)
        {
            rigidbody2D.AddForce(new Vector2(0, 10));
            print("hit");
        }
    }
}
