# UI Toolkit Troubleshooting

**Last Updated**: December 2025  
**Related Systems**: UIDocument, VisualTreeAsset, UXML, USS

---

## Minigame UI Not Visible / Gets Hidden

**Symptoms**:
- Console shows `[MinigameUIManager] Minigame UI shown` but nothing appears on screen
- Dialog UI fades out but minigame UI never appears
- UI elements exist but remain invisible
- Game accepts input but shows no visual feedback

### Root Causes & Solutions:

#### 1. **Minigame UI Has No Styling (CSS/USS)**

**Problem**: `RememberTheScriptUI` creates UI elements but they have **no visual styling**, so they're invisible even though they exist.

**Check**:
```csharp
// In RememberTheScriptUI.BuildUI():
rootElement.AddToClassList("minigame-root");  // Needs CSS!
titleLabel.AddToClassList("minigame-title");  // Needs CSS!
```

**Solution - Create USS Styles**:

Create or add to your USS file (e.g., `DialogStyles.uss`):

```css
/* Minigame Root Container */
.minigame-root {
    width: 80%;
    height: 60%;
    background-color: rgba(20, 20, 30, 0.95);
    border-color: rgb(100, 200, 255);
    border-width: 3px;
    border-radius: 10px;
    padding: 20px;
    align-items: center;
    justify-content: flex-start;
}

/* Minigame Title */
.minigame-title {
    font-size: 32px;
    color: rgb(255, 255, 100);
    -unity-font-style: bold;
    margin-bottom: 20px;
    -unity-text-align: middle-center;
}

/* Target Phrase - What to Type */
.target-phrase {
    font-size: 24px;
    color: rgb(200, 200, 200);
    background-color: rgba(0, 0, 0, 0.5);
    padding: 15px;
    margin-bottom: 20px;
    border-radius: 5px;
    -unity-text-align: middle-center;
    white-space: normal;
}

/* Typed Text - Progress Display */
.typed-text {
    font-size: 24px;
    color: rgb(0, 255, 0); /* Green for correct */
    padding: 10px;
    margin-bottom: 20px;
    -unity-text-align: middle-center;
    min-height: 30px;
}

/* Stats Container */
.stats-container {
    width: 100%;
    flex-direction: column;
    margin-top: 20px;
}

/* Mistake Counter */
.mistake-count {
    font-size: 20px;
    color: rgb(255, 255, 255);
    margin-bottom: 10px;
}

.mistake-count.warning {
    color: rgb(255, 100, 100); /* Red when close to limit */
    -unity-font-style: bold;
}

/* Mistake Bar */
.mistake-bar {
    width: 100%;
    height: 20px;
    margin-bottom: 10px;
}

.mistake-bar.warning {
    background-color: rgb(255, 0, 0);
}

/* Timer */
.timer-label {
    font-size: 20px;
    color: rgb(255, 255, 255);
    margin-top: 10px;
}

.timer-label.warning {
    color: rgb(255, 150, 0); /* Orange when time running out */
    -unity-font-style: bold;
}

/* Feedback Container */
.feedback-container {
    width: 100%;
    min-height: 100px;
    flex-direction: column;
    align-items: center;
    margin-top: 20px;
}

/* Feedback Flash (✓/✗) */
.feedback-flash {
    font-size: 24px;
    -unity-font-style: bold;
    margin: 5px;
}

/* Success Message */
.success-message {
    font-size: 32px;
    color: rgb(0, 255, 0);
    -unity-font-style: bold;
    -unity-text-align: middle-center;
    padding: 20px;
}

/* Failure Message */
.failure-message {
    font-size: 32px;
    color: rgb(255, 0, 0);
    -unity-font-style: bold;
    -unity-text-align: middle-center;
    padding: 20px;
}
```

**Apply the USS**:
1. Open your Dialog UI in UI Builder
2. Click on the root element
3. In Inspector → Style Sheets → Add Style Sheet
4. Select your USS file
5. Save UXML

---

#### 2. **Minigame Container Not Added to Correct Parent**

**Check**:
```csharp
// In MinigameUIManager.ShowMinigameUI():
dialogBox.Add(minigameContainer);  // Is dialogBox visible?
```

**Solution**:
```csharp
// Verify dialogBox is visible before adding minigame:
Debug.Log($"DialogBox display: {dialogBox.style.display.value}");
Debug.Log($"DialogBox opacity: {dialogBox.style.opacity.value}");

// Ensure dialogBox stays visible:
dialogBox.style.display = DisplayStyle.Flex;
dialogBox.style.opacity = 1f;
```

