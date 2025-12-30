using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("LoadPrincipleScene", 2f);
    }

    void LoadPrincipleScene() 
    {
        SceneManager.LoadScene("PrincipleScene");
    }
}
