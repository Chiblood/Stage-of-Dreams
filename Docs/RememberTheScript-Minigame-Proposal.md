# RememberTheScript() Minigame - Design Proposal

**Project**: Stage of Dreams  
**Feature**: RememberTheScript() Typing Minigame  
**Created**: January 2025  
**Status**: ⏳ Proposal - Ready for Implementation  
**Based on**: CalmDialog Minigame (Approach 3 - Integrated Dialog System)

---

## 1. Overview

### Concept
RememberTheScript() is a **typing accuracy minigame** where the player must type out a scripted line exactly as shown on screen to demonstrate their character's ability to remember and deliver lines correctly. Mistypes adjust the audience score and reset the current word/phrase, requiring the player to start over.

### Core Mechanics
- **Display** a word or phrase that the player must type
- **Track** player input letter-by-letter
- **Color-code** letters as they are typed (green = correct, red = error)
- **Penalize** mistypes by decreasing audience score
- **Reset** to the beginning of the string when a mistake is made
- **Success** when the entire string is typed correctly

---

## 2. Integration Strategy

### Approach: Integrated Dialog System (Approach 3)
Following the proven CalmDialog pattern:

✅ **Minigame settings stored in DialogNode**  
✅ **DialogNavigator handles retry logic**  
✅ **Validation happens during choice selection**  
✅ **UI updates show real-time progress**  
✅ **No separate MinigameManager needed**  

This ensures:
- Minigames are part of the dialog flow
- Easy to author in Unity Inspector
- Consistent with existing architecture
- Minimal code duplication

---

## 3. Technical Implementation

### 3.1 DialogNode Extensions

Add new fields to `DialogNode.cs`:

```csharp
#region Minigame Settings - Remember The Script

[Header("Remember The Script Minigame")]
[SerializeField] private bool _isRememberScriptNode = false;
[SerializeField] private string _targetPhrase = ""; // What the player must type
[SerializeField] private bool _caseSensitive = false; // Should match case exactly
[SerializeField] private bool _ignoreWhitespace = false; // Ignore spaces/tabs
[SerializeField] private int _maxMistakes = 3; // How many mistakes allowed before strong penalty
[SerializeField] private float _scorePerMistake = -5f; // Audience score decrease per mistake
[SerializeField] private float _scoreOnSuccess = 20f; // Audience score increase on success
[SerializeField] private bool _autoRetryOnMistake = true; // Reset immediately on mistake
[SerializeField] private string _mistakeNodeName = ""; // Optional: Node to jump to on too many mistakes

#endregion

#region Remember Script Properties

public bool IsRememberScriptNode
{
    get => _isRememberScriptNode;
    set => _isRememberScriptNode = value;
}

public string TargetPhrase
{
    get => _targetPhrase ?? string.Empty;
    set => _targetPhrase = value;
}

public bool CaseSensitive
{
    get => _caseSensitive;
    set => _caseSensitive = value;
}

public bool IgnoreWhitespace
{
    get => _ignoreWhitespace;
    set => _ignoreWhitespace = value;
}

public int MaxMistakes
{
    get => _maxMistakes;
    set => _maxMistakes = Mathf.Max(0, value);
}

public float ScorePerMistake
{
    get => _scorePerMistake;
    set => _scorePerMistake = value;
}

public float ScoreOnSuccess
{
    get => _scoreOnSuccess;
    set => _scoreOnSuccess = value;
}

public bool AutoRetryOnMistake
{
    get => _autoRetryOnMistake;
    set => _autoRetryOnMistake = value;
}

public string MistakeNodeName
{
    get => _mistakeNodeName ?? string.Empty;
    set => _mistakeNodeName = value;
}

#endregion
```

---

### 3.2 DialogNavigator Enhancements

Add tracking fields and validation logic to `DialogNavigator.cs`:

