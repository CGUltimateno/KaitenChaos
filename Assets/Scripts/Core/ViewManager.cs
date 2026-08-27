using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-05
// AI assistance: Claude (autocomplete, architecture review)

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    public enum Station { Order, Ingredients, Filling, Cutting, Serving }

    public bool IsOrderViewActive => orderView.activeSelf;
    public bool IsMainMenuActive => mainMenuView.activeSelf;
    public bool IsStationBarActive => stationBar.activeSelf;
    public Station CurrentStation { get; private set; } = Station.Order;

    [SerializeField] private GameObject mainMenuView;
    [SerializeField] private GameObject orderView;
    [SerializeField] private GameObject stationViewIngredients;
    [SerializeField] private GameObject stationViewFilling;
    [SerializeField] private GameObject stationViewCutting;
    [SerializeField] private GameObject stationViewServing;
    [SerializeField] private GameObject dayEndView;
    [SerializeField] private GameObject stationBar;
    [SerializeField] private GameObject guidebookRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        ShowMainMenu();
        DayManager.Instance.OnDayEnded += ShowDayEndView;
    }

    public void ShowMainMenu()
    {
        Guidebook.Instance.Close();
        mainMenuView.SetActive(true);
        orderView.SetActive(false);
        stationViewIngredients.SetActive(false);
        stationViewFilling.SetActive(false);
        stationViewCutting.SetActive(false);
        stationViewServing.SetActive(false);
        stationBar.SetActive(false);
        dayEndView.SetActive(false);
        OrdersView.Instance.gameObject.SetActive(false);
        guidebookRoot.SetActive(false);
    }

    public void PlayGame()
    {
        DayManager.Instance.StartGame();
        mainMenuView.SetActive(false);
        guidebookRoot.SetActive(true);
        ShowOrderView();
    }

    public void ReturnToMainMenu()
    {
        PauseManager.Instance.ForceResume();
        DishManager.Instance.ClearCurrentDish();
        ServingStation.Instance.ResetState();
        SeatManager.Instance.ClearAllSeats();
        SeatManager.Instance.ResetForNewDay();
        TicketTray.Instance.ClearAll();
        DayManager.Instance.ReturnToMenu();
        ShowMainMenu();
    }

    public void ShowOrderView()
    {
        CurrentStation = Station.Order;
        orderView.SetActive(true);
        stationViewIngredients.SetActive(false);
        stationViewFilling.SetActive(false);
        stationViewCutting.SetActive(false);
        stationViewServing.SetActive(false);
        stationBar.SetActive(true);
        dayEndView.SetActive(false);
        OrdersView.Instance.gameObject.SetActive(true);
        foreach (var c in SeatManager.Instance.GetAllCustomers())
            c.SetVisible(true);
    }

    public void ShowIngredientsStation()
    {
        CurrentStation = Station.Ingredients;
        orderView.SetActive(false);
        stationViewIngredients.SetActive(true);
        stationViewFilling.SetActive(false);
        stationViewCutting.SetActive(false);
        stationViewServing.SetActive(false);
        foreach (var c in SeatManager.Instance.GetAllCustomers())
            c.SetVisible(false);
    }

    public void ShowFillingStation()
    {
        CurrentStation = Station.Filling;
        orderView.SetActive(false);
        stationViewIngredients.SetActive(false);
        stationViewFilling.SetActive(true);
        stationViewCutting.SetActive(false);
        stationViewServing.SetActive(false);
        foreach (var c in SeatManager.Instance.GetAllCustomers())
            c.SetVisible(false);
    }

    public void ShowCuttingStation()
    {
        CurrentStation = Station.Cutting;
        orderView.SetActive(false);
        stationViewIngredients.SetActive(false);
        stationViewFilling.SetActive(false);
        stationViewCutting.SetActive(true);
        stationViewServing.SetActive(false);
        foreach (var c in SeatManager.Instance.GetAllCustomers())
            c.SetVisible(false);
    }

    public void ShowServingStation()
    {
        CurrentStation = Station.Serving;
        orderView.SetActive(false);
        stationViewIngredients.SetActive(false);
        stationViewFilling.SetActive(false);
        stationViewCutting.SetActive(false);
        stationViewServing.SetActive(true);
        foreach (var c in SeatManager.Instance.GetAllCustomers())
            c.SetVisible(false);
    }

    public void ShowStation(Station station)
    {
        switch (station)
        {
            case Station.Order: ShowOrderView(); break;
            case Station.Ingredients: ShowIngredientsStation(); break;
            case Station.Filling: ShowFillingStation(); break;
            case Station.Cutting: ShowCuttingStation(); break;
            case Station.Serving: ShowServingStation(); break;
        }
    }

    private void ShowDayEndView()
    {
        orderView.SetActive(false);
        stationViewIngredients.SetActive(false);
        stationViewFilling.SetActive(false);
        stationViewCutting.SetActive(false);
        stationViewServing.SetActive(false);
        stationBar.SetActive(false);
        OrdersView.Instance.gameObject.SetActive(false);
        dayEndView.SetActive(true);
        dayEndView.GetComponent<DayEndView>().ShowSummary();
    }
}
