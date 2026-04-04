using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private GameObject pausePanel;

    [Header("Scenes")]
    [SerializeField] private string restartSceneName = "TestCombat";
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string Hub = "Hub";
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private PlayerMotor playerMotor;
    [SerializeField] private PlayerAimController playerAimController;
    [SerializeField] private WeaponController weaponController;

    private bool IsSettingsOpen()
    {
        return settingsPanel != null && settingsPanel.activeSelf;
    }

    public bool IsPaused { get; private set; }


    private void Start()
    {
        if (playerMotor == null)
            playerMotor = FindFirstObjectByType<PlayerMotor>();

        if (playerAimController == null)
            playerAimController = FindFirstObjectByType<PlayerAimController>();

        if (weaponController == null)
            weaponController = FindFirstObjectByType<WeaponController>();

        if (playerInputHandler == null)
        {
            playerInputHandler = FindFirstObjectByType<PlayerInputHandler>();
        }

        SetPaused(false);
    }

    private void Update()
    {
        if (playerInputHandler == null)
            return;

        if (playerInputHandler.ConsumePausePressed())
        {
            if (IsSettingsOpen())
            {
                CloseSettingsMenu();
                return;
            }

            TogglePause();
        }
    }

    public void TogglePause()
    {
        SetPaused(!IsPaused);
    }

    public void ResumeGame()
    {
        SetPaused(false);
    }

    public void RestartLevel()
    {
        SetPaused(false);
        SceneManager.LoadScene(restartSceneName);
    }

    public void ReturnToMainMenu()
    {
        SetPaused(false);
        SceneManager.LoadScene(mainMenuSceneName);
    }
    public void ReturnToHub()
    {
        SetPaused(false);
        SceneManager.LoadScene(Hub);
    }

    private void SetPaused(bool paused)
    {
        IsPaused = paused;

        if (pausePanel != null)
        {
            pausePanel.SetActive(paused);
        }

        Time.timeScale = paused ? 0f : 1f;

        if (playerMotor != null)
            playerMotor.enabled = !paused;

        if (playerAimController != null)
            playerAimController.enabled = !paused;

        if (weaponController != null)
            weaponController.enabled = !paused;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OpenSettingsMenu()
    {
        if (!IsPaused)
        {
            SetPaused(true);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettingsMenu()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }
}