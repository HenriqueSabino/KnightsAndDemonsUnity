using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = Vector2.right * 5;
    }
}
