using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameplaySceneName = "Hub";

    public void StartGame()
    {
        if (GameSession.Instance != null)
        {
            GameSession.Instance.ResetSession();
            GameSession.Instance.AddBonusMaxHealth(20);
        }

        SceneManager.LoadScene(gameplaySceneName);
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