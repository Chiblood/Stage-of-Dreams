using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Example script demonstrating how to use the focused Spotlight system
/// for movement patterns and character tracking in a theater game.
/// Now includes Light2D integration and lighting management.
/// </summary>
public class SpotlightController : MonoBehaviour
{
    [Header("Spotlight References")]
    [SerializeField] private Spotlight mainSpotlight;
    [SerializeField] private Transform[] performers; // Actors/characters to follow
    
    [Header("Performance Settings")]
    [SerializeField] private float performanceTime = 30f;
    [SerializeField] private bool autoStartPerformance = false;
    
    [Header("Dialog Integration")]
    [SerializeField] private Spotlight[] spotlightsForDialog; // Spotlights that can be controlled by dialogs
    
    [Header("Lighting Control")]
    [SerializeField] private LightingManager lightingManager; // Global lighting control
    [SerializeField] private bool enableLightingEffects = true; // Whether to control global lighting
    
    // Performance state
    private int currentPerformerIndex = 0;
    private float performanceStartTime;
    private bool performanceActive = false;
    
    private void Start()
    {
        if (mainSpotlight == null)
            mainSpotlight = FindFirstObjectByType<Spotlight>();
        
        if (lightingManager == null)
            lightingManager = FindFirstObjectByType<LightingManager>();
        
        // Subscribe to spotlight events
        if (mainSpotlight != null)
        {
            mainSpotlight.OnCharacterEnteredSpotlight += HandleCharacterEnteredSpotlight;
            mainSpotlight.OnCharacterExitedSpotlight += HandleCharacterExitedSpotlight;
            mainSpotlight.OnSpotlightMoved += HandleSpotlightMoved;
            mainSpotlight.OnSpotlightAppeared += HandleSpotlightAppeared;
            mainSpotlight.OnSpotlightDisappeared += HandleSpotlightDisappeared;
        }
        
        // Subscribe to all dialog-controlled spotlights
        foreach (var spotlight in spotlightsForDialog)
        {
            if (spotlight != null)
            {
                spotlight.OnSpotlightAppeared += () => Debug.Log($"Dialog spotlight {spotlight.name} appeared");
                spotlight.OnSpotlightDisappeared += () => Debug.Log($"Dialog spotlight {spotlight.name} disappeared");
            }
        }
        
        // Subscribe to lighting manager events
        if (lightingManager != null && enableLightingEffects)
        {
            lightingManager.OnDarkModeEnabled += HandleDarkModeEnabled;
            lightingManager.OnDarkModeDisabled += HandleDarkModeDisabled;
            lightingManager.OnLightingTransitionComplete += HandleLightingTransitionComplete;
        }
        
        if (autoStartPerformance)
        {
            StartPerformance();
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (mainSpotlight != null)
        {
            mainSpotlight.OnCharacterEnteredSpotlight -= HandleCharacterEnteredSpotlight;
            mainSpotlight.OnCharacterExitedSpotlight -= HandleCharacterExitedSpotlight;
            mainSpotlight.OnSpotlightMoved -= HandleSpotlightMoved;
            mainSpotlight.OnSpotlightAppeared -= HandleSpotlightAppeared;
            mainSpotlight.OnSpotlightDisappeared -= HandleSpotlightDisappeared;
        }
        
        if (lightingManager != null)
        {
            lightingManager.OnDarkModeEnabled -= HandleDarkModeEnabled;
            lightingManager.OnDarkModeDisabled -= HandleDarkModeDisabled;
            lightingManager.OnLightingTransitionComplete -= HandleLightingTransitionComplete;
        }
    }
    
    private void Update()
    {
        if (performanceActive)
        {
            UpdatePerformance();
        }
        
        // Example input handling for testing
        HandleTestInput();
    }
    
    #region Dialog Integration Methods - These can be called by Dialog Nodes and Choices
    
    /// <summary>
    /// Show a specific spotlight by name (for use in Dialog Node events)
    /// </summary>
    public void ShowSpotlightByName(string spotlightName)
    {
        Spotlight spotlight = FindSpotlightByName(spotlightName);
        if (spotlight != null)
        {
            spotlight.AppearSpotlight();
            Debug.Log($"Dialog triggered spotlight '{spotlightName}' to appear");
        }
        else
        {
            Debug.LogWarning($"Could not find spotlight with name '{spotlightName}'");
        }
    }
    
    /// <summary>
    /// Hide a specific spotlight by name (for use in Dialog Node events)
    /// </summary>
    public void HideSpotlightByName(string spotlightName)
    {
        Spotlight spotlight = FindSpotlightByName(spotlightName);
        if (spotlight != null)
        {
            spotlight.DisappearSpotlight();
            Debug.Log($"Dialog triggered spotlight '{spotlightName}' to disappear");
        }
        else
        {
            Debug.LogWarning($"Could not find spotlight with name '{spotlightName}'");
        }
    }
    
    // Individual Spotlight Control Methods (These will appear in Unity Inspector dropdown)
    /// <summary>Show Spotlight1 (for Unity Inspector UnityEvent dropdown)</summary>
    public void ShowSpotlight1() { ShowSpotlightByName("Spotlight1"); }
    /// <summary>Show Spotlight2 (for Unity Inspector UnityEvent dropdown)</summary>
    public void ShowSpotlight2() { ShowSpotlightByName("Spotlight2"); }
    /// <summary>Show Spotlight3 (for Unity Inspector UnityEvent dropdown)</summary>
    public void ShowSpotlight3() { ShowSpotlightByName("Spotlight3"); }
    /// <summary>Show specific spotlight by GameObject name</summary>
    public void ShowMainStageSpotlight() { ShowSpotlightByName("MainStageSpotlight"); }
    /// <summary>Show specific spotlight by GameObject name</summary>
    public void ShowLeftSpotlight() { ShowSpotlightByName("LeftSpotlight"); }
    /// <summary>Show specific spotlight by GameObject name</summary>
    public void ShowRightSpotlight() { ShowSpotlightByName("RightSpotlight"); }
    
    /// <summary>Hide Spotlight1 (for Unity Inspector UnityEvent dropdown)</summary>
    public void HideSpotlight1() { HideSpotlightByName("Spotlight1"); }
    /// <summary>Hide Spotlight2 (for Unity Inspector UnityEvent dropdown)</summary>
    public void HideSpotlight2() { HideSpotlightByName("Spotlight2"); }
    /// <summary>Hide Spotlight3 (for Unity Inspector UnityEvent dropdown)</summary>
    public void HideSpotlight3() { HideSpotlightByName("Spotlight3"); }
    /// <summary>Hide specific spotlight by GameObject name</summary>
    public void HideMainStageSpotlight() { HideSpotlightByName("MainStageSpotlight"); }
    /// <summary>Hide specific spotlight by GameObject name</summary>
    public void HideLeftSpotlight() { HideSpotlightByName("LeftSpotlight"); }
    /// <summary>Hide specific spotlight by GameObject name</summary>
    public void HideRightSpotlight() { HideSpotlightByName("RightSpotlight"); }
    
    // Color Control Methods (These will appear in Unity Inspector dropdown)
    /// <summary>Set spotlight to red color (for Unity Inspector UnityEvent dropdown)</summary>
    public void SetSpotlightRedColor() { SetSpotlightColor("MainSpotlight", Color.red); }
    /// <summary>Set spotlight to blue color (for Unity Inspector UnityEvent dropdown)</summary>
    public void SetSpotlightBlueColor() { SetSpotlightColor("MainSpotlight", Color.blue); }
    /// <summary>Set spotlight to white color (for Unity Inspector UnityEvent dropdown)</summary>
    public void SetSpotlightWhiteColor() { SetSpotlightColor("MainSpotlight", Color.white); }
    /// <summary>Set spotlight to yellow color (for Unity Inspector UnityEvent dropdown)</summary>
    public void SetSpotlightYellowColor() { SetSpotlightColor("MainSpotlight", Color.yellow); }
    
    // Intensity Control Methods (These will appear in Unity Inspector dropdown)
    /// <summary>Set spotlight to dim intensity (for Unity Inspector UnityEvent dropdown)</summary>
    public void SetSpotlightDim() { SetSpotlightIntensity("MainSpotlight", 10f); }
    /// <summary>Set spotlight to normal intensity (for Unity Inspector UnityEvent dropdown)</summary>
    public void SetSpotlightNormal() { SetSpotlightIntensity("MainSpotlight", 20f); }
    /// <summary>Set spotlight to bright intensity (for Unity Inspector UnityEvent dropdown)</summary>
    public void SetSpotlightBright() { SetSpotlightIntensity("MainSpotlight", 30f); }
    
    /// <summary>
    /// Show the main spotlight (for use in Dialog Node events)
    /// </summary>
    public void ShowMainSpotlight()
    {
        if (mainSpotlight != null)
        {
            mainSpotlight.AppearSpotlight();
            Debug.Log("Dialog triggered main spotlight to appear");
        }
    }
    
    /// <summary>
    /// Hide the main spotlight (for use in Dialog Node events)
    /// </summary>
    public void HideMainSpotlight()
    {
        if (mainSpotlight != null)
        {
            mainSpotlight.DisappearSpotlight();
            Debug.Log("Dialog triggered main spotlight to disappear");
        }
    }
    
    /// <summary>
    /// Show all dialog-controlled spotlights
    /// </summary>
    public void ShowAllSpotlights()
    {
        foreach (var spotlight in spotlightsForDialog)
        {
            if (spotlight != null)
                spotlight.AppearSpotlight();
        }
        Debug.Log("Dialog triggered all spotlights to appear");
    }
    
    /// <summary>
    /// Hide all dialog-controlled spotlights
    /// </summary>
    public void HideAllSpotlights()
    {
        foreach (var spotlight in spotlightsForDialog)
        {
            if (spotlight != null)
                spotlight.DisappearSpotlight();
        }
        Debug.Log("Dialog triggered all spotlights to disappear");
    }
    
    /// <summary>
    /// Show spotlight instantly by name (no fade animation)
    /// </summary>
    public void ShowSpotlightInstantByName(string spotlightName)
    {
        Spotlight spotlight = FindSpotlightByName(spotlightName);
        if (spotlight != null)
        {
            spotlight.ShowSpotlightInstant();
            Debug.Log($"Dialog instantly showed spotlight '{spotlightName}'");
        }
    }
    
    /// <summary>
    /// Hide spotlight instantly by name (no fade animation)
    /// </summary>
    public void HideSpotlightInstantByName(string spotlightName)
    {
        Spotlight spotlight = FindSpotlightByName(spotlightName);
        if (spotlight != null)
        {
            spotlight.HideSpotlightInstant();
            Debug.Log($"Dialog instantly hid spotlight '{spotlightName}'");
        }
    }
    
    /// <summary>
    /// Set spotlight color by name (for use in Dialog Node events)
    /// </summary>
    public void SetSpotlightColor(string spotlightName, Color color)
    {
        Spotlight spotlight = FindSpotlightByName(spotlightName);
        if (spotlight != null)
        {
            spotlight.SetColor(color);
            Debug.Log($"Dialog set spotlight '{spotlightName}' color to {color}");
        }
    }
    
    /// <summary>
    /// Set spotlight intensity by name (for use in Dialog Node events)
    /// </summary>
    public void SetSpotlightIntensity(string spotlightName, float intensity)
    {
        Spotlight spotlight = FindSpotlightByName(spotlightName);
        if (spotlight != null)
        {
            spotlight.SetIntensity(intensity);
            Debug.Log($"Dialog set spotlight '{spotlightName}' intensity to {intensity}");
        }
    }
    
    #endregion
    
    #region Lighting Control Methods - For Dialog Integration
    
    /// <summary>
    /// Enable dramatic/dark lighting mode (for use in Dialog Node events)
    /// </summary>
    public void EnableDarkLighting()
    {
        if (lightingManager != null)
        {
            lightingManager.EnableDarkMode();
            Debug.Log("Dialog enabled dark lighting mode");
        }
    }
    
    /// <summary>
    /// Disable dark lighting and return to normal (for use in Dialog Node events)
    /// </summary>
    public void DisableDarkLighting()
    {
        if (lightingManager != null)
        {
            lightingManager.DisableDarkMode();
            Debug.Log("Dialog disabled dark lighting mode");
        }
    }
    
    /// <summary>
    /// Toggle between dark and normal lighting (for use in Dialog Node events)
    /// </summary>
    public void ToggleDarkLighting()
    {
        if (lightingManager != null)
        {
            lightingManager.ToggleDarkMode();
            Debug.Log("Dialog toggled lighting mode");
        }
    }
    
    /// <summary>
    /// Set custom ambient lighting intensity (for use in Dialog Node events)
    /// </summary>
    public void SetAmbientLighting(float intensity)
    {
        if (lightingManager != null)
        {
            lightingManager.SetAmbientIntensity(intensity);
            Debug.Log($"Dialog set ambient lighting to {intensity}");
        }
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Find a spotlight by its GameObject name
    /// </summary>
    private Spotlight FindSpotlightByName(string name)
    {
        // First check main spotlight
        if (mainSpotlight != null && mainSpotlight.gameObject.name == name)
            return mainSpotlight;
        
        // Check dialog-controlled spotlights
        foreach (var spotlight in spotlightsForDialog)
        {
            if (spotlight != null && spotlight.gameObject.name == name)
                return spotlight;
        }
        
        // As fallback, search all spotlights in scene
        Spotlight[] allSpotlights = FindObjectsByType<Spotlight>(FindObjectsSortMode.None);
        foreach (var spotlight in allSpotlights)
        {
            if (spotlight.gameObject.name == name)
                return spotlight;
        }
        
        return null;
    }
    
    #endregion
    
    #region Performance Management
    
    /// <summary>
    /// Start a performance sequence that follows different performers
    /// </summary>
    [ContextMenu("Start Performance")]
    public void StartPerformance()
    {
        if (mainSpotlight == null || performers.Length == 0) return;
        
        performanceActive = true;
        performanceStartTime = Time.time;
        currentPerformerIndex = 0;
        
        // Enable dramatic lighting for performance
        if (enableLightingEffects && lightingManager != null)
        {
            lightingManager.EnableDarkMode();
        }
        
        // Ensure main spotlight is visible for performance
        mainSpotlight.AppearSpotlight();
        
        // Start by following the first performer
        mainSpotlight.SetTarget(performers[currentPerformerIndex]);
        
        Debug.Log("Performance started! Spotlight following performers with dramatic lighting.");
    }
    
    /// <summary>
    /// Stop the current performance
    /// </summary>
    [ContextMenu("Stop Performance")]
    public void StopPerformance()
    {
        performanceActive = false;
        mainSpotlight.StopMovement();
        
        // Return to normal lighting
        if (enableLightingEffects && lightingManager != null)
        {
            lightingManager.DisableDarkMode();
        }
        
        Debug.Log("Performance stopped. Spotlight set to static and normal lighting restored.");
    }
    
    /// <summary>
    /// Update performance logic
    /// </summary>
    private void UpdatePerformance()
    {
        float timeElapsed = Time.time - performanceStartTime;
        
        // Switch between performers based on time or other logic
        if (timeElapsed > performanceTime / performers.Length * (currentPerformerIndex + 1))
        {
            SwitchToNextPerformer();
        }
        
        // End performance after total time
        if (timeElapsed > performanceTime)
        {
            StopPerformance();
        }
    }
    
    /// <summary>
    /// Switch spotlight to follow the next performer
    /// </summary>
    private void SwitchToNextPerformer()
    {
        currentPerformerIndex = (currentPerformerIndex + 1) % performers.Length;
        mainSpotlight.SetTarget(performers[currentPerformerIndex]);
        
        Debug.Log($"Spotlight now following performer {currentPerformerIndex}: {performers[currentPerformerIndex].name}");
    }
    
    #endregion
    
    #region Movement Pattern Examples
    
    /// <summary>
    /// Start random spotlight movement for dramatic effect
    /// </summary>
    [ContextMenu("Start Random Movement")]
    public void StartRandomMovement()
    {
        if (mainSpotlight == null) return;
        
        Vector2 stageSize = new Vector2(10f, 6f); // Adjust based on your stage size
        mainSpotlight.StartRandomMovement(stageSize);
        
        Debug.Log("Spotlight started random movement across the stage.");
    }
    
    /// <summary>
    /// Move spotlight to a specific position
    /// </summary>
    public void MoveSpotlightToPosition(Vector2 position)
    {
        if (mainSpotlight == null) return;
        
        mainSpotlight.StopMovement();
        mainSpotlight.MoveToPosition(position);
        
        Debug.Log($"Spotlight moved to position: {position}");
    }
    
    /// <summary>
    /// Follow a specific character
    /// </summary>
    public void FollowCharacter(Transform character)
    {
        if (mainSpotlight == null || character == null) return;
        
        mainSpotlight.SetTarget(character);
        
        Debug.Log($"Spotlight now following: {character.name}");
    }
    
    #endregion
    
    #region Event Handlers
    
    /// <summary>
    /// Handle when a character enters the spotlight
    /// </summary>
    private void HandleCharacterEnteredSpotlight(ISpotlightCharacter character)
    {
        Debug.Log($"?? {character.GetCharacterName()} is now in the spotlight!");
        
        // Example: Trigger special effects, music changes, etc.
        // EffectsManager.Instance?.TriggerSpotlightEffects(character);
        // AudioManager.Instance?.PlaySpotlightMusic();
        
        // Example: Check if this character should trigger dialog
        // DialogueTrigger trigger = GetDialogTriggerForCharacter(character);
        // if (trigger != null && trigger.triggerOnSpotlight)
        // {
        //     trigger.ManualTrigger();
        // }
    }
    
    /// <summary>
    /// Handle when a character exits the spotlight
    /// </summary>
    private void HandleCharacterExitedSpotlight(ISpotlightCharacter character)
    {
        Debug.Log($"?? {character.GetCharacterName()} left the spotlight.");
        
        // Example: Stop special effects, restore normal lighting, etc.
        // EffectsManager.Instance?.StopSpotlightEffects(character);
    }
    
    /// <summary>
    /// Handle when the spotlight moves
    /// </summary>
    private void HandleSpotlightMoved(Vector2 newPosition)
    {
        // Example: Update lighting effects, camera follow, etc.
        // LightingManager.Instance?.UpdateSpotlightPosition(newPosition);
        // CameraController.Instance?.TrackSpotlight(newPosition);
    }
    
    /// <summary>
    /// Handle when the spotlight appears
    /// </summary>
    private void HandleSpotlightAppeared()
    {
        Debug.Log("?? Main spotlight has appeared!");
        
        // Example: Play spotlight appearance sound, update UI, etc.
        // AudioManager.Instance?.PlaySpotlightAppearSound();
        // UIManager.Instance?.ShowSpotlightActive();
    }
    
    /// <summary>
    /// Handle when the spotlight disappears
    /// </summary>
    private void HandleSpotlightDisappeared()
    {
        Debug.Log("?? Main spotlight has disappeared!");
        
        // Example: Play spotlight disappearance sound, update UI, etc.
        // AudioManager.Instance?.PlaySpotlightDisappearSound();
        // UIManager.Instance?.HideSpotlightActive();
    }
    
    /// <summary>
    /// Handle when dark lighting mode is enabled
    /// </summary>
    private void HandleDarkModeEnabled()
    {
        Debug.Log("?? Dark lighting mode enabled - dramatic atmosphere activated!");
        
        // Example: Trigger atmospheric effects, change music, etc.
        // AudioManager.Instance?.PlayDramaticMusic();
        // EffectsManager.Instance?.EnableDramaticEffects();
    }
    
    /// <summary>
    /// Handle when dark lighting mode is disabled
    /// </summary>
    private void HandleDarkModeDisabled()
    {
        Debug.Log("?? Normal lighting restored - atmosphere normalized.");
        
        // Example: Return to normal effects and music
        // AudioManager.Instance?.PlayNormalMusic();
        // EffectsManager.Instance?.DisableDramaticEffects();
    }
    
    /// <summary>
    /// Handle when lighting transition is complete
    /// </summary>
    private void HandleLightingTransitionComplete()
    {
        Debug.Log("? Lighting transition complete!");
        
        // Example: Trigger events that should happen after lighting changes
        // EventManager.Instance?.TriggerLightingTransitionComplete();
    }
    
    #endregion
    
    #region Test Input (Remove in production)
    
    /// <summary>
    /// Handle test input for spotlight and lighting control
    /// </summary>
    private void HandleTestInput()
    {
        if (!Application.isPlaying) return;
        
        // Spotlight movement tests
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            mainSpotlight?.StopMovement();
            Debug.Log("Test: Spotlight set to static");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartRandomMovement();
            Debug.Log("Test: Spotlight random movement");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3) && performers.Length > 0)
        {
            FollowCharacter(performers[0]);
            Debug.Log("Test: Spotlight following first performer");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartPerformance();
            Debug.Log("Test: Performance started");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StopPerformance();
            Debug.Log("Test: Performance stopped");
        }
        
        // Spotlight visibility tests
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            mainSpotlight?.AppearSpotlight();
            Debug.Log("Test: Spotlight appear");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            mainSpotlight?.DisappearSpotlight();
            Debug.Log("Test: Spotlight disappear");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            mainSpotlight?.ToggleSpotlight();
            Debug.Log("Test: Spotlight toggle");
        }
        
