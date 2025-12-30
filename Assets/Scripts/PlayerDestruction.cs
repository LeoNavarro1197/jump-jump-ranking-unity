using UnityEngine;

public class PlayerDestruction : MonoBehaviour
{
    public int rows;  // Número de filas en que se divide el sprite
    public int columns; // Número de columnas en que se divide el sprite
    public float explosionForce; // Fuerza con la que se dispersan los fragmentos
    public GameObject fragmentPrefab, explosion; // Prefab base para los fragmentos

    public GameObject buttonReload, buttonLeft, buttonRight;

    public bool DEATH = false;

    public SpriteRenderer spriteRenderer;
    public PlayerControl playerControl;
    private FinalScore finalScore;
    public Animator animationCamera;

    SoundManager soundManager;

    void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        finalScore = FindFirstObjectByType<FinalScore>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void DestroyPlayer()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            Debug.LogError("No se encontró un SpriteRenderer o el sprite es nulo.");
            return;
        }

        if (finalScore.isBackgroundBlackHole)
        {
            finalScore.isBackgroundBlackHole = false;
            spriteRenderer.color = Color.black;
        }

        Texture2D texture = spriteRenderer.sprite.texture;
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        Vector2 pivot = spriteRenderer.sprite.pivot / spriteRenderer.sprite.pixelsPerUnit;

        float widthPerFragment = spriteSize.x / columns;
        float heightPerFragment = spriteSize.y / rows;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector2 fragmentPosition = new Vector2(
                    transform.position.x + (j * widthPerFragment) - (spriteSize.x / 2) + (widthPerFragment / 2),
                    transform.position.y + (i * heightPerFragment) - (spriteSize.y / 2) + (heightPerFragment / 2)
                );

                GameObject fragment = Instantiate(fragmentPrefab, playerControl.transform.position, Quaternion.identity);
                SpriteRenderer fragmentRenderer = fragment.GetComponent<SpriteRenderer>();

                if (fragmentRenderer != null)
                {
                    // Crear un nuevo sprite para cada fragmento
                    Rect spriteRect = new Rect(j * (texture.width / columns), i * (texture.height / rows),
                                               texture.width / columns, texture.height / rows);
                    fragmentRenderer.sprite = Sprite.Create(texture, spriteRect, new Vector2(0.5f, 0.5f), texture.width / spriteSize.x);
                }

                Rigidbody2D rb = fragment.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Vector2 randomForce = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)) * explosionForce;
                    rb.AddForce(randomForce, ForceMode2D.Impulse);
                }
            }
        }

        gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Death" || collision.gameObject.name == "DeathLeft" || collision.gameObject.name == "DeathRigth")
        {
            buttonLeft.SetActive(false);
            buttonRight.SetActive(false);
            buttonReload.SetActive(true);
            Instantiate(explosion, playerControl.transform.position, Quaternion.identity);
            soundManager.SelectClip(0, 1f);
            DestroyPlayer();
            animationCamera.CrossFadeInFixedTime("camara", 0f);
            DEATH = true;
            playerControl.START = false;
            //Invoke("TimeDestruccion", 2);
        }
    }
}
