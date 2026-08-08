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

    private List<float> cutPositions = new List<float>();
    private List<GameObject> cutMarks = new List<GameObject>();

    public float LineStartX => lineStart.position.x;
    public float LineWidth => lineEnd.position.x - lineStart.position.x;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        Texture2D tex = Texture2D.whiteTexture;
        rollRenderer.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        rollRenderer.material = new Material(Shader.Find("Sprites/Default"));
        rollRenderer.color = new Color(0.9f, 0.85f, 0.7f); // placeholder tan color
    }

    public void LoadDish(Dish dish)
    {
        ClearMarks();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && cutPositions.Count < cutCount)
        {
            Vector3 mouseWorld = GetMouseWorldPos();
            if (rollCollider.bounds.Contains(mouseWorld))
                PlaceCutMark(mouseWorld.x);
        }

        if (Input.GetKeyDown(KeyCode.Return))
            FinalizeCut();
    }

    private void PlaceCutMark(float worldX)
    {
        cutPositions.Add(worldX);

        GameObject mark = new GameObject($"CutMark_{cutPositions.Count}");
        mark.transform.SetParent(transform, false);
        mark.transform.position = new Vector3(worldX, lineStart.position.y, 0);
        mark.transform.localScale = Vector3.one * 20f;

        SpriteRenderer sr = mark.AddComponent<SpriteRenderer>();
        Texture2D tex = Texture2D.whiteTexture;
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        sr.material = new Material(Shader.Find("Sprites/Default"));
        sr.color = Color.red;
        sr.sortingOrder = 2;

        cutMarks.Add(mark);
    }

    private void ClearMarks()
    {
        foreach (var mark in cutMarks)
            Destroy(mark);
        cutMarks.Clear();
        cutPositions.Clear();
    }

    private void FinalizeCut()
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
        ViewManager.Instance.ShowOrderView(); // TEMP until Serve station exists
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mouseScreen);
    }
}
