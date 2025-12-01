/* DialogNavigator.cs
 * Handles navigation through dialog trees independently from UI display. Is automatically used and called by DialogManager and does not need to be attached to any GameObject.
 * 
 * How to use in Unity:
 * 1. Create an instance of DialogNavigator in your dialog system or manager class.
 * 2. Use the provided methods to start dialogs, navigate nodes, and handle choices.
 * 3. Subscribe to events for UI updates and custom actions.
 * 4. Ensure NPCContent and DialogNode classes are properly set up for dialog data.
 * 
 */

using UnityEngine;
using System;

/// <summary>
/// Handles navigation through dialog trees independently from UI display.
/// Manages the current state of dialog progression and provides events for UI updates.
/// </summary>
public class DialogNavigator
{
    // Events for UI to subscribe to
    public event Action<DialogNode> OnNodeChanged;
    public event Action<DialogChoice, NPCContent> OnCustomActionTriggered;
    public event Action OnDialogEnded;
    
    // RememberTheScript Minigame Events
    public event Action<DialogNode> OnRememberScriptStarted;
    public event Action<string, string> OnRememberScriptProgress; // (currentTyped, targetPhrase)
    public event Action<char, char, int> OnRememberScriptMistake; // (typed, expected, mistakeCount)
    public event Action OnRememberScriptReset;
    public event Action<string, int, float> OnRememberScriptSuccess; // (completedPhrase, mistakeCount, timeRemaining)
    public event Action<int> OnRememberScriptFailure; // (mistakeCount)
    public event Action OnRememberScriptEnded;
    
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
    
    /// <summary>
    /// Start navigating a dialog tree from an NPC.
    /// </summary>
    public bool StartDialog(NPCContent npc, string treeNameOverride = null)
    {
        if (npc == null)
        {
            Debug.LogWarning("[DialogNavigator] Cannot start dialog - NPC is null");
            return false;
        }
        
        // Get the appropriate dialog tree
        DialogTree tree = string.IsNullOrEmpty(treeNameOverride) 
            ? npc.GetMainDialogTree() 
            : npc.GetDialogTree(treeNameOverride);
            
        if (tree == null || !tree.IsValid())
        {
            Debug.LogWarning($"[DialogNavigator] Cannot start dialog - No valid dialog tree found for {npc.npcName}");
            return false;
        }
        
        currentNPC = npc;
        currentTree = tree;
        
        // Notify NPC that dialog started
        currentNPC.OnDialogStarted();
        
        // Navigate to starting node
        NavigateToNode(tree.GetStartingNode());
        
        Debug.Log($"[DialogNavigator] Started dialog with {npc.npcName} using tree '{tree.treeName}'");
        
        return true;
    }
    
    /// <summary>
    /// Navigate to a specific dialog node
    /// </summary>
    public void NavigateToNode(DialogNode node)
    {
        if (node == null)
        {
            Debug.LogWarning("[DialogNavigator] Cannot navigate to null node - ending dialog");
            EndDialog();
            return;
        }
        
        // Execute end events for previous node
        if (currentNode != null)
        {
            ExecuteNodeEndEvents(currentNode);
        }
        
        // Update current node
        currentNode = node;
        
        // Execute start events for new node
        ExecuteNodeStartEvents(currentNode);
        
        // Check if this node triggers RememberTheScript minigame
        if (node.IsRememberScriptNode)
        {
            InitializeRememberScript(node);
        }
        
        // Notify UI that node changed
        OnNodeChanged?.Invoke(currentNode);
        
        Debug.Log($"[DialogNavigator] Navigated to node: {node.CharacterName}: '{node.DialogText.Substring(0, Mathf.Min(30, node.DialogText.Length))}...?'");
    }
    
