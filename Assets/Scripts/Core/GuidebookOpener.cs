using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-07
// AI assistance: Claude (autocomplete, architecture review)

public class GuidebookOpener : MonoBehaviour
{
    private void OnMouseDown()
    {
        Guidebook.Instance.Open();
    }
}
