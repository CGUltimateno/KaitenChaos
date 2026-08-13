using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-07
// AI assistance: Claude (autocomplete, architecture review)

public class Draggable : MonoBehaviour
{
    public System.Action<Vector3> onDropped;
    public System.Action onPickedUp;
    private bool dragging;
    private Vector3 offset;

    private void OnMouseDown()
    {
        dragging = true;
        offset = transform.position - GetMouseWorldPos();
        onPickedUp?.Invoke();
    }

    private void Update()
    {
        if (dragging)
            transform.position = GetMouseWorldPos() + offset;
    }

    private void OnMouseUp()
    {
        if (!dragging) return;
        dragging = false;
        onDropped?.Invoke(GetMouseWorldPos());
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mouseScreen);
    }
}