```csharp
#region Remember Script State

private string currentTypedText = "";
private int mistakeCount = 0;
private bool isRememberScriptActive = false;
private DialogNode currentRememberScriptNode = null;

#endregion

#region Remember Script Methods

/// <summary>
/// Initialize Remember Script minigame state
/// </summary>
private void InitializeRememberScript(DialogNode node)
{
    if (node == null || !node.IsRememberScriptNode) return;
    
    isRememberScriptActive = true;
    currentRememberScriptNode = node;
    currentTypedText = "";
    mistakeCount = 0;
    
    LogDebug($"[RememberScript] Started - Target: '{node.TargetPhrase}'");
    
    // Fire event for UI to set up typing interface
    OnRememberScriptStarted?.Invoke(node);
}

/// <summary>
/// Process a character input for Remember Script minigame
/// Returns: true if correct, false if mistake
/// </summary>
public bool ProcessRememberScriptInput(char inputChar)
{
    if (!isRememberScriptActive || currentRememberScriptNode == null)
    {
        LogWarning("[RememberScript] Not active - cannot process input");
        return false;
    }
    
    string targetPhrase = currentRememberScriptNode.TargetPhrase;
    
    // Apply processing rules
    if (!currentRememberScriptNode.CaseSensitive)
    {
        inputChar = char.ToLower(inputChar);
        targetPhrase = targetPhrase.ToLower();
    }
    
    // Check if we should ignore whitespace
    if (currentRememberScriptNode.IgnoreWhitespace && char.IsWhiteSpace(inputChar))
    {
        LogDebug("[RememberScript] Whitespace ignored");
        return true; // Ignore but don't penalize
    }
    
    // Get expected character at current position
    int currentIndex = currentTypedText.Length;
    
    if (currentIndex >= targetPhrase.Length)
    {
        LogWarning("[RememberScript] Already at end of phrase");
        return false;
    }
    
    char expectedChar = targetPhrase[currentIndex];
    
    // Compare
    if (inputChar == expectedChar)
    {
        // Correct!
        currentTypedText += inputChar;
        
        LogDebug($"[RememberScript] Correct! Progress: {currentTypedText.Length}/{targetPhrase.Length}");
        
        // Fire event for UI update (show green letter)
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
        HandleRememberScriptMistake(inputChar, expectedChar);
        return false;
    }
}

/// <summary>
/// Handle a mistake in Remember Script minigame
/// </summary>
private void HandleRememberScriptMistake(char typed, char expected)
{
    mistakeCount++;
    
    LogDebug($"[RememberScript] Mistake! Typed '{typed}' but expected '{expected}'. Total mistakes: {mistakeCount}");
    
    // Adjust audience score
    AdjustAudienceScore(currentRememberScriptNode.ScorePerMistake);
    
    // Fire mistake event
    OnRememberScriptMistake?.Invoke(typed, expected, mistakeCount);
    
    // Reset typed text
    currentTypedText = "";
    
    // Fire reset event for UI
    OnRememberScriptReset?.Invoke();
    
    // Check if too many mistakes
    if (mistakeCount >= currentRememberScriptNode.MaxMistakes)
    {
        HandleRememberScriptFailure();
    }
}

/// <summary>
/// Handle successful completion of Remember Script minigame
/// </summary>
private void HandleRememberScriptSuccess()
{
    LogDebug($"[RememberScript] Success! Completed with {mistakeCount} mistakes");
    
    // Adjust audience score
    AdjustAudienceScore(currentRememberScriptNode.ScoreOnSuccess);
    
    // Fire success event
    OnRememberScriptSuccess?.Invoke(currentTypedText, mistakeCount);
    
    // Clean up state
    CleanupRememberScript();
    
    // Advance to next node (first choice, or auto-advance)
    if (currentNode.HasChoices && currentNode.Choices.Count > 0)
    {
        SelectChoice(0); // Select first choice (typically "Continue")
    }
    else if (currentNode.HasAutoAdvance)
    {
        AdvanceDialog();
    }
}

/// <summary>
/// Handle failure due to too many mistakes
/// </summary>
private void HandleRememberScriptFailure()
{
    LogDebug($"[RememberScript] Failed! Too many mistakes: {mistakeCount}/{currentRememberScriptNode.MaxMistakes}");
    
    // Fire failure event
    OnRememberScriptFailure?.Invoke(mistakeCount);
    
    // Check if there's a specific failure node
    if (!string.IsNullOrEmpty(currentRememberScriptNode.MistakeNodeName))
    {
        var failureNode = currentTree.FindNodeByName(currentRememberScriptNode.MistakeNodeName);
        if (failureNode != null)
        {
            LogDebug($"[RememberScript] Jumping to failure node: {failureNode.GetDisplayName()}");
            CleanupRememberScript();
            MoveToNode(failureNode);
            return;
        }
    }
    
    // Otherwise, reset and retry
    mistakeCount = 0;
    currentTypedText = "";
    
    // Fire retry event
    OnRememberScriptRetry?.Invoke();
}

/// <summary>
/// Clean up Remember Script state
/// </summary>
private void CleanupRememberScript()
{
    isRememberScriptActive = false;
    currentRememberScriptNode = null;
    currentTypedText = "";
    mistakeCount = 0;
    
    // Fire cleanup event
    OnRememberScriptEnded?.Invoke();
}

#endregion

#region Remember Script Events

/// <summary> Fired when Remember Script minigame starts </summary>
public event System.Action<DialogNode> OnRememberScriptStarted;

/// <summary> Fired when progress is made (correct letter) </summary>
public event System.Action<string, string> OnRememberScriptProgress; // (currentText, targetPhrase)

/// <summary> Fired when a mistake is made </summary>
public event System.Action<char, char, int> OnRememberScriptMistake; // (typed, expected, mistakeCount)

/// <summary> Fired when text resets after mistake </summary>
public event System.Action OnRememberScriptReset;

/// <summary> Fired on successful completion </summary>
public event System.Action<string, int> OnRememberScriptSuccess; // (completedText, totalMistakes)

/// <summary> Fired on failure due to too many mistakes </summary>
public event System.Action<int> OnRememberScriptFailure; // (totalMistakes)

/// <summary> Fired when retrying after failure </summary>
public event System.Action OnRememberScriptRetry;

/// <summary> Fired when minigame ends (cleanup) </summary>
public event System.Action OnRememberScriptEnded;

#endregion
```

