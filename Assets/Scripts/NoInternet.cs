using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NoInternet : MonoBehaviour
{
    FirebaseLeaderboardManager leaderboardManager;

    public bool isThereInternet = false;

    void Start()
    {
        leaderboardManager = FindFirstObjectByType<FirebaseLeaderboardManager>();
    }

    IEnumerator CheckInternet()
    {
        while (true)
        {
            UnityWebRequest request = new UnityWebRequest("https://www.google.com");
            request.timeout = 5;
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("No hay internet");
                isThereInternet = false;

                int score = PlayerPrefs.GetInt("CurrentScore", 0);
                string username = PlayerPrefs.GetString("Username");

                if (!string.IsNullOrEmpty(username))
                {
                    leaderboardManager.profileUsernameTxt.text = username;
                    leaderboardManager.profileUserscoreTxt.text = score.ToString();
                }

                if (leaderboardManager.spinner.activeSelf)
                {
                    leaderboardManager.spinner.SetActive(false);
                    leaderboardManager.noInternetPanel.SetActive(true);
                }
            }
            else
            {
                Debug.Log("Internet disponible");

                if (!isThereInternet)
                {
                    isThereInternet = true;
                    // [NUEVO CODIGO] Inicializamos y sincronizamos inmediatamente
                    leaderboardManager.FirebaseInicialize();
                    leaderboardManager.ListenForScoreUpdates();
                }
            }
            yield return new WaitForSeconds(10f);
        }
    }

    public void StartCheckInternet()
    {
        StartCoroutine(CheckInternet());
    }
}