using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-25
// AI assistance: Claude (autocomplete, architecture review)

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private AudioSource clickSound;

    private void OnMouseDown()
    {
        clickSound.Play();
        SettingsManager.Instance.OpenSettings();
    }
}
