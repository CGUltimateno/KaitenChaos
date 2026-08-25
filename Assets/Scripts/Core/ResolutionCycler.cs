using UnityEngine;
using TMPro;

// Implemented by Mohamed Osman on 2026-08-25
// AI assistance: Claude (autocomplete, architecture review)

public class ResolutionCycler : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;

    private Resolution[] resolutions;
    private int index;

    private void OnEnable()
    {
        resolutions = Screen.resolutions;
        index = FindCurrentIndex();
        UpdateLabel();
    }

    public void Step(int direction)
    {
        index = (index + direction + resolutions.Length) % resolutions.Length;
        Resolution r = resolutions[index];
        SettingsManager.Instance.SetResolution(r.width, r.height);
        UpdateLabel();
    }

    private int FindCurrentIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                return i;
        }
        return resolutions.Length - 1;
    }

    private void UpdateLabel()
    {
        Resolution r = resolutions[index];
        label.text = $"{r.width} x {r.height}";
    }
}
