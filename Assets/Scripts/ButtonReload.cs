using UnityEngine;

public class ButtonReload : MonoBehaviour
{
    public PlayerControl playerControl;
    public PlayerDestruction playerDestruction;
    public FinalScore finalScore;
    public GameObject player, floor;
    public Rigidbody2D rbFloor;
    public Transform positionPlayer, positionFloor;
    public GameObject buttonLeft, buttonRight;
    public bool resetBackground = false;

    public void ButtonReloaded()
    {
        player.SetActive(true);
        player.transform.position = positionPlayer.position;
        floor.transform.position = positionFloor.position;
        rbFloor.bodyType = RigidbodyType2D.Static;
        finalScore.scoreInt = 0;
        finalScore.scoreText.text = "0";

        playerControl.moveDirection = 0;

        buttonLeft.SetActive(true);
        buttonRight.SetActive(true);

        playerControl.CoroutineStart();
        playerDestruction.DEATH = false;

        gameObject.SetActive(false);

        resetBackground = true;
    }
}
