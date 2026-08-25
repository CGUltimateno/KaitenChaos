using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-25
// AI assistance: Claude (autocomplete, architecture review)

public class ResolutionArrowButton : MonoBehaviour
{
    [SerializeField] private ResolutionCycler cycler;
    [SerializeField] private int direction = 1;
    [SerializeField] private AudioSource clickSound;

    private void OnMouseDown()
    {
        clickSound.Play();
        cycler.Step(direction);
    }
}
