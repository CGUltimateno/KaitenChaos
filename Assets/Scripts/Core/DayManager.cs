using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-06
// AI assistance: Claude (autocomplete, architecture review)

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    public enum DayState { Spawning, LastOrders, DayEnded }

    [SerializeField] private float dayLength = 60f;

    public DayState CurrentState { get; private set; }
    public bool CanSpawnNewCustomers => CurrentState == DayState.Spawning;
    public event System.Action OnDayEnded;

    public int DayNumber { get; private set; }
    public int DishesServed { get; private set; }
    public float TotalStars { get; private set; }
    public float BestRating { get; private set; }
    public int PerfectDishCount { get; private set; }

    private float dayTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        DayNumber = 1;
        BeginDay();
    }

    public void StartNextDay()
    {
        DayNumber++;
        BeginDay();
    }

    private void BeginDay()
    {
        CurrentState = DayState.Spawning;
        dayTimer = 0f;
        DishesServed = 0;
        TotalStars = 0f;
        BestRating = 0f;
        PerfectDishCount = 0;
        Debug.Log($"[DayManager] Day {DayNumber} started — {dayLength}s shift.");
    }

    private void Update()
    {
        if (CurrentState == DayState.DayEnded)
            return;

        dayTimer += Time.deltaTime;

        if (CurrentState == DayState.Spawning && dayTimer >= dayLength)
        {
            CurrentState = DayState.LastOrders;
            Debug.Log("[DayManager] Shift over — last orders! No new customers.");
        }

        if (CurrentState == DayState.LastOrders && AllSeatsEmpty())
        {
            CurrentState = DayState.DayEnded;
            Debug.Log("[DayManager] All customers served — day ended.");
            OnDayEnded?.Invoke();
        }
    }

    public void RecordServedDish(float stars)
    {
        DishesServed++;
        TotalStars += stars;
        if (stars > BestRating)
            BestRating = stars;
        if (stars >= 5f)
            PerfectDishCount++;
    }

    private bool AllSeatsEmpty()
    {
        foreach (var c in SeatManager.Instance.GetAllCustomers())
        {
            return false;
        }
        return true;
    }
}
