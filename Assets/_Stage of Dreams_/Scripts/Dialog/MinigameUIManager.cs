/* MinigameUIManager.cs
 * Manages minigame UI display and coordinates with DialogNavigator events.
 * Handles transitions between standard dialog and minigame interfaces.
 * 
 * How to use:
 * 1. This component is automatically added to DialogManager GameObject
 * 2. DialogManager calls Initialize() passing navigator and dialogBox references
 * 3. Subscribes to DialogNavigator minigame events automatically
 * 4. Displays appropriate minigame UI when events fire
 * 
 * Assembly-CSharp
 */

using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

/// <summary>
/// Manages minigame UI display and coordinates with DialogNavigator events.
/// Handles transitions between standard dialog and minigame interfaces.
/// Event-driven architecture - listens to DialogNavigator and updates UI.
/// </summary>
public class MinigameUIManager : MonoBehaviour
{
    #region Editor Fields
    [Header("UI References")]
    [SerializeField] private GroupBox dialogBox; // Reference to existing DialogBox from DialogManager
    
    [Header("Settings")]
    [SerializeField] private float transitionDuration = 0.3f;
    [SerializeField] private bool enableDebugLogs = true;
    #endregion
    
    #region State
    private bool isMinigameActive = false;
    private MinigameType currentMinigameType = MinigameType.None;
    private VisualElement minigameContainer;
    private DialogNavigator navigator;
    #endregion
    
    #region Minigame UI Components
    private RememberTheScriptUI rememberScriptUI;
    #endregion
    
    #region UI Element References (for hiding/showing)
    private Label dialogLabel;
    private Button[] choiceButtons;
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// Initialize the MinigameUIManager with references to navigator and dialog UI.
    /// Called by DialogManager during its initialization.
    /// </summary>
    /// <param name="dialogNavigator">The DialogNavigator instance to subscribe to</param>
    /// <param name="targetDialogBox">The GroupBox containing dialog UI</param>
    public void Initialize(DialogNavigator dialogNavigator, GroupBox targetDialogBox)
    {
        if (dialogNavigator == null)
        {
            LogError("Cannot initialize with null DialogNavigator");
            return;
        }
        
        if (targetDialogBox == null)
        {
            LogError("Cannot initialize with null DialogBox");
            return;
        }
        
        navigator = dialogNavigator;
        dialogBox = targetDialogBox;
        
        // Subscribe to minigame events
        SubscribeToEvents();
        
        // Create minigame container
        CreateMinigameContainer();
        
        // Cache references to dialog UI elements (for hiding/showing)
        CacheDialogUIReferences();
        
        LogDebug("MinigameUIManager initialized successfully");
    }
    
    /// <summary>
    /// Subscribe to all DialogNavigator minigame events
    /// </summary>
    private void SubscribeToEvents()
    {
        if (navigator == null) return;
        
        // RememberTheScript events (match actual DialogNavigator signatures)
        navigator.OnRememberScriptStarted += HandleRememberScriptStarted;
        navigator.OnRememberScriptProgress += HandleRememberScriptProgress;
        navigator.OnRememberScriptMistake += HandleRememberScriptMistake;
        navigator.OnRememberScriptReset += HandleRememberScriptReset;
        navigator.OnRememberScriptSuccess += HandleRememberScriptSuccess;
        navigator.OnRememberScriptFailure += HandleRememberScriptFailure;
        navigator.OnRememberScriptEnded += HandleRememberScriptEnded;
        
        LogDebug("Subscribed to DialogNavigator minigame events");
    }
    
    /// <summary>
    /// Create the container that will hold minigame UI elements
    /// </summary>
    private void CreateMinigameContainer()
    {
        minigameContainer = new VisualElement();
        minigameContainer.name = "MinigameContainer";
        minigameContainer.AddToClassList("minigame-container");
        minigameContainer.style.display = DisplayStyle.None;
        
        LogDebug("Minigame container created");
    }
    
    /// <summary>
    /// Cache references to dialog UI elements so we can hide/show them
    /// </summary>
    private void CacheDialogUIReferences()
    {
        if (dialogBox == null) return;
        
        // Find dialog label
        dialogLabel = dialogBox.Q<Label>("GivenDialogLabel");
        
        // Find choice buttons
        choiceButtons = new Button[5];
        choiceButtons[0] = dialogBox.Q<Button>("DialogOption1Btn");
        choiceButtons[1] = dialogBox.Q<Button>("DialogOption2Btn");
        choiceButtons[2] = dialogBox.Q<Button>("DialogOption3Btn");
        choiceButtons[3] = dialogBox.Q<Button>("DialogOption4Btn");
        choiceButtons[4] = dialogBox.Q<Button>("DialogOption5Btn");
        
        LogDebug("Cached dialog UI element references");
    }
    
    #endregion
    
    #region Event Handlers - RememberTheScript
    
