using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballFollow : MonoBehaviour
{
    private Rigidbody2D rig;
    private SpriteRenderer sprite;
    public float maxSpeed;
    public int damage;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        rig.velocity = Random.insideUnitCircle;
    }

    // Update is called once per frame
    void Update()
    {
        rig.AddForce((Player.instance.transform.position - transform.position).normalized);

        if (rig.velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            rig.velocity = rig.velocity.normalized * maxSpeed;
        }
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
