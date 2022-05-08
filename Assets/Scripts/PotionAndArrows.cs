using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionAndArrows : MonoBehaviour
{
    public int HealAmount;
    public int ArrowAmount;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player.instance.Heal(HealAmount);
            Player.instance.IncreaseArrows(ArrowAmount);
            Destroy(gameObject);
        }
    }
}