---

#### 3. **Display Property Not Set**

**Problem**: `minigameContainer` is created but never made visible.

**Check**:
```csharp
// In MinigameUIManager.CreateMinigameContainer():
minigameContainer.style.display = DisplayStyle.None;  // Hidden by default!
```

**Solution**: Ensure `ShowMinigameUI()` sets display:
```csharp
private void ShowMinigameUI()
{
    // ... add to dialogBox ...
    
    // CRITICAL: Set to visible!
    minigameContainer.style.display = DisplayStyle.Flex;
    minigameContainer.style.opacity = 1f;
    
    LogDebug("Minigame UI shown");
}
```

---

#### 4. **Fade-In Coroutine Not Completing**

**Problem**: `FadeIn()` coroutine may not run properly.

**Debug**:
```csharp
private IEnumerator FadeIn(VisualElement element, float duration)
{
    Debug.Log($"[FadeIn] Starting fade for {element.name}");
    element.style.display = DisplayStyle.Flex;
    element.style.opacity = 0f;
    
    float elapsed = 0f;
    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        element.style.opacity = t;
        Debug.Log($"[FadeIn] Opacity: {t}");  // Add this!
        yield return null;
    }
    element.style.opacity = 1f;
    Debug.Log($"[FadeIn] Complete for {element.name}");  // Add this!
}
```

---

#### 5. **Z-Order / Layering Issue**

**Problem**: Minigame UI is behind other elements.

**Solution**:
```csharp
// Set higher sort order for minigame:
minigameContainer.BringToFront();

// Or use explicit z-index in USS:
.minigame-root {
    position: absolute;  /* Takes it out of normal flow */
    z-index: 1000;       /* High value = on top */
}
```

---

### Diagnostic Checklist:

**When minigame UI doesn't appear, check in this order:**

1. ✅ **Console shows "Minigame UI shown"?**
   - ❌ No → Check event subscription in `MinigameUIManager.SubscribeToEvents()`
   - ✅ Yes → Continue to #2

2. ✅ **Is USS file attached to UIDocument?**
   - Open UI Builder → Check Style Sheets panel
   - If not attached → Add your USS file
   - Test: Add a bright background color to `.minigame-root`

3. ✅ **Are CSS classes applied?**
   ```csharp
   Debug.Log($"Root classes: {string.Join(", ", rootElement.GetClasses())}");
   ```

4. ✅ **Is display set to Flex?**
   ```csharp
   Debug.Log($"Container display: {minigameContainer.style.display.value}");
   Debug.Log($"Root display: {rootElement.style.display.value}");
   ```

5. ✅ **Check worldBound (non-zero = visible area):**
   ```csharp
   Debug.Log($"Container bounds: {minigameContainer.worldBound}");
   ```

6. ✅ **Is minigameContainer added to hierarchy?**
   ```csharp
   Debug.Log($"Container parent: {minigameContainer.parent?.name}");
   Debug.Log($"Container child count: {minigameContainer.childCount}");
   ```

---

### Quick Test - Force Show Minigame UI:

Add this context menu to `MinigameUIManager.cs`:

```csharp
[ContextMenu("Force Show Minigame UI (Test)")]
private void ForceShowMinigameUITest()
{
    if (rememberScriptUI == null)
    {
        rememberScriptUI = new RememberTheScriptUI();
        rememberScriptUI.Initialize("Test Phrase", 3, 30f);
    }
    
    currentMinigameType = MinigameType.RememberTheScript;
    isMinigameActive = true;
    
    ShowMinigameUI();
    
    // Force visibility
    minigameContainer.style.display = DisplayStyle.Flex;
    minigameContainer.style.opacity = 1f;
    minigameContainer.style.backgroundColor = new StyleColor(Color.blue); // Debug color
    
    Debug.Log($"[TEST] Minigame UI forced visible");
    Debug.Log($"Container bounds: {minigameContainer.worldBound}");
}
```

Then in Play Mode:
1. Right-click MinigameUIManager component
2. Click "Force Show Minigame UI (Test)"
3. Check if blue background appears
4. If no blue → USS/styling issue
5. If blue but no text → Label styling issue

---

### Common Fix: Minimal Working USS

If you have **no styling at all**, use this minimal USS to make text visible:

