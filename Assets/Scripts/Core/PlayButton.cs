using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-20
// AI assistance: Claude (autocomplete, architecture review)

public class PlayButton : MonoBehaviour
{

    [SerializeField] private AudioSource mainPlay;
    private void OnMouseDown()
    {
        ViewManager.Instance.PlayGame();
        mainPlay.Play();
    }
}
