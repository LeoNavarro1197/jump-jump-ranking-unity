using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEngine.GraphicsBuffer;

public class CarControl : MonoBehaviour
{
    public PlayerControl playerControl;
    public CarMovementControl carMovementControl;
    public CarPool carPool;

    Rigidbody2D rb;
    public int moveDirectionHorizontal;
    public float speedHorizontal;

    public Transform player; // Referencia al personaje
    public float offsetY = 0f; // Desplazamiento vertical opcional

    private GameObject assignedPoint; // Punto asignado al auto
    private float initialPlayerY; // Posición Y inicial del player
    private float initialCarY; // Posición Y inicial del auto
    private bool hasInitialized = false; // Evitar cálculos antes de estar listo

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject targetCarPool = GameObject.Find("CarPool");
        carPool = targetCarPool.GetComponent<CarPool>();

        GameObject targetPlayer = GameObject.Find("Player");
        player = targetPlayer.GetComponent<Transform>();

        GameObject nombrePlayer = GameObject.Find("Player");
        playerControl = nombrePlayer.GetComponent<PlayerControl>();

        GameObject nombreCar = GameObject.Find("CarMovementControl");
        carMovementControl = nombreCar.GetComponent<CarMovementControl>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
        banderaA = true;
        banderaB = true;
        moveDirectionHorizontal = 0;

        // Asignar un punto de la lista cuando el auto se activa
        if (carPool.pointList != null && carPool.pointList.Count > 0)
        {
            assignedPoint = carPool.pointList[Random.Range(0, carPool.pointList.Count)];

            // Asignar posición EXACTA del punto en el momento de activarse
            transform.position = assignedPoint.transform.position;

            // Guardar la posición inicial del auto y del jugador
            initialCarY = transform.position.y;
            initialPlayerY = player.position.y;

            // Indicar que todo está listo para calcular en Update()
            hasInitialized = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasInitialized || player == null || assignedPoint == null)
            return;

        // Mantener el auto en la misma posición X y Z, solo modificando la Y
        float inverseY = initialCarY - (player.position.y - initialPlayerY);
        transform.position = new Vector3(transform.position.x, inverseY + offsetY, transform.position.z);
    }

    void FixedUpdate()
    {
        if (playerControl.start)
        {
            rb.linearVelocity = new Vector2(moveDirectionHorizontal * speedHorizontal, 0f);
        }
    }

    bool banderaA = true;
    bool banderaB = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "SenseLocationA" && banderaA)
        {
            speedHorizontal = Random.Range(carMovementControl.speedHorizontalMinimo, carMovementControl.speedHorizontalMaximo);  // Obtener un valor Random para el moviento en horizontal de los carros
            moveDirectionHorizontal = 1;
            banderaA = false;
        }
        else if(collision.transform.tag == "SenseLocationB" && banderaB)
        {
            speedHorizontal = Random.Range(carMovementControl.speedHorizontalMinimo, carMovementControl.speedHorizontalMaximo);  // Obtener un valor Random para el moviento en horizontal de los carros
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
