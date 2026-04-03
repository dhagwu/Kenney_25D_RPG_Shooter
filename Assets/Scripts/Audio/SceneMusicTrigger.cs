using UnityEngine;

public class SceneMusicTrigger : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private bool loop = true;

    private void Start()
    {
        if (AudioManager.Instance != null && musicClip != null)
        {
            AudioManager.Instance.PlayMusic(musicClip, loop);
        }
    }
}