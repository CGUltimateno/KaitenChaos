using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-05
// AI assistance: Claude (autocomplete, architecture review)

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    public bool IsOrderViewActive => orderView.activeSelf;

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
        Debug.Log("[ViewManager] Showing Main Menu");
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
        Debug.Log("[ViewManager] Showing Order View");
    }

    public void ShowIngredientsStation()
    {
        orderView.SetActive(false);
        stationViewIngredients.SetActive(true);
        stationViewFilling.SetActive(false);
        stationViewCutting.SetActive(false);
        stationViewServing.SetActive(false);
        foreach (var c in SeatManager.Instance.GetAllCustomers())
            c.SetVisible(false);
        Debug.Log("[ViewManager] Showing Ingredients Station");
    }

    public void ShowFillingStation()
    {
        orderView.SetActive(false);
        stationViewIngredients.SetActive(false);
        stationViewFilling.SetActive(true);
        stationViewCutting.SetActive(false);
        stationViewServing.SetActive(false);
        foreach (var c in SeatManager.Instance.GetAllCustomers())
            c.SetVisible(false);
        Debug.Log("[ViewManager] Showing Filling Station");
    }

    public void ShowCuttingStation()
    {
        orderView.SetActive(false);
        stationViewIngredients.SetActive(false);
        stationViewFilling.SetActive(false);
        stationViewCutting.SetActive(true);
        stationViewServing.SetActive(false);
        foreach (var c in SeatManager.Instance.GetAllCustomers())
            c.SetVisible(false);
        Debug.Log("[ViewManager] Showing Cutting Station");
    }

    public void ShowServingStation()
    {
        orderView.SetActive(false);
        stationViewIngredients.SetActive(false);
        stationViewFilling.SetActive(false);
        stationViewCutting.SetActive(false);
        stationViewServing.SetActive(true);
        foreach (var c in SeatManager.Instance.GetAllCustomers())
            c.SetVisible(false);
        Debug.Log("[ViewManager] Showing Serving Station");
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
        Debug.Log("[ViewManager] Day ended — no more input.");
    }
}
