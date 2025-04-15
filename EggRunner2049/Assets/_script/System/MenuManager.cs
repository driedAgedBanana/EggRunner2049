using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject WelcomeScene;
    public GameObject MainScene;
    public GameObject TutorialAsk;
    public GameObject TutorialPanel;

    private bool _hasStarted = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WelcomeScene.SetActive(true);
        MainScene.SetActive(false);
        TutorialAsk.SetActive(false);
        TutorialPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey && !_hasStarted)
        {
            WelcomeScene.SetActive(false);
            MainScene.SetActive(true);
            _hasStarted = true;
        }
    }

    public void ToTutorialAsk()
    {
        WelcomeScene.SetActive(false);
        MainScene.SetActive(false);
        TutorialPanel.SetActive(false);
        TutorialAsk.SetActive(true);
    }

    public void ToTutorialPanel()
    {
        WelcomeScene.SetActive(false);
        MainScene.SetActive(false);
        TutorialAsk.SetActive(false);
        TutorialPanel.SetActive(true);
    }

    public void BackToMenu()
    {
        WelcomeScene.SetActive(false);
        MainScene.SetActive(true);
        TutorialAsk.SetActive(false);
        TutorialPanel.SetActive(false);
    }

    public void ToArena()
    {
        SceneManager.LoadScene("Arena");
        Time.timeScale = 1f;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
