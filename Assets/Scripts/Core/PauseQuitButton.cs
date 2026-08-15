using UnityEngine;

public class PauseQuitButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        Application.Quit();
    }
}
