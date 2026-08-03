using System.Collections.Generic;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-03
// AI assistance: VS Code (autocomplete), Claude (architecture review)

public static class OrderGenerator
{
    private static readonly string[] Traits = { "classic", "sweet", "spicy", "healthy", "rich" };

    public static Ticket Generate(int sourceSeat)
    {
        string trait = Traits[Random.Range(0, Traits.Length)];
        int maxRoll = GameDatabase.Instance.GetMaxRollSize(trait);
        int count = Random.Range(2, maxRoll + 1);

        List<OrderTypeData> orders = GameDatabase.Instance.Orders;
        string phrase = "";
        foreach (var order in orders)
        {
            if (order.type == trait && order.phrases != null && order.phrases.Count > 0)
            {
                phrase = order.phrases[Random.Range(0, order.phrases.Count)];
                break;
            }
        }

        return new Ticket
        {
            cravingTrait = trait,
            requestedCount = count,
            phrase = phrase,
            sourceSeat = sourceSeat,
            loggedToTray = false
        };
    }
}
