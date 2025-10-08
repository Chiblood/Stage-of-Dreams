# Enhanced Dialog Tree Creation Guide

## Overview
Your dialog tree system has been enhanced with automatic node creation, editor helpers, and tree management tools. You can now easily create complex dialog trees both in the Unity editor and programmatically.

## Key Features Added

### ? **Enhanced DialogNode Class**
- **AddChoice()** - Automatically add choices to nodes
- **AddChoiceToNode()** - Add choice that links to specific node
- **CreateNextNode()** - Create and link sequential nodes
- **SetNextNode()** - Link nodes for auto-advance
- **RemoveChoice()** - Remove choices by index

### ? **Enhanced DialogTree Class**
- **CreateStartingNode()** - Initialize the tree
- **AddChoiceNode()** - Add branching dialog options
- **AddSequentialNode()** - Add linear dialog progression
- **CreateLinearConversation()** - Build entire linear conversations
- **RefreshNodeList()** - Automatically track all nodes
- **ValidateTree()** - Check for issues and get statistics
- **PrintTreeStructure()** - Debug tree structure

### ? **Custom Editor (DialogTreeEditor)**
- **Quick Add Section** - Easy node creation in inspector
- **Advanced Tools** - Template builders and utilities
- **Validation Tools** - Tree debugging and statistics
- **Example Builders** - One-click complex dialog creation

## How to Create Dialog Trees

### Method 1: Using the Unity Editor (Recommended)

1. **Create a DialogTree Asset**:
   ```
   Right-click in Project ? Create ? Dialog System ? Dialog Tree
   ```

2. **Use the Enhanced Inspector**:
   - **Quick Add Nodes** section for basic creation
   - **Advanced Tools** for complex structures
   - **Validation & Debug** for testing

3. **Quick Creation Steps**:
   ```
   1. Enter Speaker Name and Dialog Text
   2. Click "Create Starting Node"
   3. Click "Add Sequential Node" or "Add Choice Node"
   4. Repeat as needed
   ```

### Method 2: Programmatic Creation

```csharp
// Create a DialogTree asset first, then:
DialogTree tree = yourDialogTreeAsset;

// Create starting node
var start = tree.CreateStartingNode("Director", "Welcome to the stage!");

// Add a choice branch
var choice1 = tree.AddChoiceNode(start, "I'm ready!", "Director", 
    "Excellent! Let's begin the performance!", false, "start_performance");

// Add a sequential node (auto-advance)
var next = tree.AddSequentialNode(choice1, "Player", "Thank you!", true, 2f);
```

### Method 3: Using Templates

```csharp
// Linear conversation
string[] speakers = {"Director", "Player", "Director"};
string[] texts = {"Hello!", "Hi there!", "Ready to start?"};
bool[] playerSpeaking = {false, true, false};

tree.CreateLinearConversation(speakers, texts, playerSpeaking);
```

## Example Dialog Trees

### 1. Simple Performance Introduction
```
Director: "Ready for your performance?"
??? "I'm ready!" ? Director: "Break a leg!" ? [start_performance]
??? "I'm nervous" ? Director: "That's normal!" ? Player: "Thanks!"
??? "What's my role?" ? Director: "You're the hero!" ? Player: "Got it!"
```

### 2. Complex Audition Tree
```
Casting Director: "What role do you want?"
??? "Lead role" ? "What type?"
?   ??? "Hero" ? [Performance Test] ? [grant_hero_role]
?   ??? "Villain" ? [Performance Test] ? [grant_villain_role]
?   ??? "Comedy" ? [Performance Test] ? [grant_comedy_role]
??? "Supporting" ? "Great choice!" ? [grant_support_role]
??? "Ensemble" ? "Team player!" ? [grant_ensemble_role]
```

## Editor Features

### Quick Add Section
- **Create Starting Node** - Initialize your tree
- **Add Sequential Node** - Continue linear conversation
- **Add Choice Node** - Create branching options
- **Custom Action ID** - Add game functionality

### Advanced Tools
- **Create Example Linear Conversation** - Pre-built template
- **Create Branching Example** - Complex choice template
- **Clear All Nodes** - Start over (with confirmation)

