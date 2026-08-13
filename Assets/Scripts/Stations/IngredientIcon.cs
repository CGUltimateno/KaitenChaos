using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-05
// AI assistance: Claude (autocomplete, architecture review)

public class IngredientIcon : MonoBehaviour
{
    public string ingredientId;
    public bool isSelected;

    private SpriteRenderer spriteRenderer;
    private GameObject checkmarkObj;

    public void Init(string id, GameObject checkmarkPrefab)
    {
        ingredientId = id;
        isSelected = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();

        Bounds bounds = spriteRenderer.bounds;
        Vector3 checkmarkPos = new Vector3(bounds.max.x, bounds.min.y, transform.position.z);

        checkmarkObj = Instantiate(checkmarkPrefab, checkmarkPos, Quaternion.identity, IngredientStation.Instance.transform);
        checkmarkObj.GetComponent<SpriteRenderer>().sortingOrder = 5;
        checkmarkObj.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!isSelected && !IngredientStation.Instance.CanSelectMore())
        {
            Debug.Log($"[IngredientIcon] Can't select {ingredientId} — all 6 prep boxes are full.");
            return;
        }

        isSelected = !isSelected;
        checkmarkObj.SetActive(isSelected);
        Debug.Log($"[IngredientIcon] {ingredientId} {(isSelected ? "SELECTED" : "deselected")}");
    }

    public void Deselect()
    {
        isSelected = false;
        checkmarkObj.SetActive(false);
    }
}
