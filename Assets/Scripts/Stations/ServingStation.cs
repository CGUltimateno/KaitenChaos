using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-08
// AI assistance: Claude (autocomplete, architecture review)

public class ServingStation : MonoBehaviour
{
    public static ServingStation Instance { get; private set; }

    [SerializeField] private Transform plateSlot;
    [SerializeField] private BoxCollider2D conveyorBeltCollider;

    private GameObject dishIconObj;
    public Ticket BoundTicket { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void LoadDish(Dish dish)
    {
        BoundTicket = null;

        if (dishIconObj != null)
            Destroy(dishIconObj);

        dishIconObj = new GameObject("PlatedDish");
        dishIconObj.transform.SetParent(transform, false);
        dishIconObj.transform.position = plateSlot.position;
        dishIconObj.transform.localScale = Vector3.one * 40f;

        SpriteRenderer sr = dishIconObj.AddComponent<SpriteRenderer>();
        Texture2D tex = Texture2D.whiteTexture;
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        sr.material = new Material(Shader.Find("Sprites/Default"));
        sr.color = new Color(0.95f, 0.85f, 0.6f); // placeholder plated-dish color
        sr.sortingOrder = 2;

        dishIconObj.AddComponent<BoxCollider2D>();
        Draggable draggable = dishIconObj.AddComponent<Draggable>();
        draggable.onDropped = OnDishDropped;
    }

    public void BindTicket(Ticket ticket)
    {
        BoundTicket = ticket;
        Debug.Log($"[ServingStation] Bound ticket: {ticket.cravingTrait} — \"{ticket.phrase}\"");
    }

    private void OnDishDropped(Vector3 worldPos)
    {
        if (conveyorBeltCollider.bounds.Contains(worldPos))
            FinalizeServe();
        else
            dishIconObj.transform.position = plateSlot.position;
    }

    public void TryDropTicketAt(Ticket ticket, Vector3 worldPos)
    {
        // TODO: check against ticket-slot boxes, bind if inside one
    }

    private void FinalizeServe()
    {
        if (BoundTicket == null)
        {
            Debug.Log("[ServingStation] No ticket bound yet — can't serve.");
            dishIconObj.transform.position = plateSlot.position;
            return;
        }

        Debug.Log("[ServingStation] Serving! (scoring/customer-serving logic comes next step)");
        // next step: scoring + SeatManager/TicketTray/DishManager cleanup goes here
    }
}
