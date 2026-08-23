using UnityEngine;

public class ConfirmCutButton : MonoBehaviour
{
    [SerializeField] private AudioSource clickSound;
    private void OnMouseDown()
    {
        clickSound.Play();
        CuttingStation.Instance.FinalizeCut();
    }
}
