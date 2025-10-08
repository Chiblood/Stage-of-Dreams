# 2D Player Interaction System Guide

## Overview
The PlayerInteraction system has been refactored to work specifically with 2D physics and integrate seamlessly with the unified dialog system. It now handles both NPC conversations (via DialogueTrigger) and general object interactions.

## Key Changes Made

### ? **2D Physics Integration**
- **Physics2D.OverlapCircleAll** - Replaced 3D `Physics.OverlapSphere`
- **Vector2.Distance** - Replaced 3D `Vector3.Distance`
- **Collider2D** - Now uses 2D colliders instead of 3D
- **2D Gizmos** - Enhanced visualization for 2D interaction ranges

### ? **Dialog System Integration**
- **DialogueTrigger Priority** - Automatically detects and delegates to DialogueTrigger
- **NPC Name Display** - Shows proper NPC names in interaction prompts
- **Dialog State Awareness** - Respects active dialogs and choice states
- **Unified Input Handling** - Works with both Input System and legacy input

### ? **Enhanced Features**
- **Better UI Prompts** - Context-aware interaction text
- **Range Visualization** - Improved Gizmos for 2D development
- **Public API** - Extended interface for external systems

## Setup Instructions

### 1. **Player GameObject Setup**
```
PlayerGameObject
??? PlayerScript (movement controller)
??? PlayerInteraction (this system)
??? PlayerInput (Unity Input System)
??? Rigidbody2D (for 2D movement)
??? Collider2D (for interaction detection)
```

### 2. **Interactable Objects Setup**

#### For Regular Objects:
```
InteractableObject
??? SimpleInteractable (or custom Interactable)
??? Collider2D (on interactableLayer)
??? SpriteRenderer (visual)
```

#### For NPCs/Dialog:
```
NPCObject
??? NPCContent (dialog data)
??? DialogueTrigger (dialog triggering)
??? Collider2D (on interactableLayer) 
??? SpriteRenderer (visual)
```

### 3. **Layer Configuration**
1. Create an "Interactable" layer in your project
2. Assign this layer to all interactable objects
3. Set the `interactableLayer` in PlayerInteraction to this layer

## Usage Examples

### Creating a Simple Interactable
```csharp
public class ChestInteractable : Interactable
{
    [SerializeField] private bool isOpen = false;
    [SerializeField] private Sprite openSprite;
    [SerializeField] private Sprite closedSprite;
    [SerializeField] private string[] lootItems;
    
    protected override void OnInteract()
    {
        if (!isOpen)
        {
            OpenChest();
        }
        else
        {
            Debug.Log("Chest is already open!");
        }
    }
    
    private void OpenChest()
    {
        isOpen = true;
        GetComponent<SpriteRenderer>().sprite = openSprite;
        
        foreach (string item in lootItems)
        {
            Debug.Log($"Found: {item}");
            // Add to inventory here
        }
    }
}
```

### Creating a Performance Prop
```csharp
public class StageTheatreProp : Interactable
{
    [SerializeField] private string propName = "Stage Light";
    [SerializeField] private bool canUseInPerformance = true;
    
    protected override void OnInteract()
    {
        if (canUseInPerformance)
        {
            Debug.Log($"Used {propName} in performance!");
            // Trigger performance enhancement
            // Could integrate with your turn-based system
        }
        else
        {
            Debug.Log($"{propName} is not available right now.");
        }
    }
    
    public override string InteractionText => $"Use {propName}";
    
    public void SetUsableInPerformance(bool canUse)
    {
        canUseInPerformance = canUse;
        SetInteractionEnabled(canUse);
    }
}
```

### Setting Up NPCs
```csharp
// No additional code needed! Just use the existing system:
// 1. Add NPCContent component with DialogTree
// 2. Add DialogueTrigger component  
// 3. Add Collider2D on interactable layer
// PlayerInteraction will automatically detect and handle it
```

## Integration with Your Dialog System

### Automatic NPC Detection
```csharp
// PlayerInteraction automatically detects NPCs:
var dialogTrigger = currentInteractable.GetComponent<DialogueTrigger>();
if (dialogTrigger != null)
{
    // Handle as NPC dialog
    dialogTrigger.ManualTrigger();
}
else
{
    // Handle as regular interactable
    currentInteractable.Interact();
}
```

### Smart UI Prompts
```csharp
// For NPCs: "Press E to talk to Stage Director"
// For Objects: "Press E to interact with Chest"
// Context is determined automatically
```

## Configuration Options

### In Inspector
- **Interaction Range**: How close player must be (2D distance)
- **Interactable Layer**: Which layer to check for interactables
- **Interaction Key**: Fallback key when Input System unavailable
- **UI Elements**: Interaction prompt and text display

### Runtime API
```csharp
playerInteraction.SetInteractionRange(5f);
playerInteraction.SetInteractionEnabled(false);
bool inRange = playerInteraction.IsInRangeOf(targetTransform);
playerInteraction.ForceInteract(); // Manually trigger interaction
```

## Best Practices

### 1. **Layer Organization**
- Use a dedicated "Interactable" layer
- Keep interaction objects on this layer only
- Set up proper layer collision matrix

### 2. **Performance Optimization**
- Use appropriate interaction ranges (not too large)
- Consider pooling for frequently created interactables
- Use the layer mask to limit physics queries

### 3. **User Experience**
- Keep interaction ranges consistent across similar objects
- Provide clear visual feedback for interactable objects
- Test interaction ranges with actual player movement

### 4. **For Your Theater Game**
```csharp
// Example: Stage props that become available during performance
public class PerformanceProp : Interactable
{
    [SerializeField] private bool availableDuringPerformance = false;
    
    public override bool CanInteract => 
        base.CanInteract && 
        (availableDuringPerformance || !InPerformanceMode());
    
    private bool InPerformanceMode()
    {
        // Check if player is currently in performance/spotlight mode
        return PlayerScript.Instance?.inSpotlight ?? false;
    }
}
```

## Debugging Tools

### Scene View Visualization
- **Yellow Circle**: Interaction range
- **Green Line**: Connection to current interactable
- **Flat Circle**: 2D-specific range visualization

### Console Logging
```csharp
// The system provides detailed logging:
// "Player entered range of ChestInteractable"
// "Player left range of ChestInteractable" 
// "DialogueTrigger detected - delegating to dialog system"
```

### Runtime Inspection
```csharp
// Check current state in code:
if (playerInteraction.HasCurrentInteractable())
{
    var current = playerInteraction.GetCurrentInteractable();
    Debug.Log($"Current: {current.name} - Can interact: {current.CanInteract}");
}
```

## Compatibility Notes

### ? **Works With**
- Unity Input System (preferred)
- Legacy Input System (fallback)
- Your existing DialogManager singleton
- DialogueTrigger unified system
- NPCContent and DialogTree assets

### ?? **Migration Notes**
- Old 3D interaction scripts need layer mask updates
- Ensure all interactables have Collider2D (not Collider)
- Update any custom interaction ranges for 2D distances

### ?? **Theater Game Integration**
- Perfect for stage prop interactions
- Integrates with spotlight performance system
- Supports context-sensitive interactions
- Ready for turn-based performance mechanics

The refactored system provides a solid foundation for 2D interactions while maintaining clean integration with your unified dialog system!