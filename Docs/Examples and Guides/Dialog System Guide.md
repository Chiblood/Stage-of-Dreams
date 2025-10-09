# Integrated Dialog System Guide

## Overview
The dialog system now uses a unified, integrated approach with clear separation of responsibilities:
- **DialogManager** - Singleton UI manager for dialog display
- **DialogNavigator** - Pure navigation logic
- **DialogueTrigger** - Unified trigger system for all dialog activation
- **NPCContent** - Dialog content and custom actions
- **DialogNode/DialogTree** - Data structures for dialog flow

## Architecture: DialogNode ? DialogTree ? NPCContent ? DialogueTrigger ? DialogManager (Singleton)

## Key Components

### 1. DialogManager (Singleton)
**Purpose**: Global UI manager for dialog display
- **Access**: `DialogManager.Instance` for easy global access
- **Responsibilities**: Display text, handle buttons, manage UI state
- **Singleton Benefits**: One dialog UI, cross-scene persistence, easy access

### 2. DialogueTrigger (Unified Trigger System)
**Purpose**: Single trigger system replacing multiple separate triggers
- **Trigger Types**: 
  - **Spotlight**: Triggers when player enters spotlight (requires interaction)
  - **Interaction**: Triggers when player is nearby and presses interact key
  - **Proximity**: Triggers automatically when player gets close
- **Configurable**: Can combine multiple trigger types
- **Reusable**: Can retrigger with configurable delays

### 3. Spotlight (Focused Responsibility)
**Purpose**: Only handles spotlight detection and player state
- **No Dialog Logic**: Delegates dialog triggering to DialogueTrigger
- **Clean Separation**: Focuses on lighting/effect logic only
- **Integration**: Works seamlessly with DialogueTrigger

### 4. NPCContent & DialogTree
**Purpose**: Content definition and custom behaviors
- **Dialog Trees**: ScriptableObject assets for reusable conversations
- **Custom Actions**: Handle game-specific behavior via string IDs
- **Event Hooks**: OnDialogStarted, OnDialogEnded for game integration

## Setting Up the Integrated System

### Step 1: Create the Dialog Tree
```
Right-click in Project ? Create ? Dialog System ? Dialog Tree
```

Configure nodes:
```
Node 1: "The spotlight finds you! Ready to perform?"
??? Choice: "Begin performance" ? Custom Action: "start_performance"
??? Choice: "I need practice" ? Custom Action: "request_practice"
??? Choice: "Tell me about this scene" ? Node 2

Node 2: "This scene represents overcoming stage fright..."
??? Choice: "I understand" ? End Dialog
```

### Step 2: Set Up GameObject Components
```csharp
// On your spotlight GameObject:
GameObject spotlightObj = new GameObject("PerformanceSpotlight");

// Add required components
Spotlight spotlight = spotlightObj.AddComponent<Spotlight>();
DialogueTrigger trigger = spotlightObj.AddComponent<DialogueTrigger>();
ExampleSpotlightNPC npc = spotlightObj.AddComponent<ExampleSpotlightNPC>();

// Configure spotlight
spotlight.radius = 1.5f;

// Configure trigger (via inspector or code)
trigger.triggerOnSpotlight = true;
trigger.requireInteractionInput = true;
trigger.targetNPC = npc;

// Assign dialog tree to NPC
npc.mainDialogTree = yourDialogTreeAsset;
```

### Step 3: Configure in Inspector
- **Spotlight**: Set radius and link to trigger
- **DialogueTrigger**: 
  - Enable "Trigger On Spotlight"
  - Enable "Require Interaction Input"
  - Assign target NPC
- **NPCContent**: Assign your Dialog Tree asset

## Usage Patterns

### 1. Spotlight Performance Scenes
```csharp
public class PerformanceSpotlight : MonoBehaviour
{
    [SerializeField] private Spotlight spotlight;
    [SerializeField] private DialogueTrigger dialogTrigger;
    
    private void Start()
    {
        // Configure for performance scenes
        dialogTrigger.SetTriggerType(
            spotlight: true, 
            interaction: false, 
            proximity: false
        );
    }
}
```

