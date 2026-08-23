using UnityEngine;
using System.Collections.Generic;

// Implemented by: Cise Nil Köksal on 06.08.2026
//AI: ChatGPT (For debugging appropriate vector values, implementing draggable logic)
public class OrdersView : MonoBehaviour
{
    public static OrdersView Instance { get; private set; }
    [SerializeField] private AudioSource clickSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isExpanded = false;
    [SerializeField] private Transform arrowTransform;
    private Transform expandedBackground;
    private List<GameObject> orderNotes;
    private Vector3 target;
    private Quaternion arrowTargetRotation = Quaternion.identity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        orderNotes = new List<GameObject>();
        expandedBackground = transform.Find("OrdersExpanded");

        float targetLocalZ = expandedBackground.lossyScale.z != 0
            ? -expandedBackground.position.z / expandedBackground.lossyScale.z
            : 0f;

        foreach (Transform t in expandedBackground)
        {
            GameObject note = t.gameObject;
            note.SetActive(false);
            orderNotes.Add(note);

            Vector3 localPos = note.transform.localPosition;
            note.transform.localPosition = new Vector3(localPos.x, localPos.y, targetLocalZ);
        }
        orderNotes.Sort((a, b) => b.transform.localPosition.y.CompareTo(a.transform.localPosition.y));

        target = new Vector3(1f, 0f, 0f);

        SpriteRenderer panelSr = expandedBackground.GetComponent<SpriteRenderer>();
        if (panelSr != null)
            panelSr.sortingOrder = 98;

        foreach (GameObject note in orderNotes)
        {
            foreach (var sr in note.GetComponentsInChildren<SpriteRenderer>(true))
                sr.sortingOrder = 99;
            foreach (var tmp in note.GetComponentsInChildren<TMPro.TextMeshPro>(true))
                tmp.sortingOrder = 100;
        }

        SpriteRenderer headingSr = transform.Find("OrdersHeading")?.GetComponent<SpriteRenderer>();
        if (headingSr != null)
            headingSr.sortingOrder = 101;

        TMPro.TextMeshPro headingTmp = transform.Find("OrdersHeadingText")?.GetComponent<TMPro.TextMeshPro>();
        if (headingTmp != null)
            headingTmp.sortingOrder = 102;
    }

    public void OnMouseDown()
    {
        clickSound.Play();
        Debug.Log("Clicked");
        if (isExpanded)
            CloseTray();
        else
            OpenTray();
    }

    private void OpenTray()
    {
        isExpanded = true;
        foreach (GameObject note in orderNotes)
        {
            OrderNote orderNote = note.GetComponent<OrderNote>();
            if (orderNote.ticket != null)
                note.SetActive(true);
        }
        target = new Vector3(1f, 1f, 1f);
        arrowTargetRotation = Quaternion.Euler(0f, 0f, 180f);
    }

    private void CloseTray()
    {
        isExpanded = false;
        target = new Vector3(1f, 0f, 0f);
        arrowTargetRotation = Quaternion.identity;
        foreach (GameObject note in orderNotes)
        {
            note.SetActive(false);
        }
    }

    void Update()
    {
        expandedBackground.localScale = Vector3.Lerp(expandedBackground.localScale, target, 4f * Time.deltaTime);

        if (arrowTransform != null)
            arrowTransform.localRotation = Quaternion.Slerp(arrowTransform.localRotation, arrowTargetRotation, 4f * Time.deltaTime);

        if (isExpanded && Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mouseWorld);
            bool clickedInsideOrdersView = hit != null && hit.transform.IsChildOf(transform);
            if (!clickedInsideOrdersView)
                CloseTray();
        }
    }
    public void AddTicket(Ticket ticket)
    {
        foreach (GameObject note in orderNotes)
        {
            OrderNote orderNote = note.GetComponent<OrderNote>();

            if (orderNote.ticket == null)
            {
                AssignTicketToNote(note, orderNote, ticket);
                return;
            }
        }
    }

    private void AssignTicketToNote(GameObject note, OrderNote orderNote, Ticket ticket)
    {
        orderNote.ticket = ticket;
        note.SetActive(true);

        Vector3 homeLocalPos = note.transform.localPosition;
        Draggable draggable = note.GetComponent<Draggable>();

        draggable.onDropped = (position) =>
        {
            bool bound = ServingStation.Instance.TryDropTicketAt(orderNote.ticket, position);
            note.transform.localPosition = homeLocalPos;
            if (bound)
            {
                orderNote.ticket = null;
                CompactNotes();
            }
        };
    }

    private void CompactNotes()
    {
        List<Ticket> activeTickets = new List<Ticket>();
        foreach (GameObject note in orderNotes)
        {
            OrderNote orderNote = note.GetComponent<OrderNote>();
            if (orderNote.ticket != null)
                activeTickets.Add(orderNote.ticket);
            orderNote.ticket = null;
            note.SetActive(false);
        }

        foreach (Ticket ticket in activeTickets)
        {
            foreach (GameObject note in orderNotes)
            {
                OrderNote orderNote = note.GetComponent<OrderNote>();
                if (orderNote.ticket == null)
                {
                    AssignTicketToNote(note, orderNote, ticket);
                    break;
                }
            }
        }
    }

    public void ClearAllNotes()
    {
        foreach (GameObject note in orderNotes)
        {
            OrderNote orderNote = note.GetComponent<OrderNote>();
            orderNote.ticket = null;
            note.SetActive(false);
        }
        CloseTray();
    }
}
