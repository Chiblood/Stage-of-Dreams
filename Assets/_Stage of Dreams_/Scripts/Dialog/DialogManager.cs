/* DialogManager.cs
 * 
 * Simple Dialog Manager that displays dialog UI and handles user input.
 * Uses DialogNavigator for tree navigation logic (DialogManager is focused only on UI display)
 * 
 * PARTIAL CLASS - Split into:
 * - DialogManager.cs (this file) - Core logic and initialization
 * - DialogManager.Input.cs - Input handling
 * - DialogManager.EditorSupport.cs - Debug context menus
 * 
 * How to use in Unity:
 * 1. Attach this script to a GameObject in your scene (e.g., an empty GameObject).
 * 2. Assign a UIDocument component with a VisualTreeAsset for the dialog UI.
 * 3. Call StartDialog(NPCContent npc) to initiate a dialog sequence.
 */

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Collections; 

/// <summary>
/// Simple Dialog Manager that displays dialog UI and handles user input.
/// Uses DialogNavigator for tree navigation logic - focused only on UI display.
/// Singleton pattern for easy global access.
/// </summary>
public partial class DialogManager : MonoBehaviour
{
    #region Editor Fields
    [Header("UI References")]
    [SerializeField] private UIDocument uiDocument; // The UI Document component
    [SerializeField] private VisualTreeAsset dialogVisualTree; // The Visual Tree Asset for dialog UI
    
    [Header("Minigame UI")] // NEW
    [SerializeField] private MinigameUIManager minigameUIManager; // Manages minigame UI display
    
    [Header("Input Settings")]
    [SerializeField] private bool enableInputLogging = false; // Debug input handling
    
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool validateOnStart = true;
    #endregion

    #region UI Elements
    private VisualElement rootElement; // The root of the UI document
    private GroupBox dialogBox; // The main dialog box container
    private Label dialogLabel; // The label for displaying dialog text

    // Choice buttons
    private Button choiceButton1; 
    private Button choiceButton2; 
    private Button choiceButton3;
    private Button choiceButton4;
    private Button choiceButton5;

    // Player input handling
    public PlayerInput playerInput; // Reference to PlayerInput component for input actions
    private InputAction interactAction; // Action for interacting with the dialog
    #endregion

    #region State Management
    // Navigation logic (handled by a DialogNavigator.cs)
    private DialogNavigator navigator;
    
    // UI State
    private bool isUIActive = false;
    private bool isInitialized = false;
    
    // Current dialog context
    private NPCContent currentNPC = null;
    private DialogTree currentTree = null;
    
    // Integration tracking
    private int dialogSessionCount = 0;
    private float lastDialogStartTime = 0f;
    #endregion

    // Singleton pattern which allows easy access from other scripts and ensures only one instance exists
    public static DialogManager Instance { get; private set; }
    
    // Events for integration with external systems
    public System.Action<NPCContent, DialogTree> OnDialogStarted;
    public System.Action<NPCContent> OnDialogEnded;
    public System.Action<DialogNode> OnNodeDisplayed;
    public System.Action<DialogChoice, NPCContent> OnCustomActionHandled;

