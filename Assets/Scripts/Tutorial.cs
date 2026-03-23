using UnityEngine;
using System.Collections;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public GameObject tutorialOnePanel, tutorialTwoPanel, player;
    public GameObject buttonLeft, buttonRight, leftArrow, rightArrow, marcoButtonLeft, marcoButtonRight;
    public GameObject panelTutorial1, panelTutorial2;

    public TextMeshProUGUI pressContinueTxt, buttonLeftTxt, buttonRightTxt, pressContinueTxt2;

    public AudioSource UP, UPB;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //PlayerPrefs.DeleteKey("TutorialOneCompleted");
        //PlayerPrefs.DeleteKey("TutorialTwoCompleted");

#if UNITY_STANDALONE
        PCAplication();
#elif UNITY_ANDROID || UNITY_IOS
    AndroidAplication();
#endif
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && panelTutorial1.activeSelf)
        {
            panelTutorial1.SetActive(false);
            Time.timeScale = 1f;
            UP.mute = false; UPB.mute = true;
            tutorialTwoPanel.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.Space) && panelTutorial2.activeSelf)
        {
            panelTutorial2.SetActive(false);
            Time.timeScale = 1f;
            UP.mute = false; UPB.mute = true;
            tutorialTwoPanel.SetActive(false);
        }
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
#if UNITY_STANDALONE
        leftArrow.SetActive(false); rightArrow.SetActive(false);
#elif UNITY_ANDROID || UNITY_IOS
        marcoButtonLeft.SetActive(false); marcoButtonRight.SetActive(false);
#endif
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

    void AndroidAplication()
    {
        buttonLeftTxt.text = "Tap Button Left";
        buttonRightTxt.text = "Tap Button Right";
        pressContinueTxt.text = "Tap To Continue";
        pressContinueTxt.text = "Tap To Continue";
        marcoButtonLeft.SetActive(true); marcoButtonRight.SetActive(true);
    }

    void PCAplication()
    {
        buttonLeftTxt.text = "Left Arrow";
        buttonRightTxt.text = "Right Arrow";
        pressContinueTxt.text = "Press Space To Continue";
        pressContinueTxt2.text = "Press Space To Continue";
        leftArrow.SetActive(true); rightArrow.SetActive(true);
    }
}