        // Lighting tests
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ToggleDarkLighting();
            Debug.Log("Test: Toggle dark lighting");
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (lightingManager != null)
            {
                lightingManager.SetDarkModeInstant(!lightingManager.IsDarkMode());
                Debug.Log("Test: Instant lighting toggle");
            }
        }
    }
    
    #endregion
    
    #region Public Interface
    
    /// <summary>
    /// Check if any character is currently in the spotlight
    /// </summary>
    public bool HasCharacterInSpotlight()
    {
        return mainSpotlight?.HasCharacterInSpotlight() ?? false;
    }
    
    /// <summary>
    /// Get all characters currently in the spotlight
    /// </summary>
    public ISpotlightCharacter[] GetCharactersInSpotlight()
    {
        return mainSpotlight?.GetCharactersInSpotlight() ?? new ISpotlightCharacter[0];
    }
    
    /// <summary>
    /// Set the spotlight to follow the player
    /// </summary>
    public void FollowPlayer()
    {
        var player = FindFirstObjectByType<PlayerScript>();
        if (player != null)
        {
            FollowCharacter(player.transform);
        }
    }
    
    /// <summary>
    /// Get the current lighting state
    /// </summary>
    public bool IsDarkLightingEnabled()
    {
        return lightingManager?.IsDarkMode() ?? false;
    }
    
    #endregion
}