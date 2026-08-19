using UnityEngine;

public class ContinueButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        DayEndView.Instance.ContinueToNextShift();
    }
}
