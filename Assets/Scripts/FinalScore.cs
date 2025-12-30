using UnityEngine;
using TMPro;

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

    private void Start()
    {
        soundManager = FindFirstObjectByType<SoundManager>();
        changeColor = FindFirstObjectByType<ChangeColor>();
        noInternet = FindFirstObjectByType<NoInternet>();
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

            int highscoreLocal = PlayerPrefs.GetInt("CurrentScore", 0);

            if (scoreInt > highscoreLocal)
            {
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
                case 5:
                    onApplicationChange();
                    isBackgroundRed = true;
                    break;
                case 10:
                    onApplicationChange();
                    isBackgroundBlue = true;
                    break;
                case 15:
                    onApplicationChange();
                    isBackgroundYellow = true;
                    break;
                case 20:
                    onApplicationChange();
                    isBackgroundBlackHole = true;
                    break;
                case 25:
                    onApplicationChange();
                    isBackgroundNormal = true;
                    break;
            }
            playerControl.POINT = false;
        }
    }

    void ResetSizeText()
    {
        scoreText.fontSize = 60;
    }

    void onApplicationChange()
    {
        changeColor.hasStartedLerpBackgroundRed = false;
        changeColor.hasStartedLerpBackgroundBlue = false;
        changeColor.hasStartedLerpBackgroundYellow = false;
        changeColor.hasStartedLerpBackgroundBlackHole = false;
        changeColor.hasStartedLerpBackgroundNormal = false;
        scoreText.fontSize = 90;
        soundManager.SelectClip(5, 1.5f);
        Invoke("ResetSizeText", .1f);
    }
}