using UnityEngine;
using TMPro;

public class FinalScore : MonoBehaviour
{
    public FirebaseLeaderboardManager firebaseLeaderboardManager;
    public TMP_Text scoreText;
    public int scoreInt = 0;

    public TMP_Text profileUserscoreTxt;

    public void PressButton()
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
    }
}
