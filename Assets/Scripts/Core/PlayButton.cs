using UnityEngine;

public class PlayButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        ViewManager.Instance.PlayGame();
    }
}
