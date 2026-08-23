using UnityEngine;

public class SubmitButton : MonoBehaviour
{
    [SerializeField] private AudioSource clickSound;
    private void OnMouseDown()
    {
        clickSound.Play();
        IngredientStation.Instance.SubmitIngredients();
    }
}
