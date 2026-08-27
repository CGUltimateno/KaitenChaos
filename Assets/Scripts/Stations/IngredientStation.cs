using System.Collections.Generic;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-05
// AI assistance: Claude (autocomplete, architecture review)

public class IngredientStation : MonoBehaviour
{
    public static IngredientStation Instance { get; private set; }

    private List<IngredientIcon> icons = new List<IngredientIcon>();

    [SerializeField] private Transform crateContainer;
    [SerializeField] private int maxSelectable = 6;
    [SerializeField] private GameObject checkmarkPrefab;


    private const string CratePrefix = "ingredient_crate_";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < crateContainer.childCount; i++)
        {
            Transform crate = crateContainer.GetChild(i);
            string id = crate.name.StartsWith(CratePrefix) ? crate.name.Substring(CratePrefix.Length) : crate.name;

            if (!GameDatabase.Instance.Ingredients.ContainsKey(id))
                continue;

            IngredientIcon icon = crate.gameObject.AddComponent<IngredientIcon>();
            icon.Init(id, checkmarkPrefab);
            icons.Add(icon);
        }
    }

    public List<string> GetSelectedIds()
    {
        List<string> selected = new List<string>();
        foreach (var icon in icons)
        {
            if (icon.isSelected)
                selected.Add(icon.ingredientId);
        }
        return selected;
    }

    public bool CanSelectMore()
    {
        return GetSelectedIds().Count < maxSelectable;
    }

    public void SubmitIngredients()
    {
        List<string> selected = GetSelectedIds();
        Dish dish = DishManager.Instance.StartNewDish(selected);
        ViewManager.Instance.ShowFillingStation();
        FillingStation.Instance.LoadDish(dish);

        foreach (var icon in icons)
            icon.Deselect();
    }
}