### Validation Tools
- **Validate Tree** - Check structure and get statistics
- **Print Structure** - Console output of tree hierarchy
- **Refresh Nodes** - Update internal node tracking
- **Tree Statistics** - Node count, choices, end nodes

## Custom Actions Integration

### Adding Custom Actions
```csharp
// When creating choice nodes
tree.AddChoiceNode(parent, "Start performance", "Director", 
    "Break a leg!", false, "start_performance");

// The custom action ID "start_performance" will be handled by your NPC:
public override void HandleCustomAction(string actionId)
{
    switch (actionId)
    {
        case "start_performance":
            // Start your turn-based performance system
            break;
    }
}
```

### Theater Game Examples
```csharp
// Performance-related actions
"start_performance"     // Begin turn-based mode
"give_encouragement"    // Boost confidence
"explain_scene"        // Show tutorial
"grant_hero_role"      // Assign role to player
"unlock_next_scene"    // Progress story
```

## Best Practices

### 1. **Planning Your Tree**
```
Start Simple:
Opening ? 2-3 Choices ? Responses ? End

Then Expand:
Add sub-branches, custom actions, character development
```

### 2. **Naming Conventions**
```
Speakers: "Director", "Player", "Stage Manager"
Actions: "start_", "give_", "unlock_", "explain_"
Choices: Keep short and clear
```

### 3. **Testing Your Trees**
```csharp
// Always validate after creation
tree.ValidateTree();

// Print structure to console for debugging
tree.PrintTreeStructure();

// Check statistics
var nodes = tree.GetAllNodes();
Debug.Log($"Tree has {nodes.Count} nodes");
```

### 4. **Performance Optimization**
- Use `RefreshNodeList()` sparingly (automatic in editor)
- Keep individual dialog texts concise
- Limit choice branches to 3-4 options per node

## Common Patterns

### Linear Storytelling
```csharp
tree.CreateLinearConversation(speakers, texts, playerFlags);
```

### Branching Choices
```csharp
var question = tree.CreateStartingNode("NPC", "What do you choose?");
tree.AddChoiceNode(question, "Option A", "NPC", "Response A");
tree.AddChoiceNode(question, "Option B", "NPC", "Response B");
```

### Looping Conversations
```csharp
var menu = tree.CreateStartingNode("Vendor", "What can I help you with?");
var buy = tree.AddChoiceNode(menu, "Buy items", "Vendor", "Here's my shop!");
tree.AddChoiceNode(buy, "Back to menu", "Vendor", "Anything else?", false)
    .SetTarget(menu); // Loop back
```

### Action-Driven Dialogs
```csharp
tree.AddChoiceNode(node, "Start battle", "Enemy", 
    "Prepare yourself!", false, "start_combat");
```

## Integration with Your Game

### With NPCContent
```csharp
public class StageDirectorNPC : NPCContent
{
    private void Start()
    {
        // Your dialog tree asset is assigned to mainDialogTree
        // The enhanced tree builder makes it easy to create
    }
    
    public override void HandleCustomAction(string actionId)
    {
        switch (actionId)
        {
            case "start_performance":
                PerformanceManager.StartPerformance();
                break;
            case "give_role":
                PlayerManager.AssignRole(selectedRole);
                break;
        }
    }
}
```

### With DialogueTrigger
Your enhanced trees work seamlessly with the existing DialogueTrigger system - no changes needed!

## Troubleshooting

### Common Issues
1. **"No starting node"** - Create one using "Create Starting Node"
2. **"Tree loops infinitely"** - Add end nodes (nodes with no choices/next)
3. **"Custom actions not working"** - Check action ID spelling in NPC code
4. **"Can't see all nodes"** - Click "Refresh Nodes" in validation section

### Debug Commands
```csharp
tree.ValidateTree();        // Check for issues
tree.PrintTreeStructure();  // See full structure
tree.RefreshNodeList();     // Update node tracking
```

The enhanced dialog tree system makes it easy to create complex, branching conversations perfect for your theater game while maintaining compatibility with your existing dialog system!