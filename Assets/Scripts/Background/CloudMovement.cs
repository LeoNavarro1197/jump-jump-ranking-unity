using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public PlayerControl playerControl;
    public PlayerDestruction playerDestruction;

    Rigidbody2D rb;
    public int moveDirectionHorizontal;
    public float speedHorizontal;

    public float speedCloud, intervalo;

    public List<GameObject> pointList;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject nombrePlayer = GameObject.Find("Player");
        playerControl = nombrePlayer.GetComponent<PlayerControl>();

        GameObject nombrePlayerDestruction = GameObject.Find("Player");
        playerDestruction = nombrePlayerDestruction.GetComponent<PlayerDestruction>();
    }

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        moveDirectionHorizontal = 0;
    }

    void FixedUpdate()
    {
        // Iniciar juego
        if (playerControl.START && !playerDestruction.DEATH)
        {
            // Seguir la direccion del player mas una pequeña velocidad para dar la sensacion de subida del player
            float invertedVelocity = -playerControl.GetComponent<Rigidbody2D>().linearVelocity.y;
            rb.linearVelocity = new Vector2(moveDirectionHorizontal * speedHorizontal, invertedVelocity * intervalo - speedCloud);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveDirectionHorizontal * speedHorizontal, 0);
        }
    }

    // Cambiar la direccion del carro cuando choque con el colisionador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "SenseLocationA")
        {
            moveDirectionHorizontal = 1;
            transform.rotation = Quaternion.Euler(0, 180, 0);

            int randomPosition = Random.RandomRange(0, pointList.Count);
            transform.position = pointList[randomPosition].transform.position;
        }
        else if (collision.transform.tag == "SenseLocationB")
        {
            moveDirectionHorizontal = -1;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            int randomPosition = Random.RandomRange(0, pointList.Count);
            transform.position = pointList[randomPosition].transform.position;
        }
    }
}
