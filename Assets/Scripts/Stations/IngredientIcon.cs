using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-05
// AI assistance: Claude (autocomplete, architecture review)

public class IngredientIcon : MonoBehaviour
{
    public string ingredientId;
    public bool isSelected;

    private SpriteRenderer spriteRenderer;
    private Color baseColor;

    public void Init(string id)
    {
        ingredientId = id;
        isSelected = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        baseColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();
    }

    private void OnMouseDown()
    {
        if (!isSelected && !IngredientStation.Instance.CanSelectMore())
        {
            Debug.Log($"[IngredientIcon] Can't select {ingredientId} — all 6 prep boxes are full.");
            return;
        }

        isSelected = !isSelected;
        spriteRenderer.color = isSelected
            ? new Color(0.2f, 0.9f, 0.3f, 1f)
            : baseColor;
        Debug.Log($"[IngredientIcon] {ingredientId} {(isSelected ? "SELECTED" : "deselected")}");
    }

    public void Deselect()
    {
        isSelected = false;
        spriteRenderer.color = baseColor;
    }
}
