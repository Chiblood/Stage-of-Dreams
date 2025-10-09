# Dialog System Inspector Editing Guide - HEADER CONFLICTS RESOLVED

## ? **CRITICAL ISSUE FIXED: Header Attribute Conflicts**

### ?? **The Problem You Discovered:**
**Unity's `[Header]` attributes in data classes conflict with custom PropertyDrawers and CustomEditors!**

When you use `[Header]` attributes in `DialogTree.cs` and `DialogNode.cs`, they create overlapping and messy layouts because:
- The data class headers render first
- Then the PropertyDrawer tries to draw its own headers
- This causes **double headers, overlapping text, and broken layouts**

### ? **The Solution: Separation of Concerns**

**Fixed by removing ALL `[Header]` attributes from data classes:**
- ? **DialogTree.cs**: Removed all `[Header]` attributes
- ? **DialogNode.cs**: Removed all `[Header]` attributes  
- ? **DialogChoice.cs**: Removed all `[Header]` attributes
- ? **PropertyDrawers**: Handle ALL visual organization
- ? **CustomEditor**: Handles main tree-level organization

## ?? **Clean Inspector Layout (No Conflicts)**

### **Current Clean Structure:**

```
DialogTree Inspector:
???????????????????????????????????????????
? ? Enhanced DialogTree Editor (info)    ?  ? CustomEditor
???????????????????????????????????????????
? Tree Information                        ?  ? CustomEditor
?  ?? Tree Name                          ?  ? Raw field
?  ?? Description                        ?  ? Raw field  
?  ?? Auto Update Node List              ?  ? Raw field
???????????????????????????????????????????
? Dialog Flow                             ?  ? CustomEditor
?  ?? ? Starting Node                    ?  ? PropertyDrawer takes over
?      ?? Dialog Content                 ?  ? PropertyDrawer header
?      ?   ?? Node Name                  ?  ? PropertyDrawer field
?      ?   ?? Speaker Name               ?  ? PropertyDrawer field
?      ?   ?? Dialog Text               ?  ? PropertyDrawer field
?      ?   ?? Is Player Speaking        ?  ? PropertyDrawer field
?      ?? Tree Structure                 ?  ? PropertyDrawer header
?      ?   ?? Parent Node               ?  ? PropertyDrawer field
?      ?? ? Events (collapsible)        ?  ? PropertyDrawer header
?      ?   ?? On Dialog Start           ?  ? PropertyDrawer field
?      ?   ?? On Dialog End             ?  ? PropertyDrawer field
?      ?? Flow Control                   ?  ? PropertyDrawer header
?      ?   ?? Auto Advance Delay        ?  ? PropertyDrawer field
?      ?   ?? Next Node [Create][Delete] ?  ? PropertyDrawer field + buttons
?      ?   ?? ? Choices                 ?  ? PropertyDrawer field
?      ?       ?? ? Choice: "Text" ? [node] ? ? DialogChoicePropertyDrawer
?      ?           ?? Choice Display    ?  ? ChoicePropertyDrawer header
?      ?           ?? Target Config     ?  ? ChoicePropertyDrawer header  
?      ?           ?? Actions           ?  ? ChoicePropertyDrawer header
???????????????????????????????????????????
? ? Quick Actions                        ?  ? CustomEditor
? ? Quick Tree Builder                   ?  ? CustomEditor
? ? Advanced Tools                       ?  ? CustomEditor
???????????????????????????????????????????
```

## ?? **Best Practices for Unity Custom Editors**

### **? DO:**
- **Data Classes**: Keep them clean, no `[Header]` attributes
- **PropertyDrawers**: Handle all field-level organization
- **CustomEditors**: Handle high-level layout and management tools
- **Tooltips**: Use `[Tooltip]` for field descriptions (doesn't conflict)
- **TextArea**: Use `[TextArea]` for multi-line text (doesn't conflict)

### **? DON'T:**
- **Never use `[Header]` in classes with PropertyDrawers**
- **Don't mix default inspector with custom organization**
- **Avoid duplicate visual elements**

### **?? Proper Architecture:**

```csharp
// ? GOOD: Data class - clean, no headers
[System.Serializable]
public class DialogNode
{
    // No [Header] attributes!
    [Tooltip("Helpful tooltip here")]
    public string nodeName;
    
    public string speakerName;
    
    [TextArea(3, 6)] // This is fine
    public string dialogText;
    
    // PropertyDrawer will organize these visually
}

// ? GOOD: PropertyDrawer - handles ALL organization
[CustomPropertyDrawer(typeof(DialogNode))]
public class DialogNodePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // PropertyDrawer creates headers and organization
        EditorGUI.LabelField(rect, "Dialog Content", EditorStyles.boldLabel);
        EditorGUI.LabelField(rect, "Flow Control", EditorStyles.boldLabel);
        // etc.
    }
}
```

## ?? **Fixed Files Summary:**

### **DialogNode.cs Changes:**
- ? Removed: `[Header("Node Identification")]`
- ? Removed: `[Header("Dialog Content")]`  
- ? Removed: `[Header("Flow Control")]`
- ? Removed: `[Header("Tree Structure")]`
- ? Removed: `[Header("Events")]`
- ? Kept: `[Tooltip]` and `[TextArea]` (these don't conflict)
- ? Added: Comments explaining PropertyDrawer handles organization

### **DialogChoice.cs Changes:**
- ? Removed: `[Header("Choice Display")]`
- ? Removed: `[Header("Target")]`
- ? Removed: `[Header("Actions")]`
- ? Added: Comments explaining PropertyDrawer handles organization

### **DialogTree.cs Changes:**
- ? Removed: `[Header("Tree Information")]`
- ? Removed: `[Header("Dialog Flow")]`  
- ? Removed: `[Header("Tree Management")]`
- ? Added: Comment explaining CustomEditor handles organization

## ?? **How to Identify Header Conflicts:**

### **Symptoms:**
- Overlapping text in inspector
- Headers appearing twice
- Misaligned fields
- Broken foldouts
- PropertyDrawer content appearing in wrong places

### **Quick Fix:**
1. **Remove ALL `[Header]` from data classes**
2. **Let PropertyDrawers handle organization**
3. **Use CustomEditor for high-level structure**

### **Testing:**
1. Select your DialogTree asset
2. Expand the Starting Node
3. Verify clean, organized layout with no overlaps
4. All headers should come from PropertyDrawers, not data classes

## ?? **Result: Professional, Clean Inspector**

### **Before (with conflicts):**
```
[Header from data class]
Dialog Content [Header from PropertyDrawer]  ? OVERLAP!
[Header from data class]
Speaker Name
Flow Control [Header from PropertyDrawer]     ? MESS!
Dialog Text
```

### **After (clean):**
```
Dialog Content                               ? PropertyDrawer only
?? Node Name
?? Speaker Name  
?? Dialog Text
Flow Control                                 ? PropertyDrawer only
?? Auto Advance Delay
?? Choices
```

## ?? **Key Lesson:**

**"One source of truth for visual organization"** - Either the data class OR the PropertyDrawer should handle headers, never both!

**Your discovery helped create a much more professional and maintainable dialog editing system! ???****The dialog tree editing experience is now smooth, professional, and fully functional! ??**