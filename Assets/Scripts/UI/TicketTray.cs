using System.Collections.Generic;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-03
// AI assistance: Claude (autocomplete, architecture review)

public class TicketTray : MonoBehaviour
{
    public static TicketTray Instance { get; private set; }

    public List<Ticket> tickets = new List<Ticket>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddTicket(Ticket ticket)
    {
        tickets.Add(ticket);
        Debug.Log($"[TicketTray] Added ticket: {ticket.requestedCount}x {ticket.cravingTrait} — \"{ticket.phrase}\" (total: {tickets.Count})");
        OrdersView.Instance.AddTicket(ticket);
    }

    public void RemoveTicket(Ticket ticket)
    {
        tickets.Remove(ticket);
        Debug.Log($"[TicketTray] Removed ticket: {ticket.cravingTrait} (remaining: {tickets.Count})");
    }

    public void ClearAll()
    {
        tickets.Clear();
        OrdersView.Instance.ClearAllNotes();
    }
}
