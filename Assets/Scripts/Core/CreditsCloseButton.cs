using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-27
// AI assistance: Claude (autocomplete, architecture review)

public class CreditsCloseButton : MonoBehaviour
{
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private AudioSource clickSound;

    private void OnMouseDown()
    {
        clickSound.Play();
        creditsPanel.SetActive(false);
    }
}