    /// <summary>
    /// Called when RememberTheScript minigame starts
    /// </summary>
    private void HandleRememberScriptStarted(DialogNode node)
    {
        if (node == null || !node.IsRememberScriptNode)
        {
            LogError("RememberScriptStarted event fired but node is invalid");
            return;
        }
        
        LogDebug($"RememberTheScript started: '{node.TargetPhrase}' | Max mistakes: {node.MaxMistakes} | Time: {node.TimeLimit}s");
        
        currentMinigameType = MinigameType.RememberTheScript;
        isMinigameActive = true;
        
        // Initialize RememberTheScript UI
        if (rememberScriptUI == null)
        {
            rememberScriptUI = new RememberTheScriptUI();
        }
        
        rememberScriptUI.Initialize(node.TargetPhrase, node.MaxMistakes, node.TimeLimit);
        
        // Transition from dialog to minigame
        StartCoroutine(TransitionToMinigame());
    }
    
    /// <summary>
    /// Called when player makes progress in typing
    /// </summary>
    private void HandleRememberScriptProgress(string currentTyped, string targetPhrase)
    {
        if (!isMinigameActive) return;
        
        // Calculate current index from what's been typed
        int currentIndex = currentTyped.Length;
        
        // Assume correct (will get mistake event if wrong)
        rememberScriptUI?.UpdateProgress(currentIndex, 0, true);
        
        LogDebug($"Progress: Typed '{currentTyped}' / '{targetPhrase}'");
    }
    
    /// <summary>
    /// Called when player makes a mistake
    /// </summary>
    private void HandleRememberScriptMistake(char typed, char expected, int mistakeCount)
    {
        if (!isMinigameActive) return;
        
        rememberScriptUI?.UpdateProgress(0, mistakeCount, false);
        
        LogDebug($"Mistake! Typed '{typed}' expected '{expected}'. Total mistakes: {mistakeCount}");
    }
    
    /// <summary>
    /// Called when minigame resets (after mistake)
    /// </summary>
    private void HandleRememberScriptReset()
    {
        LogDebug("Minigame reset");
        // UI will be updated by next progress event
    }
    
    /// <summary>
    /// Called when player successfully completes the minigame
    /// </summary>
    private void HandleRememberScriptSuccess(string completedPhrase, int mistakeCount, float timeRemaining)
    {
        LogDebug($"Success! Completed '{completedPhrase}' with {mistakeCount} mistakes. Time remaining: {timeRemaining}s");
        
        // Calculate score and time taken
        float timeTaken = rememberScriptUI != null ? (timeRemaining > 0 ? 30f - timeRemaining : 0f) : 0f;
        float score = 100f - (mistakeCount * 10f); // Simple scoring
        
        rememberScriptUI?.ShowSuccess(score, timeTaken);
        
        // Transition back to dialog after delay (for success animation)
        StartCoroutine(TransitionBackToDialog(2f));
    }
    
    /// <summary>
    /// Called when player fails the minigame
    /// </summary>
    private void HandleRememberScriptFailure(int mistakeCount)
    {
        string reason = $"Too many mistakes: {mistakeCount}";
        LogDebug($"Failure: {reason}");
        
        rememberScriptUI?.ShowFailure(reason);
        
        // Transition back to dialog after delay (for failure message)
        StartCoroutine(TransitionBackToDialog(2f));
    }
    
    /// <summary>
    /// Called when minigame ends (cleanup)
    /// </summary>
    private void HandleRememberScriptEnded()
    {
        LogDebug("RememberTheScript ended");
        // Cleanup handled by transition
    }
    
    #endregion
    
    #region Transitions
    
    /// <summary>
    /// Transition from standard dialog UI to minigame UI
    /// </summary>
    private IEnumerator TransitionToMinigame()
    {
        // Fade out dialog box
        yield return FadeOut(dialogBox, transitionDuration);
        
        // Hide dialog elements
        HideDialogElements();
        
        // Show minigame UI
        ShowMinigameUI();
        
        // Fade in minigame
        yield return FadeIn(minigameContainer, transitionDuration);
    }
    
    /// <summary>
    /// Transition from minigame UI back to standard dialog
    /// </summary>
    private IEnumerator TransitionBackToDialog(float delay)
    {
        // Wait for delay (for success/failure animation to show)
        yield return new WaitForSeconds(delay);
        
        // Fade out minigame
        yield return FadeOut(minigameContainer, transitionDuration);
        
        // Hide minigame UI
        HideMinigameUI();
        
        // Show dialog elements
        ShowDialogElements();
        
        // Fade in dialog
        yield return FadeIn(dialogBox, transitionDuration);
        
        // Reset state
        isMinigameActive = false;
        currentMinigameType = MinigameType.None;
        
        LogDebug("Transitioned back to dialog");
    }
    
