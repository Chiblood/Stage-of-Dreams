# ?? DialogManager UI Toolkit Connection Guide

## ? **Issues Fixed**

### 1. **Element Name Mismatches**
- **Fixed**: DialogManager was looking for `"GivenDialog"` but UXML has `"GivenDialogLabel"`
- **Fixed**: Button names updated to match UXML: `"DialogOption1Btn"`, `"DialogOption2Btn"`, etc.

### 2. **UI Toolkit Integration**
- **Enhanced**: Added CSS class support (`dialog-visible`, `choice-visible`)
- **Enhanced**: Better element visibility management
- **Enhanced**: Improved USS styling with transitions and hover effects

### 3. **Debugging Tools**
- **Added**: Multiple context menu debugging options
- **Added**: UI element testing and validation
- **Added**: Element hierarchy inspection

## ??? **Setup Instructions**

### Step 1: Setup GameObjects in Unity
1. Create an empty GameObject named "DialogManager"
2. Add the `DialogManager` component
3. Add a `UIDocument` component to the same GameObject

### Step 2: Assign UI Assets
In the DialogManager inspector:
1. **UIDocument**: Will auto-assign from the same GameObject
2. **Dialog Visual Tree**: Drag your `DialogOverlayVisualTree.uxml` file
3. **Enable Debug Logs**: Check for initial setup testing

### Step 3: Configure UI Document
In the UIDocument component:
1. **Source Asset**: Should be set to your `DialogOverlayVisualTree.uxml`
2. **Sort Order**: Set to a high value (e.g., 1000) to ensure dialog appears on top

### Step 4: Test the Connection
Use the context menu options on DialogManager (right-click component in inspector):

#### ?? **Debugging Context Menus:**
- **"Validate Setup"** - Checks all components and connections
- **"Test UI Elements"** - Verifies UXML elements can be found
- **"Test Show Dialog"** - Shows a test dialog (Play mode only)
- **"Test Hide Dialog"** - Hides the dialog (Play mode only)
- **"Print Current State"** - Shows current dialog manager state

## ?? **UI Styling Features**

### CSS Classes Added:
- `.dialog-visible` - Applied when dialog is shown
- `.choice-visible` - Applied to visible choice buttons
- Smooth transitions for show/hide animations
- Hover and active states for buttons

### USS Improvements:
- Better button styling with hover effects
- Improved dialog box appearance with borders and padding
- Responsive text wrapping for dialog content
- Smooth scale and opacity transitions

## ?? **Testing Your Setup**

### 1. **Basic Connection Test**
```csharp
// In Play mode, right-click DialogManager component:
// 1. "Validate Setup" - Should show green checkmarks
// 2. "Test UI Elements" - Should find all elements
// 3. "Test Show Dialog" - Should display test dialog
```

### 2. **Integration Test with NPCs**
```csharp
// Setup test NPC:
// 1. Create GameObject with NPCContent component
// 2. Create DialogTree asset and assign to NPC
// 3. Add DialogueTrigger component
// 4. Configure trigger settings and test
```

### 3. **Visual Validation**
- Dialog should appear with smooth scale/fade animation
- Buttons should highlight on hover
- Text should wrap properly in dialog area
- Dialog should be properly centered/positioned

## ?? **Common Issues & Solutions**

### Issue: "DialogBox GroupBox not found"
**Solution**: Check that your UXML file is properly assigned and element names match

### Issue: "No UIDocument component found"
**Solution**: Add UIDocument component to the same GameObject as DialogManager

### Issue: "Dialog doesn't appear"
**Solution**: 
- Check UIDocument Sort Order (should be high)
- Verify UXML asset is assigned
- Check CSS styling isn't hiding elements

### Issue: "Buttons don't respond"
**Solution**:
- Verify button names in UXML match the code
- Check that buttons are properly initialized
- Use "Test UI Elements" to verify button detection

## ?? **UXML Structure Reference**

Your current structure:
```xml
<ui:GroupBox name="DialogBox">
    <ui:Label name="GivenDialogLabel" />
    <ui:Button name="DialogOption1Btn" />
    <ui:Button name="DialogOption2Btn" />
    <ui:Button name="DialogOption3Btn" />
    <ui:Button name="DialogOption4Btn" />
    <ui:Button name="DialogOption5Btn" />
</ui:GroupBox>
```

All element names now match the DialogManager queries! ?

## ?? **Next Steps**

1. **Test Basic Connection**: Use the debugging context menus
2. **Create Test Dialog**: Set up a simple NPC with DialogTree
3. **Style Customization**: Modify USS file for your visual design
4. **Integration Testing**: Test with your existing NPCs and DialogueTriggers

## ?? **Advanced Customization**

### Custom CSS Classes
Add your own classes to USS file and apply them via:
```csharp
dialogBox.AddToClassList("your-custom-class");
```

### Dynamic Styling
```csharp
// Example: Change dialog colors based on NPC type
private void StyleDialogForNPC(NPCContent npc)
{
    if (npc.npcType == NPCType.Enemy)
        dialogBox.AddToClassList("enemy-dialog");
    else
        dialogBox.AddToClassList("friendly-dialog");
}
```

Your DialogManager is now fully connected to UI Toolkit with comprehensive debugging tools! ??