using UnityEngine;

public class SubmitButton : MonoBehaviour
{
    private void OnMouseDown()
    {
        IngredientStation.Instance.SubmitIngredients();
    }
}
