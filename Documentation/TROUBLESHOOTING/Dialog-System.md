# Dialog System Troubleshooting

**Last Updated**: December 2025  
**Related Systems**: DialogManager, DialogNavigator, DialogTree, NPCContent

---

## Dialog UI Not Appearing

**Symptoms**:
- Dialog is triggered but nothing appears on screen
- Console shows "DialogBox found!" but UI remains invisible
- Dialog navigation works (can hear/log events) but UI doesn't show

**Possible Causes & Solutions**:

### 1. UI Elements Not Properly Connected
```csharp
// Check in Unity Inspector on DialogManager GameObject:
// ✓ UIDocument component assigned
// ✓ Visual Tree Asset assigned
// ✓ Panel Settings assigned (in UIDocument)
```

### 2. DialogBox CSS Display Issue
- **Check**: Open DialogManager in scene, select UIDocument
- **Run**: DialogManager → Context Menu → "Force Show Dialog UI" (in Play Mode)
- **Verify**: Console shows world bounds are non-zero

### 3. Missing UI Toolkit Setup
- Verify `GivenDialogLabel` and `DialogBox` exist in UXML
- Check button names: `DialogOption1Btn`, `DialogOption2Btn`, etc.
- Run: DialogManager → Context Menu → "Test UI Elements"

### 4. Z-Order / Sort Order Issue
- Check UIDocument → Sort Order (try values: 0, 100, 1000)
- Verify Panel Settings → Scale Mode is set correctly

**Quick Fix**:
```csharp
// In Play Mode, select DialogManager GameObject
// Context Menu → "Force Show Dialog UI"
// Check Console for diagnostic output
```

---

## Input System Errors in Minigames

**Symptoms**:
- **Error**: `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, but you have switched active Input handling to Input System package in Player Settings`
- Minigame freezes when trying to capture keyboard input
- Typing has no effect in RememberTheScript minigame
- Console shows stack trace mentioning `UnityEngine.Input.get_inputString()`

**Root Cause**:
Code is using the **old Input System** (`UnityEngine.Input.inputString`) but the project is configured to use the **new Input System** package. The new Input System doesn't expose `Input.inputString`.

**Location of Issue**:
```csharp
// DialogManager.Input.cs, line ~80:
string inputString = Input.inputString; // ❌ OLD - Throws error
```

**Solution - Use New Input System Keyboard API**:

```csharp
// DialogManager.Input.cs - FIXED approach:
private void HandleRememberTheScriptInput()
{
    // Use new Input System Keyboard API
    var keyboard = Keyboard.current;
    
    if (keyboard == null)
    {
        return; // No keyboard available
    }
    
    // Check all character keys for typing input
    string typedChars = "";
    
    // Check for alphabetic keys (a-z)
    for (int i = (int)Key.A; i <= (int)Key.Z; i++)
    {
        var key = (Key)i;
        if (keyboard[key].wasPressedThisFrame)
        {
            char c = GetCharFromKey(key, keyboard);
            if (c != '\0')
            {
                typedChars += c;
            }
        }
    }
    
    // Check for number keys, space, punctuation...
    // (See full implementation in DialogManager.Input.cs)
    
    // Process typed characters
    foreach (char c in typedChars)
    {
        navigator.ProcessRememberScriptInput(c);
    }
}
```

**Key Points**:
- ✅ **Use**: `Keyboard.current` from `UnityEngine.InputSystem`
- ✅ **Check**: Individual keys with `.wasPressedThisFrame`
- ✅ **Handle**: Shift state for uppercase letters and symbols
- ❌ **Don't use**: `Input.inputString` (old system)
- ❌ **Don't use**: `Input.GetKeyDown()` (old system)

**Testing After Fix**:
1. Start Play Mode
2. Trigger a RememberTheScript minigame node
3. Type on keyboard
4. Characters should appear and be validated
5. Console should show: `[RememberTheScript] Character typed: 'X'`

