using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    [SerializeField] private Transform clockHandle;


    private void Update()
    {
        if (DayManager.Instance == null) return;

        bool visible = DayManager.Instance.CurrentState == DayManager.DayState.Spawning
            || DayManager.Instance.CurrentState == DayManager.DayState.LastOrders;
   
        if (!visible) return;
        float progress = DayManager.Instance.DayProgress;

        float angle = Mathf.Lerp(-90f, 90f, progress);

        clockHandle.localRotation = Quaternion.Euler(0f, 0f, angle);

    }

}
