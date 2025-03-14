using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Firebase.Database;
using TMPro;
using System.Collections.Generic;

public class FirebaseLeaderboardManager : MonoBehaviour
{

    /*
     * 1- inicializar firebase database
     * 2- obtener total de usuarios
     * 3- mirar usuarios existentes en la database
     * 4- recuperar datos de perfil de usuario
     * 5- recuperar la IU de la tabla de clasificación
     * 6- mostrar la tabla de clasificación en la UI
     * 7- add UI events SignIn, SignOut and close Leaderboard button Events
     * 8- make sure in assets folder you must have streaming asset folder if not you have then close and open the project again
     *    if still streaming folder not there then create new folder name it as streamingassets folder and put the google-services
     *    file there so rigth now i have no streamingassets
     */

    public GameObject usernamePanel, userprofilePanel, leaderboardPanel, startPanel, leadreboardContent, userDataPrefab;
    public TMP_Text profileUsernameTxt, profileUserscoreTxt, errorUsernameTxt;
    public TMP_InputField usernameInput;

    public int score, totalUsers = 0;
    public string username = "";

    public FinalScore finalScore;
    public TMP_Text scoreTextInGame;

    // Firebase Database Type to get database reference
    // Tipo de base de datos de Firebase para obtener la referencia de la base de datos
    private DatabaseReference db;

    void Start()
    {
        FirebaseInicialize();
        ListenForScoreUpdates();
    }

    void Update()
    {

    }

    public void ShowLeaderboard()
    {
        StartCoroutine(FetchLeaderBoardData());
    }

    public void SignInWithUsername()
    {
        StartCoroutine(CheckUserExistInDatabase());
    }

    public void CloseLeaderboard()
    {
        if (leadreboardContent.transform.childCount > 0)
        {
            for (int i = 0; i < leadreboardContent.transform.childCount; i++)
            {
                Destroy(leadreboardContent.transform.GetChild(i).gameObject);
            }
        }

        leaderboardPanel.SetActive(false);
        userprofilePanel.SetActive(true);
        startPanel.SetActive(true);
    }

    public void SignOut()
    {
        PlayerPrefs.DeleteKey("PlayerID");
        PlayerPrefs.DeleteKey("Username");

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

    void FirebaseInicialize()
    {
        db = FirebaseDatabase.DefaultInstance.GetReference("/Leaderboard/");

        // Need to create firebase child added function which check if new user added or not
        // Es necesario crear una función agregada secundaria de Firebase que verifique si se agregó un nuevo usuario o no
        db.ChildAdded += HandleChildAdded;

        // now fetch total users count
        // ahora recupera el recuento total de usuarios
        GetTotalUsers();

        int playerID = PlayerPrefs.GetInt("PlayerID");
        if (playerID != 0)
        {
            db.Child("User_" + playerID.ToString()).Child("score").ValueChanged += HandleScoreChanged;
        }

        StartCoroutine(FetchUserProfileData(playerID));
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
            score = int.Parse(args.Snapshot.Value.ToString());
            profileUserscoreTxt.text = score.ToString();
            //Debug.Log("Puntaje actualizado en tiempo real: " + score);
        }
    }

    public void ListenForScoreUpdates()
    {
        string username = PlayerPrefs.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("No username found in PlayerPrefs");
            return;
        }

