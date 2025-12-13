using UnityEngine;
using TMPro;


public class FinalScore : MonoBehaviour
{
    public FirebaseLeaderboardManager firebaseLeaderboardManager;

    public PlayerControl playerControl;
    private SoundManager soundManager;
    [SerializeField] ChangeColor changeColor;
    public TMP_Text scoreText;
    public int scoreInt = 0;

    public TMP_Text profileUserscoreTxt;

    public bool isBackgroundRed = false, isBackgroundBlue = false;

    private void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        changeColor = FindFirstObjectByType<ChangeColor>();
    }

    private void Update()
    {
        if (playerControl.POINT)
        {
            scoreInt = scoreInt + 1;

            scoreText.text = scoreInt.ToString();
            scoreText.fontSize = 90;
            soundManager.SelectClip(4, 1f);
            Invoke("ResetSizeText", .1f);
            firebaseLeaderboardManager.UpdateUserScore(scoreInt);

            if (scoreInt > firebaseLeaderboardManager.score)
            {
                profileUserscoreTxt.text = scoreInt.ToString();
                PlayerPrefs.SetInt("CurrentScore", scoreInt);
                PlayerPrefs.Save();
            }

            switch (scoreInt)
            {
                case 5:
                    onApplicationChange();
                    isBackgroundRed = true;
                    break;
                /*case 10:
                    onApplicationChange();
                    isBackgroundBlue = true;
                    break;*/
            }
            playerControl.POINT = false;
        }
    }

    void ResetSizeText()
    {
        scoreText.fontSize = 60;
        //scoreText.color = Color.white;
    }

    void onApplicationChange()
    {
        changeColor.hasStartedLerpBackgroundRed = false;
        //scoreText.color = Color.blue;
        scoreText.fontSize = 90;
        soundManager.SelectClip(5, 1.5f);
        Invoke("ResetSizeText", .1f);
    }
}
