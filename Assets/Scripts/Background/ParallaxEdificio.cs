using UnityEngine;

public class ParallaxEdificio : MonoBehaviour
{
    Rigidbody2D rb;
    public PlayerControl playerControl;
    public PlayerDestruction playerDestruction;
    public float speedBackground;

    public ButtonReload buttonReload;
    public Vector3 principlePosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject targetCarControl = GameObject.Find("Player");
        playerControl = targetCarControl.GetComponent<PlayerControl>();

        GameObject nombrePlayerDestruction = GameObject.Find("Player");
        playerDestruction = nombrePlayerDestruction.GetComponent<PlayerDestruction>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerControl.START && !playerDestruction.DEATH)
        {
            // Seguir la direccion del player mas una pequeña velocidad para dar la sensacion de subida del player
            float invertedVelocity = -playerControl.GetComponent<Rigidbody2D>().linearVelocity.y;
            rb.linearVelocity = new Vector2(0, invertedVelocity * 0.1f - speedBackground);
        }
        else if (playerDestruction.DEATH)
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
    }

    private void Update()
    {
        if (buttonReload.resetBackground)
        {
            this.transform.position = principlePosition;
            buttonReload.resetBackground = false;
        }
    }
}
