using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private PlayerMovement _player;

    public GameObject pauseGame;
    public GameObject deathScreen;
    private bool _isPaused;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseGame.SetActive(false);
        _isPaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_isPaused)
            {
                PauseGame();
            }
            else if (_isPaused)
            {
                Continue();
            }
        }
    }

    public void PauseGame()
    {
        pauseGame.SetActive(true);
        Time.timeScale = 0f;
        _player.enabled = false;
        _isPaused = true;

        _player.CancelDash();

        if (_player.trailRenderer != null)
        {
            _player.trailRenderer.emitting = false;
            _player.trailRenderer.Clear();
        }

    }
    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Continue()
    {
        pauseGame.SetActive(false);
        Time.timeScale = 1f;
        _player.enabled = true;
        _isPaused = false;
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);
        Time.timeScale = 0f;
    }


    public void BackToMenu()
    {
        _player.CancelDash();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}