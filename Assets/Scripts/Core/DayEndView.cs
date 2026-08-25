using UnityEngine;
using TMPro;

// Implemented by Mohamed Osman on 2026-08-14
// AI assistance: Claude (autocomplete, architecture review)

public class DayEndView : MonoBehaviour
{
    public static DayEndView Instance { get; private set; }

    [SerializeField] private TextMeshPro summaryText;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void ShowSummary()
    {
        int dishes = DayManager.Instance.DishesServed;
        float stars = DayManager.Instance.TotalStars;
        float bestRating = DayManager.Instance.BestRating;
        int perfectDishes = DayManager.Instance.PerfectDishCount;
        int turnedAway = SeatManager.Instance.TurnedAwayCount;
        float avgStars = dishes > 0 ? stars / dishes : 0f;

        summaryText.text = $"Day {DayManager.Instance.DayNumber} Complete!\n\n" +
            $"Customers Served: {dishes}\n" +
            $"Average Rating: {avgStars:F1} / 5\n" +
            $"Best Dish: {bestRating:F1} / 5\n" +
            $"Perfect Dishes: {perfectDishes}\n" +
            $"Customers Turned Away: {turnedAway}";
    }

    public void ContinueToNextShift()
    {
        DayManager.Instance.StartNextDay();
        SeatManager.Instance.ResetForNewDay();
        ViewManager.Instance.ShowOrderView();
    }
}
