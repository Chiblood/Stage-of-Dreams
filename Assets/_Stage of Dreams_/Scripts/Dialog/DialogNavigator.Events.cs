/* DialogNavigator.Events.cs
 * 
 * Partial class containing event declarations and state fields for DialogNavigator.
 * Separates event definitions from core navigation logic.
 */

using System;

/// <summary>
/// Event declarations portion of DialogNavigator.
/// Contains all events that UI and other systems can subscribe to.
/// </summary>
public partial class DialogNavigator
{
    #region Core Dialog Events

    // Events for UI to subscribe to
    public event Action<DialogNode> OnNodeChanged;
    public event Action<DialogChoice, NPCContent> OnCustomActionTriggered;
    public event Action OnDialogEnded;

    #endregion

    #region RememberTheScript Minigame Events

    public event Action<DialogNode> OnRememberScriptStarted;
    public event Action<string, string> OnRememberScriptProgress; // (currentTyped, targetPhrase)
    public event Action<char, char, int> OnRememberScriptMistake; // (typed, expected, mistakeCount)
    public event Action OnRememberScriptReset;
    public event Action<string, int, float> OnRememberScriptSuccess; // (completedPhrase, mistakeCount, timeRemaining)
    public event Action<int> OnRememberScriptFailure; // (mistakeCount)
    public event Action OnRememberScriptEnded;

    #endregion

    #region State Fields

    // Current navigation state
    private DialogNode currentNode;
    private NPCContent currentNPC;
    private DialogTree currentTree;

    // RememberTheScript minigame state
    private bool isRememberScriptActive = false;
    private DialogNode rememberScriptNode = null;
    private string currentTypedText = "";
    private int mistakeCount = 0;
    private float timeRemaining = 0f;
    private float startTime = 0f;

    #endregion

    #region Public Properties

    /// <summary>
    /// Check if navigation is currently active
    /// </summary>
    public bool IsActive => currentNode != null;

    /// <summary>
    /// Get the current dialog node being displayed
    /// </summary>
    public DialogNode CurrentNode => currentNode;

    /// <summary>
    /// Get the current NPC being talked to
    /// </summary>
    public NPCContent CurrentNPC => currentNPC;

    /// <summary>
    /// Check if RememberTheScript minigame is currently active
    /// </summary>
    public bool IsRememberScriptActive => isRememberScriptActive;

    /// <summary>
    /// Get the current typed text for RememberTheScript minigame
    /// </summary>
    public string CurrentTypedText => currentTypedText;

    /// <summary>
    /// Get the current mistake count for RememberTheScript minigame
    /// </summary>
    public int MistakeCount => mistakeCount;

    /// <summary>
    /// Get the time remaining for RememberTheScript minigame
    /// </summary>
    public float TimeRemaining => timeRemaining;

    #endregion
}
