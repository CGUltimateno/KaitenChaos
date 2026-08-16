using UnityEngine;

public class PauseButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        PauseManager.Instance.TogglePause();
    }
}
