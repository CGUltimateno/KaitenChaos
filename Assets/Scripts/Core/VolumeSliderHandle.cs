using UnityEngine;

// Implemented by Mohamed Osman on 2026-08-25
// AI assistance: Claude (autocomplete, architecture review)

public class VolumeSliderHandle : MonoBehaviour
{
    public enum Target { Music, Sfx }

    [SerializeField] private Target target;
    [SerializeField] private Transform trackStart;
    [SerializeField] private Transform trackEnd;
    [SerializeField] private Transform fill;

    private bool dragging;
    private float yOffsetFromTrack;
    private float zOffsetFromTrack;
    private float fillFixedY;
    private float fillFixedZ;
    private float fillScaleY;
    private float fillScaleZ;

    private void Awake()
    {
        yOffsetFromTrack = transform.position.y - trackStart.position.y;
        zOffsetFromTrack = transform.position.z - trackStart.position.z;

        if (fill != null)
        {
            fillFixedY = fill.localPosition.y;
            fillFixedZ = fill.localPosition.z;
            fillScaleY = fill.localScale.y;
            fillScaleZ = fill.localScale.z;
        }
    }

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (SettingsManager.Instance == null)
            return;

        float current = target == Target.Music ? SettingsManager.Instance.MusicVolume : SettingsManager.Instance.SfxVolume;
        SetHandlePosition(current);
        SetFillWidth(current);
    }

    private void SetHandlePosition(float value01)
    {
        float x = Mathf.Lerp(trackStart.position.x, trackEnd.position.x, Mathf.Clamp01(value01));
        transform.position = new Vector3(x, trackStart.position.y + yOffsetFromTrack, trackStart.position.z + zOffsetFromTrack);
    }

    private void SetFillWidth(float value01)
    {
        if (fill == null)
            return;

        float trackWidth = trackEnd.position.x - trackStart.position.x;
        float width = trackWidth * Mathf.Clamp01(value01);
        float startX = fill.parent != null ? fill.parent.InverseTransformPoint(trackStart.position).x : trackStart.localPosition.x;

        fill.localScale = new Vector3(width, fillScaleY, fillScaleZ);
        fill.localPosition = new Vector3(startX + width / 2f, fillFixedY, fillFixedZ);
    }

    private void OnMouseDown()
    {
        dragging = true;
    }

    private void Update()
    {
        if (!dragging)
            return;

        if (!Input.GetMouseButton(0))
        {
            dragging = false;
            return;
        }

        float clampedX = Mathf.Clamp(GetMouseWorldPos().x, trackStart.position.x, trackEnd.position.x);
        transform.position = new Vector3(clampedX, trackStart.position.y + yOffsetFromTrack, trackStart.position.z + zOffsetFromTrack);

        float value01 = (clampedX - trackStart.position.x) / (trackEnd.position.x - trackStart.position.x);
        if (target == Target.Music)
            SettingsManager.Instance.SetMusicVolume(value01);
        else
            SettingsManager.Instance.SetSfxVolume(value01);

        SetFillWidth(value01);
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
