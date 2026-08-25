using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-15
// AI assistance: Claude (autocomplete, architecture review)

public class PauseButton : MonoBehaviour
{
    [SerializeField] private AudioSource clickSound;
    private void OnMouseDown()
    {
        clickSound.Play();
        PauseManager.Instance.TogglePause();
    }
}
