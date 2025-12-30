using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class FirebaseLeaderboardManager : MonoBehaviour
{
    // ... (Variables existentes se mantienen igual)
    public GameObject loadPanel, soundManagerObject, usernamePanel, userprofilePanel, leaderboardPanel, optionsPanel, bloqueadorPanel, startPanel, spinner, leadreboardContent, noInternetPanel, userDataPrefab, buttonLeft, buttonRight, buttonReload;
    public TMP_Text profileUsernameTxt, profileUserscoreTxt, errorUsernameTxt, rankTxt;
    public TMP_InputField usernameInput;

    public int score, totalUsers = 0;
    public string username = "";

    public FinalScore finalScore;
    public TMP_Text scoreTextInGame;
    NoInternet noInternet;

    private DatabaseReference db;
    private SoundManager soundManager;

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        soundManager = FindFirstObjectByType<SoundManager>();
        noInternet = FindFirstObjectByType<NoInternet>();

        Invoke("LoadingSession", 2f);
    }

    void LoadingSession()
    {
        loadPanel.SetActive(false);
        soundManagerObject.SetActive(true);

        noInternet.StartCheckInternet();

        if (PlayerPrefs.GetString("Username") != "")
        {
            usernamePanel.SetActive(false);
            spinner.SetActive(true);
        }
    }

    public void NoInternetPanel()
    {
        userprofilePanel.SetActive(true);
        noInternetPanel.SetActive(false);
    }

    public void ShowLeaderboard()
    {
        buttonLeft.SetActive(false);
        buttonRight.SetActive(false);

        soundManager.SelectClip(6, .5f);

        if (noInternet.isThereInternet)
        {
            StartCoroutine(FetchLeaderBoardData());
        }
        else if (!noInternet.isThereInternet)
        {
            leaderboardPanel.SetActive(true);
        }
    }

    public void SignInWithUsername()
    {
        soundManager.SelectClip(3, 1f);
        if (!noInternet.isThereInternet)
        {
            errorUsernameTxt.text = "You are not connected to the internet";
        }
        else
        {
            StartCoroutine(CheckUserExistInDatabase());
        }    
    }

    public void CloseLeaderboard()
    {
        buttonLeft.SetActive(true);
        buttonRight.SetActive(true);

        if (leadreboardContent.transform.childCount > 0)
        {
            for (int i = 0; i < leadreboardContent.transform.childCount; i++)
            {
                Destroy(leadreboardContent.transform.GetChild(i).gameObject);
            }
        }

        leaderboardPanel.SetActive(false);
        userprofilePanel.SetActive(true);
        startPanel.SetActive(false);
        buttonLeft.SetActive(true);
        buttonRight.SetActive(true);
        buttonReload.SetActive(true);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        bloqueadorPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        bloqueadorPanel.SetActive(false);
    }

    public void SignOut()
    {
        PlayerPrefs.DeleteKey("PlayerID");
        PlayerPrefs.DeleteKey("Username");
        PlayerPrefs.DeleteKey("CurrentScore");

        usernameInput.text = "";
        profileUsernameTxt.text = "";
        profileUserscoreTxt.text = "";
        score = 0;
        username = "";
        usernamePanel.SetActive(true);
        userprofilePanel.SetActive(false);
        finalScore.scoreInt = 0;
        scoreTextInGame.text = "";
    }

    public void FirebaseInicialize()
    {
        db = FirebaseDatabase.DefaultInstance.GetReference("/Leaderboard/");
        db.ChildAdded += HandleChildAdded;
        GetTotalUsers();

        int playerID = PlayerPrefs.GetInt("PlayerID");
        if (playerID != 0)
        {
            db.Child("User_" + playerID.ToString()).Child("score").ValueChanged += HandleScoreChanged;
        }

        StartCoroutine(FetchUserProfileData(playerID));

        SyncOfflineScore();
        noInternetPanel.SetActive(false);
    }

    // [NUEVO CODIGO] Modificado para actualizar la UI apenas termine la subida
    public void SyncOfflineScore()
    {
        int localHighScore = PlayerPrefs.GetInt("CurrentScore", 0);
        string currentUsername = PlayerPrefs.GetString("Username");

        if (string.IsNullOrEmpty(currentUsername) || localHighScore <= 0) return;

        db.OrderByChild("username").EqualTo(currentUsername).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        int cloudScore = int.Parse(childSnapshot.Child("score").Value.ToString());
                        if (localHighScore > cloudScore)
                        {
                            db.Child(childSnapshot.Key).Child("score").SetValueAsync(localHighScore).ContinueWith(updateTask => {
                                if (updateTask.IsCompleted)
                                {
                                    Debug.Log("Sincronización Exitosa: Puntaje offline subido a Firebase.");
                                    // [NUEVO CODIGO] Actualizamos la variable score local para que coincida
                                    score = localHighScore;
                                }
                            });
                        }
                    }
                }
            }
        });
    }

    void HandleScoreChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        if (args.Snapshot.Exists && args.Snapshot.Value != null)
        {
            int cloudScore = int.Parse(args.Snapshot.Value.ToString());

            // [NUEVO CODIGO] Solo actualizamos la UI si lo que viene de la nube es mayor o igual a lo que tenemos
            if (cloudScore >= PlayerPrefs.GetInt("CurrentScore", 0))
            {
                score = cloudScore;
                profileUserscoreTxt.text = score.ToString();
                PlayerPrefs.SetInt("CurrentScore", score);
                PlayerPrefs.Save();
            }
        }
    }

    public void ListenForScoreUpdates()
    {
        string username = PlayerPrefs.GetString("Username");

        if (string.IsNullOrEmpty(username)) return;

        db.OrderByChild("username").EqualTo(username).ValueChanged += (object sender, ValueChangedEventArgs args) =>
        {
            if (args.DatabaseError != null) return;

            foreach (DataSnapshot childSnapshot in args.Snapshot.Children)
            {
                if (childSnapshot.Exists)
                {
                    int currentScore = int.Parse(childSnapshot.Child("score").Value.ToString());
                }
            }
        };
    }


    public void UpdateUserScore(int newScore)
    {
        string username = PlayerPrefs.GetString("Username");
        if (string.IsNullOrEmpty(username)) return;

        db.OrderByChild("username").EqualTo(username).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        string userKey = childSnapshot.Key;
                        int currentScore = int.Parse(childSnapshot.Child("score").Value.ToString());

                        if (newScore > currentScore)
                        {
                            db.Child(userKey).Child("score").SetValueAsync(newScore);
                        }
                    }
                }
            }
        });
    }


    void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null) return;
        GetTotalUsers();
    }

    void GetTotalUsers()
    {
        db.ValueChanged += (object sender2, ValueChangedEventArgs e2) =>
        {
            if (e2.DatabaseError != null) return;
            totalUsers = int.Parse(e2.Snapshot.ChildrenCount.ToString());
        };
    }

    IEnumerator CheckUserExistInDatabase()
    {
        var task = db.OrderByChild("username").EqualTo(usernameInput.text).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompleted && !task.IsFaulted)
        {
            DataSnapshot snapshot = task.Result;

            if (snapshot != null && snapshot.HasChildren)
            {
                errorUsernameTxt.text = "This username already exists";
            }
            else
            {
                string input = usernameInput.text;
                if (usernameInput.text.Length > 12 || usernameInput.text.Length < 3)
                {
                    errorUsernameTxt.text = "Your username must be between 3 and 12 characters";
                }
                else if (!Regex.IsMatch(input, "^[a-zA-Z0-9_]*$"))
                {
                    errorUsernameTxt.text = "Only letters, numbers and underscore (_) allowed";
                }
                else
                {
                    errorUsernameTxt.text = "";
                    PushUserData();
                    PlayerPrefs.SetInt("PlayerID", totalUsers + 1);
                    PlayerPrefs.SetString("Username", usernameInput.text);
                    PlayerPrefs.SetInt("CurrentScore", 0);

                    StartCoroutine(delayFetchProfile());
                }
            }
        }
    }

    IEnumerator delayFetchProfile()
    {
        spinner.SetActive(true);
        yield return new WaitForSeconds(1f);
        StartCoroutine(FetchUserProfileData(totalUsers));
    }

    void PushUserData()
    {
        db.Child("User_" + (totalUsers).ToString()).Child("username").SetValueAsync(usernameInput.text);
        db.Child("User_" + (totalUsers).ToString()).Child("score").SetValueAsync(0);
    }

    IEnumerator FetchUserProfileData(int playerID)
    {
        playerID -= 1;
        if (playerID != 0)
        {
            var task = db.Child("User_" + playerID.ToString()).GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.HasChildren)
                {
                    username = snapshot.Child("username").Value.ToString();

                    // [NUEVO CODIGO] PRIORIDAD LOCAL:
                    // Si el puntaje local de PlayerPrefs es mayor al de la nube, usamos el local
                    int cloudScore = int.Parse(snapshot.Child("score").Value.ToString());
                    int localScore = PlayerPrefs.GetInt("CurrentScore", 0);

                    if (localScore > cloudScore)
                    {
                        score = localScore;
                        Debug.Log("Usando Score Local temporalmente mientras se sincroniza...");
                    }
                    else
                    {
                        score = cloudScore;
                        PlayerPrefs.SetInt("CurrentScore", score);
                        PlayerPrefs.Save();
                    }

                    spinner.SetActive(false);
                    profileUsernameTxt.text = username;
                    profileUserscoreTxt.text = "" + score;
                    userprofilePanel.SetActive(true);
                    buttonLeft.SetActive(false);
                    buttonRight.SetActive(false);
                    usernamePanel.SetActive(false);
                }
            }
        }
    }

    IEnumerator FetchLeaderBoardData()
    {
        var task = db.OrderByChild("score").LimitToLast(100).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompleted && !task.IsFaulted)
        {
            DataSnapshot snapshot = task.Result;
            List<LeaderboardData> listLeaderboardEntry = new List<LeaderboardData>();

            foreach (DataSnapshot childSnapShot in snapshot.Children)
            {
                string username2 = childSnapShot.Child("username").Value.ToString();
                int score = int.Parse(childSnapShot.Child("score").Value.ToString());
                listLeaderboardEntry.Add(new LeaderboardData(username2, score));
            }
            DisplayLeaderboardData(listLeaderboardEntry);
        }
    }

    void DisplayLeaderboardData(List<LeaderboardData> leaderboardData)
    {
        int rankCount = 0;
        for (int i = leaderboardData.Count - 1; i >= 0; i--)
        {
            rankCount = rankCount + 1;
            GameObject obj = Instantiate(userDataPrefab);
            obj.transform.parent = leadreboardContent.transform;
            obj.transform.localScale = Vector3.one;

            obj.GetComponent<UserDataUI>().userRankTxt.text = "Rank " + rankCount;
            obj.GetComponent<UserDataUI>().usernameTxt.text = "" + leaderboardData[i].username;
            obj.GetComponent<UserDataUI>().userScoreTxt.text = "" + leaderboardData[i].score;

            if (leaderboardData[i].username == PlayerPrefs.GetString("Username"))
            {
                rankTxt.text = rankCount.ToString();
            }
        }
        leaderboardPanel.SetActive(true);
        userprofilePanel.SetActive(false);
        buttonLeft.SetActive(false);
        buttonRight.SetActive(false);
    }
}

public class LeaderboardData
{
    public string username;
    public int score;

    public LeaderboardData(string username, int score)
    {
        this.username = username;
        this.score = score;
    }
}