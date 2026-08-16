using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-08
// AI assistance: Claude (autocomplete, architecture review)

public class ServingStation : MonoBehaviour
{
    public static ServingStation Instance { get; private set; }

    [SerializeField] private Transform plateSlot;
    [SerializeField] private BoxCollider2D conveyorBeltCollider;
    [SerializeField] private BoxCollider2D trashCollider;
    [SerializeField] private TicketSlot[] ticketSlots;
    [SerializeField] private Sprite classicDishSprite;
    [SerializeField] private Sprite sweetDishSprite;
    [SerializeField] private Sprite healthyDishSprite;
    [SerializeField] private Sprite spicyDishSprite;

    private GameObject dishIconObj;
    private TicketSlot boundSlot;
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
        dishIconObj.transform.localScale = Vector3.one;

        SpriteRenderer sr = dishIconObj.AddComponent<SpriteRenderer>();
        sr.sprite = GetPlateSprite(ScoringService.GetDominantTrait(dish));
        sr.material = new Material(Shader.Find("Sprites/Default"));
        sr.sortingOrder = 2;

        BoxCollider2D dishCollider = dishIconObj.AddComponent<BoxCollider2D>();
        dishCollider.size = sr.sprite.bounds.size;
        Draggable draggable = dishIconObj.AddComponent<Draggable>();
        draggable.onDropped = OnDishDropped;
    }

    private Sprite GetPlateSprite(string trait)
    {
        switch (trait)
        {
            case "sweet": return sweetDishSprite;
            case "healthy": return healthyDishSprite;
            case "spicy": return spicyDishSprite;
            default: return classicDishSprite; // classic + rich share this — no dedicated art for rich
        }
    }

    public void BindTicket(Ticket ticket)
    {
        BoundTicket = ticket;
        Debug.Log(ticket != null
            ? $"[ServingStation] Bound ticket: {ticket.cravingTrait} — \"{ticket.phrase}\""
            : "[ServingStation] Ticket unbound.");
    }

    private void OnDishDropped(Vector3 worldPos)
    {
        if (conveyorBeltCollider.bounds.Contains(worldPos))
            FinalizeServe();
        else if (trashCollider.bounds.Contains(worldPos))
            DiscardDish();
        else
            dishIconObj.transform.position = plateSlot.position;
    }

    public bool TryDropTicketAt(Ticket ticket, Vector3 worldPos)
    {
        foreach (var slot in ticketSlots)
        {
            if (slot.ContainsPoint(worldPos))
            {
                if (boundSlot != null && boundSlot != slot)
                    boundSlot.ClearIcon();
                if (BoundTicket != null && BoundTicket != ticket)
                    OrdersView.Instance.AddTicket(BoundTicket);

                slot.Bind(ticket);
                boundSlot = slot;
                return true;
            }
        }
        return false;
    }

    public void UnbindFromSlot(TicketSlot slot, Ticket ticket)
    {
        if (boundSlot == slot)
        {
            BindTicket(null);
            boundSlot = null;
        }

        if (ticket != null)
            OrdersView.Instance.AddTicket(ticket);
    }

    private void FinalizeServe()
    {
        if (BoundTicket == null)
        {
            Debug.Log("[ServingStation] No ticket bound yet — can't serve.");
            dishIconObj.transform.position = plateSlot.position;
            return;
        }

        Dish dish = DishManager.Instance.CurrentDish;
        Ticket ticket = BoundTicket;
        Customer customer = SeatManager.Instance.GetCustomerAtSeat(ticket.sourceSeat);

        int cravingScore = ScoringService.BindTicketToDish(dish, ticket);
        dish.fillingScore = ScoringService.ComputeFillingScore(dish, FillingStation.Instance.LineStartX, FillingStation.Instance.LineWidth);
        dish.patienceScore = customer != null
            ? ScoringService.ComputePatienceScore(customer.PatienceTimer, customer.MaxPatience)
            : 100;

        int overallScore = ScoringService.ComputeOverallScore(dish);
        int stars = ScoringService.ScoreToStars(overallScore);
        DayManager.Instance.RecordServedDish(stars);

        Debug.Log($"[ServingStation] Served! Craving match: {cravingScore}%, Filling: {dish.fillingScore}%, Cut: {dish.rollCutScore}%, Patience: {dish.patienceScore}%, Overall: {overallScore}% ({stars}★)");

        if (customer != null)
        {
            customer.SetState(Customer.State.Served);
            SeatManager.Instance.FreeSeat(ticket.sourceSeat);
        }

        TicketTray.Instance.RemoveTicket(ticket);
        DishManager.Instance.ClearCurrentDish();

        Destroy(dishIconObj);
        dishIconObj = null;
        BoundTicket = null;
        boundSlot?.ClearIcon();
        boundSlot = null;

        ViewManager.Instance.ShowOrderView();
    }

    private void DiscardDish()
    {
        Debug.Log("[ServingStation] Dish discarded.");

        if (BoundTicket != null)
            OrdersView.Instance.AddTicket(BoundTicket);

        DishManager.Instance.ClearCurrentDish();

        Destroy(dishIconObj);
        dishIconObj = null;
        BoundTicket = null;
        boundSlot?.ClearIcon();
        boundSlot = null;

        ViewManager.Instance.ShowOrderView();
    }
}
