using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootEntry : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string hubSceneName = "Hub";

    private bool hasBooted;

    private void Start()
    {
        if (hasBooted) return;
        hasBooted = true;
        StartCoroutine(BootRoutine());
    }

    private IEnumerator BootRoutine()
    {
        // 等一帧，确保 Boot 场景里的全局管理器都完成 Awake
        yield return null;

        switch (LaunchRequest.PendingAction)
        {
            case LaunchAction.NewGame:
                HandleNewGame();
                LaunchRequest.PendingAction = LaunchAction.None;
                SceneManager.LoadScene(hubSceneName);
                yield break;

            case LaunchAction.Continue:
                bool loaded = HandleContinue();
                LaunchRequest.PendingAction = LaunchAction.None;
                SceneManager.LoadScene(loaded ? hubSceneName : mainMenuSceneName);
                yield break;

            default:
                // 启动到主菜单时，必须保持自动保存关闭
                if (SaveManager.Instance != null)
                {
                    SaveManager.Instance.DisableAutoSave();
                }

                SceneManager.LoadScene(mainMenuSceneName);
                yield break;
        }
    }

    private void HandleNewGame()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DisableAutoSave();
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DeleteSave();
        }
        else if (SaveManager.HasSaveFileOnDisk())
        {
            File.Delete(SaveManager.GetSavePath());
        }

        if (GameSession.Instance != null)
        {
            GameSession.Instance.ResetSession();
            GameSession.Instance.AddBonusMaxHealth(20);
        }

        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.ResetAllQuests();
        }

        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.EnableAutoSave(true);
        }

        Debug.Log("[BootEntry] New Game initialized.");
    }

    private bool HandleContinue()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.DisableAutoSave();
        }

        if (!SaveManager.HasSaveFileOnDisk())
        {
            Debug.LogWarning("[BootEntry] Continue failed: no save file on disk.");
            return false;
        }

        if (SaveManager.Instance == null)
        {
            Debug.LogWarning("[BootEntry] Continue failed: SaveManager missing.");
            return false;
        }

        bool loaded = SaveManager.Instance.LoadNow();

        if (loaded)
        {
            SaveManager.Instance.EnableAutoSave(false);
        }

        Debug.Log($"[BootEntry] Continue load result = {loaded}");
        return loaded;
    }
}