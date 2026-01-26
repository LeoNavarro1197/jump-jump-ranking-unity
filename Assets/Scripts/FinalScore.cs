using UnityEngine;
using TMPro;
using System.Collections;

public class FinalScore : MonoBehaviour
{
    public FirebaseLeaderboardManager firebaseLeaderboardManager;
    NoInternet noInternet;

    public PlayerControl playerControl;
    private SoundManager soundManager;
    [SerializeField] ChangeColor changeColor;
    public TMP_Text scoreText;
    public int scoreInt = 0;

    public TMP_Text profileUserscoreTxt;

    public bool isBackgroundRed = false, isBackgroundBlue = false, isBackgroundYellow = false, isBackgroundBlackHole = false, isBackgroundNormal = false;

    public int red = 5, blue = 10, yellow = 15, black = 20, normal = 25;

    private void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        changeColor = FindFirstObjectByType<ChangeColor>();
        noInternet = FindFirstObjectByType<NoInternet>();
    }

    private void FixedUpdate()
    {
        if (playerControl.POINT)
        {
            scoreInt = scoreInt + 1;

            scoreText.text = scoreInt.ToString();
            scoreText.fontSize = 170;
            soundManager.SelectClip(4, 1f);
            Invoke("ResetSizeText", .1f);

            int highscoreLocal = PlayerPrefs.GetInt("CurrentScore", 0);

            if (scoreInt == highscoreLocal)
            {
                soundManager.SelectClip(8, 2.5f);
            }

            if (scoreInt > highscoreLocal)
            {
                profileUserscoreTxt.fontSize = 50;
                PlayerPrefs.SetInt("CurrentScore", scoreInt);
                PlayerPrefs.Save();
                profileUserscoreTxt.text = scoreInt.ToString();

                if (noInternet.isThereInternet)
                {
                    firebaseLeaderboardManager.UpdateUserScore(scoreInt);
                }
            }

            switch (scoreInt)
            {
                case int s when s == red:
                    onApplicationChange();
                    isBackgroundRed = true;
                    break;
                case int s when s == blue:
                    onApplicationChange();
                    isBackgroundBlue = true;
                    break;
                case int s when s == yellow:
                    onApplicationChange();
                    isBackgroundYellow = true;
                    break;
                case int s when s == black:
                    onApplicationChange();
                    isBackgroundBlackHole = true;
                    break;
                case int s when s == normal:
                    onApplicationChange();
                    isBackgroundNormal = true;
                    break;
            }
            playerControl.POINT = false;
        }
    }

    void ResetSizeText()
    {
        scoreText.fontSize = 120;
        profileUserscoreTxt.fontSize = 45;
    }

    void onApplicationChange()
    {
        changeColor.hasStartedLerpBackgroundRed = false; changeColor.hasStartedLerpBackgroundBlue = false; changeColor.hasStartedLerpBackgroundYellow = false; changeColor.hasStartedLerpBackgroundBlackHole = false; changeColor.hasStartedLerpBackgroundNormal = false;
        scoreText.fontSize = 100;
        soundManager.SelectClip(5, 3.5f);
        Invoke("ResetSizeText", .1f);
    }
}