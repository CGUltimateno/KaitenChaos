using System.Collections.Generic;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-07
// AI assistance: Claude (autocomplete, architecture review)

public class DishManager : MonoBehaviour
{
    public static DishManager Instance { get; private set; }

    public Dish CurrentDish { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public Dish StartNewDish(List<string> chosenIngredientIds)
    {
        Dish dish = new Dish();
        dish.chosenIngredientIds = chosenIngredientIds;
        CurrentDish = dish;
        return dish;
    }

    public void ClearCurrentDish()
    {
        CurrentDish = null;
    }
}
