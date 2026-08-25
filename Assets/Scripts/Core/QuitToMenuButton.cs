using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-20
// AI assistance: Claude (autocomplete, architecture review)

public class QuitToMenuButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        ViewManager.Instance.ReturnToMainMenu();
    }
}
