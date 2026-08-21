using UnityEngine;

public class QuitGameButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        Application.Quit();
    }
}
