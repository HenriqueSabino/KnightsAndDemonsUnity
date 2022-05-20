using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Rigidbody2D rig;
    private SpriteRenderer sprite;
    public float speed;
    public int damage;
    public Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        if (direction == Vector2.left)
        {
            sprite.flipX = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rig.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.TakeDamage(damage, (other.transform.position + Vector3.up * 0.5f - transform.position).normalized);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}
