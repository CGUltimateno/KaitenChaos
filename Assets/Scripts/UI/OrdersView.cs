using UnityEngine;
using System.Collections.Generic;

// Implemented by: Cise Nil Köksal on 06.08.2026
//AI: ChatGPT (For debugging appropriate vector values, implementing draggable logic)
public class OrdersView : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool isExpanded = false;
    private Transform expandedBackground;
    private List<GameObject> orderNotes;
    private Vector3 target;

    private void Awake()
    {
        orderNotes = new List<GameObject>();
        expandedBackground = transform.Find("OrdersExpanded");

        foreach (Transform t in expandedBackground)
        {
            GameObject note = t.gameObject;
            note.SetActive(false);
            orderNotes.Add(note);
        }
        target = new Vector3(1f, 0f, 0f);

    }

    public void OnMouseDown()
    {
        Debug.Log("Clicked");
        isExpanded = !isExpanded;
        if (isExpanded)
        {
            foreach (GameObject note in orderNotes)
            {
                note.SetActive(true);
            }
            target = new Vector3(1f, 1f, 1f);

        }
        else
        {
            target = new Vector3(1f, 0f, 0f);
            foreach (GameObject note in orderNotes)
            {
                note.SetActive(false);
            }
        }
    }

    void Update()
    {
        expandedBackground.localScale = Vector3.Lerp(expandedBackground.localScale, target, 10f * Time.deltaTime);

    }
    public void AddTicket(Ticket ticket)
    {
        foreach (GameObject note in orderNotes)
        {
            OrderNote orderNote = note.GetComponent<OrderNote>();

            if (orderNote.ticket == null)
            {
                orderNote.ticket = ticket;
                note.gameObject.SetActive(true);

                Draggable draggable = note.GetComponent<Draggable>();

                draggable.onDropped = (position) =>
                {
                    ServingStation.Instance.TryDropTicketAt(orderNote.ticket, position);
                };

                return;
            }
        }
    }
}