**Related Documentation**:
- Unity Input System Package docs: https://docs.unity3d.com/Packages/com.unity.inputsystem@latest
- Keyboard API: https://docs.unity3d.com/Packages/com.unity.inputsystem@latest/api/UnityEngine.InputSystem.Keyboard.html

---

## Dialog UI Freezes When Minigame Starts

**Symptoms**:
- Dialog displays correctly before minigame
- Console shows minigame initialization
- Dialog becomes unresponsive to input
- **Keyboard typing has no effect**

**Root Cause**:
Split responsibility for input handling - minigame logic exists in DialogNavigator but no component actively captures keyboard input during minigames.

**Architecture Issue**:
```csharp
// Current (broken) flow:
Player types → ??? (nobody listening) → Nothing happens

// Expected flow:
Player types → DialogManager/DialogNavigator captures → ProcessTyping() → UI updates
```

**Solution - Add Input Handling**:

```csharp
// In DialogManager.Update():
private void Update()
{
    // Check if minigame is active and handle its input
    if (minigameUIManager != null && minigameUIManager.IsMinigameActive)
    {
        HandleMinigameInput(); // Route to appropriate handler
        return; // Don't process standard dialog input
    }
    
    // ...existing dialog input handling...
}

private void HandleMinigameInput()
{
    // Route to specific minigame input handler
    switch (minigameUIManager.CurrentMinigameType)
    {
        case MinigameType.RememberTheScript:
            HandleRememberTheScriptInput();
            break;
    }
}
```

**Related Files**:
- `DialogManager.cs` - Input handling
- `DialogManager.Input.cs` - Partial class with input methods
- `DialogNavigator.RememberTheScript.cs` - Minigame validation logic
- `MinigameUIManager.cs` - UI state management

**Expected Behavior After Fix**:
1. Dialog displays normally
2. Player triggers minigame
3. Dialog UI hides, minigame UI shows
4. Player types → characters captured → validation runs → UI updates
5. Minigame completes → returns to dialog

---

## Navigation Issues

### Dialog Doesn't Advance

**Check**:
1. Node has valid child or choices
2. Auto-advance delay is set (if using auto-advance)
3. Player input is enabled
4. DialogNavigator is active

**Debug**:
```csharp
// DialogManager context menu: "Print Current State"
// Check Console for current node, choices, auto-advance settings
```

### Choices Not Appearing

**Causes**:
1. Node.HasChoices returns false
2. Choice buttons not found in UI
3. Buttons hidden by CSS

**Fix**:
```csharp
// Check node in inspector:
// - Choices list has entries
// - Each choice has valid text
// - Target nodes are assigned
```

---

## Event System Issues

### Custom Actions Not Triggering

**Check**:
1. Choice has `ChoiceId` set
2. NPCContent has `HandleCustomAction()` implemented
3. DialogNavigator `OnCustomActionTriggered` event is subscribed

**Debug**:
```csharp
// Add breakpoint in NPCContent.HandleCustomAction()
// Or add Debug.Log to verify it's being called
```

---

## Quick Diagnostics

**DialogManager Context Menus** (Right-click component in Inspector):
- `Validate Setup` - Check all components connected
- `Print Current State` - Show current dialog state
- `Test UI Elements` - Verify UI structure
- `Force Show Dialog UI` - Force dialog visible for testing
- `Debug Dialog Layout` - Check UI positioning

**Common Console Messages**:
- `[DialogManager] Dialog session #X started` - Dialog started successfully
- `[DialogNavigator] Navigated to node: X` - Node changed
- `[DialogManager] Choice X clicked` - Player selected choice
- `[DialogNavigator] Ending dialog` - Dialog ended normally

**Common Error Messages**:
- `InvalidOperationException: You are trying to read Input using UnityEngine.Input` - See "Input System Errors in Minigames" section above
- `NullReferenceException: DialogNavigator` - DialogNavigator not initialized, check DialogManager initialization
- `DialogBox not found` - UI element missing in UXML, check UI Toolkit setup