**Integrate with existing HandleNodeChanged**:
```csharp
private void HandleNodeChanged(DialogNode node)
{
    // ... existing code ...
    
    // Check for Remember Script minigame
    if (node.IsRememberScriptNode)
    {
        InitializeRememberScript(node);
    }
    
    // Fire OnNodeChanged event
    OnNodeChanged?.Invoke(node);
}
```

---

### 3.3 DialogManager UI Updates

Add UI handling for Remember Script minigame:

```csharp
#region Remember Script UI

[Header("Remember Script UI")]
[SerializeField] private VisualElement rememberScriptContainer;
[SerializeField] private Label targetPhraseLabel;
[SerializeField] private Label typedTextLabel;
[SerializeField] private Label mistakesLabel;
[SerializeField] private VisualElement progressBar;

private bool isRememberScriptUIActive = false;

#endregion

#region Remember Script Event Handlers

private void OnEnable()
{
    // ... existing subscriptions ...
    
    if (navigator != null)
    {
        navigator.OnRememberScriptStarted += HandleRememberScriptStarted;
        navigator.OnRememberScriptProgress += HandleRememberScriptProgress;
        navigator.OnRememberScriptMistake += HandleRememberScriptMistake;
        navigator.OnRememberScriptReset += HandleRememberScriptReset;
        navigator.OnRememberScriptSuccess += HandleRememberScriptSuccess;
        navigator.OnRememberScriptFailure += HandleRememberScriptFailure;
        navigator.OnRememberScriptEnded += HandleRememberScriptEnded;
    }
}

private void OnDisable()
{
    // ... existing unsubscriptions ...
    
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
}

private void HandleRememberScriptStarted(DialogNode node)
{
    LogDebug("[RememberScript UI] Started");
    
    // Hide normal dialog UI
    HideChoices();
    
    // Show Remember Script UI
    ShowRememberScriptUI(node);
}

private void ShowRememberScriptUI(DialogNode node)
{
    // Get UI elements
    if (rememberScriptContainer == null)
    {
        rememberScriptContainer = rootElement.Q<VisualElement>("RememberScriptContainer");
    }
    
    if (rememberScriptContainer == null)
    {
        LogError("[RememberScript UI] Container not found");
        return;
    }
    
    // Show container
    rememberScriptContainer.style.display = DisplayStyle.Flex;
    isRememberScriptUIActive = true;
    
    // Set target phrase
    if (targetPhraseLabel != null)
    {
        targetPhraseLabel.text = node.TargetPhrase;
    }
    
    // Reset typed text
    if (typedTextLabel != null)
    {
        typedTextLabel.text = "";
    }
    
    // Reset mistakes
    if (mistakesLabel != null)
    {
        mistakesLabel.text = $"Mistakes: 0/{node.MaxMistakes}";
    }
    
    LogDebug("[RememberScript UI] UI shown");
}

private void HandleRememberScriptProgress(string currentText, string targetPhrase)
{
    // Update typed text with color coding
    if (typedTextLabel != null)
    {
        // Build rich text with green for correct letters
        string richText = $"<color=green>{currentText}</color>";
        
        // Add remaining letters in gray
        if (currentText.Length < targetPhrase.Length)
        {
            richText += $"<color=#888888>{targetPhrase.Substring(currentText.Length)}</color>";
        }
        
        typedTextLabel.text = richText;
    }
    
    // Update progress bar
    if (progressBar != null)
    {
        float progress = (float)currentText.Length / targetPhrase.Length;
        progressBar.style.width = Length.Percent(progress * 100);
    }
}

private void HandleRememberScriptMistake(char typed, char expected, int mistakeCount)
{
    LogDebug($"[RememberScript UI] Mistake! Expected '{expected}' but got '{typed}'");
    
    // Update mistakes display
    if (mistakesLabel != null && currentRememberScriptNode != null)
    {
        mistakesLabel.text = $"Mistakes: {mistakeCount}/{currentRememberScriptNode.MaxMistakes}";
    }
    
    // Flash red or show error animation
    if (typedTextLabel != null)
    {
        StartCoroutine(FlashRedText(typedTextLabel));
    }
    
    // Play error sound
    // TODO: PlayErrorSound();
}

private void HandleRememberScriptReset()
{
    LogDebug("[RememberScript UI] Reset");
    
    // Clear typed text
    if (typedTextLabel != null)
    {
        typedTextLabel.text = "";
    }
    
    // Reset progress bar
    if (progressBar != null)
    {
        progressBar.style.width = Length.Percent(0);
    }
}

private void HandleRememberScriptSuccess(string completedText, int totalMistakes)
{
    LogDebug($"[RememberScript UI] Success! {completedText} with {totalMistakes} mistakes");
    
    // Show success animation
    if (typedTextLabel != null)
    {
        // Full green text
        typedTextLabel.text = $"<color=green>{completedText}</color>";
        StartCoroutine(FlashGreenText(typedTextLabel));
    }
    
    // Play success sound
    // TODO: PlaySuccessSound();
    
    // Hide UI after delay
    StartCoroutine(HideRememberScriptUIAfterDelay(1.5f));
}

private void HandleRememberScriptFailure(int mistakeCount)
{
    LogDebug($"[RememberScript UI] Failed with {mistakeCount} mistakes");
    
    // Show failure message
    if (dialogLabel != null)
    {
        dialogLabel.text = "Too many mistakes! Try again.";
    }
    
    // Play failure sound
    // TODO: PlayFailureSound();
}

private void HandleRememberScriptEnded()
{
    LogDebug("[RememberScript UI] Ended");
    
    HideRememberScriptUI();
}

private void HideRememberScriptUI()
{
    if (rememberScriptContainer != null)
    {
        rememberScriptContainer.style.display = DisplayStyle.None;
    }
    
    isRememberScriptUIActive = false;
}

private IEnumerator HideRememberScriptUIAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    HideRememberScriptUI();
}

private IEnumerator FlashRedText(Label label)
{
    string originalText = label.text;
    label.text = $"<color=red>✗ Wrong!</color>";
    yield return new WaitForSeconds(0.5f);
    label.text = originalText;
}

private IEnumerator FlashGreenText(Label label)
{
    string originalText = label.text;
    label.text = $"<color=green>✓ Perfect!</color>";
    yield return new WaitForSeconds(1f);
    label.text = originalText;
}

#endregion

#region Remember Script Input Handling

private void Update()
{
    // ... existing input handling ...
    
    // Handle Remember Script input
    if (isRememberScriptUIActive && navigator != null)
    {
        // Get any key press
        if (Input.anyKeyDown)
        {
            foreach (char c in Input.inputString)
            {
                // Ignore control characters
                if (c >= 32 && c <= 126) // Printable ASCII
                {
                    navigator.ProcessRememberScriptInput(c);
                }
            }
        }
    }
}

#endregion
```

