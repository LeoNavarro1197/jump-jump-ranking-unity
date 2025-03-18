using UnityEngine;

public class Parallax : MonoBehaviour
{
    Transform background;
    public PlayerControl playerControl;
    float positionPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        background = GetComponent<Transform>();

        GameObject targetCarControl = GameObject.Find("Player");
        playerControl = targetCarControl.GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        positionPlayer = playerControl.transform.position.y;
        background.position = new Vector2 (0, -positionPlayer);
    }
}
