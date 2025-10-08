# Simple Dialog System Usage Guide

## Overview
This dialog system uses a clean, node-based approach with separated concerns:
- **DialogNavigator** - Handles tree navigation logic
- **DialogManager** - Handles UI display and input
- **DialogNode/DialogTree/NPCContent** - Data structures

## Architecture: DialogNode ? DialogTree ? NPCContent ? DialogueTrigger ? DialogNavigator ? DialogManager

## Components

### 1. DialogNavigator (New!)
**Purpose**: Pure navigation logic for dialog trees
**Responsibilities**:
- Track current position in dialog tree
- Handle choice selection and navigation
- Manage dialog state and flow
- Provide events for UI updates
- Execute custom actions via NPCs

### 2. DialogManager 
**Purpose**: UI display and user input handling
**Responsibilities**:
- Display dialog text and choices
- Handle button clicks and input
- Show/hide UI elements
- Subscribe to navigation events

### 3. Data Classes
- **DialogNode**: Individual dialog pieces with text and choices
- **DialogChoice**: Player choice options with target nodes
- **DialogTree**: Collection of linked nodes with starting point
- **NPCContent**: Container for NPC dialog trees and custom actions

## How to Set Up a Dialog

### 1. Create Dialog Nodes (in Inspector)
Dialog nodes are data containers:
- **Speaker Name**: Who is talking
- **Dialog Text**: What they say  
- **Is Player Speaking**: Check if player is speaking
- **Auto Advance Delay**: Time to auto-advance (0 = wait for input)
- **Choices**: Array of choices player can make

### 2. Create Dialog Choices (in Inspector)
For each choice:
- **Choice Text**: What the button shows
- **Target Node**: Where this choice leads (can be null to end dialog)
- **Custom Action ID**: String ID for custom behavior (optional)
- **On Choice Selected**: Unity Event (optional)

### 3. Create Dialog Tree (ScriptableObject)
- Right-click in Project ? Create ? Dialog System ? Dialog Tree
- Set the **Starting Node** to your first dialog node
- Give it a name and description

### 4. Create Your NPC Script
```csharp
public class MyNPC : NPCContent
{
    public override void HandleCustomAction(string actionId)
    {
        switch (actionId)
        {
            case "give_item":
                // Give player an item
                break;
            case "start_quest":
                // Start a quest
                break;
        }
    }
}
```

### 5. Set Up GameObject
- Create GameObject for your NPC
- Add your NPC script component
- Assign the Dialog Tree to the Main Dialog Tree field
- Add DialogueTrigger component
- Configure trigger settings (spotlight, interaction, range)

### 6. Set Up Dialog Manager
- Create GameObject with DialogManager component
- Assign UI Document and Visual Tree Asset
- Make sure your UXML has these elements:
  - GroupBox named "DialogBox"
  - Label named "GivenDialog"  
  - Buttons named "DialogOption1", "DialogOption2", "DialogOption3"

## Using the Navigation System

### Basic Usage
```csharp
// Get references
DialogManager dialogManager = FindObjectOfType<DialogManager>();
NPCContent npc = GetComponent<NPCContent>();

// Start dialog
dialogManager.StartDialog(npc);

// Start specific tree
dialogManager.StartDialog(npc, "SpecialConversation");

// Check if dialog is active
if (dialogManager.IsDialogActive())
{
    // Dialog is running
}

// Get current state
var state = dialogManager.GetNavigationState();
if (state.isActive)
{
    Debug.Log($"Talking to {state.currentNPC.npcName}");
    Debug.Log($"Current text: {state.currentNode.dialogText}");
}
```

### Advanced Navigation
```csharp
// Access navigator directly for advanced control
DialogNavigator navigator = new DialogNavigator();

// Subscribe to navigation events
navigator.OnNodeChanged += (node) => Debug.Log($"Now showing: {node.dialogText}");
navigator.OnCustomActionTriggered += (choice, npc) => Debug.Log($"Action: {choice.customActionId}");
navigator.OnDialogEnded += () => Debug.Log("Dialog finished!");

// Force navigation to specific nodes (for cutscenes)
navigator.ForceNavigateToNode(specificNode);

// Switch to another dialog tree
navigator.SwitchToTree("AnotherTree");
```

## Example Simple Dialog Structure

```
DialogNode: "Hello there!"
??? Choice: "Hi!" ? DialogNode: "Nice to meet you!" ? End
??? Choice: "Who are you?" ? DialogNode: "I'm the stage manager." ? End  
??? Choice: "Goodbye" ? End
```

## Key Benefits of Separated Architecture

1. **Testability**: Can test navigation logic independently of UI
2. **Reusability**: Navigation logic can work with different UI systems
3. **Maintainability**: Changes to navigation don't affect UI and vice versa
4. **Debugging**: Clear separation makes it easier to trace issues
5. **Extensibility**: Easy to add features like dialog history, save/load states

## Events and Custom Actions

### Navigation Events
```csharp
// Subscribe to navigation events for custom behavior
navigator.OnNodeChanged += (node) => {
    // Pause game, change music, update character animations
};

navigator.OnDialogEnded += () => {
    // Resume game, restore player control
};

navigator.OnCustomActionTriggered += (choice, npc) => {
    // Handle special game actions
};
```

### Custom Actions
Use custom action IDs in choices to trigger game behavior:
- "give_item" - Give player items
- "start_quest" - Begin quests  
- "change_scene" - Load new areas
- "play_cutscene" - Trigger cutscenes
- "unlock_ability" - Grant player abilities

Handle these in your NPC's `HandleCustomAction` method.

## Migration from Old System

If you have existing DialogManager code, the public interface remains mostly the same:
- `StartDialog(NPCContent)` still works
- `IsDialogActive()` still works  
- `AdvanceDialog()` still works
- `EndDialog()` still works

The navigation logic is now handled internally by DialogNavigator, but the external API is preserved for compatibility.