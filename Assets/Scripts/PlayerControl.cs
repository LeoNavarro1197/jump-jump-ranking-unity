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
    private BoxCollider2D boxCollider2D;

    public int countdown;
    public TMP_Text countdownText;
    public GameObject panelCountdown, panelStart, buttonLeft, buttonRight;
    public Rigidbody2D floor, rbEdificio2, rbNiebla;
    public Animator animatorNiebla;
    public GameObject floorGameobject, edificio2, niebla;

    public bool START = false;
    public bool POINT = false;
    private bool canJump = true;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
            animator.SetBool("isJump", true);
            animator.SetBool("isFall", false);
        }
        else if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = 0.4f;
            animator.SetBool("isFall", true);
            animator.SetBool("isJump", false);
        }

        if(boxCollider2D.enabled == false)
        {
            if (rb.linearVelocity.y < 0)
            {
                boxCollider2D.enabled = true;
            }
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

        for (int i = countdown; i > 0; i--)
        {
            // Contéo regresivo
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        floorGameobject.SetActive(true);
        edificio2.SetActive(true);
        niebla.SetActive(true);
        countdownText.text = "¡GO!";
        yield return new WaitForSeconds(.5f);

        START = true;
        panelCountdown.SetActive(false);
        rb.AddForce(Vector2.up * firstJump, ForceMode2D.Impulse);
        floor.bodyType = RigidbodyType2D.Dynamic; rbEdificio2.bodyType = RigidbodyType2D.Dynamic; rbNiebla.bodyType = RigidbodyType2D.Dynamic;
        animatorNiebla.enabled = false;
        boxCollider2D.enabled = false;
        buttonRight.SetActive(true);
        buttonLeft.SetActive(true);
        Invoke("DesactiveFloor", 2);
    }

    void DesactiveFloor()
    {
        floorGameobject.SetActive(false);
        edificio2.SetActive(false);
        niebla.SetActive(false);
    }

    // Aplicar fuerza cuando toca la parte de arriba de un carro
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Car" && canJump)
        {
            animator.SetBool("isGround", true);
            Invoke("isGroundFalse", .1f);
            Jump();
            
        }

        if(collision.transform.tag == "Car")
        {
            POINT = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Car")
        {
            canJump = true;
            POINT = false;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reinicia la velocidad en Y para evitar acumulación
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Fuerza de salto
        canJump = false; // Desactiva el salto hasta que realmente haya aterrizado
    }

    void isGroundFalse()
    {
        animator.SetBool("isGround", false);
        animator.SetBool("isJump", true);
    }
}
