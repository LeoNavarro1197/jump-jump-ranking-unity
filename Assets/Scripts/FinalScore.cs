using UnityEngine;
using TMPro;

public class FinalScore : MonoBehaviour
{
    public FirebaseLeaderboardManager firebaseLeaderboardManager;
    public PlayerControl playerControl;
    public TMP_Text scoreText;
    public int scoreInt = 0;

    public TMP_Text profileUserscoreTxt;

    private void Update()
    {
        if (playerControl.POINT)
        {
            scoreInt = scoreInt + 1;

            scoreText.text = scoreInt.ToString();
            firebaseLeaderboardManager.UpdateUserScore(scoreInt);

            if (scoreInt > firebaseLeaderboardManager.score)
            {
                profileUserscoreTxt.text = scoreInt.ToString();
                PlayerPrefs.SetInt("CurrentScore", scoreInt);
                PlayerPrefs.Save();
            }
            playerControl.POINT = false;
        }
    }
}
