# Focused Spotlight System Guide

## Overview
The Spotlight system has been refactored to focus purely on its core responsibilities:
1. **Movement Control** - Static, Random, and Target Following patterns
2. **Character State Management** - Tracking which characters are in the spotlight
3. **Event System** - Notifications when characters enter/exit or spotlight moves

## Key Features

### ?? **Pure Spotlight Focus**
- **No Dialog Logic** - Spotlight focuses only on movement and detection
- **Character State Management** - Updates `inSpotlight` boolean on characters
- **Movement Patterns** - Three distinct movement types for different scenarios
- **Event-Driven** - Other systems subscribe to spotlight events

### ?? **Movement Patterns**

#### 1. **Static** (Default)
```csharp
spotlight.SetMovementType(SpotlightMovementType.Static);
```
- Spotlight remains in place
- Perfect for fixed performance areas

#### 2. **Random Movement**
```csharp
spotlight.StartRandomMovement(new Vector2(10f, 6f)); // Stage bounds
```
- Moves to random positions within defined bounds
- Creates dynamic, unpredictable lighting
- Great for dramatic tension or chaos scenes

#### 3. **Target Following**
```csharp
spotlight.SetTarget(playerTransform);
spotlight.SetMovementType(SpotlightMovementType.FollowTarget);
```
- Follows a specific character/object
- Smooth movement with configurable speed
- Perfect for performance sequences

### ?? **Character Tracking System**

#### Interface-Based Design
```csharp
public interface ISpotlightCharacter
{
    Vector2 GetPosition();
    string GetCharacterName();
    void SetInSpotlight(bool inSpotlight);
    bool IsInSpotlight();
}
```

#### Automatic Detection
- Tracks all characters or specific ones
- Updates `inSpotlight` boolean automatically
- Fires events when characters enter/exit

#### Extensible for Multiple Character Types
```csharp
// Currently supports PlayerScript via PlayerCharacterWrapper
// Easy to add NPCs, enemies, or other character types
```

## Setup Instructions

### 1. **Basic Spotlight Setup**
```csharp
// On your spotlight GameObject:
Spotlight spotlight = gameObject.AddComponent<Spotlight>();

// Configure basic settings
spotlight.radius = 2f;
spotlight.moveSpeed = 3f;
```

### 2. **Character Layer Configuration**
```csharp
// Set up layer mask for character detection
spotlight.characterLayer = LayerMask.GetMask("Player", "NPC");
```

### 3. **Movement Configuration**
```csharp
// For performances - follow specific actors
spotlight.SetTarget(actorTransform);

// For dramatic scenes - random movement
spotlight.StartRandomMovement(new Vector2(stageWidth, stageHeight));

// For fixed scenes - stay static
spotlight.StopMovement();
```

## Usage Examples

### Performance Director
```csharp
public class PerformanceDirector : MonoBehaviour
{
    [SerializeField] private Spotlight mainSpotlight;
    [SerializeField] private Transform[] performers;
    
    public void StartPerformance()
    {
        // Follow each performer in sequence
        StartCoroutine(PerformanceSequence());
    }
    
    private IEnumerator PerformanceSequence()
    {
        foreach (var performer in performers)
        {
            mainSpotlight.SetTarget(performer);
            yield return new WaitForSeconds(10f); // Each performer gets 10 seconds
        }
    }
}
```

### Dynamic Stage Lighting
```csharp
public class StageLighting : MonoBehaviour
{
    [SerializeField] private Spotlight[] spotlights;
    
    public void CreateChaoticScene()
    {
        foreach (var spotlight in spotlights)
        {
            spotlight.StartRandomMovement(new Vector2(15f, 10f));
        }
    }
    
    public void FocusOnPlayer()
    {
        var player = FindFirstObjectByType<PlayerScript>();
        foreach (var spotlight in spotlights)
        {
            spotlight.SetTarget(player.transform);
        }
    }
}
```

### Character-Triggered Events
```csharp
public class SpotlightEventHandler : MonoBehaviour
{
    private void Start()
    {
        var spotlight = GetComponent<Spotlight>();
        spotlight.OnCharacterEnteredSpotlight += HandleCharacterEntered;
        spotlight.OnCharacterExitedSpotlight += HandleCharacterExited;
    }
    
    private void HandleCharacterEntered(ISpotlightCharacter character)
    {
        Debug.Log($"{character.GetCharacterName()} is now in the spotlight!");
        
        // Trigger special effects
        // Play dramatic music
        // Show UI elements
        // Start dialog if it's an NPC area
    }
}
```

## Integration with Dialog System

