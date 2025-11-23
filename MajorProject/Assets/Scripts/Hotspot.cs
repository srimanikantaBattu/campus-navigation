using UnityEngine;

[System.Serializable]
public class Hotspot
{
    [Header("UI")]
    public string label;          // Text on the button
    public int targetIndex;       // Which panorama to go to
    public Vector2 anchoredPos;   // Button position on screen

    [Header("Direction control")]
    [Tooltip("Direction (Y rotation in degrees) where this hotspot should be visible.")]
    public float yaw;             // 0–360 degrees

    [Tooltip("How wide the visible cone is around this direction (degrees).")]
    public float viewAngle = 40f; // e.g. 40 means +/-20 degrees

    // runtime fields (filled in by PanoramaManager)
    [System.NonSerialized] public GameObject runtimeButton;
}
