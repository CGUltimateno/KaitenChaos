using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-07
// AI assistance: Claude (autocomplete, architecture review)

public class FillingIcon : MonoBehaviour
{
    public string ingredientId;
    private SpriteRenderer sr;
    private GroupSlot groupSlot;

    public void Init(string id, GroupSlot groupSlot)
    {
        ingredientId = id;
        this.groupSlot = groupSlot;
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false; // invisible until picked up — the group art is all you see at rest

        if (GetComponent<BoxCollider2D>() == null)
            gameObject.AddComponent<BoxCollider2D>();

        Draggable draggable = gameObject.AddComponent<Draggable>();
        draggable.onPickedUp = OnPickedUp;
        draggable.onDropped = pos => FillingStation.Instance.RecordPlacement(ingredientId, pos);
    }

    private void OnPickedUp()
    {
        sr.enabled = true;
        groupSlot?.MarkTaken();
    }
}
