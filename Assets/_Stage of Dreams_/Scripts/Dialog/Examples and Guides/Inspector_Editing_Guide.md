# Dialog System Inspector Editing Guide

## ? SerializeReference Solution

The dialog system now uses `[SerializeReference]` instead of `[SerializeField]` to solve the serialization depth limit issue while still allowing Inspector editing.

## How to Edit Dialog Trees in Unity Inspector

### 1. **Creating Dialog Trees**

1. **Right-click in Project window** ? Create ? Dialog System ? Dialog Tree
2. **Name your tree** (e.g., "NPCGreeting", "ShopDialog")
3. **Select the DialogTree asset** to see it in the Inspector

### 2. **Using the Custom Inspector**

The DialogTree now has an enhanced custom Inspector with:

- **Tree Information**: Basic metadata fields
- **Dialog Flow**: Starting node and auto-updated node list  
- **SerializeReference Info**: Confirms no depth limit issues

### 3. **Editing Dialog Nodes**

**Starting Node Field**:
- Expand the "Starting Node" field in the Inspector
- Edit speaker name, dialog text, player speaking flag
- Add choices by expanding the "Choices" array
- Set next node for auto-advance conversations

**Custom Property Drawers**:
- DialogNode fields now have custom property drawers
- DialogChoice fields also have custom property drawers
- Both support full editing in the Inspector with foldouts

### 4. **Working with Choices**

To add choices to a node:
1. Expand the node in Inspector
2. Expand the "Choices" array
3. Increase the Size field
4. Configure each choice:
   - **Choice Text**: What the player sees
   - **Custom Action ID**: For triggering game events
   - **Target Node**: Where this choice leads
   - **On Choice Selected**: UnityEvent for this choice

### 5. **Node Navigation**

**Auto-advance nodes**:
- Set "Next Node" field for linear progression
- Set "Auto Advance Delay" > 0 for timed advance

**Choice-based nodes**:
- Leave "Next Node" empty
- Add choices that lead to other nodes
- Each choice can have its own target node

### 6. **Best Practices**

1. **Start Simple**: Create a starting node first
2. **Build Incrementally**: Add one branch at a time
3. **Test Frequently**: Use the validation tools
4. **Use Descriptive Names**: Clear speaker names and choice text
5. **Plan Structure**: Sketch your dialog flow before building

### 7. **Validation Tools**

Right-click on DialogTree asset for context menu options:
- **Validate Tree**: Check for issues
- **Print Tree Structure**: Debug output to console
- **Refresh Node List**: Update the auto-generated node list

### 8. **No More Serialization Errors** ?

The `[SerializeReference]` approach means:
- ? No "Serialization depth limit exceeded" errors
- ? Full Inspector editing support
- ? Circular reference support
- ? Complex branching dialog trees work perfectly
- ? All UnityEvents still work
- ? Undo/Redo support maintained

### 9. **Key Differences from Before**

**Before (SerializeField)**:
- ? Serialization depth limit errors
- ? Couldn't create complex trees
- ? Inspector would break with deep references

**Now (SerializeReference)**:
- ? No depth limits
- ? Complex trees work perfectly  
- ? Full Inspector editing support
- ? Better performance
- ? Proper Unity serialization

## Example: Creating a Simple Dialog

1. Create new DialogTree asset
2. In Inspector, expand "Starting Node"
3. Set Speaker Name: "Shopkeeper"
4. Set Dialog Text: "Welcome to my shop! What can I help you with?"
5. Expand "Choices" array and set Size to 2
6. First choice: "I'd like to buy something"
7. Second choice: "Just browsing, thanks"
8. Create target nodes for each choice
9. Build your conversation tree!

The system now provides the best of both worlds - no serialization errors AND full Inspector editing capabilities!