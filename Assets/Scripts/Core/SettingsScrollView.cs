using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-25
// AI assistance: Claude (autocomplete, architecture review)

public class SettingsScrollView : MonoBehaviour
{
    [SerializeField] private Transform content;
    [SerializeField] private float scrollSpeed = 4f;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;

    private void Update()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (scroll == 0f)
            return;

        float y = Mathf.Clamp(content.localPosition.y - scroll * scrollSpeed, minY, maxY);
        content.localPosition = new Vector3(content.localPosition.x, y, content.localPosition.z);
    }
}