---

### 3.4 UI Toolkit (UXML) Structure

Add to your dialog UI `.uxml` file:

```xml
<ui:VisualElement name="RememberScriptContainer" style="display: none; flex-grow: 1;">
    <ui:Label name="RememberScriptTitle" text="Remember the Script!" style="font-size: 24px; -unity-text-align: center; margin-bottom: 20px;" />
    
    <ui:Label name="TargetPhraseLabel" text="Target phrase appears here" style="font-size: 18px; -unity-text-align: center; margin-bottom: 10px;" />
    
    <ui:VisualElement name="ProgressBarContainer" style="background-color: #333; height: 20px; margin-bottom: 20px;">
        <ui:VisualElement name="ProgressBar" style="background-color: #4CAF50; height: 100%; width: 0%;" />
    </ui:VisualElement>
    
    <ui:Label name="TypedTextLabel" text="" style="font-size: 20px; -unity-text-align: center; margin-bottom: 20px; min-height: 40px;" />
    
    <ui:Label name="MistakesLabel" text="Mistakes: 0/3" style="font-size: 16px; -unity-text-align: center; color: #FF6666;" />
</ui:VisualElement>
```

---

### 3.5 PropertyDrawer Enhancements

Update `DialogNodePropertyDrawer.cs` to show Remember Script settings:

