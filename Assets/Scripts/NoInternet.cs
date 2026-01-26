using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class NoInternet : MonoBehaviour
{
    FirebaseLeaderboardManager leaderboardManager;
    public bool isThereInternet = false;
    private bool initialized = false; // Para evitar doble suscripción a eventos

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
                if (PlayerPrefs.GetString("Username") == "")
                {
                    isThereInternet = false;
                    yield return new WaitForSeconds(1f);
                    continue; // Si no hay username, no hacemos nada
                }
                
                isThereInternet = false;
                leaderboardManager.rankTxt.gameObject.SetActive(false);
                leaderboardManager.textRankTxt.gameObject.SetActive(false);
                leaderboardManager.noInternetTxt.gameObject.SetActive(true);

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
                leaderboardManager.rankTxt.gameObject.SetActive(true);
                leaderboardManager.textRankTxt.gameObject.SetActive(true);
                leaderboardManager.noInternetTxt.gameObject.SetActive(false);

                if (!isThereInternet)
                {
                    isThereInternet = true;
                    // Solo inicializamos una vez los eventos pesados
                    if (!initialized)
                    {
                        leaderboardManager.spinner.SetActive(true);
                        leaderboardManager.FirebaseInicialize();
                        leaderboardManager.ListenForScoreUpdates();
                        initialized = true;
                    }
                    else
                    {
                        // Si ya estaba inicializado, solo mandamos el score que hicimos offline
                        leaderboardManager.SyncOfflineScore();
                    }
                }
            }
            yield return new WaitForSeconds(5f);
        }
    }

    public void StartCheckInternet()
    {
        StartCoroutine(CheckInternet());
    }
}