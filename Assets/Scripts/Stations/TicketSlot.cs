using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-09
// AI assistance: Claude (autocomplete, architecture review)

public class TicketSlot : MonoBehaviour
{
    [SerializeField] private GameObject orderNotePrefab;
    [SerializeField] private float boundNoteScale = 0.5f;

    private BoxCollider2D zone;
    private GameObject boundIconObj;
    private Ticket boundTicket;

    private void Awake()
    {
        zone = GetComponent<BoxCollider2D>();
        if (zone == null)
            zone = gameObject.AddComponent<BoxCollider2D>();
    }

    public bool ContainsPoint(Vector3 worldPos)
    {
        bool hit = zone.OverlapPoint(worldPos);
        Debug.Log($"[TicketSlot] {name} bounds center: {zone.bounds.center}, checking: {worldPos}, hit: {hit}"); // TEMP diagnostic
        return hit;
    }

    public void Bind(Ticket ticket)
    {
        ServingStation.Instance.BindTicket(ticket);
        boundTicket = ticket;

        if (boundIconObj != null)
            Destroy(boundIconObj);
        boundIconObj = Instantiate(orderNotePrefab, ServingStation.Instance.transform);
        boundIconObj.transform.position = transform.position + boundNoteOffset;
        boundIconObj.transform.localScale = Vector3.one * boundNoteScale;

        OrderNote orderNote = boundIconObj.GetComponent<OrderNote>();
        orderNote.ticket = ticket;
        foreach (var sr in boundIconObj.GetComponentsInChildren<SpriteRenderer>(true))
            sr.sortingOrder = 99;
        foreach (var tmp in boundIconObj.GetComponentsInChildren<TMPro.TextMeshPro>(true))
            tmp.sortingOrder = 100;

        Draggable draggable = boundIconObj.GetComponent<Draggable>();
        draggable.onDropped = _ => Unbind();
    }

    public void ClearIcon()
    {
        if (boundIconObj != null)
            Destroy(boundIconObj);
        boundIconObj = null;
        boundTicket = null;
    }

    private void Unbind()
    {
        ServingStation.Instance.UnbindFromSlot(this, boundTicket);
        ClearIcon();
    }
}
