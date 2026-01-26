using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialOnePanel, tutorialTwoPanel, player;
    public GameObject buttonLeft, buttonRight;

    public AudioSource UP, UPB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PlayerPrefs.DeleteKey("TutorialOneCompleted");
        //PlayerPrefs.DeleteKey("TutorialTwoCompleted");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartTutorial()
    {
        yield return new WaitForSeconds(.1f);

        if (PlayerPrefs.GetString("TutorialOneCompleted") == "")
        {
            ViewTutorialOne();
        }
    }

    public IEnumerator StartTutorialTwo()
    {
        yield return new WaitForSeconds(0f);

        if (PlayerPrefs.GetString("TutorialTwoCompleted") == "")
        {
            ViewTutorialTwo();
        }
    }

    void ViewTutorialOne()
    {
        tutorialOnePanel.SetActive(true);
        buttonLeft.SetActive(false); buttonRight.SetActive(false);
        UP.mute = true; UPB.mute = false;
        PlayerPrefs.SetString("TutorialOneCompleted", "isTutorialOneComplete");
        PlayerPrefs.Save();
        Time.timeScale = 0f;
    }

    public void CloseTutorialOne()
    {
        Time.timeScale = 1f;
        UP.mute = false; UPB.mute = true;
        tutorialOnePanel.SetActive(false);
        buttonLeft.SetActive(true); buttonRight.SetActive(true);
    }

    void ViewTutorialTwo()
    {
        tutorialTwoPanel.SetActive(true);
        buttonLeft.SetActive(false); buttonRight.SetActive(false);
        UP.mute = true; UPB.mute = false;
        PlayerPrefs.SetString("TutorialTwoCompleted", "isTutorialTwoComplete");
        PlayerPrefs.Save();
        Time.timeScale = 0f;
    }

    public void CloseTutorialTwo()
    {
        Time.timeScale = 1f;
        UP.mute = false; UPB.mute = true;
        tutorialTwoPanel.SetActive(false);
        buttonLeft.SetActive(true); buttonRight.SetActive(true);
    }
}
