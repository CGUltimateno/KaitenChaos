using UnityEngine;

public class StationBarButton : MonoBehaviour
{
    public enum Station { Order, Ingredients, Filling, Cutting, Serving }

    [SerializeField] private Station station;

    private void OnMouseDown()
    {
        switch (station)
        {
            case Station.Order: ViewManager.Instance.ShowOrderView(); break;
            case Station.Ingredients: ViewManager.Instance.ShowIngredientsStation(); break;
            case Station.Filling: ViewManager.Instance.ShowFillingStation(); break;
            case Station.Cutting: ViewManager.Instance.ShowCuttingStation(); break;
            case Station.Serving: ViewManager.Instance.ShowServingStation(); break;
        }
    }
}
