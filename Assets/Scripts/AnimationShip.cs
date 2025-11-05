using UnityEngine;

public class AnimationShip : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            Debug.Log("Bouncing");
        }
    }
}
