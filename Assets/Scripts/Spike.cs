using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public int Damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.TakeDamage(Damage, (other.transform.position + Vector3.up * 0.5f - transform.position).normalized);
        }
    }
}
