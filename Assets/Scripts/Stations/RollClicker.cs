using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-15
// AI assistance: Claude (autocomplete, architecture review)

public class RollClicker : MonoBehaviour
{
    [SerializeField] private float requiredDragDistance = 1.5f;

    private bool dragging;
    private Vector3 startPos;

    private void OnMouseDown()
    {
        dragging = true;
        startPos = GetMouseWorldPos();
    }

    private void OnMouseUp()
    {
        if (!dragging) return;
        dragging = false;

        float dragDistance = Vector3.Distance(startPos, GetMouseWorldPos());
        if (dragDistance >= requiredDragDistance)
            CuttingStation.Instance.AdvanceRoll();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mouseScreen);
    }
}
