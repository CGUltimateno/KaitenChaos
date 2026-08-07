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
        List<string> ingredientIds = new List<string>(GameDatabase.Instance.Ingredients.Keys);

        for (int i = 0; i < crateContainer.childCount && i < ingredientIds.Count; i++)
        {
            Transform crate = crateContainer.GetChild(i);
            IngredientIcon icon = crate.gameObject.AddComponent<IngredientIcon>();
            icon.Init(ingredientIds[i]);
            icons.Add(icon);
        }

        Debug.Log($"[IngredientStation] Initialized {icons.Count} ingredient crates.");
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            List<string> selected = GetSelectedIds();
            Dish dish = DishManager.Instance.StartNewDish(selected);
            FillingStation.Instance.LoadDish(dish);
            ViewManager.Instance.ShowFillingStation();

            foreach (var icon in icons)
                icon.Deselect();
        }
    }
}
