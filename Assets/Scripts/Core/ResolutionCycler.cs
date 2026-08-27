using System.Linq;
using UnityEngine;
using TMPro;

// Implemented by Mohamed Osman on 2026-08-25
// AI assistance: Claude (autocomplete, architecture review)

public class ResolutionCycler : MonoBehaviour
{
    [SerializeField] private TextMeshPro label;
    [SerializeField] private GameObject leftArrow;
    [SerializeField] private GameObject rightArrow;

    private static readonly Resolution[] StandardResolutions =
    {
        new Resolution { width = 1280, height = 720 },
        new Resolution { width = 1366, height = 768 },
        new Resolution { width = 1600, height = 900 },
        new Resolution { width = 1920, height = 1080 },
        new Resolution { width = 2560, height = 1440 },
        new Resolution { width = 3840, height = 2160 },
    };

    private Resolution[] resolutions;
    private int index;

    private void OnEnable()
    {
        Resolution native = Screen.currentResolution;
        resolutions = StandardResolutions
            .Where(r => r.width <= native.width && r.height <= native.height)
            .ToArray();
        RefreshInteractable();
    }

    public void RefreshInteractable()
    {
        index = FindCurrentIndex();
        UpdateLabel();

        bool interactable = !SettingsManager.Instance.IsFullscreen;
        leftArrow.SetActive(interactable);
        rightArrow.SetActive(interactable);
        label.gameObject.SetActive(interactable);
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
        int width = SettingsManager.Instance.CurrentWidth;
        int height = SettingsManager.Instance.CurrentHeight;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == width && resolutions[i].height == height)
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
