using System.Drawing;
using UnityEngine;

public class Arrounds : MonoBehaviour
{
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "ArroundA1")
        {
            rb.linearVelocityY = 0f;
        }

        if (collision.transform.tag == "ArroundB1")
        {
            rb.linearVelocityY = 0f;
        }

        if (collision.transform.tag == "ArroundA2")
        {
            rb.linearVelocityY = 0f;
        }

        if (collision.transform.tag == "ArroundB2")
        {
            rb.linearVelocityY = 0f;
        }
    }
}
