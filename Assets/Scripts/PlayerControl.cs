using UnityEngine;
using System.Collections;
using TMPro;
using Unity.VisualScripting;

public class PlayerControl : MonoBehaviour
{
    FirebaseLeaderboardManager firebaseLeaderboardManager;
    public CarPool carPool;
    public CarMovementControl carMovementControl;
    public SoundManager soundManager;
    public ButtonReload buttonReload;
    public PlayerDestruction playerDestruction;
    Tutorial tutorial;

    public float horizontalSpeed = 5f;
    public float airControl = 0.1f;
    public float jumpForce = 10f, firstJump = 9.8f;

    public float moveDirection = 0f;
    private Rigidbody2D rb;
    //private BoxCollider2D boxCollider2D;
    private CapsuleCollider2D boxCollider2D;

    public int countdown;
    public TMP_Text countdownText;
    public GameObject panelCountdown, panelStart, buttonLeft, buttonRight;
    public Rigidbody2D floor, rbEdificio2, rbNiebla, leftRespawn, rightRespawn;
    public Animator animatorNiebla;
    public GameObject floorGameobject, edificio2, niebla;

    public bool START = false;
    public bool POINT = false;
    private bool canJump = true;

    public bool STARTCOROUTINEPC = true;

    private Animator animator;

    public bool isMusicSlow = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        soundManager = FindFirstObjectByType<SoundManager>();
        tutorial = FindFirstObjectByType<Tutorial>();
        firebaseLeaderboardManager = FindFirstObjectByType<FirebaseLeaderboardManager>();
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
            // Si no se presiona nada, la velocidad se reduce suavemente
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
#if UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!firebaseLeaderboardManager.leaderboardPanel.activeSelf && !buttonReload.gameObject.activeSelf)
            {
                if (STARTCOROUTINEPC)
                {
                    STARTCOROUTINEPC = false;
                    Debug.Log("Iniciar Coroutine desde PC");
                    CoroutineStart();
                    soundManager.SelectClip(3, 1f);
                }
            }
        }
#endif
    }

    // Cambiar la dirección del player
    void MoveLeft() { moveDirection = -1; transform.rotation = Quaternion.Euler(0, 0, 0); }
    void MoveRight() { moveDirection = 1; transform.rotation = Quaternion.Euler(0, 180, 0); }
    void StopMoving() { moveDirection = 0; }

#if UNITY_STANDALONE
    private void Update()
    {
        buttonLeft.SetActive(false); buttonRight.SetActive(false);

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            StopMoving();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            StopMoving();
        }
    }
#elif UNITY_ANDROID || UNITY_IOS
    public void MoveLeftMobile() { MoveLeft(); }
    public void MoveRightMobile() { MoveRight(); }
    public void StopMovingMobile() { StopMoving(); }
#endif

    // Boton Start en la UI
    public void CoroutineStart()
    {
        StartCoroutine(StartCountdown());
        soundManager.SelectClip(3, 1f);
    }

    // Funcion para empezar el juego
    public IEnumerator StartCountdown()
    {
        buttonReload.spritePlayerDestruction.color = Color.white;
        panelCountdown.SetActive(true); panelStart.SetActive(false);

        isMusicSlow = true;

        for (int i = countdown; i > 0; i--)
        {
            // Conteo regresivo
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        floorGameobject.SetActive(true);
        edificio2.SetActive(true);
        niebla.SetActive(true);
        countdownText.text = "¡GO!";
        yield return new WaitForSeconds(.5f);

        STARTCOROUTINEPC = false;
        START = true;

        if (PlayerPrefs.GetString("TutorialOneCompleted") == "")
        {
            StartCoroutine(tutorial.StartTutorial());
        }

        soundManager.SelectClip(2, 1f);
        panelCountdown.SetActive(false);
        rb.AddForce(Vector2.up * firstJump, ForceMode2D.Impulse);
        Invoke("FloorGravity", .05f);
        animatorNiebla.enabled = false; boxCollider2D.enabled = false; buttonRight.SetActive(true); buttonLeft.SetActive(true);
        Invoke("DesactiveFloor", .75f);
    }

    void FloorGravity()
    {
        floor.bodyType = RigidbodyType2D.Dynamic; rbEdificio2.bodyType = RigidbodyType2D.Dynamic; rbNiebla.bodyType = RigidbodyType2D.Dynamic;
    }

    void DesactiveFloor()
    {
        floorGameobject.SetActive(false); edificio2.SetActive(false); niebla.SetActive(false);
    }

    // Aplicar fuerza cuando toca la parte de arriba de un carro
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.tag == "Car" && canJump)
        {
            soundManager.SelectClip(1, 1f);
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
            StartCoroutine(PointFalse());
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reiniciar la velocidad en Y para evitar acumulación
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        canJump = false;
    }

    void isGroundFalse()
    {
        animator.SetBool("isGround", false);
        animator.SetBool("isJump", true);
    }

    IEnumerator PointFalse()
    {
        yield return new WaitForSeconds(.05f);
        POINT = false;
    }
}