    #region Initialization and Setup
    private void Awake()
    {
        // Singleton pattern which allows easy access from other scripts and ensures only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LogDebug("DialogManager singleton initialized");
        }
        else if (Instance != this)
        {
            LogWarning("Multiple DialogManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        InitializeDialogManager();
    }
    
    /// <summary>
    /// Initialize all components of the DialogManager
    /// </summary>
    private void InitializeDialogManager()
    {
        if (isInitialized)
        {
            LogWarning("DialogManager already initialized");
            return;
        }
        
        bool initSuccess = true;
        
        // Initialize components in order
        if (!InitializeNavigator())
        {
            LogError("Failed to initialize DialogNavigator");
            initSuccess = false;
        }
        
        if (!InitializeUI())
        {
            LogError("Failed to initialize UI");
            initSuccess = false;
        }
        
        if (!InitializeMinigameUI()) // NEW
        {
            LogError("Failed to initialize minigame UI");
            initSuccess = false;
        }
        
        if (!InitializeInput())
        {
            LogError("Failed to initialize input");
            initSuccess = false;
        }
        
        if (initSuccess)
        {
            isInitialized = true;
            HideDialog();
            
            if (validateOnStart)
            {
                ValidateSetup();
            }
            
            LogDebug("DialogManager initialization completed successfully");
        }
        else
        {
            LogError("DialogManager initialization failed - some components may not work correctly");
        }
    }
    
    /// <summary>
    /// Initialize dialog navigator which handles dialog tree navigation logic.
    /// </summary>
    private bool InitializeNavigator() 
    {
        try
        {
            navigator = new DialogNavigator();
            
            // Subscribe to navigation events
            navigator.OnNodeChanged += HandleNodeChanged;
            navigator.OnCustomActionTriggered += HandleCustomAction;
            navigator.OnDialogEnded += HandleDialogEnded;
            
            LogDebug("DialogNavigator initialized successfully");
            return true;
        }
        catch (System.Exception ex)
        {
            LogError($"Failed to initialize DialogNavigator: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Initialize UI components and elements
    /// </summary>
    private bool InitializeUI()
    {
        try
        {
            if (uiDocument == null)
                uiDocument = GetComponent<UIDocument>();
            
            if (uiDocument == null)
            {
                LogError("No UIDocument component found - dialog UI will not work");
                return false;
            }
            
            if (dialogVisualTree == null)
            {
                LogError("No VisualTreeAsset assigned - dialog UI will not display");
                return false;
            }
            
            uiDocument.visualTreeAsset = dialogVisualTree;
            rootElement = uiDocument.rootVisualElement;
            
            if (rootElement == null)
            {
                LogError("Failed to get root visual element");
                return false;
            }
            
            // Get UI elements - Updated to match your UXML structure
            dialogBox = rootElement.Q<GroupBox>("DialogBox");
            dialogLabel = rootElement.Q<Label>("GivenDialogLabel"); // Updated name
            choiceButton1 = rootElement.Q<Button>("DialogOption1Btn"); // Updated names
            choiceButton2 = rootElement.Q<Button>("DialogOption2Btn");
            choiceButton3 = rootElement.Q<Button>("DialogOption3Btn");
            choiceButton4 = rootElement.Q<Button>("DialogOption4Btn");
            choiceButton5 = rootElement.Q<Button>("DialogOption5Btn");

            // Validate required elements
            if (dialogBox == null)
            {
                LogError("DialogBox GroupBox not found in UI - check UXML structure");
                return false;
            }
            
            if (dialogLabel == null)
            {
                LogError("GivenDialogLabel Label not found in UI - check UXML structure");
                return false;
            }

            // Setup button events (only for buttons that exist)
            if (choiceButton1 != null)
                choiceButton1.clicked += () => OnChoiceClicked(0);
            if (choiceButton2 != null)
                choiceButton2.clicked += () => OnChoiceClicked(1);
            if (choiceButton3 != null)
                choiceButton3.clicked += () => OnChoiceClicked(2);
            if (choiceButton4 != null)
                choiceButton4.clicked += () => OnChoiceClicked(3);
            if (choiceButton5 != null)
                choiceButton5.clicked += () => OnChoiceClicked(4);
            
            LogDebug("UI components initialized successfully");
            LogDebug($"Found UI elements - DialogBox: {dialogBox != null}, Label: {dialogLabel != null}, Buttons: {GetButtonCount()}");
            return true;
        }
        catch (System.Exception ex)
        {
            LogError($"Failed to initialize UI: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Initialize minigame UI manager
    /// </summary>
    private bool InitializeMinigameUI()
    {
        try
        {
            // Create MinigameUIManager component if not assigned
            if (minigameUIManager == null)
            {
                minigameUIManager = gameObject.AddComponent<MinigameUIManager>();
            }
            
            // Initialize with navigator and dialogBox references
            minigameUIManager.Initialize(navigator, dialogBox);
            
            LogDebug("Minigame UI initialized successfully");
            return true;
        }
        catch (System.Exception ex)
        {
            LogError($"Failed to initialize minigame UI: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Initialize input handling for dialog interaction
    /// </summary>
    private bool InitializeInput()
    {
        try
        {
            if (playerInput == null)
            {
                playerInput = FindFirstObjectByType<PlayerInput>();
            }
            
            if (playerInput != null)
            {
                interactAction = playerInput.actions["Interact"];
                
                if (interactAction != null)
                {
                    LogDebug("Interact action initialized successfully");
                    return true;
                }
                else
                {
                    LogWarning("Interact action not found in PlayerInput - manual advance may not work");
                    return false;
                }
            }
            else
            {
                LogWarning("PlayerInput not found - manual advance may not work");
                return false;
            }
        }
        catch (System.Exception ex)
        {
            LogError($"Failed to initialize input: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Get the number of choice buttons found
    /// </summary>
    private int GetButtonCount()
    {
        int count = 0;
        if (choiceButton1 != null) count++;
        if (choiceButton2 != null) count++;
        if (choiceButton3 != null) count++;
        if (choiceButton4 != null) count++;
        if (choiceButton5 != null) count++;
        return count;
    }
    #endregion

    /// <summary>
    /// Enhanced validation with UI debugging
    /// </summary>
    private void ValidateSetup()
    {
        LogDebug("=== DialogManager Setup Validation ===");
        
        // Check navigator
        if (navigator == null)
        {
            LogError("DialogNavigator is null");
        }
        else
        {
            LogDebug("DialogNavigator is initialized");
        }
        
        // Check UI components
        if (uiDocument == null) LogError(" UIDocument is null");
        else LogDebug("UIDocument is assigned");
        
        if (dialogVisualTree == null) LogError(" VisualTreeAsset is null");
        else LogDebug("VisualTreeAsset is assigned");
        
        if (dialogBox == null) LogError(" DialogBox not found");
        else 
        {
            LogDebug("DialogBox found!");
            LogDebug($"  - Current display: {dialogBox.style.display.value}");
            LogDebug($"  - Current opacity: {dialogBox.style.opacity.value}");
            LogDebug($"  - Has dialog-visible class: {dialogBox.ClassListContains("dialog-visible")}");
            LogDebug($"  - World bounds: {dialogBox.worldBound}");
            LogDebug($"  - Visible: {dialogBox.visible}");
        }
        
        if (dialogLabel == null) LogError(" DialogLabel not found");
        else LogDebug("DialogLabel found");
        
        // Check choice buttons
        int buttonCount = 0;
        if (choiceButton1 != null) buttonCount++;
        if (choiceButton2 != null) buttonCount++;
        if (choiceButton3 != null) buttonCount++;
        if (choiceButton4 != null) buttonCount++;
        if (choiceButton5 != null) buttonCount++;
        
        LogDebug($"Found {buttonCount} choice buttons");
        
        // UI Document specific checks
        if (uiDocument != null)
        {
            LogDebug($"UI Document Settings:");
            LogDebug($"  - Enabled: {uiDocument.enabled}");
            LogDebug($"  - GameObject active: {uiDocument.gameObject.activeInHierarchy}");
            LogDebug($"  - Sort order: {uiDocument.sortingOrder}");
            LogDebug($"  - Panel settings: {uiDocument.panelSettings}");
            
            if (rootElement != null)
            {
                LogDebug($"Root Element:");
                LogDebug($"  - Visible: {rootElement.visible}");
                LogDebug($"  - Display: {rootElement.style.display.value}");
                LogDebug($"  - Child count: {rootElement.childCount}");
            }
        }
        
        // Check input
        if (playerInput == null) LogWarning("!! PlayerInput is null - some input may not work");
        else LogDebug("PlayerInput is assigned");
        
        if (interactAction == null) LogWarning("!! Interact action is null - manual advance may not work");
        else LogDebug("Interact action is configured");
        
        LogDebug("=== Validation Complete ===");
    }
    
    /// <summary> 
    /// Start a dialog with an NPC (public interface)
    /// This is the main entry point called by DialogueTrigger
    /// </summary>
    public bool StartDialog(NPCContent npc)
    {
        return StartDialogInternal(npc, null);
    }
    
    /// <summary>
    /// Start a dialog with a specific tree (public interface)
    /// This is called by DialogueTrigger when a specific tree is requested
    /// </summary>
    public bool StartDialog(NPCContent npc, string treeName)
    {
        return StartDialogInternal(npc, treeName);
    }
    
    /// <summary>
    /// Internal method to handle dialog starting with comprehensive validation
    /// </summary>
    private bool StartDialogInternal(NPCContent npc, string treeNameOverride)
    {
        // Pre-validation
        if (!isInitialized)
        {
            LogError("DialogManager not initialized - cannot start dialog");
            return false;
        }
        
        if (navigator == null)
        {
            LogError("DialogNavigator not available - cannot start dialog");
            return false;
        }
        
        if (npc == null)
        {
            LogError("Cannot start dialog - NPC is null");
            return false;
        }
        
        // Check if dialog is already active
        if (IsDialogActive())
        {
            LogWarning($"Dialog already active with {currentNPC?.npcName} - cannot start new dialog with {npc.npcName}");
            return false;
        }
        
        // Validate NPC content
        if (!ValidateNPCForDialog(npc, treeNameOverride))
        {
            return false;
        }
        
        // Get the dialog tree that will be used
        DialogTree targetTree = string.IsNullOrEmpty(treeNameOverride) 
            ? npc.GetMainDialogTree() 
            : npc.GetDialogTree(treeNameOverride);
        
        LogDebug($"Starting dialog with {npc.npcName} using tree: {targetTree?.treeName ?? "Main"}");
        
        // Store current context
        currentNPC = npc;
        currentTree = targetTree;
        dialogSessionCount++;
        lastDialogStartTime = Time.time;
        
        // Start dialog through navigator
        bool success = false;
        try
        {
            success = navigator.StartDialog(npc, treeNameOverride);
        }
        catch (System.Exception ex)
        {
            LogError($"Exception starting dialog navigation: {ex.Message}");
            success = false;
        }
        
        if (success)
        {
            isUIActive = true;
            
            // Fire events for external systems
            OnDialogStarted?.Invoke(currentNPC, currentTree);
            
            LogDebug($"Dialog session #{dialogSessionCount} started successfully with {npc.npcName}");
            return true;
        }
        else
        {
            // Clean up on failure
            currentNPC = null;
            currentTree = null;
            
            LogError($"Failed to start dialog navigation with {npc.npcName}");
            return false;
        }
    }
    
    /// <summary>
    /// Validate that an NPC is ready for dialog
    /// </summary>
    private bool ValidateNPCForDialog(NPCContent npc, string treeNameOverride)
    {
        if (npc == null)
        {
            LogError("NPC is null");
            return false;
        }
        
        if (!npc.HasValidDialogContent())
        {
            LogError($"NPC '{npc.npcName}' has no valid dialog content");
            return false;
        }
        
        // Check specific tree if requested
        if (!string.IsNullOrEmpty(treeNameOverride))
        {
            var requestedTree = npc.GetDialogTree(treeNameOverride);
            if (requestedTree == null)
            {
                LogError($"NPC '{npc.npcName}' does not have dialog tree '{treeNameOverride}'");
                return false;
            }
            
            if (!requestedTree.IsValid())
            {
                LogError($"Dialog tree '{treeNameOverride}' on NPC '{npc.npcName}' is not valid");
                return false;
            }
        }
        else
        {
            // Check main tree
            var mainTree = npc.GetMainDialogTree();
            if (mainTree == null || !mainTree.IsValid())
            {
                LogError($"NPC '{npc.npcName}' does not have a valid main dialog tree");
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Handle when the navigator changes to a new node
    /// This is where DialogNavigator communicates back to DialogManager
    /// </summary>
    private void HandleNodeChanged(DialogNode node)
    {
        if (node == null)
        {
            LogWarning("Navigator sent null node");
            return;
        }
        
        LogDebug($"HandleNodeChanged called - Speaker: {node.CharacterName}, Text: {node.DialogText}");
        LogDebug($"Node has choices: {node.HasChoices}, Should auto-advance: {node.HasAutoAdvance}");
        
        DisplayNode(node);
        
        // Fire event for external systems
        OnNodeDisplayed?.Invoke(node);
        
        // Handle auto-advance
        if (node.HasAutoAdvance && node.AutoAdvanceDelay > 0)
        {
            LogDebug($"Node has auto-advance with delay: {node.AutoAdvanceDelay}s");
            StartCoroutine(AutoAdvanceAfterDelay(node.AutoAdvanceDelay));
        }
    }
    
    /// <summary>
    /// Handle custom actions triggered by choices
    /// This bridges DialogNavigator custom actions with the UI layer
    /// </summary>
    private void HandleCustomAction(DialogChoice choice, NPCContent npc)
    {
        LogDebug($"Custom action triggered: {choice.ChoiceId} from {npc.npcName}");
        
        // Fire event for external systems
        OnCustomActionHandled?.Invoke(choice, npc);
        
        // Custom actions are handled by the NPC and navigator
        // UI just needs to know it happened for potential UI updates
    }
    
    /// <summary>
    /// Handle when dialog navigation ends
    /// This completes the dialog lifecycle
    /// </summary>
    private void HandleDialogEnded()
    {
        LogDebug($"Dialog ended with {currentNPC?.npcName}");
        
        // Store reference for event before clearing
        var endedNPC = currentNPC;
        
        // Clean up UI
        HideDialog();
        isUIActive = false;
        
        // Re-enable player movement explicitly
        var playerScript = FindFirstObjectByType<PlayerScript>();
        if (playerScript != null)
        {
            playerScript.EnableMovement();
            LogDebug("Player movement re-enabled after dialog ended");
        }
        
        // Fire event for external systems
        OnDialogEnded?.Invoke(endedNPC);
        
        // Clear current context
        currentNPC = null;
        currentTree = null;
    }
    
    /// <summary>
    /// Display a dialog node in the UI
    /// </summary>
    private void DisplayNode(DialogNode node)
    {
        if (node == null) 
        {
            LogWarning("Cannot display null node");
            return;
        }
        
        if (dialogBox == null || dialogLabel == null)
        {
            LogError("UI components not available - cannot display node");
            return;
        }
        
        LogDebug($"DisplayNode called with text: '{node.DialogText}'");
        LogDebug($"DialogBox current display style: {dialogBox.style.display.value}");
        LogDebug($"DialogBox current opacity: {dialogBox.style.opacity.value}");
        
        // Show the dialog box - handle all visual properties explicitly
        dialogBox.style.display = DisplayStyle.Flex;
        dialogBox.style.opacity = 1f;
        dialogBox.style.scale = new Scale(Vector2.one); // Reset scale to normal
        dialogBox.AddToClassList("dialog-visible");
        
        // Force a layout update to ensure the UI refreshes
        dialogBox.MarkDirtyRepaint();
        
        LogDebug($"After setting styles - Display: {dialogBox.style.display.value}, Opacity: {dialogBox.style.opacity.value}");
        
        // Set dialog text with speaker name
        string fullText = string.IsNullOrEmpty(node.CharacterName) 
            ? node.DialogText 
            : $"{node.CharacterName}: {node.DialogText}";
            
        // Add advancement hint if no choices
        if (!node.HasChoices)
        {
            fullText += "\n\n<color=#888888>[Press E to continue]</color>";
        }
        
        dialogLabel.text = fullText;
        
        LogDebug($"Set dialog text to: '{fullText}'");
        
        // Handle choices
        if (node.HasChoices)
        {
            ShowChoices(node.Choices);
            LogDebug($"Displayed {node.Choices.Count} choices");
        }
        else
        {
            HideChoices();
            LogDebug("No choices - dialog will advance manually or automatically");
        }
        
        // Additional verification
        LogDebug($"DialogBox visible in hierarchy: {dialogBox.visible}");
        LogDebug($"DialogBox resolvedStyle display: {dialogBox.resolvedStyle.display}");
        LogDebug($"DialogBox resolvedStyle opacity: {dialogBox.resolvedStyle.opacity}");
    }
    
    /// <summary> Show choice buttons for the current node. </summary>
    private void ShowChoices(System.Collections.Generic.List<DialogChoice> choices)
    {
        // Hide all buttons first
        SetChoiceButtonVisibility(choiceButton1, false);
        SetChoiceButtonVisibility(choiceButton2, false);
        SetChoiceButtonVisibility(choiceButton3, false);
        SetChoiceButtonVisibility(choiceButton4, false);
        SetChoiceButtonVisibility(choiceButton5, false);

        // Show buttons for available choices (up to 5 buttons max)
        int maxButtons = 5;
        for (int i = 0; i < choices.Count && i < maxButtons; i++)
        {
            Button button = GetChoiceButton(i);
            if (button != null && choices[i] != null)
            {
                button.text = choices[i].ChoiceText;
                SetChoiceButtonVisibility(button, true);
            }
        }
        
        if (choices.Count > maxButtons)
        {
            LogWarning($"Node has {choices.Count} choices but UI only supports {maxButtons} buttons");
        }
    }
    
    /// <summary>
    /// Safely set choice button visibility using both display style and CSS classes
    /// </summary>
    private void SetChoiceButtonVisibility(Button button, bool visible)
    {
        if (button != null)
        {
            button.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
            
            // Use CSS classes for additional styling control
            if (visible)
            {
                button.AddToClassList("choice-visible");
            }
            else
            {
                button.RemoveFromClassList("choice-visible");
            }
        }
    }
    
    /// <summary> Hide all choice buttons </summary>
    private void HideChoices()
    {
        SetChoiceButtonVisibility(choiceButton1, false);
        SetChoiceButtonVisibility(choiceButton2, false);
        SetChoiceButtonVisibility(choiceButton3, false);
        SetChoiceButtonVisibility(choiceButton4, false);
        SetChoiceButtonVisibility(choiceButton5, false);
    }
    
    /// <summary>
    /// Hide the dialog UI
    /// </summary>
    private void HideDialog()
    {
        if (dialogBox != null)
        {
            LogDebug("Hiding dialog UI");
            dialogBox.style.display = DisplayStyle.None;
            dialogBox.style.opacity = 0f;
            dialogBox.style.scale = new Scale(new Vector2(0.8f, 0.8f)); // Scale down
            dialogBox.RemoveFromClassList("dialog-visible");
            dialogBox.MarkDirtyRepaint();
        }
        HideChoices();
    }
    
    /// <summary>
    /// Auto-advance after a delay
    /// </summary>
    private IEnumerator AutoAdvanceAfterDelay(float delay)
    {
        LogDebug($"Auto-advance in {delay} seconds");
        yield return new WaitForSeconds(delay);
        
        // Only auto-advance if dialog is still active and current node should auto-advance
        if (navigator != null)
        {
            var state = navigator.GetCurrentState();
            if (state.isActive && state.shouldAutoAdvance && !state.hasChoices)
            {
                LogDebug("Auto-advancing dialog");
                navigator.AdvanceDialog();
            }
            else
            {
                LogDebug("Auto-advance cancelled - dialog state changed");
            }
        }
    }
    
    /// <summary>
    /// Get choice button by index
    /// </summary>
    private Button GetChoiceButton(int index)
    {
        return index switch
        {
            0 => choiceButton1,
            1 => choiceButton2,
            2 => choiceButton3,
            3 => choiceButton4,
            4 => choiceButton5,
            _ => null
        };
    }
    
    /// <summary> Handle when a choice button is clicked </summary>
    private void OnChoiceClicked(int choiceIndex)
    {
        LogDebug($"Choice {choiceIndex} clicked");
        
        if (navigator == null)
        {
            LogError("Navigator not available - cannot process choice");
            return;
        }
        
        try
        {
            navigator.SelectChoice(choiceIndex);
        }
        catch (System.Exception ex)
        {
            LogError($"Exception selecting choice {choiceIndex}: {ex.Message}");
        }
    }
    
    /// <summary> Call to DialogNavigator to advance dialog, use with nodes with no choices. </summary>
    public void AdvanceDialog()
    {
        if (navigator == null)
        {
            LogError("Navigator not available - cannot advance dialog");
            return;
        }
        
        LogDebug("Advancing dialog");
        
        try
        {
            navigator.AdvanceDialog();
        }
        catch (System.Exception ex)
        {
            LogError($"Exception advancing dialog: {ex.Message}");
        }
    }
    
    /// <summary>
    /// End the current dialog (public interface)
    /// </summary>
    public void EndDialog()
    {
        if (navigator == null)
        {
            LogError("Navigator not available - cannot end dialog");
            return;
        }
        
        LogDebug("Ending dialog manually");
        
        try
        {
            navigator.EndDialog();
        }
        catch (System.Exception ex)
        {
            LogError($"Exception ending dialog: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Check if dialog is currently active
    /// </summary>
    public bool IsDialogActive()
    {
        return isUIActive && navigator != null && navigator.IsActive;
    }
    
    /// <summary>
    /// Get the current navigation state (for debugging/external systems)
    /// </summary>
    public DialogNavigationState GetNavigationState()
    {
        return navigator?.GetCurrentState() ?? new DialogNavigationState();
    }
    
    /// <summary>
    /// Get current dialog context information
    /// </summary>
    public (NPCContent npc, DialogTree tree, int sessionCount) GetCurrentDialogContext()
    {
        return (currentNPC, currentTree, dialogSessionCount);
    }
    
    /// <summary>
    /// Check if the DialogManager is properly initialized and ready
    /// </summary>
    public bool IsReady()
    {
        return isInitialized && navigator != null && uiDocument != null;
    }
    
    private void OnDestroy()
    {
        // Clear singleton reference
        if (Instance == this)
        {
            Instance = null;
            LogDebug("DialogManager singleton destroyed");
        }
        
        // Unsubscribe from navigation events
        if (navigator != null)
        {
            navigator.OnNodeChanged -= HandleNodeChanged;
            navigator.OnCustomActionTriggered -= HandleCustomAction;
            navigator.OnDialogEnded -= HandleDialogEnded;
        }
        
        // Clear state
        currentNPC = null;
        currentTree = null;
        isUIActive = false;
        isInitialized = false;
    }
    
    #region Public Properties
    public bool IsUIActive => isUIActive;
    public bool IsInitialized => isInitialized;
    public NPCContent CurrentNPC => currentNPC;
    public DialogTree CurrentTree => currentTree;
    public int DialogSessionCount => dialogSessionCount;
    #endregion
    
    #region Logging Methods
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[DialogManager] {message}");
        }
    }
    
    private void LogWarning(string message)
    {
        Debug.LogWarning($"[DialogManager] {message}");
    }
    
    private void LogError(string message)
    {
        Debug.LogError($"[DialogManager] {message}");
    }
    #endregion
}
