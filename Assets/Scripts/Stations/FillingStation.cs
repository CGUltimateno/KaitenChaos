using System.Collections.Generic;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-07
// AI assistance: Claude (autocomplete, architecture review)

public class FillingStation : MonoBehaviour
{
    public static FillingStation Instance { get; private set; }

    [SerializeField] private SpriteRenderer matRenderer;
    [SerializeField] private BoxCollider2D matCollider;
    [SerializeField] private Transform[] prepBoxSlots = new Transform[6];
    [SerializeField] private Transform lineStart;
    [SerializeField] private Transform lineEnd;

    public float LineStartX => lineStart.position.x;
    public float LineWidth => lineEnd.position.x - lineStart.position.x;

    private bool hasSeenHint;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void LoadDish(Dish dish)
    {
        ClearIcons();
        matRenderer.color = Color.white;

        if (!hasSeenHint)
        {
            Debug.Log("[FillingStation] First time here — check the guidebook (G) to see how to place your ingredients!");
            hasSeenHint = true;
        }

        for (int i = 0; i < dish.chosenIngredientIds.Count; i++)
        {
            GameObject iconObj = new GameObject($"Filling_{dish.chosenIngredientIds[i]}");
            iconObj.transform.SetParent(transform, false);
            iconObj.transform.position = prepBoxSlots[i].position;
            iconObj.transform.localScale = Vector3.one * 40f;

            SpriteRenderer sr = iconObj.AddComponent<SpriteRenderer>();
            Texture2D tex = Texture2D.whiteTexture;
            sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            sr.material = new Material(Shader.Find("Sprites/Default"));
            sr.color = Color.gray;
            sr.sortingOrder = 2;

            iconObj.AddComponent<FillingIcon>().Init(dish.chosenIngredientIds[i]);
        }
    }

    private void ClearIcons()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }

    public void RecordPlacement(string ingredientId, Vector3 worldPos)
    {
        if (DishManager.Instance.CurrentDish == null)
            return;

        DishManager.Instance.CurrentDish.fillingPlacements[ingredientId] = worldPos;
    }

    public void OnRiceDropped(Vector3 worldPos)
    {
        if (DishManager.Instance.CurrentDish == null)
            return;

        if (matCollider.bounds.Contains(worldPos))
        {
            DishManager.Instance.CurrentDish.ricePlaced = true;
            matRenderer.color = new Color(1f, 0.95f, 0.8f); // TEMP tint standing in for real rice art
            Debug.Log("[FillingStation] Rice placed.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (DishManager.Instance.CurrentDish == null)
            {
                Debug.Log("[FillingStation] No dish in progress yet.");
                return;
            }

            Debug.Log($"[FillingStation] Recorded {DishManager.Instance.CurrentDish.fillingPlacements.Count} placements, rice: {DishManager.Instance.CurrentDish.ricePlaced}");
            ClearIcons();
            matRenderer.color = Color.white;
            ViewManager.Instance.ShowOrderView(); // TEMP until Station 3 exists
        }
    }
}
