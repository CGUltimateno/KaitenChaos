using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Implemented by: Cise Nil Köksal on 06.08.2026
// Extended by Mohamed Osman on 13.08.2026   
//AI assistance: Claude (autocomplete, architecture review)

public class OrderNote : MonoBehaviour
{
    private static readonly Dictionary<string, string> TraitColors = new Dictionary<string, string>
    {
        { "classic", "#8B5E3C" },
        { "sweet",   "#FF6FA8" },
        { "spicy",   "#FF4C29" },
        { "healthy", "#4CAF50" },
        { "rich",    "#C9A227" },
    };

    public Ticket ticket;

    private TextMeshPro noteText;
    private Ticket lastTicket;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        noteText = GetComponentInChildren<TextMeshPro>();

        if (noteText != null)
        {
            noteText.enableAutoSizing = true;
            noteText.fontSizeMin = 3;
            noteText.fontSizeMax = 8;
            noteText.overflowMode = TextOverflowModes.Truncate;
        }
    }

    void Update()
    {
        if (ticket != lastTicket)
        {
            lastTicket = ticket;
            RefreshDisplay();
        }
    }

    private void RefreshDisplay()
    {
        if (noteText == null)
            return;

        if (ticket == null)
        {
            noteText.text = "";
            return;
        }

        string trait = ticket.cravingTrait;
        string color = TraitColors.TryGetValue(trait, out string hex) ? hex : "#444444";
        string traitLabel = trait.Length > 0 ? char.ToUpper(trait[0]) + trait.Substring(1) : trait;

        noteText.text = $"<b><size=115%>{ticket.requestedCount}x <color={color}>{traitLabel}</color></size></b>\n<i>\"{ticket.phrase}\"</i>";
    }
}
