using UnityEngine;

public class Parallax : MonoBehaviour
{
    Rigidbody2D rb;
    public PlayerControl playerControl;
    public PlayerDestruction playerDestruction;
    public float speedBackground;
    SpriteRenderer sr;

    public GameObject back;

    private float backgroundHeight;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D> ();

        GameObject targetCarControl = GameObject.Find("Player");
        playerControl = targetCarControl.GetComponent<PlayerControl>();

        GameObject nombrePlayerDestruction = GameObject.Find("Player");
        playerDestruction = nombrePlayerDestruction.GetComponent<PlayerDestruction>();

        sr = GetComponent<SpriteRenderer>();

        float screenHeight = Camera.main.orthographicSize * 2;
        float screenWidth = screenHeight * Screen.width / Screen.height;

        Vector2 spriteSize = sr.sprite.bounds.size;
        Vector3 scale = transform.localScale;

        // Calculamos el factor de escala para mantener la proporción
        float scaleFactor = Mathf.Max(screenWidth / spriteSize.x, screenHeight / spriteSize.y);

        transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);

        backgroundHeight = sr.bounds.size.y; // Altura del fondo en unidades del mundo

        // Posicionar el fondo actual en el centro de la pantalla
        transform.position = new Vector2(0, Camera.main.transform.position.y);

        // Posicionamos el segundo fondo encima del primero
        back.transform.position = new Vector2(transform.position.x, transform.position.y + backgroundHeight);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerControl.start && !playerDestruction.DEATH)
        {
            // Seguir la direccion del player mas una pequeña velocidad para dar la sensacion de subida del player
            float invertedVelocity = -playerControl.GetComponent<Rigidbody2D>().linearVelocity.y;
            rb.linearVelocity = new Vector2(0, invertedVelocity - speedBackground);
        }
        else if (playerDestruction.DEATH)
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
        
        // Si el fondo sale de la pantalla, lo movemos arriba del otro
        if (transform.position.y < -backgroundHeight)
        {
            RepositionBackground();
        }
    }

    void RepositionBackground()
    {
        // Encuentra cuál fondo está más arriba y mueve el actual arriba de ese
        GameObject higherBackground = (transform.position.y > back.transform.position.y) ? this.gameObject : back;
        GameObject lowerBackground = (higherBackground == this.gameObject) ? back : this.gameObject;

        lowerBackground.transform.position = new Vector2(lowerBackground.transform.position.x, higherBackground.transform.position.y + backgroundHeight);
    }
}
