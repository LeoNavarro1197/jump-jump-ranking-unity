using UnityEngine;
using System.Collections;

public class CarControl : MonoBehaviour
{
    public PlayerControl playerControl;
    public PlayerDestruction playerDestruction;
    public CarMovementControl carMovementControl;
    public Material material;
    public Animator animator;
    public SpriteRenderer spriteRenderer, chispas_0;

    Rigidbody2D rb;
    public int moveDirectionHorizontal;
    public float speedHorizontal;

    public float speedCar;

    SpriteRenderer sr;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject nombrePlayer = GameObject.Find("Player");
        playerControl = nombrePlayer.GetComponent<PlayerControl>();

        GameObject nombrePlayerDestruction = GameObject.Find("Player");
        playerDestruction = nombrePlayerDestruction.GetComponent<PlayerDestruction>();

        GameObject nombreCar = GameObject.Find("CarMovementControl");
        carMovementControl = nombreCar.GetComponent<CarMovementControl>();

        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        banderaA = true;
        banderaB = true;
        moveDirectionHorizontal = 0;
    }

    void FixedUpdate()
    {
        // Iniciar juego
        if (playerControl.START && !playerDestruction.DEATH)
        {
            // Seguir la direccion del player mas una pequeña velocidad para dar la sensacion de subida del player
            float invertedVelocity = -playerControl.GetComponent<Rigidbody2D>().linearVelocity.y;
            rb.linearVelocity = new Vector2(moveDirectionHorizontal * speedHorizontal, invertedVelocity - speedCar);
        }
        else
        {
            rb.linearVelocity = new Vector2(moveDirectionHorizontal * speedHorizontal, 0);
        }
    }

    bool banderaA = true;
    bool banderaB = true;

    // Cambiar la direccion del carro cuando choque con el colisionador
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "SenseLocationA" && banderaA)
        {
            speedHorizontal = Random.Range(carMovementControl.speedHorizontalMinimo, carMovementControl.speedHorizontalMaximo);
            moveDirectionHorizontal = 1;
            transform.rotation = Quaternion.Euler(0, 180, 0);
            banderaA = false;
        }
        else if(collision.transform.tag == "SenseLocationB" && banderaB)
        {
            speedHorizontal = Random.Range(carMovementControl.speedHorizontalMinimo, carMovementControl.speedHorizontalMaximo);
            moveDirectionHorizontal = -1;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            banderaB = false;
        }

        if (collision.transform.tag == "SenseLocationB" && !banderaA)
        {
            gameObject.SetActive(false);
        }
        if (collision.transform.tag == "SenseLocationA" && !banderaB)
        {
            gameObject.SetActive(false);
        }

        if (collision.transform.tag == "SenseLocationA" || collision.transform.tag == "SenseLocationB")
        {
            animator.SetBool("isPlay", false);
            spriteRenderer.sprite = chispas_0.sprite;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Player")
        {
            sr.material.SetFloat("Offset_Y", 0.05f);
            sr.material.SetFloat("Intensity_Y", 0.17f);
            sr.material.SetFloat("Speed", 0.5f);

            animator.SetBool("isPlay", true);

            Invoke("ResetMaterial", .5f);
            Invoke("ResetParticles", .5f);
        }
    }

    void ResetMaterial()
    {
        sr.material.SetFloat("Offset_Y", 0f);
        sr.material.SetFloat("Intensity_Y", 0f);
        sr.material.SetFloat("Speed", 0f);
    }

    void ResetParticles()
    {
        animator.SetBool("isPlay", false);
    }
}