### 2. NPC Conversations
```csharp
public class NPCInteraction : MonoBehaviour
{
    [SerializeField] private DialogueTrigger dialogTrigger;
    
    private void Start()
    {
        // Configure for regular NPC interactions
        dialogTrigger.SetTriggerType(
            spotlight: false, 
            interaction: true, 
            proximity: false
        );
    }
}
```

### 3. Automatic Story Moments
```csharp
public class StoryTrigger : MonoBehaviour
{
    [SerializeField] private DialogueTrigger dialogTrigger;
    
    private void Start()
    {
        // Configure for automatic story triggers
        dialogTrigger.SetTriggerType(
            spotlight: false, 
            interaction: false, 
            proximity: true
        );
    }
}
```

## Custom Actions Integration

### Define Actions in Your NPC
```csharp
public class StageDirectorNPC : NPCContent
{
    public override void HandleCustomAction(string actionId)
    {
        switch (actionId.ToLower())
        {
            case "start_performance":
                // Start turn-based performance mode
                PerformanceManager.Instance?.StartPerformance();
                break;
                
            case "give_feedback":
                // Show performance feedback UI
                FeedbackUI.Instance?.ShowFeedback();
                break;
                
            case "unlock_next_scene":
                // Progress to next area
                SceneProgressManager.Instance?.UnlockScene("NextArea");
                break;
        }
    }
}
```

### Use Actions in Dialog Choices
```
Choice: "I'm ready to perform!" 
? Custom Action ID: "start_performance"
? Target Node: null (ends dialog)
```

## Event System Integration

### Subscribe to Dialog Events
```csharp
public class GameManager : MonoBehaviour
{
    private void Start()
    {
        // Global dialog events
        var triggers = FindObjectsOfType<DialogueTrigger>();
        foreach (var trigger in triggers)
        {
            trigger.OnDialogTriggered += PauseGameplay;
            trigger.OnDialogEnded += ResumeGameplay;
        }
    }
    
    private void PauseGameplay()
    {
        Time.timeScale = 0f;
        // Disable player movement, pause audio, etc.
    }
    
    private void ResumeGameplay()
    {
        Time.timeScale = 1f;
        // Re-enable player movement, resume audio, etc.
    }
}
```

## Benefits of Integrated System

### ? **Unified Triggering**
- One system handles all trigger types
- No duplicate trigger logic
- Consistent behavior across game

### ? **Clean Separation**
- Spotlight focuses on detection only
- DialogueTrigger handles activation logic
- DialogManager handles UI only
- NPCContent handles game logic

### ? **Easy Configuration**
- Inspector-driven setup
- No code required for basic dialogs
- Flexible trigger combinations

### ? **Reusable Components**
- DialogTree assets are reusable
- NPCContent classes are extensible
- DialogueTrigger works with any content

### ? **Event Integration**
- Easy to hook into dialog lifecycle
- Supports complex game state changes
- Clean integration with other systems

## Migration from Old System

### Old Spotlight System
```csharp
// OLD: Spotlight handled its own dialog
spotlight.TriggerSpotlightDialog();

// NEW: Spotlight works with DialogueTrigger
// No code needed - configured in inspector
```

### Old Interaction System
```csharp
// OLD: PlayerInteraction called DialogManager directly
DialogManager.Instance.StartDialog(npc);

// NEW: PlayerInteraction uses DialogueTrigger
dialogTrigger.ManualTrigger();
```

### Old Custom Dialog Logic
```csharp
// OLD: Custom trigger scripts everywhere
public class CustomTrigger : MonoBehaviour { ... }

// NEW: Configure DialogueTrigger via inspector or runtime
dialogTrigger.SetTriggerType(spotlight: true, interaction: false, proximity: false);
```

## Best Practices

1. **One DialogueTrigger per conversation source**
2. **Use DialogTree assets for reusable content**
3. **Handle custom actions in NPCContent classes**
4. **Subscribe to trigger events for game state management**
5. **Configure trigger types based on interaction design**
6. **Use Gizmos to visualize trigger ranges in editor**

The integrated system provides a clean, maintainable approach to dialog management while maintaining the flexibility needed for complex game interactions.