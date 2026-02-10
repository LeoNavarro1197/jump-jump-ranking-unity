using UnityEngine;

public class ButtonReload : MonoBehaviour
{
    public PlayerControl playerControl;
    public PlayerDestruction playerDestruction;
    public FinalScore finalScore;
    [SerializeField] CarPool carPool;
    public GameObject player, floor, edificio, niebla;
    public Rigidbody2D rbFloor, rbEdificio2, rbNiebla;
    public Transform positionPlayer, positionFloor;
    public GameObject buttonLeft, buttonRight;
    public bool resetBackground = false;

    [SerializeField] Animator animationRedPlanet, animationBluePlanet, animationYellowPlanet, animationBlackHolePlanet;
    [SerializeField] SpriteRenderer[] sprites;
    [SerializeField] public SpriteRenderer spritePlayerDestruction, spritePlayer;

    bool bandera = true;

    public void ButtonReloaded()
    {
        player.SetActive(true);
        player.transform.position = positionPlayer.position;
        floor.transform.position = positionFloor.position;
        edificio.transform.position = new Vector2(positionFloor.position.x, positionFloor.position.y + .7f);
        niebla.transform.position = new Vector2(positionFloor.position.x, positionFloor.position.y + .2f);
        rbFloor.bodyType = RigidbodyType2D.Static; rbEdificio2.bodyType = RigidbodyType2D.Static; rbNiebla.bodyType = RigidbodyType2D.Static;
        finalScore.scoreInt = 0;
        finalScore.scoreText.text = "0";

        playerControl.moveDirection = 0;

        playerControl.CoroutineStart();
        playerDestruction.DEATH = false;

        gameObject.SetActive(false);

        resetBackground = true;

        animationRedPlanet.SetBool("isMovement", false);
        animationBluePlanet.SetBool("isMovement", false);
        animationYellowPlanet.SetBool("isMovement", false);
        animationBlackHolePlanet.SetBool("isMovement", false);
        SpriteToWhite();
    }

    void SpriteToWhite()
    {
        spritePlayer.color = Color.white;
        spritePlayerDestruction.color = Color.white;

        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = Color.white;
        }

        for (int i = 0; i < carPool.carListSprite.Count; i++)
        {
            carPool.carListSprite[i].color = Color.white;
        }
    }

#if UNITY_STANDALONE
    void FixedUpdate()
    {
        if (playerDestruction.DEATH)
        {
            bandera = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && playerDestruction.DEATH == true && bandera)
        {
            bandera = false;
            ButtonReloaded();
        }
    }
#endif
}
