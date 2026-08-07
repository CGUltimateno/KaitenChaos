using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-07
// AI assistance: Claude (autocomplete, architecture review)

public class FillingIcon : MonoBehaviour
{
    public string ingredientId;

    public void Init(string id)
    {
        ingredientId = id;

        if (GetComponent<BoxCollider2D>() == null)
            gameObject.AddComponent<BoxCollider2D>();

        Draggable draggable = gameObject.AddComponent<Draggable>();
        draggable.onDropped = pos => FillingStation.Instance.RecordPlacement(ingredientId, pos);
    }
}
