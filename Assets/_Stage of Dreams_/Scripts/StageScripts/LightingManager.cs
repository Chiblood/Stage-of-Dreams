/* LightingManager.cs
 * Manages global lighting and darkness in the theater environment.
 * Controls ambient lighting, global Light2D, and overall scene brightness.
 * Can be used to create dramatic lighting effects for theatrical performances.
 */

using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Manages global lighting and darkness for theatrical effects.
/// Controls ambient lighting, global lights, and scene brightness.
/// </summary>
public class LightingManager : MonoBehaviour
{
    [Header("Global Lighting")]
    [SerializeField] private Light2D globalLight; // Main global light (usually set to Global type)
    [SerializeField] private Camera mainCamera; // Reference to main camera for background color
    
    [Header("Lighting States")]
    [SerializeField] private float normalAmbientIntensity = 1f; // Normal lighting intensity
    [SerializeField] private float darkAmbientIntensity = 0.1f; // Dark/dramatic lighting intensity
    [SerializeField] private Color normalBackgroundColor = Color.black; // Normal background color
    [SerializeField] private Color darkBackgroundColor = Color.black; // Dark background color
    
    [Header("Transition Settings")]
    [SerializeField] private float transitionSpeed = 2f; // Speed of lighting transitions
    [SerializeField] private bool startInDarkMode = false; // Whether to start with dramatic lighting
    
    [Header("Named Spotlights (For Dialog Events)")]
    [SerializeField] private Spotlight[] namedSpotlights; // Array of spotlights for direct dialog access
    
    // Current state
    private bool isDarkMode = false;
    private bool isTransitioning = false;
    private float currentIntensity;
    private float targetIntensity;
    private Color currentBackgroundColor;
    private Color targetBackgroundColor;
    
    // Events
    public System.Action OnDarkModeEnabled;
    public System.Action OnDarkModeDisabled;
    public System.Action OnLightingTransitionComplete;
    
