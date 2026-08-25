using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    [SerializeField] private Transform clockHandle;


    private void Update()
    {
        float progress = DayManager.Instance.DayProgress;

        float angle = Mathf.Lerp(-90f, 90f, progress);

        clockHandle.localRotation = Quaternion.Euler(0f, 0f, angle);

    }

}