    /// <summary>
    /// Execute start events for a node
    /// </summary>
    private void ExecuteNodeStartEvents(DialogNode node)
    {
        if (node == null) return;
        
        // Execute new DialogEvent system
        if (node.StartEvents != null && node.StartEvents.Count > 0)
        {
            Debug.Log($"[DialogNavigator] Executing {node.StartEvents.Count} start events for node");
            foreach (var evt in node.StartEvents)
            {
                if (evt != null && evt.IsValid())
                {
                    try
                    {
                        evt.Execute();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[DialogNavigator] Error executing start event: {ex.Message}");
                    }
                }
            }
        }
        
        // Execute legacy UnityEvents for backwards compatibility
        if (node.OnDialogStart != null)
        {
            try
            {
                node.OnDialogStart.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[DialogNavigator] Error executing OnDialogStart UnityEvent: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Execute end events for a node
    /// </summary>
    private void ExecuteNodeEndEvents(DialogNode node)
    {
        if (node == null) return;
        
        // Execute new DialogEvent system
        if (node.EndEvents != null && node.EndEvents.Count > 0)
        {
            Debug.Log($"[DialogNavigator] Executing {node.EndEvents.Count} end events for node");
            foreach (var evt in node.EndEvents)
            {
                if (evt != null && evt.IsValid())
                {
                    try
                    {
                        evt.Execute();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[DialogNavigator] Error executing end event: {ex.Message}");
                    }
                }
            }
        }
        
        // Execute legacy UnityEvents for backwards compatibility
        if (node.OnDialogEnd != null)
        {
            try
            {
                node.OnDialogEnd.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[DialogNavigator] Error executing OnDialogEnd UnityEvent: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Select a choice by index
    /// </summary>
    public void SelectChoice(int choiceIndex)
    {
        if (currentNode == null)
        {
            Debug.LogWarning("[DialogNavigator] No active node - cannot select choice");
            return;
        }
        
        if (!currentNode.HasChoices)
        {
            Debug.LogWarning("[DialogNavigator] Current node has no choices");
            return;
        }
        
        if (choiceIndex < 0 || choiceIndex >= currentNode.Choices.Count)
        {
            Debug.LogError($"[DialogNavigator] Choice index {choiceIndex} out of range (0-{currentNode.Choices.Count - 1})");
            return;
        }
        
        var selectedChoice = currentNode.Choices[choiceIndex];
        if (selectedChoice == null)
        {
            Debug.LogError($"[DialogNavigator] Choice at index {choiceIndex} is null");
            return;
        }
        
        Debug.Log($"[DialogNavigator] Selected choice: '{selectedChoice.ChoiceText}'");
        
        // Execute choice events
        ExecuteChoiceEvents(selectedChoice);
        
        // Check for custom action ID (for backwards compatibility)
        if (!string.IsNullOrEmpty(selectedChoice.ChoiceId))
        {
            Debug.Log($"[DialogNavigator] Triggering custom action: {selectedChoice.ChoiceId}");
            OnCustomActionTriggered?.Invoke(selectedChoice, currentNPC);
            
            // Also notify NPC
            if (currentNPC != null)
            {
                currentNPC.HandleCustomAction(selectedChoice.ChoiceId);
            }
        }
        
        // Navigate to target node
        if (selectedChoice.TargetNode != null)
        {
            NavigateToNode(selectedChoice.TargetNode);
        }
        else if (selectedChoice.HasNamedTarget)
        {
            // Try to resolve named target
            if (currentTree != null)
            {
                var targetNode = currentTree.FindNodeByName(selectedChoice.TargetNodeName);
                if (targetNode != null)
                {
                    NavigateToNode(targetNode);
                }
                else
                {
                    Debug.LogError($"[DialogNavigator] Could not find named node '{selectedChoice.TargetNodeName}' - ending dialog");
                    EndDialog();
                }
            }
        }
        else
        {
            Debug.LogWarning("[DialogNavigator] Choice has no valid target - ending dialog");
            EndDialog();
        }
    }
    
    /// <summary>
    /// Execute choice events
    /// </summary>
    private void ExecuteChoiceEvents(DialogChoice choice)
    {
        if (choice == null) return;
        
        // Execute choice events
        if (choice.ChoiceEvents != null && choice.ChoiceEvents.Count > 0)
        {
            Debug.Log($"[DialogNavigator] Executing {choice.ChoiceEvents.Count} choice events");
            foreach (var evt in choice.ChoiceEvents)
            {
                if (evt != null && evt.IsValid())
                {
                    try
                    {
                        evt.Execute();
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError($"[DialogNavigator] Error executing choice event: {ex.Message}");
                    }
                }
            }
        }
        
        // Execute legacy UnityEvents for backwards compatibility
        if (choice.OnChoiceSelected != null)
        {
            try
            {
                choice.OnChoiceSelected.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[DialogNavigator] Error executing choice UnityEvent: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Advance dialog for nodes without choices (auto-advance or manual advance)
    /// </summary>
    public void AdvanceDialog()
    {
        if (currentNode == null)
        {
            Debug.LogWarning("[DialogNavigator] No active node - cannot advance");
            return;
        }
        
        if (currentNode.HasChoices)
        {
            Debug.LogWarning("[DialogNavigator] Current node has choices - use SelectChoice instead");
            return;
        }
        
        // Check if there's a child node to advance to
        if (currentNode.ChildNode != null)
        {
            Debug.Log("[DialogNavigator] Advancing to child node");
            NavigateToNode(currentNode.ChildNode);
        }
        else
        {
            Debug.Log("[DialogNavigator] No more nodes - ending dialog");
            EndDialog();
        }
    }
    
    /// <summary>
    /// Jump to a specific named tree within the current NPC
    /// </summary>
    public bool SwitchToTree(string treeName)
    {
        if (currentNPC == null)
        {
            Debug.LogWarning("[DialogNavigator] No current NPC - cannot switch trees");
            return false;
        }
        
        var newTree = currentNPC.GetDialogTree(treeName);
        if (newTree == null || !newTree.IsValid())
        {
            Debug.LogWarning($"[DialogNavigator] Cannot switch to tree '{treeName}' - tree not found or invalid");
            return false;
        }
        
        Debug.Log($"[DialogNavigator] Switching to tree '{treeName}'");
        
        // Execute end events for current node before switching
        if (currentNode != null)
        {
            ExecuteNodeEndEvents(currentNode);
        }
        
        currentTree = newTree;
        NavigateToNode(newTree.GetStartingNode());
        
        return true;
    }
    
    /// <summary>
    /// Force navigation to a specific node (useful for scripted sequences)
    /// </summary>
    public void ForceNavigateToNode(DialogNode node)
    {
        if (node == null)
        {
            Debug.LogWarning("[DialogNavigator] Cannot force navigate to null node");
            return;
        }
        
        Debug.Log($"[DialogNavigator] Force navigating to node: {node.GetDisplayName()}");
        NavigateToNode(node);
    }
    
    /// <summary>
    /// End the current dialog session
    /// </summary>
    public void EndDialog()
    {
        if (currentNode == null && currentNPC == null)
        {
            Debug.LogWarning("[DialogNavigator] No active dialog to end");
            return;
        }
        
        Debug.Log($"[DialogNavigator] Ending dialog with {currentNPC?.npcName ?? "unknown NPC"}");
        
        // Execute end events for current node
        if (currentNode != null)
        {
            ExecuteNodeEndEvents(currentNode);
        }
        
        // Notify NPC that dialog ended
        if (currentNPC != null)
        {
            currentNPC.OnDialogEnded();
        }
        
        // Fire event for UI and external systems
        OnDialogEnded?.Invoke();
        
        // Clear state
        currentNode = null;
        currentNPC = null;
        currentTree = null;
    }
    
    /// <summary>
    /// Get information about the current dialog state
    /// </summary>
    public DialogNavigationState GetCurrentState()
    {
        return new DialogNavigationState
        {
            isActive = IsActive,
            currentNode = currentNode,
            currentTree = currentTree,
            currentNPC = currentNPC,
            hasChoices = currentNode?.HasChoices ?? false,
            shouldAutoAdvance = (currentNode?.HasAutoAdvance ?? false) && (currentNode?.AutoAdvanceDelay > 0),
            autoAdvanceDelay = currentNode?.AutoAdvanceDelay ?? 0f,
            choiceCount = currentNode?.Choices?.Count ?? 0,
            isRememberScriptActive = isRememberScriptActive,
            rememberScriptProgress = isRememberScriptActive ? currentTypedText : "",
            rememberScriptMistakes = mistakeCount,
            rememberScriptTimeRemaining = timeRemaining
        };
    }
    
    #region RememberTheScript Minigame Logic
    
    /// <summary>
    /// Initialize RememberTheScript minigame for a node
    /// </summary>
    private void InitializeRememberScript(DialogNode node)
    {
        if (node == null || !node.IsRememberScriptNode)
        {
            Debug.LogWarning("[RememberTheScript] Cannot initialize - node is not configured for minigame");
            return;
        }
        
        if (string.IsNullOrEmpty(node.TargetPhrase))
        {
            Debug.LogError("[RememberTheScript] Cannot initialize - target phrase is empty");
            return;
        }
        
        isRememberScriptActive = true;
        rememberScriptNode = node;
        currentTypedText = "";
        mistakeCount = 0;
        timeRemaining = node.TimeLimit;
        startTime = 0f;
        
        Debug.Log($"[RememberTheScript] Initialized - Target: '{node.TargetPhrase}' | Max Mistakes: {node.MaxMistakes} | Time Limit: {node.TimeLimit}s");
        
        // Fire event for UI to set up minigame interface
        OnRememberScriptStarted?.Invoke(node);
    }
    
    /// <summary>
    /// Process a character input for RememberTheScript minigame
    /// Returns true if correct, false if mistake
    /// </summary>
    public bool ProcessRememberScriptInput(char inputChar)
    {
        if (!isRememberScriptActive || rememberScriptNode == null)
        {
            Debug.LogWarning("[RememberTheScript] Not active - cannot process input");
            return false;
        }
        
        string targetPhrase = rememberScriptNode.TargetPhrase;
        
        // Apply case sensitivity setting
        char processedInput = inputChar;
        string processedTarget = targetPhrase;
        
        if (!rememberScriptNode.CaseSensitive)
        {
            processedInput = char.ToLower(inputChar);
            processedTarget = targetPhrase.ToLower();
        }
        
        // Get expected character at current position
        int currentIndex = currentTypedText.Length;
        
        if (currentIndex >= processedTarget.Length)
        {
            Debug.LogWarning("[RememberTheScript] Already at end of phrase");
            return false;
        }
        
        char expectedChar = processedTarget[currentIndex];
        
        // Compare
        if (processedInput == expectedChar)
        {
            // Correct!
            currentTypedText += targetPhrase[currentIndex]; // Use original case from target
            
            Debug.Log($"[RememberTheScript] Correct! Progress: {currentTypedText.Length}/{targetPhrase.Length}");
            
            // Fire progress event for UI
            OnRememberScriptProgress?.Invoke(currentTypedText, targetPhrase);
            
            // Check if completed
            if (currentTypedText.Length >= targetPhrase.Length)
            {
                HandleRememberScriptSuccess();
            }
            
            return true;
        }
        else
        {
            // Mistake!
            HandleRememberScriptMistake(inputChar, targetPhrase[currentIndex]);
            return false;
        }
    }
    
    /// <summary>
    /// Update timer for RememberTheScript minigame (call from Update loop in DialogManager)
    /// </summary>
    public void UpdateRememberScriptTimer(float deltaTime)
    {
        if (!isRememberScriptActive || rememberScriptNode == null)
            return;
        
        if (rememberScriptNode.TimeLimit <= 0)
            return; // No time limit
        
        timeRemaining -= deltaTime;
        
        if (timeRemaining <= 0)
        {
            Debug.Log("[RememberTheScript] Time's up!");
            HandleRememberScriptTimeOut();
        }
    }
    
    /// <summary>
    /// Handle a mistake in RememberTheScript minigame
    /// </summary>
    private void HandleRememberScriptMistake(char typed, char expected)
    {
        mistakeCount++;
        
        Debug.Log($"[RememberTheScript] Mistake! Typed '{typed}' expected '{expected}'. Total mistakes: {mistakeCount}/{rememberScriptNode.MaxMistakes}");
        
        // Apply score penalty
        AdjustAudienceScore(rememberScriptNode.ScorePerMistake);
        
        // Fire mistake event for UI
        OnRememberScriptMistake?.Invoke(typed, expected, mistakeCount);
        
        // Reset typed text
        currentTypedText = "";
        
        // Fire reset event for UI
        OnRememberScriptReset?.Invoke();
        
        // Check if too many mistakes
        if (mistakeCount >= rememberScriptNode.MaxMistakes)
        {
            HandleRememberScriptFailure();
        }
    }
    
    /// <summary>
    /// Handle successful completion of RememberTheScript minigame
    /// </summary>
    private void HandleRememberScriptSuccess()
    {
        if (!isRememberScriptActive || rememberScriptNode == null)
            return;
        
        float timeTaken = Time.time - startTime;
        
        Debug.Log($"[RememberTheScript] Success! Completed '{currentTypedText}' with {mistakeCount} mistakes in {timeTaken:F1}s");
        
        // Apply success score
        AdjustAudienceScore(rememberScriptNode.ScoreOnSuccess);
        
        // Fire success event for UI
        OnRememberScriptSuccess?.Invoke(currentTypedText, mistakeCount, timeRemaining);
        
        // Clean up state
        CleanupRememberScript();
        
        // Advance dialog (wait for UI animation, then continue)
        if (currentNode.HasChoices && currentNode.Choices.Count > 0)
        {
            // If node has choices, don't auto-select - let player choose
            Debug.Log("[RememberTheScript] Success - waiting for player to select choice");
        }
        else if (currentNode.HasAutoAdvance)
        {
            Debug.Log("[RememberTheScript] Success - auto-advancing dialog");
            AdvanceDialog();
        }
        else
        {
            Debug.Log("[RememberTheScript] Success - ending dialog");
            EndDialog();
        }
    }
    
    /// <summary>
    /// Handle failure due to too many mistakes
    /// </summary>
    private void HandleRememberScriptFailure()
    {
        if (!isRememberScriptActive || rememberScriptNode == null)
            return;
        
        Debug.Log($"[RememberTheScript] Failed! Too many mistakes: {mistakeCount}/{rememberScriptNode.MaxMistakes}");
        
        // Fire failure event for UI
        OnRememberScriptFailure?.Invoke(mistakeCount);
        
        // Check if there's a specific failure node
        if (!string.IsNullOrEmpty(rememberScriptNode.FailureNodeName))
        {
            var failureNode = currentTree?.FindNodeByName(rememberScriptNode.FailureNodeName);
            if (failureNode != null)
            {
                Debug.Log($"[RememberTheScript] Jumping to failure node: {failureNode.GetDisplayName()}");
                CleanupRememberScript();
                NavigateToNode(failureNode);
                return;
            }
            else
            {
                Debug.LogWarning($"[RememberTheScript] Failure node '{rememberScriptNode.FailureNodeName}' not found");
            }
        }
        
        // Otherwise, reset and retry
        Debug.Log("[RememberTheScript] Resetting for retry");
        mistakeCount = 0;
        currentTypedText = "";
        timeRemaining = rememberScriptNode.TimeLimit;
        startTime = Time.time;
        
        // Fire reset event
        OnRememberScriptReset?.Invoke();
    }
    
    /// <summary>
    /// Handle time running out
    /// </summary>
    private void HandleRememberScriptTimeOut()
    {
        Debug.Log("[RememberTheScript] Time out!");
        
        // Treat timeout as failure
        mistakeCount = rememberScriptNode.MaxMistakes; // Force failure
        HandleRememberScriptFailure();
    }
    
    /// <summary>
    /// Clean up RememberTheScript state
    /// </summary>
    private void CleanupRememberScript()
    {
        isRememberScriptActive = false;
        rememberScriptNode = null;
        currentTypedText = "";
        mistakeCount = 0;
        timeRemaining = 0f;
        startTime = 0f;
        
        // Fire cleanup event
        OnRememberScriptEnded?.Invoke();
    }
    
    /// <summary>
    /// Adjust audience score (integrates with GameStateManager if available)
    /// </summary>
    private void AdjustAudienceScore(float scoreChange)
    {
        // Try to find GameStateManager
        var gameState = UnityEngine.Object.FindFirstObjectByType<GameStateManager>();
        
        if (gameState != null)
        {
            // Use AdjustApplause for all score changes (handles both positive and negative)
            gameState.AdjustApplause(scoreChange);
            
            string sign = scoreChange >= 0 ? "+" : "";
            Debug.Log($"[RememberTheScript] Adjusted applause: {sign}{scoreChange}");
        }
        else
        {
            Debug.LogWarning("[RememberTheScript] GameStateManager not found - score change not applied");
        }
    }
    
    #endregion
}

/// <summary>
/// Snapshot of current dialog navigation state
/// </summary>
public struct DialogNavigationState
{
    public bool isActive;
    public DialogNode currentNode;
    public DialogTree currentTree;
    public NPCContent currentNPC;
    public bool hasChoices;
    public bool shouldAutoAdvance;
    public float autoAdvanceDelay;
    public int choiceCount;
    
    // RememberTheScript minigame state
    public bool isRememberScriptActive;
    public string rememberScriptProgress;
    public int rememberScriptMistakes;
    public float rememberScriptTimeRemaining;
}