### Event-Based Integration
```csharp
public class SpotlightDialogBridge : MonoBehaviour
{
    [SerializeField] private DialogueTrigger associatedTrigger;
    
    private void Start()
    {
        var spotlight = GetComponent<Spotlight>();
        spotlight.OnCharacterEnteredSpotlight += CheckForDialog;
    }
    
    private void CheckForDialog(ISpotlightCharacter character)
    {
        // Only trigger for player characters
        if (character is PlayerCharacterWrapper && associatedTrigger != null)
        {
            // Let DialogueTrigger handle the actual dialog logic
            associatedTrigger.ManualTrigger();
        }
    }
}
```

### Performance Integration
```csharp
public class PerformanceSpotlight : MonoBehaviour
{
    [SerializeField] private Spotlight spotlight;
    
    public void StartPerformanceMode()
    {
        // Follow player during their performance
        var player = FindFirstObjectByType<PlayerScript>();
        spotlight.SetTarget(player.transform);
        
        // Subscribe to spotlight events for performance feedback
        spotlight.OnCharacterEnteredSpotlight += OnPerformanceStart;
        spotlight.OnCharacterExitedSpotlight += OnPerformanceEnd;
    }
    
    private void OnPerformanceStart(ISpotlightCharacter character)
    {
        // Start turn-based performance system
        // PerformanceManager.Instance?.StartPerformance();
    }
    
    private void OnPerformanceEnd(ISpotlightCharacter character)
    {
        // End performance, show results
        // PerformanceManager.Instance?.EndPerformance();
    }
}
```

## API Reference

### Core Methods
```csharp
// Movement Control
spotlight.SetMovementType(SpotlightMovementType.Static);
spotlight.SetTarget(Transform target);
spotlight.StartRandomMovement(Vector2 bounds);
spotlight.StopMovement();
spotlight.MoveToPosition(Vector2 position);

// Character Detection
ISpotlightCharacter[] characters = spotlight.GetCharactersInSpotlight();
bool hasCharacters = spotlight.HasCharacterInSpotlight();
bool isCharacterInSpotlight = spotlight.IsCharacterInSpotlight(character);

// Utility
spotlight.RefreshTrackedCharacters();
spotlight.ForceUpdateCharacterDetection();
```

### Events
```csharp
spotlight.OnCharacterEnteredSpotlight += HandleEntered;
spotlight.OnCharacterExitedSpotlight += HandleExited;
spotlight.OnSpotlightMoved += HandleMoved;
```

### Properties
```csharp
spotlight.radius = 2.5f;
spotlight.moveSpeed = 4f;
spotlight.movementBounds = new Vector2(12f, 8f);
spotlight.randomMoveInterval = 3f;
```

## Theater Game Examples

### Scene Transitions
```csharp
public void TransitionToNewAct()
{
    // Spotlight follows key character to new area
    spotlight.SetTarget(leadActor.transform);
    
    // Once they reach position, start random dramatic movement
    StartCoroutine(DelayedRandomMovement(5f));
}
```

### Audience Interaction
```csharp
private void HandleCharacterInSpotlight(ISpotlightCharacter character)
{
    // Increase audience attention
    // AudienceManager.Instance?.FocusOn(character);
    
    // Apply spotlight pressure effects
    // if (character is PlayerCharacterWrapper player)
    // {
    //     player.AddSpotlightPressure(0.1f);
    // }
}
```

### Multi-Spotlight Coordination
```csharp
public class MultiSpotlightDirector : MonoBehaviour
{
    [SerializeField] private Spotlight[] spotlights;
    
    public void CreateSpotlightChase()
    {
        // Each spotlight follows the player in sequence
        for (int i = 0; i < spotlights.Length; i++)
        {
            StartCoroutine(DelayedFollow(spotlights[i], i * 2f));
        }
    }
}
```

## Benefits of Focused Design

### ? **Clear Responsibilities**
- Spotlight only handles movement and detection
- No dialog or UI logic mixed in
- Easy to understand and maintain

### ? **Event-Driven Architecture**
- Other systems subscribe to spotlight events
- Loose coupling between systems
- Easy to add new behaviors

### ? **Flexible Movement**
- Three distinct movement patterns
- Easy to switch between them at runtime
- Configurable parameters for each type

### ? **Performance Optimized**
- Efficient character tracking
- Layer-based detection
- Minimal update overhead

### ? **Theater Game Ready**
- Perfect for performance sequences
- Supports dramatic lighting effects
- Easy integration with turn-based systems

The focused Spotlight system provides a solid foundation for theatrical lighting effects while maintaining clean separation from dialog and other game systems!