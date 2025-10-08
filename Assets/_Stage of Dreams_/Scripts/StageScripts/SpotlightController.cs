using UnityEngine;

/// <summary>
/// Example script demonstrating how to use the focused Spotlight system
/// for movement patterns and character tracking in a theater game.
/// </summary>
public class SpotlightController : MonoBehaviour
{
    [Header("Spotlight References")]
    [SerializeField] private Spotlight mainSpotlight;
    [SerializeField] private Transform[] performers; // Actors/characters to follow
    
    [Header("Performance Settings")]
    [SerializeField] private float performanceTime = 30f;
    [SerializeField] private bool autoStartPerformance = false;
    
    // Performance state
    private int currentPerformerIndex = 0;
    private float performanceStartTime;
    private bool performanceActive = false;
    
    private void Start()
    {
        if (mainSpotlight == null)
            mainSpotlight = FindFirstObjectByType<Spotlight>();
        
        // Subscribe to spotlight events
        if (mainSpotlight != null)
        {
            mainSpotlight.OnCharacterEnteredSpotlight += HandleCharacterEnteredSpotlight;
            mainSpotlight.OnCharacterExitedSpotlight += HandleCharacterExitedSpotlight;
            mainSpotlight.OnSpotlightMoved += HandleSpotlightMoved;
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
        
        // Start by following the first performer
        mainSpotlight.SetTarget(performers[currentPerformerIndex]);
        
        Debug.Log("Performance started! Spotlight following performers.");
    }
    
    /// <summary>
    /// Stop the current performance
    /// </summary>
    [ContextMenu("Stop Performance")]
    public void StopPerformance()
    {
        performanceActive = false;
        mainSpotlight.StopMovement();
        
        Debug.Log("Performance stopped. Spotlight set to static.");
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
    
    #endregion
    
    #region Test Input (Remove in production)
    
    /// <summary>
    /// Handle test input for spotlight control
    /// </summary>
    private void HandleTestInput()
    {
        if (!Application.isPlaying) return;
        
        // Number keys to test different movement patterns
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
    
    #endregion
}