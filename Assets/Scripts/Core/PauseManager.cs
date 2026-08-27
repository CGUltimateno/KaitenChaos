using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-15
// AI assistance: Claude (autocomplete, architecture review)

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private GameObject pauseOverlay;
    [SerializeField] private Collider2D pauseBackgroundCollider;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        pauseOverlay.SetActive(false);
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        Time.timeScale = IsPaused ? 0f : 1f;
        pauseOverlay.SetActive(IsPaused);
    }

    public void SetBackgroundBlocking(bool blocking)
    {
        pauseBackgroundCollider.enabled = blocking;
    }

    public void ForceResume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        pauseOverlay.SetActive(false);
    }
}
