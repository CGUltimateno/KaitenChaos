using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-25
// AI assistance: Claude (autocomplete, architecture review)

public class VolumeSliderHandle : MonoBehaviour
{
    public enum Target { Music, Sfx }

    [SerializeField] private Target target;
    [SerializeField] private Transform trackStart;
    [SerializeField] private Transform trackEnd;

    private bool dragging;
    private float fixedY;
    private float fixedZ;

    private void Awake()
    {
        fixedY = transform.position.y;
        fixedZ = transform.position.z;
    }

    private void OnEnable()
    {
        float current = target == Target.Music ? SettingsManager.Instance.MusicVolume : SettingsManager.Instance.SfxVolume;
        SetHandlePosition(current);
    }

    private void SetHandlePosition(float value01)
    {
        float x = Mathf.Lerp(trackStart.position.x, trackEnd.position.x, Mathf.Clamp01(value01));
        transform.position = new Vector3(x, fixedY, fixedZ);
    }

    private void OnMouseDown()
    {
        dragging = true;
    }

    private void Update()
    {
        if (!dragging)
            return;

        float clampedX = Mathf.Clamp(GetMouseWorldPos().x, trackStart.position.x, trackEnd.position.x);
        transform.position = new Vector3(clampedX, fixedY, fixedZ);

        float value01 = (clampedX - trackStart.position.x) / (trackEnd.position.x - trackStart.position.x);
        if (target == Target.Music)
            SettingsManager.Instance.SetMusicVolume(value01);
        else
            SettingsManager.Instance.SetSfxVolume(value01);
    }

    private void OnMouseUp()
    {
        dragging = false;
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = -Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mouseScreen);
    }
}
