using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-05
// AI assistance: Claude (autocomplete, architecture review)

public class ViewManager : MonoBehaviour
{
    public static ViewManager Instance { get; private set; }

    public bool IsOrderViewActive => orderView.activeSelf;

    [SerializeField] private GameObject orderView;
    [SerializeField] private GameObject stationViewIngredients;
    [SerializeField] private GameObject stationViewFilling;
    [SerializeField] private GameObject stationViewCutting;
    [SerializeField] private GameObject stationViewServing;

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
        ShowOrderView();
        DayManager.Instance.OnDayEnded += ShowDayEndView;
    }

    public void ShowOrderView()
    {
        orderView.SetActive(true);
        stationViewIngredients.SetActive(false);
        stationViewFilling.SetActive(false);
        stationViewCutting.SetActive(false);
        stationViewServing.SetActive(false);
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
        Debug.Log("[ViewManager] Day ended — no more input.");
    }

    private void Update()
    {
        if (DayManager.Instance.CurrentState == DayManager.DayState.DayEnded)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ShowOrderView();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ShowIngredientsStation();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            ShowFillingStation();
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ShowCuttingStation();
        if (Input.GetKeyDown(KeyCode.Alpha5))
            ShowServingStation();
    }
}
