# UI Toolkit Troubleshooting

**Last Updated**: December 2025  
**Related Systems**: UIDocument, VisualTreeAsset, UXML, USS

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