```csharp
// After Minigame Settings (Calm Dialog) section:

// Remember Script Minigame Section
var isRememberScriptProp = property.FindPropertyRelative("_isRememberScriptNode");
if (isRememberScriptProp != null)
{
    yPos += lineHeight;
    EditorGUI.LabelField(new Rect(contentRect.x, yPos, contentRect.width, singleLine),
        "Remember Script Minigame", EditorStyles.boldLabel);
    yPos += lineHeight;
    
    EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, singleLine),
        isRememberScriptProp, new GUIContent("Is Remember Script Node", "Enable typing minigame"));
    yPos += lineHeight;
    
    if (isRememberScriptProp.boolValue)
    {
        EditorGUI.indentLevel++;
        
        // Target Phrase
        var targetPhraseProp = property.FindPropertyRelative("_targetPhrase");
        if (targetPhraseProp != null)
        {
            float textAreaHeight = EditorGUI.GetPropertyHeight(targetPhraseProp);
            EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, textAreaHeight),
                targetPhraseProp, new GUIContent("Target Phrase", "The exact phrase player must type"));
            yPos += textAreaHeight + 2;
        }
        
        // Case Sensitive
        var caseSensitiveProp = property.FindPropertyRelative("_caseSensitive");
        if (caseSensitiveProp != null)
        {
            EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, singleLine),
                caseSensitiveProp, new GUIContent("Case Sensitive", "Require exact case match"));
            yPos += lineHeight;
        }
        
        // Ignore Whitespace
        var ignoreWhitespaceProp = property.FindPropertyRelative("_ignoreWhitespace");
        if (ignoreWhitespaceProp != null)
        {
            EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, singleLine),
                ignoreWhitespaceProp, new GUIContent("Ignore Whitespace", "Don't penalize for missing spaces"));
            yPos += lineHeight;
        }
        
        // Max Mistakes
        var maxMistakesProp = property.FindPropertyRelative("_maxMistakes");
        if (maxMistakesProp != null)
        {
            EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, singleLine),
                maxMistakesProp, new GUIContent("Max Mistakes", "Maximum mistakes before failure"));
            yPos += lineHeight;
        }
        
        // Score Per Mistake
        var scorePerMistakeProp = property.FindPropertyRelative("_scorePerMistake");
        if (scorePerMistakeProp != null)
        {
            EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, singleLine),
                scorePerMistakeProp, new GUIContent("Score Per Mistake", "Audience score change per mistake"));
            yPos += lineHeight;
        }
        
        // Score On Success
        var scoreOnSuccessProp = property.FindPropertyRelative("_scoreOnSuccess");
        if (scoreOnSuccessProp != null)
        {
            EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, singleLine),
                scoreOnSuccessProp, new GUIContent("Score On Success", "Audience score reward on completion"));
            yPos += lineHeight;
        }
        
        // Auto Retry On Mistake
        var autoRetryProp = property.FindPropertyRelative("_autoRetryOnMistake");
        if (autoRetryProp != null)
        {
            EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, singleLine),
                autoRetryProp, new GUIContent("Auto Retry", "Reset immediately on mistake"));
            yPos += lineHeight;
        }
        
        // Mistake Node Name
        var mistakeNodeNameProp = property.FindPropertyRelative("_mistakeNodeName");
        if (mistakeNodeNameProp != null)
        {
            EditorGUI.PropertyField(new Rect(contentRect.x, yPos, contentRect.width, singleLine),
                mistakeNodeNameProp, new GUIContent("Failure Node", "Node to jump to on too many mistakes"));
            yPos += lineHeight;
        }
        
        EditorGUI.indentLevel--;
    }
}
```

