using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanoramaManager : MonoBehaviour
{
    [Header("Assign all panoramas here")]
    public PanoramaData[] panoramas;

    [Header("UI")]
    public RectTransform hotspotParent;      // Panel that will hold hotspot buttons
    public GameObject hotspotButtonPrefab;   // Prefab of the UI Button

    [Header("Transition")]
    public float fadeDuration = 2f;

    private Material currentMaterial;
    private int currentIndex = 0;
    private bool isTransitioning = false;

    private readonly List<Hotspot> activeHotspots = new List<Hotspot>();

    void Start()
    {
        if (panoramas == null || panoramas.Length == 0)
        {
            Debug.LogError("No panoramas assigned in PanoramaManager!");
            return;
        }

        // Make sure the HotspotPanel's image does NOT block clicks
        var panelImg = hotspotParent.GetComponent<Image>();
        if (panelImg != null)
            panelImg.raycastTarget = false;

        // Start with first panorama
        currentMaterial = new Material(panoramas[0].panoramaMaterial);
        ApplySkybox(currentMaterial);
        LoadHotspots(0);
    }

    void Update()
    {
        UpdateHotspotVisibility();
    }

    /// <summary>
    /// Public method used by hotspots to switch to another panorama.
    /// </summary>
    public void SwitchTo(int index)
    {
        Debug.Log("SwitchTo CALLED → " + index);

        if (isTransitioning || index == currentIndex)
        {
            Debug.Log("Already transitioning or same index");
            return;
        }

        if (index < 0 || index >= panoramas.Length)
        {
            Debug.LogError("Invalid index: " + index);
            return;
        }

        StartCoroutine(FadeTransition(index));
    }

    private IEnumerator FadeTransition(int targetIndex)
    {
        isTransitioning = true;

        Material startMat = new Material(currentMaterial);
        Material targetMat = new Material(panoramas[targetIndex].panoramaMaterial);

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / fadeDuration);

            // Interpolate rotation explicitly if you use _Rotation on the skybox shader
            float startRot = startMat.GetFloat("_Rotation");
            float targetRot = targetMat.GetFloat("_Rotation");
            currentMaterial.SetFloat("_Rotation", Mathf.Lerp(startRot, targetRot, p));

            // Interpolate other properties (incl. texture / exposure etc.)
            currentMaterial.Lerp(startMat, targetMat, p);

            ApplySkybox(currentMaterial);
            yield return null;
        }

        currentMaterial = targetMat;
        currentIndex = targetIndex;

        ApplySkybox(currentMaterial);
        LoadHotspots(targetIndex);

        Debug.Log("Transition complete → now at: " + targetIndex);

        isTransitioning = false;
    }

    private void ApplySkybox(Material mat)
    {
        RenderSettings.skybox = mat;

        var sky = Camera.main.GetComponent<Skybox>();
        if (sky != null)
            sky.material = mat;

        DynamicGI.UpdateEnvironment();
    }

    // ---------------- HOTSPOTS ----------------

    private void LoadHotspots(int index)
    {
        // Clear old buttons
        foreach (Transform child in hotspotParent)
            Destroy(child.gameObject);

        activeHotspots.Clear();

        PanoramaData pd = panoramas[index];
        if (pd.hotspots == null)
            return;

        // Create a button for each hotspot
        foreach (var hs in pd.hotspots)
        {
            GameObject btnObj = Instantiate(hotspotButtonPrefab, hotspotParent);

            Text txt = btnObj.GetComponentInChildren<Text>();
            if (txt != null)
                txt.text = hs.label;

            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchoredPosition = hs.anchoredPos;

// NEW: rotate arrow per hotspot
            rt.localEulerAngles = new Vector3(0f, 0f, hs.rotationZ);


            Button b = btnObj.GetComponent<Button>();

            b.onClick.RemoveAllListeners();
            int target = hs.targetIndex;

            b.onClick.AddListener(() =>
            {
                Debug.Log("HOTSPOT CLICKED → Target = " + target);
                SwitchTo(target);
            });

            hs.runtimeButton = btnObj;
            activeHotspots.Add(hs);
        }

        UpdateHotspotVisibility();
    }

    private void UpdateHotspotVisibility()
    {
        if (activeHotspots.Count == 0) return;

        float yaw = Camera.main.transform.eulerAngles.y;

        foreach (var hs in activeHotspots)
        {
            if (hs.runtimeButton == null) continue;

            float delta = Mathf.DeltaAngle(yaw, hs.yaw);
            bool visible = Mathf.Abs(delta) <= hs.viewAngle * 0.5f;

            hs.runtimeButton.SetActive(visible);
        }
    }
}
