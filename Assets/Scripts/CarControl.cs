using UnityEngine;

public class CarControl : MonoBehaviour
{
    public PlayerControl playerControl;
    public PlayerDestruction playerDestruction;
    public CarMovementControl carMovementControl;

    Rigidbody2D rb;
    public int moveDirectionHorizontal;
    public float speedHorizontal;

    public float speedCar;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject nombrePlayer = GameObject.Find("Player");
        playerControl = nombrePlayer.GetComponent<PlayerControl>();

        GameObject nombrePlayerDestruction = GameObject.Find("Player");
        playerDestruction = nombrePlayerDestruction.GetComponent<PlayerDestruction>();

        GameObject nombreCar = GameObject.Find("CarMovementControl");
        carMovementControl = nombreCar.GetComponent<CarMovementControl>();
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
        if (playerControl.start && !playerDestruction.DEATH)
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
            banderaA = false;
        }
        else if(collision.transform.tag == "SenseLocationB" && banderaB)
        {
            speedHorizontal = Random.Range(carMovementControl.speedHorizontalMinimo, carMovementControl.speedHorizontalMaximo);
            moveDirectionHorizontal = -1;
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
    }
}
