using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-08
// AI assistance: Claude (autocomplete, architecture review)

public class GroupSlot : MonoBehaviour
{
    private string ingredientId;
    private BoxCollider2D col;

    public void Init(string id)
    {
        ingredientId = id;
        col = gameObject.AddComponent<BoxCollider2D>();
        col.enabled = false; // only listens once the piece has been taken
    }

    public void MarkTaken()
    {
        col.enabled = true;
    }

    private void OnMouseDown()
    {
        Debug.Log($"[FillingStation] Already took a piece of {ingredientId} — only one per ingredient.");
    }
}
