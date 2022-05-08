using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private SpriteRenderer sprite;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        if(Player.instance.facingRight)
            GetComponent<Rigidbody2D>().velocity = Vector2.right * 5;
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.right * -5;
            sprite.flipX = true;
        }
    }
}
