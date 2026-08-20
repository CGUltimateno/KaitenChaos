using UnityEngine;

public class QuitToMenuButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        ViewManager.Instance.ReturnToMainMenu();
    }
}
