using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "MainMenu";

    private void Start()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}