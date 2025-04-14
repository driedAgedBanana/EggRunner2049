using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject WelcomeScene;
    public GameObject MainScene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WelcomeScene.SetActive(true);
        MainScene.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey)
        {
            WelcomeScene.SetActive(false);
            MainScene.SetActive(true);
        }
    }

    public void ToArena()
    {
        SceneManager.LoadScene("Arena");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
