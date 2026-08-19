using System.Collections.Generic;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-08
// AI assistance: Claude (autocomplete, architecture review)

public class CuttingStation : MonoBehaviour
{
    public static CuttingStation Instance { get; private set; }

    [SerializeField] private BoxCollider2D rollCollider;
    [SerializeField] private SpriteRenderer rollRenderer;
    [SerializeField] private Transform lineStart;
    [SerializeField] private Transform lineEnd;
    [SerializeField] private int cutCount = 5;
    [SerializeField] private SpriteRenderer flatMatRenderer;
    [SerializeField] private Sprite flatMatSprite;
    [SerializeField] private GameObject rollObj;
    [SerializeField] private Sprite rollSprite;
    [SerializeField] private Sprite[] rollStageSprites;
    [SerializeField] private Sprite cutMarkSprite;
    [SerializeField] private Sprite cutKnifeSprite;
    [SerializeField] private Vector2 knifeOffset = new Vector2(-0.3f, -0.8f);

    private bool isRolled;
    private int rollStage;

    private List<float> cutPositions = new List<float>();
    private List<GameObject> cutMarks = new List<GameObject>();
    private List<GameObject> flatMatIcons = new List<GameObject>();

    public float LineStartX => lineStart.position.x;
    public float LineWidth => lineEnd.position.x - lineStart.position.x;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        rollRenderer.sprite = rollSprite;
        rollRenderer.material = new Material(Shader.Find("Sprites/Default"));
        flatMatRenderer.material = new Material(Shader.Find("Sprites/Default"));

        flatMatRenderer.gameObject.SetActive(false);
        rollObj.SetActive(false);
    }

    public void LoadDish(Dish dish)
    {
        ClearMarks();
        ClearFlatMatIcons();
        isRolled = false;
        rollStage = 0;
        flatMatRenderer.sprite = flatMatSprite;
        flatMatRenderer.gameObject.SetActive(true);
        rollObj.SetActive(false);

        ShowIngredientsOnFlatMat(dish);
    }

    private void ShowIngredientsOnFlatMat(Dish dish)
    {
        Bounds srcMatBounds = FillingStation.Instance.MatBounds;
        Bounds matBounds = flatMatRenderer.bounds;

        foreach (string id in dish.chosenIngredientIds)
        {
            if (!dish.fillingPlacements.TryGetValue(id, out Vector3 originalPos))
                continue;

            Sprite singleSprite = FillingStation.Instance.GetSingleSprite(id);
            if (singleSprite == null)
                continue;

            float fractionX = Mathf.Clamp01((originalPos.x - srcMatBounds.min.x) / srcMatBounds.size.x);
            float fractionY = Mathf.Clamp01((originalPos.y - srcMatBounds.min.y) / srcMatBounds.size.y);
            float mappedX = Mathf.Lerp(matBounds.min.x, matBounds.max.x, fractionX);
            float mappedY = Mathf.Lerp(matBounds.min.y, matBounds.max.y, fractionY);

            GameObject iconObj = new GameObject($"FlatMatIcon_{id}");
            iconObj.transform.SetParent(flatMatRenderer.transform, false);
            iconObj.transform.position = new Vector3(mappedX, mappedY, flatMatRenderer.transform.position.z - 0.01f);
            iconObj.transform.localScale = Vector3.one;

            SpriteRenderer sr = iconObj.AddComponent<SpriteRenderer>();
            sr.sprite = singleSprite;
            sr.material = new Material(Shader.Find("Sprites/Default"));
            sr.sortingOrder = 5;

            flatMatIcons.Add(iconObj);
        }
    }

    private void ClearFlatMatIcons()
    {
        foreach (var icon in flatMatIcons)
            Destroy(icon);
        flatMatIcons.Clear();
    }

    public void AdvanceRoll()
    {
        if (rollStage < rollStageSprites.Length)
        {
            flatMatRenderer.sprite = rollStageSprites[rollStage];
            rollStage++;
            Debug.Log($"[CuttingStation] Roll stage {rollStage}/{rollStageSprites.Length}");
        }
        else
        {
            isRolled = true;
            flatMatRenderer.gameObject.SetActive(false);
            rollObj.SetActive(true);
            Debug.Log("[CuttingStation] Rolled! Now cut it.");
        }
    }

    private void Update()
    {
        if (isRolled && Input.GetMouseButtonDown(0) && cutPositions.Count < cutCount)
        {
            Vector3 mouseWorld = GetMouseWorldPos();
            if (rollCollider.bounds.Contains(mouseWorld))
                PlaceCutMark(mouseWorld.x);
        }
    }

    private void PlaceCutMark(float worldX)
    {
        cutPositions.Add(worldX);

        GameObject mark = new GameObject($"CutMark_{cutPositions.Count}");
        mark.transform.SetParent(transform, false);
        mark.transform.position = new Vector3(worldX, rollCollider.bounds.center.y, 0);
        mark.transform.localScale = Vector3.one;

        SpriteRenderer lineSr = mark.AddComponent<SpriteRenderer>();
        lineSr.sprite = cutMarkSprite;
        lineSr.material = new Material(Shader.Find("Sprites/Default"));
        lineSr.sortingOrder = 2;

        GameObject knifeObj = new GameObject("Knife");
        knifeObj.transform.SetParent(mark.transform, false);
        knifeObj.transform.localPosition = new Vector3(knifeOffset.x, knifeOffset.y, 0f);

        SpriteRenderer knifeSr = knifeObj.AddComponent<SpriteRenderer>();
        knifeSr.sprite = cutKnifeSprite;
        knifeSr.material = new Material(Shader.Find("Sprites/Default"));
        knifeSr.sortingOrder = 3;

        cutMarks.Add(mark);
    }

    private void ClearMarks()
    {
        foreach (var mark in cutMarks)
            Destroy(mark);
        cutMarks.Clear();
        cutPositions.Clear();
    }

    public void FinalizeCut()
    {
        if (DishManager.Instance.CurrentDish == null)
        {
            Debug.Log("[CuttingStation] No dish in progress yet.");
            return;
        }

        Dish dish = DishManager.Instance.CurrentDish;
        dish.rollCutScore = ScoringService.ComputeRollCutScore(cutPositions, LineStartX, LineWidth, cutCount);
        Debug.Log($"[CuttingStation] Cut score: {dish.rollCutScore}%");

        ClearMarks();
        isRolled = false;
        flatMatRenderer.gameObject.SetActive(false);
        rollObj.SetActive(false);
        ViewManager.Instance.ShowServingStation();
        ServingStation.Instance.LoadDish(DishManager.Instance.CurrentDish);
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mouseScreen);
    }
}
