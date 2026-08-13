using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-07
// AI assistance: Claude (autocomplete, architecture review)

public class RiceBowl : MonoBehaviour
{
    private Vector3 homePosition;

    private void Awake()
    {
        homePosition = transform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.sortingOrder = 5;

        Draggable draggable = gameObject.AddComponent<Draggable>();
        draggable.onPickedUp = OnPickedUp;
        draggable.onDropped = OnDropped;
    }

    private void OnPickedUp()
    {
        FillingStation.Instance.OnRiceGrabbed();
    }

    private void OnDropped(Vector3 worldPos)
    {
        FillingStation.Instance.OnRiceDropped(worldPos);
        transform.position = homePosition;
    }
}
