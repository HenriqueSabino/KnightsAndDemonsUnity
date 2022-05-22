using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum KeyStatus
{
    NO_KEY,
    GOT_KEY,
    USED_KEY
}

public class Key : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPrefs.SetInt("Key", ((int)KeyStatus.GOT_KEY));
            Destroy(gameObject);
        }
    }
}
