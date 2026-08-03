using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-03
// AI assistance: VS Code (autocomplete), Claude (architecture review)

public class GameDatabase : MonoBehaviour
{
    public static GameDatabase Instance { get; private set; }

    public Dictionary<string, IngredientData> Ingredients { get; private set; } = new Dictionary<string, IngredientData>();
    public List<OrderTypeData> Orders { get; private set; } = new List<OrderTypeData>();

    public Dictionary<string, int> MaxRollSize { get; private set; } = new Dictionary<string, int>();
    public Dictionary<string, List<int>> BestAchievable { get; private set; } = new Dictionary<string, List<int>>();

    private static readonly string[] TraitsList = new string[] { "classic", "sweet", "spicy", "healthy", "rich" };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // load json assets from Resources/Data
        TextAsset ingredientsAsset = Resources.Load<TextAsset>("Data/ingredients");
        TextAsset ordersAsset = Resources.Load<TextAsset>("Data/orders");

        if (ingredientsAsset == null)
        {
            Debug.LogError("GameDatabase: Failed to load Data/ingredients from Resources.");
            return;
        }

        if (ordersAsset == null)
        {
            Debug.LogError("GameDatabase: Failed to load Data/orders from Resources.");
            return;
        }

        IngredientList ingredientList = JsonUtility.FromJson<IngredientList>(ingredientsAsset.text);
        OrderList orderList = JsonUtility.FromJson<OrderList>(ordersAsset.text);
        Ingredients = new Dictionary<string, IngredientData>();

        if (ingredientList != null && ingredientList.ingredients != null)
        {
            foreach (var ing in ingredientList.ingredients)
            {
                if (ing != null && !string.IsNullOrEmpty(ing.id))
                {
                    Ingredients[ing.id] = ing;
                }
            }
        }

        if (orderList != null && orderList.orders != null)
        {
            Orders = orderList.orders;
        }

        List<IngredientData> allIngredients = ingredientList != null && ingredientList.ingredients != null
            ? ingredientList.ingredients
            : new List<IngredientData>();

        MaxRollSize = new Dictionary<string, int>();
        BestAchievable = new Dictionary<string, List<int>>();

        foreach (string trait in TraitsList)
        {
            int positiveCount = allIngredients.Count(ing => ing.traits != null && ing.traits.GetValue(trait) > 0);
            MaxRollSize[trait] = Mathf.Clamp(positiveCount, 0, 4);

            List<int> values = allIngredients
                .Select(ing => ing.traits != null ? ing.traits.GetValue(trait) : 0)
                .OrderByDescending(val => val)
                .ToList();

            List<int> runningSums = new List<int> { 0 }; // N = 0
            int currentSum = 0;
            for (int i = 0; i < values.Count; i++)
            {
                currentSum += values[i];
                runningSums.Add(currentSum);
            }
            BestAchievable[trait] = runningSums;
        }
        LogTables();
    }

    private void LogTables()
    {
        StringBuilder sbMaxRoll = new StringBuilder();
        sbMaxRoll.AppendLine("[GameDatabase] MaxRollSize Table:");
        foreach (var kvp in MaxRollSize)
        {
            sbMaxRoll.AppendLine($"  {kvp.Key}: {kvp.Value}");
        }
        Debug.Log(sbMaxRoll.ToString());

        StringBuilder sbBest = new StringBuilder();
        sbBest.AppendLine("[GameDatabase] BestAchievable Table:");
        foreach (var kvp in BestAchievable)
        {
            sbBest.AppendLine($"  {kvp.Key}: [{string.Join(", ", kvp.Value)}]");
        }
        Debug.Log(sbBest.ToString());
    }

    public IngredientData GetIngredient(string id)
    {
        if (Ingredients.TryGetValue(id, out IngredientData data))
        {
            return data;
        }
        return null;
    }

    public int GetMaxRollSize(string trait)
    {
        if (MaxRollSize.TryGetValue(trait, out int maxRoll))
        {
            return maxRoll;
        }
        return 0;
    }

    public int GetBestAchievable(string trait, int n)
    {
        if (BestAchievable.TryGetValue(trait, out List<int> sums))
        {
            int clampedN = Mathf.Clamp(n, 0, sums.Count - 1);
            return sums[clampedN];
        }
        return 0;
    }
}