        db.OrderByChild("username").EqualTo(username).ValueChanged += (object sender, ValueChangedEventArgs args) =>
        {
            if (args.DatabaseError != null)
            {
                Debug.LogError("Error al escuchar cambios en Firebase: " + args.DatabaseError.Message);
                return;
            }

            foreach (DataSnapshot childSnapshot in args.Snapshot.Children)
            {
                if (childSnapshot.Exists)
                {
                    int currentScore = int.Parse(childSnapshot.Child("score").Value.ToString());
                    //Debug.Log($"Puntaje en Firebase: {currentScore}");

                    // Guardar el puntaje actual localmente
                    //PlayerPrefs.SetInt("CurrentScore", currentScore);
                    //PlayerPrefs.Save();
                }
            }
        };
    }


    public void UpdateUserScore(int newScore)
    {
        string username = PlayerPrefs.GetString("Username");

        if (string.IsNullOrEmpty(username))
        {
            Debug.LogError("No username found in PlayerPrefs");
            return;
        }

        db.OrderByChild("username").EqualTo(username).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al obtener datos del usuario.");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    foreach (DataSnapshot childSnapshot in snapshot.Children)
                    {
                        string userKey = childSnapshot.Key;
                        int currentScore = int.Parse(childSnapshot.Child("score").Value.ToString());

                        // Comparar el puntaje antes de actualizar
                        if (newScore > currentScore)
                        {
                            db.Child(userKey).Child("score").SetValueAsync(newScore);
                            //Debug.Log($" Puntaje actualizado para {username}: {newScore}");
                        }
                        else
                        {
                            //Debug.Log($" El nuevo puntaje ({newScore}) no es mayor al actual ({currentScore}). No se actualizará.");
                        }
                    }
                }
                else
                {
                    Debug.LogError("Usuario no encontrado en la base de datos.");
                }
            }
        });
    }


    void HandleChildAdded(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            return;
        }

        // if new child added then we need to fetch total users numbers in database
        // Si se agrega un nuevo hijo, entonces debemos recuperar el número total de usuarios en la base de datos.
        GetTotalUsers();
    }

    void GetTotalUsers()
    {
        // get total users from firebase database
        db.ValueChanged += (object sender2, ValueChangedEventArgs e2) =>
        {
            if (e2.DatabaseError != null)
            {
                Debug.LogError(e2.DatabaseError.Message);
                return;
            }

            totalUsers = int.Parse(e2.Snapshot.ChildrenCount.ToString());
            //Debug.Log(totalUsers);
        };
    }

    IEnumerator CheckUserExistInDatabase()
    {
        var task = db.OrderByChild("username").EqualTo(usernameInput.text).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("invalid error");
            errorUsernameTxt.text = "invalid error";
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;

            if (snapshot != null && snapshot.HasChildren)
            {
                //Debug.Log("username exist");
                errorUsernameTxt.text = "Este nombre de usuario ya existe";
            }
            else
            {
                Debug.Log("username not exist");

                // push new user data
                // set playerPrefs user ID and username for login purpose
                // show userProfile 

                PushUserData();
                PlayerPrefs.SetInt("PlayerID", totalUsers + 1);
                PlayerPrefs.SetString("Username", usernameInput.text);

                StartCoroutine(delayFetchProfile());

            }
        }
    }

    IEnumerator delayFetchProfile()
    {
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
        //Debug.Log("player id: " + playerID);
        if (playerID != 0)
        {
            var task = db.Child("User_" + playerID.ToString()).GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError("invalid error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                if (snapshot != null && snapshot.HasChildren)
                {
                    // here we fetch all user data from database and put in variables and texts
                    username = snapshot.Child("username").Value.ToString();
                    score = int.Parse(snapshot.Child("score").Value.ToString());

                    profileUsernameTxt.text = username;
                    profileUserscoreTxt.text = "" + score;
                    userprofilePanel.SetActive(true);
                    usernamePanel.SetActive(false);
                }
                else
                {
                    Debug.LogError("user id not exist");
                }
            }
        }
    }

    IEnumerator FetchLeaderBoardData()
    {
        var task = db.OrderByChild("score").LimitToLast(100).GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("invalid error");
        }
        else if (task.IsCompleted)
        {
            DataSnapshot snapshot = task.Result;

            List<LeaderboardData> listLeaderboardEntry = new List<LeaderboardData>();

            foreach (DataSnapshot childSnapShot in snapshot.Children)
            {
                string username2 = childSnapShot.Child("username").Value.ToString();
                int score = int.Parse(childSnapShot.Child("score").Value.ToString());

                //Debug.Log(username2 + "||" + score);

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
            // spawn user leaderboard data ui

            GameObject obj = Instantiate(userDataPrefab);
            obj.transform.parent = leadreboardContent.transform;
            obj.transform.localScale = Vector3.one;

            obj.GetComponent<UserDataUI>().userRankTxt.text = "Rango " + rankCount;
            obj.GetComponent<UserDataUI>().usernameTxt.text = "" + leaderboardData[i].username;
            obj.GetComponent<UserDataUI>().userScoreTxt.text = "" + leaderboardData[i].score;
        }

        leaderboardPanel.SetActive(true);
        userprofilePanel.SetActive(false);
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