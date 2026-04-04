using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private bool loop = true;

    private void Start()
    {
        Debug.Log($"[SceneMusicTrigger] 场景触发音乐: {gameObject.name}, clip={musicClip?.name}");

        if (AudioManager.Instance != null && musicClip != null)
        {
            AudioManager.Instance.PlayMusic(musicClip, loop);
        }
        else
        {
            Debug.LogWarning("[SceneMusicTrigger] AudioManager.Instance 或 musicClip 为空");
        }
    }
}