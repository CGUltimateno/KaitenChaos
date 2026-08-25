using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-15
// AI assistance: Claude (autocomplete, architecture review)

public class QuitGameButton : MonoBehaviour
{
    [SerializeField] private AudioSource mainPlay;
    private void OnMouseDown()
    {
        mainPlay.Play();
        Application.Quit();
    }
}
