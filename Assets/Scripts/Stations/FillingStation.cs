using System.Collections.Generic;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-07
// AI assistance: Claude (autocomplete, architecture review)

public class FillingStation : MonoBehaviour
{
    public static FillingStation Instance { get; private set; }

    [SerializeField] private SpriteRenderer matRenderer;
    [SerializeField] private BoxCollider2D matCollider;
    [SerializeField] private Sprite noriOnMatSprite;
    [SerializeField] private Sprite riceOnMatSprite;
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Sprite backgroundWithBowlSprite;
    [SerializeField] private Sprite backgroundWithoutBowlSprite;
    [SerializeField] private Transform[] prepBoxSlots = new Transform[6];
    [SerializeField] private Transform lineStart;
    [SerializeField] private Transform lineEnd;
    [SerializeField] private List<IngredientSpriteSet> ingredientSprites;
    [SerializeField] private float submitDragThresholdX;

    public float LineStartX => lineStart.position.x;
    public float LineWidth => lineEnd.position.x - lineStart.position.x;
    public Bounds MatBounds => matRenderer.bounds;

    private bool hasSeenHint;
    private Dictionary<string, IngredientSpriteSet> spriteLookup;
    private Vector3 matHomePosition;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        spriteLookup = new Dictionary<string, IngredientSpriteSet>();
        foreach (var set in ingredientSprites)
            spriteLookup[set.ingredientId] = set;

        matHomePosition = matRenderer.transform.position;
        Draggable matDraggable = matRenderer.gameObject.AddComponent<Draggable>();
        matDraggable.onDropped = OnMatDropped;
    }

    public void LoadDish(Dish dish)
    {
        ClearIcons();
        matRenderer.sprite = noriOnMatSprite;
        matRenderer.transform.position = matHomePosition;
        backgroundRenderer.sprite = backgroundWithBowlSprite;

        if (!hasSeenHint)
        {
            Debug.Log("[FillingStation] First time here — check the guidebook (G) to see how to place your ingredients!");
            hasSeenHint = true;
        }

        for (int i = 0; i < dish.chosenIngredientIds.Count; i++)
        {
            string id = dish.chosenIngredientIds[i];
            spriteLookup.TryGetValue(id, out IngredientSpriteSet spriteSet);

            // static group art — stays in the prep box, never moves, not draggable
            GroupSlot groupSlot = null;

            if (spriteSet != null && spriteSet.groupSprite != null)
            {
                GameObject groupObj = new GameObject($"Group_{id}");
                groupObj.transform.SetParent(transform, false);
                groupObj.transform.position = prepBoxSlots[i].position;
                groupObj.transform.localScale = Vector3.one;

                SpriteRenderer groupSr = groupObj.AddComponent<SpriteRenderer>();
                groupSr.sprite = spriteSet.groupSprite;
                groupSr.material = new Material(Shader.Find("Sprites/Default"));
                groupSr.sortingOrder = 2;

                groupSlot = groupObj.AddComponent<GroupSlot>();
                groupSlot.Init(id);
            }

            // the actual draggable single piece, sits on top
            GameObject iconObj = new GameObject($"Filling_{id}");
            iconObj.transform.SetParent(transform, false);
            iconObj.transform.position = prepBoxSlots[i].position;

            SpriteRenderer sr = iconObj.AddComponent<SpriteRenderer>();
            sr.material = new Material(Shader.Find("Sprites/Default"));
            sr.sortingOrder = 3;

            if (spriteSet != null && spriteSet.singleSprite != null)
            {
                sr.sprite = spriteSet.singleSprite;
                iconObj.transform.localScale = Vector3.one;
            }
            else
            {
                Texture2D tex = Texture2D.whiteTexture;
                sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                sr.color = Color.gray;
                iconObj.transform.localScale = Vector3.one * 40f;
            }

            iconObj.AddComponent<FillingIcon>().Init(id, groupSlot);
        }
    }

    public Sprite GetSingleSprite(string ingredientId)
    {
        return spriteLookup.TryGetValue(ingredientId, out var set) ? set.singleSprite : null;
    }

    private void ClearIcons()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
        foreach (Transform child in matRenderer.transform)
            Destroy(child.gameObject);
    }

    public void RecordPlacement(string ingredientId, Vector3 worldPos, Transform iconTransform)
    {
        if (DishManager.Instance.CurrentDish == null)
            return;

        DishManager.Instance.CurrentDish.fillingPlacements[ingredientId] = worldPos;
        iconTransform.SetParent(matRenderer.transform, true);
    }

    public void OnRiceGrabbed()
    {
        backgroundRenderer.sprite = backgroundWithoutBowlSprite;
    }

    public void OnRiceDropped(Vector3 worldPos)
    {
        if (DishManager.Instance.CurrentDish == null)
            return;

        if (matCollider.bounds.Contains(worldPos))
        {
            DishManager.Instance.CurrentDish.ricePlaced = true;
            matRenderer.sprite = riceOnMatSprite;
            Debug.Log("[FillingStation] Rice placed.");
        }
    }

    private void OnMatDropped(Vector3 worldPos)
    {
        if (DishManager.Instance.CurrentDish != null && worldPos.x >= submitDragThresholdX)
        {
            Debug.Log($"[FillingStation] Recorded {DishManager.Instance.CurrentDish.fillingPlacements.Count} placements, rice: {DishManager.Instance.CurrentDish.ricePlaced}");
            ClearIcons();
            matRenderer.sprite = noriOnMatSprite;
            matRenderer.transform.position = matHomePosition;
            CuttingStation.Instance.LoadDish(DishManager.Instance.CurrentDish);
            ViewManager.Instance.ShowCuttingStation();
        }
        else
        {
            matRenderer.transform.position = matHomePosition;
        }
    }
}