    /// <summary>
    /// Fade out a visual element over time
    /// </summary>
    private IEnumerator FadeOut(VisualElement element, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            element.style.opacity = 1f - t;
            yield return null;
        }
        element.style.opacity = 0f;
        element.style.display = DisplayStyle.None;
    }
    
    /// <summary>
    /// Fade in a visual element over time
    /// </summary>
    private IEnumerator FadeIn(VisualElement element, float duration)
    {
        element.style.display = DisplayStyle.Flex;
        element.style.opacity = 0f;
        
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            element.style.opacity = t;
            yield return null;
        }
        element.style.opacity = 1f;
    }
    
    #endregion
    
    #region UI Management
    
    /// <summary>
    /// Show the minigame UI in the dialog box
    /// </summary>
    private void ShowMinigameUI()
    {
        // Add minigame container to DialogBox
        dialogBox.Add(minigameContainer);
        
        // Add appropriate minigame component
        switch (currentMinigameType)
        {
            case MinigameType.RememberTheScript:
                if (rememberScriptUI != null)
                {
                    minigameContainer.Add(rememberScriptUI.GetRootElement());
                }
                break;
            
            // Future minigames:
            // case MinigameType.CalmDialog:
            //     minigameContainer.Add(calmDialogUI.GetRootElement());
            //     break;
        }
        
        minigameContainer.style.display = DisplayStyle.Flex;
        
        LogDebug("Minigame UI shown");
    }
    
    /// <summary>
    /// Hide the minigame UI
    /// </summary>
    private void HideMinigameUI()
    {
        minigameContainer.Clear();
        minigameContainer.style.display = DisplayStyle.None;
        
        LogDebug("Minigame UI hidden");
    }
    
    /// <summary>
    /// Hide standard dialog text and choice buttons
    /// </summary>
    private void HideDialogElements()
    {
        if (dialogLabel != null)
        {
            dialogLabel.style.display = DisplayStyle.None;
        }
        
        if (choiceButtons != null)
        {
            foreach (var button in choiceButtons)
            {
                if (button != null)
                {
                    button.style.display = DisplayStyle.None;
                }
            }
        }
        
        LogDebug("Dialog elements hidden");
    }
    
    /// <summary>
    /// Show standard dialog text and choice buttons
    /// </summary>
    private void ShowDialogElements()
    {
        if (dialogLabel != null)
        {
            dialogLabel.style.display = DisplayStyle.Flex;
        }
        
        // Note: Choice buttons visibility is managed by DialogManager's ShowChoices() method
        // We just need to ensure they're not force-hidden by us
        
        LogDebug("Dialog elements shown");
    }
    
    #endregion
    
    #region Public Properties
    
    /// <summary>
    /// Check if a minigame is currently active
    /// </summary>
    public bool IsMinigameActive => isMinigameActive;
    
    /// <summary>
    /// Get the current minigame type being displayed
    /// </summary>
    public MinigameType CurrentMinigameType => currentMinigameType;
    
    #endregion
    
    #region Cleanup
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (navigator != null)
        {
            navigator.OnRememberScriptStarted -= HandleRememberScriptStarted;
            navigator.OnRememberScriptProgress -= HandleRememberScriptProgress;
            navigator.OnRememberScriptMistake -= HandleRememberScriptMistake;
            navigator.OnRememberScriptReset -= HandleRememberScriptReset;
            navigator.OnRememberScriptSuccess -= HandleRememberScriptSuccess;
            navigator.OnRememberScriptFailure -= HandleRememberScriptFailure;
            navigator.OnRememberScriptEnded -= HandleRememberScriptEnded;
        }
        
        LogDebug("MinigameUIManager destroyed and unsubscribed from events");
    }
    
    #endregion
    
    #region Logging
    
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[MinigameUIManager] {message}");
        }
    }
    
    private void LogWarning(string message)
    {
        Debug.LogWarning($"[MinigameUIManager] {message}");
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[MinigameUIManager] {message}");
    }
    
    #endregion
    
    #region Editor Support
    
    [ContextMenu("Print Current State")]
    private void EditorPrintState()
    {
        Debug.Log("=== MinigameUIManager State ===");
        Debug.Log($"Is Minigame Active: {isMinigameActive}");
        Debug.Log($"Current Minigame Type: {currentMinigameType}");
        Debug.Log($"Navigator Reference: {(navigator != null ? "Valid" : "NULL")}");
        Debug.Log($"DialogBox Reference: {(dialogBox != null ? "Valid" : "NULL")}");
        Debug.Log($"Minigame Container: {(minigameContainer != null ? "Valid" : "NULL")}");
    }
    
    #endregion
}

/// <summary>
/// Enum for different minigame types
/// </summary>
public enum MinigameType
{
    None,
    RememberTheScript,
    // Future minigame types:
    // CalmDialog,
    // ImproveSkill,
    // etc.
}
