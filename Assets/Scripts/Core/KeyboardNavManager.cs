using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-25
// AI assistance: Claude (autocomplete, architecture review)

public class KeyboardNavManager : MonoBehaviour
{
    public static KeyboardNavManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
            return;
        }

        if (SettingsManager.Instance.IsOpen)
            return;

        if (Input.GetKeyDown(KeyCode.RightArrow))
            CycleStation(1);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            CycleStation(-1);
        else if (Input.GetKeyDown(KeyCode.Return) && ViewManager.Instance.IsMainMenuActive)
            ViewManager.Instance.PlayGame();
    }

    private void HandleEscape()
    {
        if (SettingsManager.Instance.IsOpen)
            SettingsManager.Instance.CloseSettings();
        else if (PauseManager.Instance.IsPaused)
            PauseManager.Instance.TogglePause();
        else if (!ViewManager.Instance.IsMainMenuActive)
            PauseManager.Instance.TogglePause();
    }

    private void CycleStation(int direction)
    {
        if (!ViewManager.Instance.IsStationBarActive || PauseManager.Instance.IsPaused)
            return;

        int stationCount = System.Enum.GetValues(typeof(ViewManager.Station)).Length;
        int next = ((int)ViewManager.Instance.CurrentStation + direction + stationCount) % stationCount;
        ViewManager.Instance.ShowStation((ViewManager.Station)next);
    }
}
