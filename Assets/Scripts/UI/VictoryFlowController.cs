using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryFlowController : MonoBehaviour
{
    [SerializeField] private string restartSceneName = "TestCombat";
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    public void RestartLevel()
    {
        SceneManager.LoadScene(restartSceneName);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}