```css
.minigame-root,
.minigame-title,
.target-phrase,
.typed-text,
.mistake-count,
.timer-label,
.feedback-flash,
.success-message,
.failure-message {
    color: white;
    font-size: 20px;
    background-color: black;
    padding: 10px;
}
```

This will at least make text visible so you can debug further!

---

## UI Elements Not Found

**Error**: `DialogBox GroupBox not found in UI - check UXML structure`

**Causes**:
1. Incorrect element names in UXML
2. Element hierarchy changed
3. UIDocument not assigned
4. Visual Tree Asset not assigned

**Solution**:
1. Open UXML in UI Builder
2. Verify element names match code:
   - `DialogBox` (GroupBox)
   - `GivenDialogLabel` (Label)
   - `DialogOption1Btn` through `DialogOption5Btn` (Buttons)

3. Check element structure in Hierarchy panel
4. Verify UIDocument component has Visual Tree Asset assigned

---

## UI Not Visible / Display Issues

### Symptoms:
- Elements exist but don't show
- WorldBound is (0,0,0,0)
- Resolved style shows display:none

### Solutions:

**1. Check CSS Display**:
```css
/* In USS file: */
#DialogBox {
    display: flex; /* NOT none */
    opacity: 1;
}
```

**2. Force Display via Code**:
```csharp
dialogBox.style.display = DisplayStyle.Flex;
dialogBox.style.opacity = 1f;
dialogBox.MarkDirtyRepaint();
```

**3. Check Panel Settings**:
- UIDocument → Panel Settings must be assigned
- Panel Settings → Scale Mode: Scale With Screen Size
- Reference Resolution: 1920x1080 (or your target)

**4. Check Sort Order**:
- UIDocument → Sort Order (try 0, 100, or 1000)
- Higher values render on top

---

## Layout Issues

### UI Elements Overlapping or Misplaced

**Debug**:
```csharp
// Context Menu: "Debug Dialog Layout"
// Check Console for:
// - World bounds
// - Resolved style dimensions
// - Position values
```

**Common Fixes**:
1. Set explicit widths/heights in USS
2. Use flexbox layout correctly
3. Check parent container flex-direction
4. Verify position: absolute vs relative

### UI Not Scaling Properly

**Check**:
1. Panel Settings → Scale Mode
2. Panel Settings → Reference Resolution
3. Root element has correct dimensions
4. USS uses percentage-based sizing where appropriate

---

## Button Issues

### Buttons Not Responding to Clicks

**Causes**:
1. Button not found in UXML
2. Click event not registered
3. Button z-index issue
4. Button disabled in CSS

**Debug**:
```csharp
// Check if button exists:
var btn = rootElement.Q<Button>("DialogOption1Btn");
Debug.Log($"Button found: {btn != null}");

// Check if clickable:
Debug.Log($"Button enabled: {btn.enabledSelf}");
Debug.Log($"Button display: {btn.style.display.value}");
```

**Fix**:
```csharp
// Ensure event is registered:
button.clicked += () => OnChoiceClicked(0);

// Ensure button is enabled:
button.SetEnabled(true);
button.style.display = DisplayStyle.Flex;
```

---

## Styling Issues

### USS Not Applied

**Check**:
1. USS file attached to UIDocument
2. Class names match (case-sensitive)
3. Selectors are correct (.class vs #id)
4. USS has no syntax errors

**Test**:
```csharp
// Add class via code:
element.AddToClassList("my-class");

// Verify it's added:
Debug.Log($"Has class: {element.ClassListContains("my-class")}");
```

### Inline Styles vs USS Conflicts

**Priority Order** (highest to lowest):
1. Inline styles (set via code)
2. USS styles
3. Default styles

**Solution**:
- Remove inline styles if USS should control
- Or use inline for dynamic changes only

---

## Performance Issues

### UI Lag / Slow Updates

**Causes**:
1. Too many MarkDirtyRepaint() calls
2. Complex USS selectors
3. Large number of UI elements
4. Frequent text updates

**Solutions**:
1. Batch UI updates
2. Use simple selectors
3. Hide unused elements (display:none)
4. Update text only when changed

---

## Quick Diagnostics

**Context Menu Commands** (DialogManager component):
- `Test UI Elements` - Verify all elements found
- `Debug UI Document Settings` - Check UIDocument configuration
- `Debug Dialog Layout` - Check positioning and bounds
- `Force Show Dialog UI` - Test visibility

**Context Menu Commands** (MinigameUIManager component):
- `Force Show Minigame UI (Test)` - Test minigame visibility
- `Print Current State` - Check minigame manager state
