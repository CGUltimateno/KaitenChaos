using UnityEngine;

public class PlayButton : MonoBehaviour
{

    [SerializeField] private AudioSource mainPlay;
    private void OnMouseDown()
    {
        ViewManager.Instance.PlayGame();
        mainPlay.Play();
    }
}
