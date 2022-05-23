using UnityEngine;

public class LevelEndScript : MonoBehaviour
{
    public bool CheckKey;
    public bool PreviousLevel;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (PreviousLevel)
            {
                GameManager.instance.PreviousLevel(CheckKey);
            }
            else
            {
                GameManager.instance.NextLevel(CheckKey);
            }
        }
    }
}
