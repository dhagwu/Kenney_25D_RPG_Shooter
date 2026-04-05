using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryFlowController : MonoBehaviour
{
    [SerializeField] private string restartSceneName = "TestCombat";
    [SerializeField] private string hubSceneName = "Hub";

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(restartSceneName);
    }

    public void ReturnToHub()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(hubSceneName);
    }

    // 兼容旧按钮绑定：如果你的按钮还绑着这个旧名字，也会回 Hub
    public void ReturnToMainMenu()
    {
        ReturnToHub();
    }
}