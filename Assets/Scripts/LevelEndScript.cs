using UnityEngine;

public class LevelEndScript : MonoBehaviour
{
    public bool CheckKey;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.instance.NextLevel(CheckKey);
        }
    }
}
