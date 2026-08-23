using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    [SerializeField] private AudioSource mainPlay;
    private void OnMouseDown()
    {
        mainPlay.Play();
        Application.Quit();
    }
}