---

## 4. Example Dialog Tree Setup

### Example 1: Simple Remember Script Challenge

```
Node: "act1_perf_remember_01"
Speaker: "Director"
Text: "Quick! What's your next line?"
Is Remember Script Node: ✅
Target Phrase: "The stage is my home"
Case Sensitive: ❌
Ignore Whitespace: ✅
Max Mistakes: 3
Score Per Mistake: -5
Score On Success: 20
Auto Retry: ✅

Choice: "Continue" → Next node
```

### Example 2: Hard Mode with Punctuation

```
Node: "act2_perf_remember_hard_01"
Speaker: "Director"
Text: "Deliver the climactic line!"
Is Remember Script Node: ✅
Target Phrase: "To be, or not to be?"
Case Sensitive: ❌
Ignore Whitespace: ❌ (punctuation matters)
Max Mistakes: 2
Score Per Mistake: -10
Score On Success: 30
Failure Node: "act2_perf_fail_01"
```

---

## 5. User Experience Flow

### 5.1 Successful Completion

1. **Start**: Dialog displays "Remember this line!"
2. **UI Switches**: Normal dialog UI hides, typing UI appears
3. **Display**: Target phrase shown at top in gray
4. **Typing**: Player types letter-by-letter
5. **Feedback**: Each correct letter turns green
6. **Progress**: Progress bar fills
7. **Success**: All letters green, success animation plays
8. **Reward**: Audience score increases
9. **Continue**: Dialog advances to next node

### 5.2 With Mistakes

1. **Start**: Same as above
2. **Typing**: Player mistypes a letter
3. **Feedback**: Screen flashes red, "✗ Wrong!" message
4. **Reset**: Typed text clears, starts over
5. **Penalty**: Audience score decreases slightly
6. **Retry**: Player can try again immediately
7. **Limit**: After 3 mistakes, strong penalty or failure node

---

## 6. Audio/Visual Feedback

### Sounds
- **Correct Letter**: Soft "tick" sound
- **Mistake**: Error buzz sound
- **Reset**: Whoosh sound
- **Success**: Victory chime
- **Failure**: Dramatic "boo" from audience

### Visual Effects
- **Correct Letters**: Fade to green
- **Mistake**: Flash red, shake animation
- **Progress Bar**: Smooth fill animation
- **Success**: Gold sparkle effect
- **Failure**: Red X overlay

---

## 7. Difficulty Variations

### Easy Mode
```csharp
Target Phrase: "Hello"
Case Sensitive: false
Ignore Whitespace: true
Max Mistakes: 5
```

### Normal Mode
```csharp
Target Phrase: "The show must go on"
Case Sensitive: false
Ignore Whitespace: false
Max Mistakes: 3
```

### Hard Mode
```csharp
Target Phrase: "To be, or not to be: that is the question!"
Case Sensitive: true
Ignore Whitespace: false
Max Mistakes: 1
```

---

## 8. Implementation Checklist

### Phase 1: Core Functionality
- [ ] Add Remember Script fields to `DialogNode.cs`
- [ ] Add Remember Script properties and validation
- [ ] Implement `ProcessRememberScriptInput()` in `DialogNavigator.cs`
- [ ] Add event handlers for mistakes and success
- [ ] Implement reset logic

### Phase 2: UI Integration
- [ ] Create UXML layout for Remember Script UI
- [ ] Add USS styling for typing interface
- [ ] Implement UI event handlers in `DialogManager.cs`
- [ ] Add letter coloring system (green/red)
- [ ] Implement progress bar

