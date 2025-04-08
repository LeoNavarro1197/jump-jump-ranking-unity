using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    public CarPool carPool;
    public CarMovementControl carMovementControl;

    public float horizontalSpeed = 5f;
    public float airControl = 0.1f;
    public float jumpForce = 10f, firstJump = 9.8f;

    public float moveDirection = 0f;
    private Rigidbody2D rb;

    public int countdown;
    public TMP_Text countdownText;
    public GameObject panelCountdown, panelStart, buttonLeft, buttonRight;
    public Rigidbody2D floor;

    public bool start = false;
    public bool canJump = true;
    public bool point = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Mover al player (Teclado)
        /*if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
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
        }*/
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

        // Mas control del suavisado con Lerp
        float targetVelocityX = moveDirection * horizontalSpeed;
        rb.linearVelocity = new Vector2(Mathf.Lerp(rb.linearVelocity.x, targetVelocityX, airControl), rb.linearVelocity.y);

        // Determinar caida
        if (rb.linearVelocity.y > 0)
        {
            rb.gravityScale = 1;
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = 0.4f;
        }
    }

    // Cambiar la dirección del player (UI)
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

        for (int i = countdown; i > 0; i--)
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
        floor.bodyType = RigidbodyType2D.Dynamic;
        buttonRight.SetActive(true);
        buttonLeft.SetActive(true);
    }

    // Aplicar fuerza cuando toca la parte de arriba de un carro
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Car" && canJump)
        {
            Jump();
        }

        if(collision.transform.tag == "Car")
        {
            point = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Car")
        {
            canJump = true;
            point = false;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reinicia la velocidad en Y para evitar acumulación
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Fuerza de salto
        canJump = false; // Desactiva el salto hasta que realmente haya aterrizado
    }
}
