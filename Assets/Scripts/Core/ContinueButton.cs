using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-19
// AI assistance: Claude (autocomplete, architecture review)

public class ContinueButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        DayEndView.Instance.ContinueToNextShift();
    }
}
