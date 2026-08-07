using System.Collections.Generic;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-03
// AI assistance: Claude (autocomplete, architecture review)

public class Dish
{
    private static int nextId = 0;

    public int dishId;
    public List<string> chosenIngredientIds;
    public bool ricePlaced;
    public Dictionary<string, Vector3> fillingPlacements;
    public int fillingScore;
    public int rollCutScore;
    public Ticket boundTicket;
    public int cravingMatchScore;

    public Dish()
    {
        dishId = nextId++;
        chosenIngredientIds = new List<string>();
        ricePlaced = false;
        fillingPlacements = new Dictionary<string, Vector3>();
        fillingScore = 0;
        rollCutScore = 0;
        boundTicket = null;
        cravingMatchScore = 0;
    }
}
