using System.Collections;
using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-08
//Extended by Cise Nil Köksal on 21/08/2026 - Guidebook unlocks, on 24/08/2026 - Plate animation. AI: ChatGPT (animation implementation, guidebook debugging)
// AI assistance: Claude (autocomplete, architecture review)


public class ServingStation : MonoBehaviour
{
    public static ServingStation Instance { get; private set; }

    [SerializeField] private Transform plateSlot;
    [SerializeField] private BoxCollider2D conveyorBeltCollider;
    [SerializeField] private BoxCollider2D trashCollider;
    [SerializeField] private Transform conveyorStart;
    [SerializeField] private Transform conveyorEnd;
    private bool dishMoving = false;
    [SerializeField] private float conveyorSpeed = 6f;
    [SerializeField] private TicketSlot[] ticketSlots;
    [SerializeField] private Sprite classicDishSprite;
    [SerializeField] private Sprite sweetDishSprite;
    [SerializeField] private Sprite healthyDishSprite;
    [SerializeField] private Sprite spicyDishSprite;

    [Header("Rating Popup")]
    [SerializeField] private Sprite star;
    [SerializeField] private Sprite halfStar;
    [SerializeField] private int maxStars = 5;
    [SerializeField] private float ratingDisplayDuration = 2f;
    [SerializeField] private float ratingPopInDuration = 0.3f;
    [SerializeField] private Vector3 ratingPopupOffset = new Vector3(0f, 4.5f, 0f);
    [SerializeField] private float starScale = 2f;
    [SerializeField] private float starSpacing = 1.35f;
    [SerializeField] private Color filledStarColor = new Color(1f, 0.84f, 0f);
    [SerializeField] private Color halfStarColor = new Color(0.85f, 0.6f, 0.2f);
    [SerializeField] private Color emptyStarColor = new Color(0.6f, 0.6f, 0.6f);

    private GameObject dishIconObj;
    private TicketSlot boundSlot;
    public Ticket BoundTicket { get; private set; }
    [SerializeField] private AudioSource placeSound;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }
    private void Update()
    {
        if (!dishMoving || dishIconObj == null)
            return;

        dishIconObj.transform.position = Vector3.MoveTowards(
            dishIconObj.transform.position,
            conveyorEnd.position,
            conveyorSpeed * Time.deltaTime
        );

        if (dishIconObj.transform.position == conveyorEnd.position)
        {
            dishMoving = false;
            FinalizeServe();
        }
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
            default: return classicDishSprite;
        }
    }

    public void BindTicket(Ticket ticket)
    {
        BoundTicket = ticket;
    }

    private void OnDishDropped(Vector3 worldPos)
    {
        placeSound.Play();
        if (conveyorBeltCollider.bounds.Contains(worldPos))
        {
            dishIconObj.transform.position = conveyorStart.position;
            dishIconObj.transform.localScale = new Vector3(0.73f, 0.76f, 1f);
            dishMoving = true;
        }
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
        float stars = ScoringService.ScoreToStars(overallScore);
        DayManager.Instance.RecordServedDish(stars);

        TicketTray.Instance.RemoveTicket(ticket);
        DishManager.Instance.ClearCurrentDish();

        Destroy(dishIconObj);
        dishIconObj = null;
        BoundTicket = null;
        boundSlot?.ClearIcon();
        boundSlot = null;

        ViewManager.Instance.ShowOrderView();

        if (customer != null)
        {
            customer.SetState(Customer.State.Served);
            customer.ReactToRating(stars, maxStars);
            DayManager.Instance.StartCoroutine(ShowRatingThenFreeSeat(customer, stars));
        }
        foreach (string ingredientId in dish.chosenIngredientIds)
        {
            IngredientData ingredient = GameDatabase.Instance.GetIngredient(ingredientId);

            if (ingredient != null && ingredient.traits != null)
            {
                int traitScore = ingredient.traits.GetValue(ticket.cravingTrait);
                Guidebook.Instance.UnlockTrait(
                    ingredientId,
                    ticket.cravingTrait,
                    traitScore
                );
            }
        }
    }

    private IEnumerator ShowRatingThenFreeSeat(Customer customer, float stars)
    {
        Time.timeScale = 0f;
        GameObject blocker = BuildInputBlocker();

        GameObject popup = BuildRatingPopup(customer.transform.position + ratingPopupOffset, stars);
        popup.transform.localScale = Vector3.zero;

        float t = 0f;
        while (t < ratingPopInDuration)
        {
            t += Time.unscaledDeltaTime;
            popup.transform.localScale = Vector3.one * Mathf.SmoothStep(0f, 1f, t / ratingPopInDuration);
            yield return null;
        }
        popup.transform.localScale = Vector3.one;

        yield return new WaitForSecondsRealtime(ratingDisplayDuration);

        Destroy(popup);
        Destroy(blocker);
        Time.timeScale = 1f;
        SeatManager.Instance.FreeSeat(customer.SeatIndex);
    }

    private GameObject BuildRatingPopup(Vector3 position, float stars)
    {
        GameObject popup = new GameObject("RatingPopup");
        popup.transform.position = position;

        float spacing = starSpacing;
        float startX = -(maxStars - 1) * spacing / 2f;
        int fullStars = Mathf.FloorToInt(stars);
        bool hasHalf = (stars - fullStars) >= 0.5f;

        for (int i = 0; i < maxStars; i++)
        {
            Color color;
            if (i < fullStars) color = filledStarColor;
            else if (i == fullStars && hasHalf) color = halfStarColor;
            else color = emptyStarColor;

            GameObject starObj = new GameObject($"Star_{i}");
            starObj.transform.SetParent(popup.transform, false);
            starObj.transform.localPosition = new Vector3(startX + i * spacing, 0f, 0f);
            starObj.transform.localScale = Vector3.one * starScale;

            SpriteRenderer sr = starObj.AddComponent<SpriteRenderer>();
            if (i < fullStars) sr.sprite = star;
            else if (i == fullStars && hasHalf) sr.sprite = halfStar;
            else sr.sprite = star;
            sr.material = new Material(Shader.Find("Sprites/Default"));
            sr.color = color;
            sr.sortingOrder = 200;
        }

        return popup;
    }

    private GameObject BuildInputBlocker()
    {
        GameObject blocker = new GameObject("RatingInputBlocker");
        blocker.transform.position = new Vector3(0f, 0f, -2f);
        blocker.transform.localScale = new Vector3(38.4f, 21.6f, 1f);

        SpriteRenderer sr = blocker.AddComponent<SpriteRenderer>();
        Texture2D tex = Texture2D.whiteTexture;
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        sr.material = new Material(Shader.Find("Sprites/Default"));
        sr.color = new Color(0f, 0f, 0f, 0f);
        sr.sortingOrder = 199;

        BoxCollider2D col = blocker.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 1f);

        return blocker;
    }

    public void ResetState()
    {
        if (dishIconObj != null)
            Destroy(dishIconObj);
        dishIconObj = null;
        BoundTicket = null;
        boundSlot?.ClearIcon();
        boundSlot = null;
    }

    private void DiscardDish()
    {
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
