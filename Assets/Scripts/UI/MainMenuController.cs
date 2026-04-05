using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string bootSceneName = "Boot";
    [SerializeField] private Button continueButton;

    private void Start()
    {
        RefreshContinueButton();
    }

    public void StartGame()
    {
        LaunchRequest.PendingAction = LaunchAction.NewGame;
        SceneManager.LoadScene(bootSceneName);
    }

    public void ContinueGame()
    {
        if (!SaveManager.HasSaveFileOnDisk())
        {
            Debug.LogWarning("[MainMenuController] Continue failed: no save file.");
            RefreshContinueButton();
            return;
        }

        LaunchRequest.PendingAction = LaunchAction.Continue;
        SceneManager.LoadScene(bootSceneName);
    }

    public void RefreshContinueButton()
    {
        if (continueButton != null)
        {
            continueButton.interactable = SaveManager.HasSaveFileOnDisk();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}