/* DialogNavigator.RememberTheScript.cs
 * 
 * Partial class containing RememberTheScript minigame logic for DialogNavigator.
 * Handles typing validation, mistake tracking, timer, and score integration.
 */

using UnityEngine;

/// <summary>
/// RememberTheScript minigame logic portion of DialogNavigator.
/// Manages typing challenges where player must type phrases correctly.
/// </summary>
public partial class DialogNavigator
{
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
