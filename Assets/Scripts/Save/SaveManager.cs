using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public static string GetSavePath()
    {
        return Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    public static bool HasSaveFileOnDisk()
    {
        return File.Exists(GetSavePath());
    }

    private bool suppressAutoSave;
    private bool autoSaveEnabled;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 关键修复：
        // 刚启动游戏时，先不要自动保存，避免默认数据覆盖旧存档。
        autoSaveEnabled = false;
    }

    private void OnEnable()
    {
        GameSession.OnGoldChanged += HandleGoldChanged;
        GameSession.OnProgressionChanged += HandleProgressionChanged;
        QuestManager.OnQuestDataChanged += HandleQuestDataChanged;
    }

    private void OnDisable()
    {
        GameSession.OnGoldChanged -= HandleGoldChanged;
        GameSession.OnProgressionChanged -= HandleProgressionChanged;
        QuestManager.OnQuestDataChanged -= HandleQuestDataChanged;
    }

    private void HandleGoldChanged(int _)
    {
        TryAutoSave();
    }

    private void HandleProgressionChanged()
    {
        TryAutoSave();
    }

    private void HandleQuestDataChanged()
    {
        TryAutoSave();
    }

    private void TryAutoSave()
    {
        if (!autoSaveEnabled) return;
        if (suppressAutoSave) return;
        if (GameSession.Instance == null) return;
        if (QuestManager.Instance == null) return;

        SaveNow();
    }

    public void EnableAutoSave(bool saveImmediately = false)
    {
        autoSaveEnabled = true;

        if (saveImmediately)
        {
            SaveNow();
        }

        Debug.Log("[SaveManager] Auto save ENABLED");
    }

    public void DisableAutoSave()
    {
        autoSaveEnabled = false;
        Debug.Log("[SaveManager] Auto save DISABLED");
    }

    public bool HasSaveFile()
    {
        return HasSaveFileOnDisk();
    }

    public void SaveNow()
    {
        if (GameSession.Instance == null || QuestManager.Instance == null)
        {
            Debug.LogWarning("[SaveManager] Save skipped: GameSession or QuestManager missing.");
            return;
        }

        SaveData data = new SaveData
        {
            gameSession = GameSession.Instance.BuildSaveData(),
            quests = QuestManager.Instance.BuildSaveDataList()
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSavePath(), json);

        Debug.Log($"[SaveManager] Saved -> {GetSavePath()}");
    }

    public bool LoadNow()
    {
        if (!HasSaveFileOnDisk())
        {
            Debug.LogWarning("[SaveManager] Load failed: no save file found.");
            return false;
        }

        if (GameSession.Instance == null || QuestManager.Instance == null)
        {
            Debug.LogWarning("[SaveManager] Load failed: GameSession or QuestManager missing.");
            return false;
        }

        string json = File.ReadAllText(GetSavePath());
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            Debug.LogWarning("[SaveManager] Load failed: parsed save data is null.");
            return false;
        }

        suppressAutoSave = true;

        GameSession.Instance.ApplySaveData(data.gameSession);
        QuestManager.Instance.ApplySaveDataList(data.quests);

        suppressAutoSave = false;

        Debug.Log($"[SaveManager] Loaded -> {GetSavePath()}");
        return true;
    }

    public void DeleteSave()
    {
        if (File.Exists(GetSavePath()))
        {
            File.Delete(GetSavePath());
            Debug.Log($"[SaveManager] Deleted -> {GetSavePath()}");
        }
    }
}