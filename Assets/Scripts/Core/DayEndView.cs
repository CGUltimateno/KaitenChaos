using UnityEngine;
using TMPro;

public class DayEndView : MonoBehaviour
{
    [SerializeField] private TextMeshPro summaryText;

    public void ShowSummary()
    {
        int dishes = DayManager.Instance.DishesServed;
        int stars = DayManager.Instance.TotalStars;
        int turnedAway = SeatManager.Instance.TurnedAwayCount;
        float avgStars = dishes > 0 ? (float)stars / dishes : 0f;

        summaryText.text = $"Day Complete!\n\nDishes Served: {dishes}\nTotal Stars: {stars}\nAverage: {avgStars:F1}★\nCustomers Turned Away: {turnedAway}";
    }
}
