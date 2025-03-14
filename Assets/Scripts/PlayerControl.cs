using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    public CarPool carPool;
    public CarMovementControl carMovementControl;

    public float horizontalSpeed = 5f;
    public float jumpForce = 10f, firstJump = 9.8f;
    public float moveDirection = 0f;
    private Rigidbody2D rb;

    public TMP_Text countdownText;
    public GameObject panelCountdown, panelStart;

    public bool start = false;

    [SerializeField] private List<CapsuleCollider2D> capsuleCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Obtener todos los componentes de los carros para activar el movimiento vertical de los mismos
        /*for (int i = 0; i < carPool.carList.Count; i++)
        {
            CarControl carControl = carPool.carList[i].GetComponent<CarControl>();
            capsuleCollider.Add(carControl.GetComponent<CapsuleCollider2D>());
        }*/

        // Mover al player
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection = 1;
        }
        else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection = -1;
        }
        else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
        {
            moveDirection = 0;
        }

        // ----TESTEO----
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void FixedUpdate()
    {
        // Dar una velocidad al player
        rb.linearVelocity = new Vector2(moveDirection * horizontalSpeed, rb.linearVelocity.y);
    }

    // Cambiar la dirección del player
    public void MoveLeft() { moveDirection = -1; }
    public void MoveRight() { moveDirection = 1; }
    public void StopMoving() { moveDirection = 0; }

    // Boton Start en la UI
    public void CoroutineStart()
    {
        StartCoroutine(StartCountdown());
    }

    // Funcion para empezar el juego
    public IEnumerator StartCountdown()
    {
        panelCountdown.SetActive(true);
        panelStart.SetActive(false);

        for (int i = 0; i > 0; i--)
        {
            // Contéo regresivo
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "¡GO!";
        yield return new WaitForSeconds(0.5f);

        start = true;
        panelCountdown.SetActive(false);
        rb.AddForce(Vector2.up * firstJump, ForceMode2D.Impulse);  // Aplicar Fuerza de salto

    }

    // Aplicar fuerza cuando toca la parte de arriba de un carro
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Car")
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // Desactivar colisionador del carro para poder pasar por debajo
    // FASE_DE_PRUEBA
    // Solo se llama a esta funcion cuando el personaje pasa debajo del carro (no por los lados)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "DownCollision")
        {
            for (int i = 0; i < carPool.poolSize; i++)
            {
                //capsuleCollider[i].enabled = false;
            }
            StartCoroutine(ActiveColliderCar());
        }
    }

    // Activa el colisionador nuevamente despues de pasar debajo del carro
    public IEnumerator ActiveColliderCar()
    {
        yield return new WaitForSeconds(carMovementControl.carColliderRespawn);
        for (int i = 0; i < carPool.poolSize; i++)
        {
            //capsuleCollider[i].enabled = true;
        }
    }
}