    private void Awake()
    {
        // Find components if not assigned
        if (globalLight == null)
        {
            globalLight = FindFirstObjectByType<Light2D>();
            if (globalLight != null && globalLight.lightType != Light2D.LightType.Global)
            {
                // Look for a Global light specifically
                Light2D[] allLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
                foreach (var light in allLights)
                {
                    if (light.lightType == Light2D.LightType.Global)
                    {
                        globalLight = light;
                        break;
                    }
                }
            }
        }
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                mainCamera = FindFirstObjectByType<Camera>();
            }
        }
        
        // Auto-populate named spotlights if empty
        if (namedSpotlights == null || namedSpotlights.Length == 0)
        {
            RefreshNamedSpotlights();
        }
        
        // Initialize state
        isDarkMode = startInDarkMode;
        currentIntensity = isDarkMode ? darkAmbientIntensity : normalAmbientIntensity;
        targetIntensity = currentIntensity;
        currentBackgroundColor = isDarkMode ? darkBackgroundColor : normalBackgroundColor;
        targetBackgroundColor = currentBackgroundColor;
        
        // Apply initial state
        ApplyLightingImmediate();
    }
    
    private void Update()
    {
        if (isTransitioning)
        {
            UpdateLightingTransition();
        }
    }
    
    #region Public Interface
    
    /// <summary>
    /// Enable dark/dramatic lighting mode
    /// Can be called by Dialog Nodes and Choices
    /// </summary>
    public void EnableDarkMode()
    {
        if (isDarkMode && !isTransitioning) return;
        
        Debug.Log("Enabling dark lighting mode");
        
        isDarkMode = true;
        targetIntensity = darkAmbientIntensity;
        targetBackgroundColor = darkBackgroundColor;
        isTransitioning = true;
        
        OnDarkModeEnabled?.Invoke();
    }
    
    /// <summary>
    /// Disable dark mode and return to normal lighting
    /// Can be called by Dialog Nodes and Choices
    /// </summary>
    public void DisableDarkMode()
    {
        if (!isDarkMode && !isTransitioning) return;
        
        Debug.Log("Disabling dark lighting mode");
        
        isDarkMode = false;
        targetIntensity = normalAmbientIntensity;
        targetBackgroundColor = normalBackgroundColor;
        isTransitioning = true;
        
        OnDarkModeDisabled?.Invoke();
    }
    
    /// <summary>
    /// Toggle between dark and normal lighting modes
    /// </summary>
    public void ToggleDarkMode()
    {
        if (isDarkMode)
            DisableDarkMode();
        else
            EnableDarkMode();
    }
    
    /// <summary>
    /// Instantly set dark mode without transition
    /// </summary>
    public void SetDarkModeInstant(bool darkMode)
    {
        isDarkMode = darkMode;
        currentIntensity = darkMode ? darkAmbientIntensity : normalAmbientIntensity;
        targetIntensity = currentIntensity;
        currentBackgroundColor = darkMode ? darkBackgroundColor : normalBackgroundColor;
        targetBackgroundColor = currentBackgroundColor;
        
        ApplyLightingImmediate();
        
        if (darkMode)
            OnDarkModeEnabled?.Invoke();
        else
            OnDarkModeDisabled?.Invoke();
        
        OnLightingTransitionComplete?.Invoke();
    }
    
    /// <summary>
    /// Set custom ambient lighting intensity
    /// </summary>
    public void SetAmbientIntensity(float intensity)
    {
        if (globalLight != null)
        {
            globalLight.intensity = intensity;
            currentIntensity = intensity;
            targetIntensity = intensity;
        }
    }
    
    /// <summary>
    /// Set custom background color
    /// </summary>
    public void SetBackgroundColor(Color color)
    {
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = color;
            currentBackgroundColor = color;
            targetBackgroundColor = color;
        }
    }
    
    /// <summary>
    /// Get current lighting state
    /// </summary>
    public bool IsDarkMode()
    {
        return isDarkMode;
    }
    
    /// <summary>
    /// Check if lighting is currently transitioning
    /// </summary>
    public bool IsTransitioning()
    {
        return isTransitioning;
    }
    
    #endregion
    
    #region Named Spotlight Control (For Dialog Events)
    
    /// <summary>
    /// Show a specific spotlight by index in the named spotlights array
    /// Can be called by Dialog Nodes and Choices
    /// </summary>
    public void ShowSpotlight1() { ShowSpotlightByIndex(0); }
    public void ShowSpotlight2() { ShowSpotlightByIndex(1); }
    public void ShowSpotlight3() { ShowSpotlightByIndex(2); }
    public void ShowSpotlight4() { ShowSpotlightByIndex(3); }
    public void ShowSpotlight5() { ShowSpotlightByIndex(4); }
    
    /// <summary>
    /// Hide a specific spotlight by index in the named spotlights array
    /// Can be called by Dialog Nodes and Choices
    /// </summary>
    public void HideSpotlight1() { HideSpotlightByIndex(0); }
    public void HideSpotlight2() { HideSpotlightByIndex(1); }
    public void HideSpotlight3() { HideSpotlightByIndex(2); }
    public void HideSpotlight4() { HideSpotlightByIndex(3); }
    public void HideSpotlight5() { HideSpotlightByIndex(4); }
    
    // Descriptive Named Methods (These will appear clearly in Unity Inspector dropdown)
    /// <summary>Show the main stage spotlight (first in array)</summary>
    public void ShowMainStageSpotlight() { ShowSpotlightByIndex(0); }
    /// <summary>Show the left side spotlight (second in array)</summary>
    public void ShowLeftSideSpotlight() { ShowSpotlightByIndex(1); }
    /// <summary>Show the right side spotlight (third in array)</summary>
    public void ShowRightSideSpotlight() { ShowSpotlightByIndex(2); }
    /// <summary>Show the back spotlight (fourth in array)</summary>
    public void ShowBackSpotlight() { ShowSpotlightByIndex(3); }
    
    /// <summary>Hide the main stage spotlight (first in array)</summary>
    public void HideMainStageSpotlight() { HideSpotlightByIndex(0); }
    /// <summary>Hide the left side spotlight (second in array)</summary>
    public void HideLeftSideSpotlight() { HideSpotlightByIndex(1); }
    /// <summary>Hide the right side spotlight (third in array)</summary>
    public void HideRightSideSpotlight() { HideSpotlightByIndex(2); }
    /// <summary>Hide the back spotlight (fourth in array)</summary>
    public void HideBackSpotlight() { HideSpotlightByIndex(3); }
    
    /// <summary>
    /// Show spotlight by index
    /// </summary>
    private void ShowSpotlightByIndex(int index)
    {
        if (namedSpotlights != null && index >= 0 && index < namedSpotlights.Length && namedSpotlights[index] != null)
        {
            namedSpotlights[index].AppearSpotlight();
            Debug.Log($"Dialog triggered spotlight {index + 1} ({namedSpotlights[index].name}) to appear");
        }
        else
        {
            Debug.LogWarning($"Cannot show spotlight {index + 1} - not found in named spotlights array");
        }
    }
    
    /// <summary>
    /// Hide spotlight by index
    /// </summary>
    private void HideSpotlightByIndex(int index)
    {
        if (namedSpotlights != null && index >= 0 && index < namedSpotlights.Length && namedSpotlights[index] != null)
        {
            namedSpotlights[index].DisappearSpotlight();
            Debug.Log($"Dialog triggered spotlight {index + 1} ({namedSpotlights[index].name}) to disappear");
        }
        else
        {
            Debug.LogWarning($"Cannot hide spotlight {index + 1} - not found in named spotlights array");
        }
    }
    
    /// <summary>
    /// Show all named spotlights
    /// Can be called by Dialog Nodes and Choices
    /// </summary>
    public void ShowAllNamedSpotlights()
    {
        if (namedSpotlights == null) return;
        
        foreach (var spotlight in namedSpotlights)
        {
            if (spotlight != null)
                spotlight.AppearSpotlight();
        }
        Debug.Log("Dialog triggered all named spotlights to appear");
    }
    
    /// <summary>
    /// Hide all named spotlights
    /// Can be called by Dialog Nodes and Choices
    /// </summary>
    public void HideAllNamedSpotlights()
    {
        if (namedSpotlights == null) return;
        
        foreach (var spotlight in namedSpotlights)
        {
            if (spotlight != null)
                spotlight.DisappearSpotlight();
        }
        Debug.Log("Dialog triggered all named spotlights to disappear");
    }
    
    /// <summary>
    /// Refresh the named spotlights array with all spotlights in the scene
    /// </summary>
    [ContextMenu("Refresh Named Spotlights")]
    public void RefreshNamedSpotlights()
    {
        var allSpotlights = FindObjectsByType<Spotlight>(FindObjectsSortMode.InstanceID);
        namedSpotlights = allSpotlights;
        Debug.Log($"Found and assigned {namedSpotlights.Length} spotlights for dialog control");
    }
    
    #endregion
    
    #region Private Methods
    
    /// <summary>
    /// Update lighting transition animation
    /// </summary>
    private void UpdateLightingTransition()
    {
        bool intensityComplete = false;
        bool colorComplete = false;
        
        // Transition intensity
        if (!Mathf.Approximately(currentIntensity, targetIntensity))
        {
            currentIntensity = Mathf.MoveTowards(currentIntensity, targetIntensity, transitionSpeed * Time.deltaTime);
            if (globalLight != null)
            {
                globalLight.intensity = currentIntensity;
            }
        }
        else
        {
            intensityComplete = true;
        }
        
        // Transition background color
        if (currentBackgroundColor != targetBackgroundColor)
        {
            currentBackgroundColor = Color.Lerp(currentBackgroundColor, targetBackgroundColor, transitionSpeed * Time.deltaTime);
            if (mainCamera != null)
            {
                mainCamera.backgroundColor = currentBackgroundColor;
            }
            
            // Check if close enough to target
            if (Vector4.Distance(currentBackgroundColor, targetBackgroundColor) < 0.01f)
            {
                currentBackgroundColor = targetBackgroundColor;
                if (mainCamera != null)
                {
                    mainCamera.backgroundColor = currentBackgroundColor;
                }
                colorComplete = true;
            }
        }
        else
        {
            colorComplete = true;
        }
        
        // Check if transition is complete
        if (intensityComplete && colorComplete)
        {
            isTransitioning = false;
            OnLightingTransitionComplete?.Invoke();
            Debug.Log($"Lighting transition complete - Dark Mode: {isDarkMode}");
        }
    }
    
    /// <summary>
    /// Apply lighting settings immediately without transition
    /// </summary>
    private void ApplyLightingImmediate()
    {
        if (globalLight != null)
        {
            globalLight.intensity = currentIntensity;
        }
        
        if (mainCamera != null)
        {
            mainCamera.backgroundColor = currentBackgroundColor;
        }
    }
    
    #endregion
    
    #region Editor Utilities
    
    /// <summary>
    /// Set up default lighting configuration
    /// </summary>
    [ContextMenu("Setup Default Lighting")]
    public void SetupDefaultLighting()
    {
        // Try to find or create global light
        if (globalLight == null)
        {
            Light2D[] allLights = FindObjectsByType<Light2D>(FindObjectsSortMode.None);
            foreach (var light in allLights)
            {
                if (light.lightType == Light2D.LightType.Global)
                {
                    globalLight = light;
                    break;
                }
            }
            
            if (globalLight == null)
            {
                Debug.LogWarning("No Global Light2D found. Please create one for proper lighting control.");
            }
        }
        
        // Set reasonable defaults
        normalAmbientIntensity = 1f;
        darkAmbientIntensity = 0.1f;
        normalBackgroundColor = Color.black;
        darkBackgroundColor = Color.black;
        
        // Refresh named spotlights
        RefreshNamedSpotlights();
        
        Debug.Log("Default lighting configuration applied.");
    }
    
    /// <summary>
    /// Test dark mode toggle
    /// </summary>
    [ContextMenu("Toggle Dark Mode (Test)")]
    public void TestToggleDarkMode()
    {
        ToggleDarkMode();
    }
    
    #endregion
}