using UnityEngine;

public class PauseButton : MonoBehaviour
{
    [SerializeField] private AudioSource clickSound;
    private void OnMouseDown()
    {
        clickSound.Play();
        PauseManager.Instance.TogglePause();
    }
}
