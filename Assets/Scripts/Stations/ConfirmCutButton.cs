using UnityEngine;

public class ConfirmCutButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        CuttingStation.Instance.FinalizeCut();
    }
}
