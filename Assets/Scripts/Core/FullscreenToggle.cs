using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-25
// AI assistance: Claude (autocomplete, architecture review)

public class FullscreenToggle : MonoBehaviour
{
    [SerializeField] private GameObject checkmark;
    [SerializeField] private AudioSource clickSound;

    private void OnEnable()
    {
        checkmark.SetActive(SettingsManager.Instance.IsFullscreen);
    }

    private void OnMouseDown()
    {
        clickSound.Play();
        bool newValue = !SettingsManager.Instance.IsFullscreen;
        SettingsManager.Instance.SetFullscreen(newValue);
        checkmark.SetActive(newValue);
    }
}