### Phase 3: PropertyDrawer
- [ ] Update `DialogNodePropertyDrawer.cs`
- [ ] Add collapsible Remember Script section
- [ ] Add validation warnings for empty target phrase
- [ ] Add height calculations for new fields

### Phase 4: Testing
- [ ] Test basic typing functionality
- [ ] Test mistake handling and reset
- [ ] Test case sensitivity modes
- [ ] Test whitespace handling
- [ ] Test audience score integration
- [ ] Test failure node jumping

### Phase 5: Polish
- [ ] Add audio feedback
- [ ] Add visual effects (flash, shake)
- [ ] Add success/failure animations
- [ ] Add keyboard hints (show next expected letter?)
- [ ] Add difficulty presets

---

## 9. Integration with Existing Systems

### Audience Manager
```csharp
// In DialogNavigator.cs
private void AdjustAudienceScore(float scoreChange)
{
    var audienceManager = FindFirstObjectByType<AudienceManager>();
    if (audienceManager != null)
    {
        if (scoreChange > 0)
        {
            audienceManager.AudienceApplause((int)scoreChange / 2); // Convert to 1-10 scale
        }
        else
        {
            audienceManager.AudienceReaction("boo");
        }
    }
}
```

### Game System Dialog Events
```csharp
// Create Remember Script specific events
public class RememberScriptSuccessEvent : DialogEvent
{
    public override void Execute()
    {
        // Trigger game state changes on success
        GameManager.Instance?.UnlockAbility("PerfectMemory");
    }
}
```

---

## 10. Future Enhancements

### Potential Additions
- **Time Limit**: Add countdown timer for pressure
- **Hint System**: Show first letter after delay
- **Difficulty Scaling**: Increase phrase length over time
- **Multiplayer**: Race against another actor
- **Combo System**: Chain successful completions for bonus
- **Backspace Support**: Allow corrections before confirming
- **Word-by-Word**: Complete whole words at a time

---

## 11. Testing Scenarios

### Test Case 1: Basic Typing
```
Target: "Hello"
Steps: Type H-e-l-l-o correctly
Expected: Success, green text, score +20
```

### Test Case 2: Single Mistake
```
Target: "Hello"
Steps: Type H-e-p (mistake) → Reset → H-e-l-l-o
Expected: Reset on 'p', success after correct attempt, score +15 (after penalty)
```

### Test Case 3: Max Mistakes
```
Target: "Hello"
Max Mistakes: 3
Steps: Make 3 mistakes in a row
Expected: Jump to failure node or retry with heavy penalty
```

### Test Case 4: Case Sensitivity
```
Target: "Hello"
Case Sensitive: true
Steps: Type h-e-l-l-o (lowercase h)
Expected: Mistake on first letter
```

### Test Case 5: Whitespace Handling
```
Target: "Hello World"
Ignore Whitespace: true
Steps: Type HelloWorld (no space)
Expected: Success (space ignored)
```

---

## 12. Documentation Updates

### Files to Update
- `Class Hierarchy.md`: Add Remember Script properties
- `Project_Roadmap.md`: Mark as implemented
- `Requirements.md`: Update minigame description
- `TROUBLESHOOTING.MD`: Add common Remember Script issues

---

## 13. Summary

### Key Benefits
✅ **Integrated**: Works seamlessly with existing dialog system  
✅ **Flexible**: Easy to configure difficulty per node  
✅ **Reusable**: Same architecture as CalmDialog minigame  
✅ **Extensible**: Can add new features without major refactoring  
✅ **Validated**: Follows proven implementation pattern  

### Implementation Estimate
- **Phase 1-2**: ~4-6 hours (core functionality + UI)
- **Phase 3**: ~1-2 hours (PropertyDrawer)
- **Phase 4**: ~2-3 hours (testing)
- **Phase 5**: ~3-4 hours (polish)
- **Total**: ~10-15 hours

---

**Status**: ⏳ Ready for Implementation  
**Dependencies**: CalmDialog minigame (for reference), DialogNode minigame architecture  
**Next Steps**: Review proposal → Approve → Implement Phase 1 → Test → Iterate

---

**End of Proposal**
