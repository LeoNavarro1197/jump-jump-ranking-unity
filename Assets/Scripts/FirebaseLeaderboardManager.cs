using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FirebaseLeaderboardManager : MonoBehaviour
{
    public GameObject player, loadPanel, soundManagerObject, usernamePanel, userprofilePanel, leaderboardPanel, optionsPanel, creditsPanel, bloqueadorPanel, startPanel, spinner, 
        leadreboardContent, noInternetPanel, userDataPrefab, buttonLeft, buttonRight, buttonReload;
    public TMP_Text profileUsernameTxt, profileUserscoreTxt, errorUsernameTxt, rankTxt, textRankTxt, noInternetTxt;
    public TMP_InputField usernameInput;

    public int score, totalUsers = 0;
    public string username = "";

    PlayerControl playerControl;
    VolumeSettings volumeSettings;
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
        playerControl = FindFirstObjectByType<PlayerControl>();
        volumeSettings = FindFirstObjectByType<VolumeSettings>();

        Invoke("LoadingSession", 2f);
    }

    void LoadingSession()
    {
        noInternet.StartCheckInternet();

        if (PlayerPrefs.GetString("Username") != "")
        {
            usernamePanel.SetActive(false);
            spinner.SetActive(true);
        }
        else
        {
            usernamePanel.SetActive(true);
        }
    }

    public void NoInternetPanel()
    {
        userprofilePanel.SetActive(true);
        noInternetPanel.SetActive(false);

        soundManager.SelectClip(3, 1f);
    }

    public void ShowLeaderboard()
    {
        if (playerControl.START)
        {
            Time.timeScale = 0f;
            volumeSettings.clipMusicUp.mute = true;
            volumeSettings.clipMusicUpBypass.mute = false;
        }
        
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

    public void CloseLeaderboard()
    {
        Time.timeScale = 1f;
        soundManager.SelectClip(7, .5f);

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

        if (!playerControl.START)
        {
            buttonReload.SetActive(true);
        }
        else
        {
            volumeSettings.clipMusicUp.mute = false;
            volumeSettings.clipMusicUpBypass.mute = true;
        }
    }

#if UNITY_STANDALONE
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!leaderboardPanel.activeSelf)
            {
                ShowLeaderboard();
            }
            else if (leaderboardPanel.activeSelf)
            {
                CloseLeaderboard();
            }
        }
    }
 #endif

    public void SignInWithUsername()
    {
        soundManager.SelectClip(3, 1f);

        if (!noInternet.isThereInternet)
        {
            errorUsernameTxt.text = "Check your internet connection.";
        }
        else
        {
            StartCoroutine(CheckUserExistInDatabase());
        }
    }

    public void OpenOptions()
    {
        soundManager.SelectClip(6, .5f);
        optionsPanel.SetActive(true);
        bloqueadorPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        soundManager.SelectClip(6, .5f);
        creditsPanel.SetActive(true);
        bloqueadorPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        soundManager.SelectClip(7, .5f);
        optionsPanel.SetActive(false);
        creditsPanel.SetActive(false);
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

        int playerID = PlayerPrefs.GetInt("PlayerID", -1); // Default a -1 para evitar conflictos con User_0
        if (playerID != -1)
        {
            db.Child("User_" + playerID.ToString()).Child("score").ValueChanged += HandleScoreChanged;
            StartCoroutine(FetchUserProfileData(playerID)); // Mover aquí para asegurar orden
        }

        SyncOfflineScore();
        noInternetPanel.SetActive(false);
    }

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

            // Solo actualizamos si la nube es mayor al récord local guardado
            if (cloudScore > PlayerPrefs.GetInt("CurrentScore", 0))
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
            totalUsers = (int)e2.Snapshot.ChildrenCount; // Casteo directo
        };
    }

    IEnumerator CheckUserExistInDatabase()
    {
        var task = db.OrderByChild("username").EqualTo(usernameInput.text).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompleted && !task.IsFaulted)
        {
            DataSnapshot snapshot = task.Result;

            if (!noInternet.isThereInternet)
            {
                //Check your internet connection.
                //errorUsernameTxt.text = "Check your internet connection.";
            }
            else
            {
                string input = usernameInput.text;
                if (snapshot != null && snapshot.HasChildren)
                {
                    errorUsernameTxt.text = "This username already exists";
                }
                else if (usernameInput.text.Length > 13 || usernameInput.text.Length < 3)
                {
                    errorUsernameTxt.text = "Your username must be between 3 and 13 characters";
                }
                else if (!Regex.IsMatch(input, "^[a-zA-Z0-9_]*$"))
                {
                    errorUsernameTxt.text = "Only letters, numbers and underscore (_) allowed";
                }
                else
                {
                    errorUsernameTxt.text = "";
                    int targetID = totalUsers; // Guardar ID actual
                    PushUserData(targetID);
                    PlayerPrefs.SetInt("PlayerID", targetID);
                    PlayerPrefs.SetString("Username", usernameInput.text);
                    PlayerPrefs.SetInt("CurrentScore", 0);

                    usernamePanel.SetActive(false);
                    spinner.SetActive(true);

                    StartCoroutine(delayFetchProfile(targetID));
                }
            }
        }
    }

    IEnumerator delayFetchProfile(int id)
    {
        //spinner.SetActive(true);
        yield return new WaitForSeconds(1f);
        StartCoroutine(FetchUserProfileData(id));
    }

    void PushUserData(int id)
    {
        db.Child("User_" + id.ToString()).Child("username").SetValueAsync(usernameInput.text);
        db.Child("User_" + id.ToString()).Child("score").SetValueAsync(0);
    }

    IEnumerator FetchUserProfileData(int playerID)
    {
        // Ya no resto 1 para evitar errores. Usamos el ID directo.
        if (playerID != -1)
        {
            var task = db.Child("User_" + playerID.ToString()).GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsCompleted && !task.IsFaulted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot != null && snapshot.HasChildren)
                {
                    username = snapshot.Child("username").Value.ToString();

                    int cloudScore = int.Parse(snapshot.Child("score").Value.ToString());
                    int localScore = PlayerPrefs.GetInt("CurrentScore", 0);

                    // PRIORIDAD LOCAL:
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
                    //buttonLeft.SetActive(false);
                    //buttonRight.SetActive(false);
                    usernamePanel.SetActive(false);
                    noInternetPanel.SetActive(false);
                }
            }
        }
    }

    IEnumerator FetchLeaderBoardData()
    {
        var task = db.OrderByChild("score").LimitToLast(totalUsers).GetValueAsync();
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
            if (rankCount <= 100)
            {
                GameObject obj = Instantiate(userDataPrefab);
                obj.transform.parent = leadreboardContent.transform;
                obj.transform.localScale = Vector3.one;

                obj.GetComponent<UserDataUI>().userRankTxt.text = "Rank " + rankCount;
                obj.GetComponent<UserDataUI>().usernameTxt.text = "" + leaderboardData[i].username;
                obj.GetComponent<UserDataUI>().userScoreTxt.text = "" + leaderboardData[i].score;
            }
            

            if (leaderboardData[i].username == PlayerPrefs.GetString("Username"))
            {
                rankTxt.text = rankCount.ToString();
            }
        }
        leaderboardPanel.SetActive(true);
        userprofilePanel.SetActive(false);
        //buttonLeft.SetActive(false);
        //buttonRight.SetActive(false);
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