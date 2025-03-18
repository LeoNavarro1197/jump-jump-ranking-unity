using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;

public class PlayerControl : MonoBehaviour
{
    public CarPool carPool;
    public CarMovementControl carMovementControl;

    public float horizontalSpeed = 5f;
    public float airControl = 0.1f;  // Controla qué tan rápido se detiene en el aire
    float airAcceleration = 0.1f;
    public float jumpForce = 10f, firstJump = 9.8f;

    public float moveDirection = 0f;
    private Rigidbody2D rb;

    public TMP_Text countdownText;
    public GameObject panelCountdown, panelStart;

    public bool start = false;
    private bool canJump = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    void FixedUpdate()
    {
        if (moveDirection != 0)
        {
            // Movimiento en el aire con control reducido
            rb.linearVelocity = new Vector3(moveDirection * horizontalSpeed * 0.8f, rb.linearVelocity.y, 0);
        }
        else
        {
            // Si no se presiona nada, reducimos la velocidad suavemente
            rb.linearVelocity = new Vector3(rb.linearVelocity.x * (1 - airControl), rb.linearVelocity.y, 0);
        }

        float targetVelocityX = moveDirection * horizontalSpeed;
        rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, targetVelocityX, airAcceleration), rb.linearVelocity.y);

        // Determinar caida
        if (rb.linearVelocity.y > 0)
        {
            rb.gravityScale = 1;
            Debug.Log(rb.linearVelocity);
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = 0.4f;
        }




        // ----TESTEO----
        if (Input.GetKeyUp(KeyCode.Space))
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
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
        rb.AddForce(Vector2.up * firstJump, ForceMode2D.Impulse);
    }

    // Aplicar fuerza cuando toca la parte de arriba de un carro
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Car" && canJump)
        {
            Jump();
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Car")
        {
            canJump = true;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reinicia la velocidad en Y para evitar acumulación
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        canJump = false; // Desactiva el salto hasta que realmente haya aterrizado
    }
}
