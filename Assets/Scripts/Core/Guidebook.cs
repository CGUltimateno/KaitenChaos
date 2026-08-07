using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

// Implemented by Mohamed Osman on 2026-08-07
// AI assistance: Claude (autocomplete, architecture review)

public class Guidebook : MonoBehaviour
{
    public static Guidebook Instance { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject clickCatcher;
    [SerializeField] private TextMeshPro phraseText;
    [SerializeField] private TextMeshPro ingredientText;

    private static readonly string[] Traits = { "classic", "sweet", "spicy", "healthy", "rich" };
    private bool isOpen;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        panel.SetActive(false);
        clickCatcher.SetActive(false);
    }

    public void Open()
    {
        isOpen = true;
        panel.SetActive(true);
        clickCatcher.SetActive(true);
        ShowPage(0);
    }

    public void Close()
    {
        isOpen = false;
        panel.SetActive(false);
        clickCatcher.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (isOpen) Close(); else Open();
        }

        if (isOpen)
        {
            for (int i = 0; i < Traits.Length; i++)
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                    ShowPage(i);
        }
    }

    private void ShowPage(int index)
    {
        string trait = Traits[index];
        OrderTypeData order = GameDatabase.Instance.Orders.Find(o => o.type == trait);
        phraseText.text = order != null ? string.Join("\n", order.phrases) : trait;

        List<string> relevantIds = GameDatabase.Instance.Ingredients.Values
            .Where(ing => ing.traits.GetValue(trait) > 0)
            .Select(ing => ing.id)
            .ToList();
        ingredientText.text = string.Join(", ", relevantIds);
    }
}
