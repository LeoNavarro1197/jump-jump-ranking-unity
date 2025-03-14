using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnMovement : MonoBehaviour
{
    Rigidbody2D rb;
    public CarMovementControl carMovementControl;
    public Transform leftPosition, rigthPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        // Dar una velocidad al los spawns
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, carMovementControl.moveDirectionSpawns * carMovementControl.speedSpawns);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "SpawnCollider1")
        {
            this.transform.position = leftPosition.position;
        }

        if (collision.transform.tag == "SpawnCollider2")
        {
            this.transform.position = rigthPosition.position;
        }
    }
}
