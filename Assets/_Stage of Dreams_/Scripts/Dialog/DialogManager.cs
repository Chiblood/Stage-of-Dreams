/* DialogManager.cs
 * 
 * Simple Dialog Manager that displays dialog UI and handles user input.
 * Uses DialogNavigator for tree navigation logic (DialogManager is focused only on UI display)
 * 
 * How to use in Unity:
 * 1. Attach this script to a GameObject in your scene (e.g., an empty GameObject).
 * 2. Assign a UIDocument component with a VisualTreeAsset for the dialog UI. Which can be created and edited in the UI Builder in Unity.
 * 3. Call StartDialog(NPCContent npc) to initiate a dialog sequence.
 * 4. Ensure NPCContent and DialogNode classes are properly set up for dialog data.
 * 
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
public class DialogManager : MonoBehaviour
{
    #region Editor Fields
    [Header("UI References")]
    [SerializeField] private UIDocument uiDocument; // The UI Document component
    [SerializeField] private VisualTreeAsset dialogVisualTree; // The Visual Tree Asset for dialog UI
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

    // Navigation logic (handled by a DialogNavigator.cs)
    private DialogNavigator navigator;
    
    // UI State
    private bool isUIActive = false;
    
    // Singleton pattern
    public static DialogManager Instance { get; private set; }
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.LogWarning("Multiple DialogManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        InitializeNavigator();
        InitializeUI();
        HideDialog();
    }
    /// <summary>
    /// Calls initialization for the dialog navigator which handles dialog tree navigation logic.
    /// </summary>
    private void InitializeNavigator() 
    {
        navigator = new DialogNavigator();
        
        // Subscribe to navigation events
        navigator.OnNodeChanged += HandleNodeChanged;
        navigator.OnCustomActionTriggered += HandleCustomAction;
        navigator.OnDialogEnded += HandleDialogEnded;
    }
    
    private void InitializeUI()
    {
        if (uiDocument == null)
            uiDocument = GetComponent<UIDocument>();
            
        uiDocument.visualTreeAsset = dialogVisualTree;
        rootElement = uiDocument.rootVisualElement;
        
        // Get UI elements
        dialogBox = rootElement.Q<GroupBox>("DialogBox");
        dialogLabel = rootElement.Q<Label>("GivenDialog");
        choiceButton1 = rootElement.Q<Button>("DialogOption1");
        choiceButton2 = rootElement.Q<Button>("DialogOption2");
        choiceButton3 = rootElement.Q<Button>("DialogOption3");
        choiceButton4 = rootElement.Q<Button>("DialogOption4");
        choiceButton5 = rootElement.Q<Button>("DialogOption5");

        // Setup button events
        choiceButton1.clicked += () => OnChoiceClicked(0);
        choiceButton2.clicked += () => OnChoiceClicked(1);
        choiceButton3.clicked += () => OnChoiceClicked(2);
    }
    
    /// <summary> Start a dialog with an NPC (public interface) </summary>
    public void StartDialog(NPCContent npc)
    {
        if (navigator.StartDialog(npc))
        {
            isUIActive = true;
        }
    }
    
    /// <summary>
    /// Start a dialog with a specific tree (public interface)
    /// </summary>
    public void StartDialog(NPCContent npc, string treeName)
    {
        if (navigator.StartDialog(npc, treeName))
        {
            isUIActive = true;
        }
    }
    
    /// <summary>
    /// Handle when the navigator changes to a new node
    /// </summary>
    private void HandleNodeChanged(DialogNode node)
    {
        DisplayNode(node);
        
        // Handle auto-advance
        if (node.ShouldAutoAdvance)
        {
            StartCoroutine(AutoAdvanceAfterDelay(node.autoAdvanceDelay));
        }
    }
    
    /// <summary>
    /// Handle custom actions triggered by choices
    /// </summary>
    private void HandleCustomAction(DialogChoice choice, NPCContent npc)
    {
        Debug.Log($"Custom action triggered: {choice.customActionId} from {npc.npcName}");
        // Custom actions are handled by the NPC and navigator
        // UI just needs to know it happened
    }
    
    /// <summary>
    /// Handle when dialog navigation ends
    /// </summary>
    private void HandleDialogEnded()
    {
        HideDialog();
        isUIActive = false;
    }
    
    /// <summary>
    /// Display a dialog node in the UI
    /// </summary>
    private void DisplayNode(DialogNode node)
    {
        if (node == null) return;
        
        // Show the dialog box
        dialogBox.style.display = DisplayStyle.Flex;
        
        // Set dialog text
        string fullText = string.IsNullOrEmpty(node.speakerName) 
            ? node.dialogText 
            : $"{node.speakerName}: {node.dialogText}";
        dialogLabel.text = fullText;
        
        // Handle choices
        if (node.HasChoices)
        {
            ShowChoices(node.choices);
        }
        else
        {
            HideChoices();
        }
    }
    
    /// <summary> Show choice buttons for the current node. </summary>
    private void ShowChoices(DialogChoice[] choices)
    {
        // Hide all buttons first
        choiceButton1.style.display = DisplayStyle.None;
        choiceButton2.style.display = DisplayStyle.None;
        choiceButton3.style.display = DisplayStyle.None;
        choiceButton4.style.display = DisplayStyle.None;
        choiceButton5.style.display = DisplayStyle.None;

        // Show buttons for available choices
        for (int i = 0; i < choices.Length && i < 3; i++)
        {
            Button button = GetChoiceButton(i);
            if (button != null && choices[i] != null)
            {
                button.text = choices[i].choiceText;
                button.style.display = DisplayStyle.Flex;
            }
        }
    }
    
    /// <summary> Hide all choice buttons </summary>
    private void HideChoices()
    {
        choiceButton1.style.display = DisplayStyle.None;
        choiceButton2.style.display = DisplayStyle.None;
        choiceButton3.style.display = DisplayStyle.None;
    }
    
    /// <summary> Handle when a choice button is clicked </summary>
    private void OnChoiceClicked(int choiceIndex)
    {
        navigator.SelectChoice(choiceIndex);
    }
    
    /// <summary> call to DialogNavigator to Advance dialog, use with nodes with no choices. </summary>
    public void AdvanceDialog()
    {
        navigator.AdvanceDialog();
    }
    
    /// <summary>
    /// End the current dialog (public interface)
    /// </summary>
    public void EndDialog()
    {
        navigator.EndDialog();
    }
    
    /// <summary>
    /// Hide the dialog UI
    /// </summary>
    private void HideDialog()
    {
        if (dialogBox != null)
        {
            dialogBox.style.display = DisplayStyle.None;
        }
        HideChoices();
    }
    
    /// <summary>
    /// Auto-advance after a delay
    /// </summary>
    private IEnumerator AutoAdvanceAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        // Only auto-advance if dialog is still active and current node should auto-advance
        var state = navigator.GetCurrentState();
        if (state.isActive && state.shouldAutoAdvance && !state.hasChoices)
        {
            navigator.AdvanceDialog();
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
    
    /// <summary>
    /// Check if dialog is currently active
    /// </summary>
    public bool IsDialogActive()
    {
        return isUIActive && navigator.IsActive;
    }
    
    /// <summary>
    /// Get the current navigation state (for debugging/external systems)
    /// </summary>
    public DialogNavigationState GetNavigationState()
    {
        return navigator.GetCurrentState();
    }
    
    private void Update()
    {
        // Handle input for advancing dialog (when no choices)
        var state = navigator.GetCurrentState();

        if (state.isActive && !state.hasChoices && !state.shouldAutoAdvance)
        {
            if (interactAction.triggered)
            {
                AdvanceDialog();
            }
        }
    }
    
    private void OnDestroy()
    {
        // Clear singleton reference
        if (Instance == this)
        {
            Instance = null;
        }
        
        // Unsubscribe from navigation events
        if (navigator != null)
        {
            navigator.OnNodeChanged -= HandleNodeChanged;
            navigator.OnCustomActionTriggered -= HandleCustomAction;
            navigator.OnDialogEnded -= HandleDialogEnded;
        }
    }
    public bool IsUIActive => isUIActive;
}