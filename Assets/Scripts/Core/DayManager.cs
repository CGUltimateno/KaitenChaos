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

    public int DishesServed { get; private set; }
    public int TotalStars { get; private set; }

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
        CurrentState = DayState.Spawning;
        dayTimer = 0f;
        Debug.Log($"[DayManager] Day started — {dayLength}s shift.");
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

    public void RecordServedDish(int stars)
    {
        DishesServed++;
        TotalStars += stars;
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
