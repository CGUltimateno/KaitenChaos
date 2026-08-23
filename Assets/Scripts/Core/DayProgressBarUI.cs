using UnityEngine;

public class DayProgressBarUI : MonoBehaviour
{
    private const float BarWidth = 8f;
    private const float BarHeight = 0.6f;

    [SerializeField] private SpriteRenderer background;
    [SerializeField] private SpriteRenderer fill;

    private Transform fillTransform;

    private void Awake()
    {
        fillTransform = fill.transform;
    }

    private void Update()
    {
        if (DayManager.Instance == null) return;

        bool visible = DayManager.Instance.CurrentState == DayManager.DayState.Spawning
            || DayManager.Instance.CurrentState == DayManager.DayState.LastOrders;
        background.enabled = visible;
        fill.enabled = visible;
        if (!visible) return;

        float width = BarWidth * DayManager.Instance.DayProgress;
        fillTransform.localScale = new Vector3(width, BarHeight, 1f);
        fillTransform.localPosition = new Vector3(-BarWidth / 2f + width / 2f, 0f, 0f);
    }
}