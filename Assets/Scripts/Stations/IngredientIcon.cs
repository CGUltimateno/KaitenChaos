using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-05
// AI assistance: Claude (autocomplete, architecture review)

public class IngredientIcon : MonoBehaviour
{
    public string ingredientId;
    public bool isSelected;

    private GameObject checkmarkObj;

    public void Init(string id, GameObject checkmarkPrefab)
    {
        ingredientId = id;
        isSelected = false;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
            collider = gameObject.AddComponent<BoxCollider2D>();

        Vector3 checkmarkLocalOffset = new Vector3(0.955f, -0.761f, 0f);
        Vector3 checkmarkPos = transform.TransformPoint(checkmarkLocalOffset);

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
