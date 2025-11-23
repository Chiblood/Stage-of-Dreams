# DialogEvents Scaffolding Guide

**Last Updated**: January 2025  
**Status**: ✅ SCAFFOLDING COMPLETE  
**Branch**: Feature/DialogNode-Functionality

---

## Overview

DialogEvents provide a type-safe, extensible alternative to UnityEvents for triggering game systems from dialog choices and nodes. This system is designed for future game mechanics like crowd reactions, applause meters, performance ratings, and more.

---

## Current Status

### ✅ Implemented
- DialogEvent base class with polymorphic support
- DialogChoice supports both DialogEvents and UnityEvents
- PropertyDrawer shows DialogEvents section with clear future-use label
- Pre-built event types for common game systems (see below)

### ⏳ Pending (When Game Systems Ready)
- Actual game system implementations (CrowdManager, ApplauseMeter, etc.)
- Replace debug logs in event OnExecute() methods with real system calls
- Testing with live game systems

---

## Why DialogEvents Instead of UnityEvents?

| Feature | DialogEvents | UnityEvents |
|---------|--------------|-------------|
| **Type Safety** | ✅ Compile-time checking | ❌ Runtime only |
| **Polymorphism** | ✅ Inheritance, custom events | ❌ Limited |
| **Code Reuse** | ✅ Share events across nodes | ❌ Configure each time |
| **Serialization** | ✅ SerializeReference | ✅ Built-in |
| **Inspector** | ✅ Custom drawers | ✅ Built-in drawer |
| **Learning Curve** | ⚠️ Higher | ✅ Lower |

**Decision**: Support **both** systems
- **DialogEvents**: For game systems (crowd, applause, mood, etc.)
- **UnityEvents**: For simple cases and prototyping

---

## Available Event Types

### 🎭 Crowd System Events

#### `CrowdReactionEvent`
Triggers audience reactions to player choices/performance

**Parameters**:
- `Reaction Type`: Cheer, Applause, Boo, Silence, GaspInAwe, Laughter, Mixed
- `Intensity`: 0-100%
- `Duration`: Seconds

**Example Use Cases**:
- Player makes a great improvisation → Crowd applauds
- Player forgets lines → Crowd goes silent
- Player delivers emotional moment → Crowd gasps in awe

**Future Implementation**:
```csharp
protected override void OnExecute()
{
    CrowdManager.Instance.TriggerReaction(reactionType, intensity, duration);
}
```

---

#### `CrowdGoWildEvent`
Maximum crowd reaction with optional standing ovation

**Parameters**:
- `Duration`: Seconds
- `Include Standing Ovation`: bool

**Example Use Cases**:
- Player completes perfect performance
- Dramatic reveal or plot twist
- Act finale

**Future Implementation**:
```csharp
protected override void OnExecute()
{
    CrowdManager.Instance.GoWild(duration, includeStandingOvation);
}
```

---

### 👏 Applause Meter Events

#### `ApplauseMeterEvent`
Add/remove points from performance meter

**Parameters**:
- `Points To Add`: Can be negative for mistakes
- `Show Feedback`: Display message to player
- `Feedback Message`: Text shown to player

**Example Use Cases**:
- Good choice: +10 points, "Great timing!"
- Bad choice: -5 points, "The crowd seems confused..."
- Neutral choice: 0 points, no feedback

**Future Implementation**:
```csharp
protected override void OnExecute()
{
    ApplauseMeter.Instance.AddPoints(pointsToAdd, showFeedback ? feedbackMessage : null);
}
```

---

#### `SetApplauseMeterEvent`
Set meter to specific value (useful for story beats)

**Parameters**:
- `Target Value`: 0-100%
- `Animate`: Smooth transition vs instant

**Example Use Cases**:
- Reset meter at act start: 50%
- Force dramatic moment: 100%
- Story-driven low point: 20%

**Future Implementation**:
```csharp
protected override void OnExecute()
{
    ApplauseMeter.Instance.SetValue(targetValue, animate);
}
```

---

### 🎬 Performance Quality Events

#### `PerformanceQualityEvent`
Affect specific performance aspects

**Parameters**:
- `Aspect`: Timing, Delivery, Emotion, Chemistry, Overall
- `Quality Change`: -10 to +10

**Example Use Cases**:
- Rush through lines: Timing -3
- Perfect emotional delivery: Emotion +5
- Great chemistry with partner: Chemistry +2

**Future Implementation**:
```csharp
protected override void OnExecute()
{
    PerformanceManager.Instance.AdjustQuality(aspect, qualityChange);
}
```

---

### 🌟 Mood/Atmosphere Events

#### `StageMoodEvent`
Change stage atmosphere and lighting mood

**Parameters**:
- `Mood Type`: Tense, Joyful, Melancholic, Suspenseful, Triumphant, Intimate, Chaotic
- `Intensity`: 0-100%
- `Transition Duration`: Seconds

**Example Use Cases**:
- Dramatic confrontation → Tense mood
- Comedy scene → Joyful mood
- Quiet moment → Intimate mood

**Future Implementation**:
```csharp
protected override void OnExecute()
{
    MoodManager.Instance.SetMood(moodType, intensity, transitionDuration);
}
```

---

### 🔊 Audio Events

#### `PlaySoundEffectEvent`
Play one-shot sound effect

**Parameters**:
- `Sound Effect Name`: String identifier
- `Volume`: 0-100%
- `Interrupt Others`: Stop other SFX

**Example Use Cases**:
- Door slam
- Phone ring
- Dramatic sting

---

#### `ChangeMusicEvent`
Crossfade to different music track

**Parameters**:
- `Music Track Name`: String identifier
- `Fade In Duration`: Seconds
- `Fade Out Duration`: Seconds

**Example Use Cases**:
- Transition to action music
- Romantic scene music cue
- Return to ambient track

---

### 💡 Lighting/Visual Events

#### `SpotlightFocusEvent`
Change spotlight focus

**Parameters**:
- `Target`: Player, NPC, Both, None, Custom
- `Custom Target Name`: For custom targets
- `Transition Speed`: How fast to move

**Example Use Cases**:
- Spotlight on player for monologue
- Split spotlight for duet
- Blackout spotlight for dramatic moment

---

### 🎮 Game State Events

#### `SetGameFlagEvent`
Set persistent game flag (for branching story)

**Parameters**:
- `Flag Name`: String identifier
- `Flag Value`: true/false

**Example Use Cases**:
- Set "MetDirector" = true
- Set "PerformedInAct1" = true
- Set "ToldSecretToPartner" = true

**Future Implementation**:
```csharp
protected override void OnExecute()
{
    GameState.Instance.SetFlag(flagName, flagValue);
}
```

---

## How to Use (Current)

### Adding DialogEvents to Choices

1. **Open DialogNodeEditorWindow**
   - Select DialogTree asset
   - Click "Edit Starting Node" or "Edit" next to any node

2. **Expand a Choice**
   - Go to "Choices Management" section
   - Expand the choice you want to add events to

3. **Add DialogEvent**
   - Find "Dialog Events (Future: Crowd reactions, applause, etc.)" section
   - Click "+" to add new event
   - Select event type from dropdown:
     - `CrowdReactionEvent`
     - `ApplauseMeterEvent`
     - `PerformanceQualityEvent`
     - etc.

4. **Configure Event**
   - Expand the event
   - Set parameters (reaction type, intensity, duration, etc.)
   - Set event name and description (optional but recommended)

5. **Test (Current Behavior)**
   - When choice is selected in play mode
   - Event executes and logs to Console: `[EventName] SCAFFOLDING: ...`
   - No actual game effect yet (systems not implemented)

### Adding DialogEvents to Nodes

DialogEvents can also be added to nodes directly (not just choices):

1. **Open node in editor window**
2. **Go to "Unity Events" section** (confusingly named - will be updated)
3. **Expand "On Dialog Start" or "On Dialog End"**
4. **Add DialogEvent** to start/end events list

**Example Use Cases**:
- Node starts → Change music
- Node ends → Spotlight shift
- Important dialog → Crowd reaction

---

## Current Behavior (SCAFFOLDING)

When you trigger a DialogEvent in play mode:

**What Happens**:
```
[CrowdReactionEvent] SCAFFOLDING: Crowd reaction 'Applause' at 75% intensity for 2s
```

**What Doesn't Happen**:
- No visual crowd reaction (CrowdManager not implemented)
- No applause meter changes (ApplauseMeter not implemented)
- No mood changes (MoodManager not implemented)
- No audio plays (AudioManager not fully integrated)

**Purpose**:
- Verify events are configured correctly
- Test dialog flow with events
- Plan game system integration

---

## Future Implementation Checklist

When you're ready to implement actual game systems:

### For Each Event Type:

1. **Implement Manager Class**
   ```csharp
   public class CrowdManager : MonoBehaviour
   {
       public static CrowdManager Instance { get; private set; }
       
       public void TriggerReaction(CrowdReaction type, float intensity, float duration)
       {
           // Actual implementation
       }
   }
   ```

2. **Update Event OnExecute()**
   ```csharp
   protected override void OnExecute()
   {
       // Remove debug log
       // Debug.Log($"[CrowdReactionEvent] SCAFFOLDING: ...");
       
       // Add actual call
       CrowdManager.Instance.TriggerReaction(reactionType, intensity, duration);
   }
   ```

3. **Test Event**
   - Create test dialog with event
   - Verify manager method is called
   - Verify game effect occurs
   - Check for errors/edge cases

4. **Update Documentation**
   - Remove "SCAFFOLDING" status
   - Add screenshots/examples
   - Document any limitations

---

## Example Dialog Flow with Events

### Scenario: Player chooses great improvisation line

**Dialog Setup**:
```
Director: "Quick! Improvise a line!"

Choice 1: "I've been waiting for this moment my whole life!"
  Events:
  - CrowdReactionEvent: Applause, 80%, 2s
  - ApplauseMeterEvent: +15 points, "Brilliant improvisation!"
  - PerformanceQualityEvent: Delivery +5
  
Choice 2: "Uh... line?"
  Events:
  - CrowdReactionEvent: Silence, 60%, 1.5s
  - ApplauseMeterEvent: -10 points, "The crowd seems confused..."
  - PerformanceQualityEvent: Timing -3
```

**Current Behavior** (in Console):
```
Player selects Choice 1:
[CrowdReactionEvent] SCAFFOLDING: Crowd reaction 'Applause' at 80% intensity for 2s
[ApplauseMeterEvent] SCAFFOLDING: +15 applause points. Feedback: Brilliant improvisation!
[PerformanceQualityEvent] SCAFFOLDING: Delivery +5
```

**Future Behavior**:
- Crowd NPCs play applause animation
- Applause meter UI updates (+15)
- Feedback message appears on screen
- Performance stats UI shows Delivery improved
- Audio plays crowd applause sound

---

## Tips for Planning Events

### 1. Use Meaningful Names
```csharp
// ❌ Bad
eventName = "Event1"

// ✅ Good
eventName = "GreatImprovisationApplause"
```

### 2. Add Descriptions
```csharp
description = "Crowd reacts positively to player's brilliant improvisation"
```

### 3. Balance Point Values
- Small good choice: +5 to +10
- Great choice: +15 to +20
- Small mistake: -5 to -10
- Major mistake: -15 to -20

### 4. Layer Events
Don't just use one event - combine them!
```
Good Choice:
- CrowdReactionEvent (visual)
- ApplauseMeterEvent (score)
- PerformanceQualityEvent (stat)
- PlaySoundEffectEvent (audio)
```

### 5. Test Without Game Systems
Use debug logs to verify:
- Events fire in correct order
- Parameters are correct
- No errors occur

---

## Creating Custom Events

Want to add your own event types?

### Template:

```csharp
[System.Serializable]
public class YourCustomEvent : DialogEvent
{
    [SerializeField] private string yourParameter;
    
    public string YourParameter
    {
        get => yourParameter;
        set => yourParameter = value;
    }
    
    protected override void OnExecute()
    {
        // TODO: When YourSystem is implemented, call:
        // YourSystem.Instance.DoSomething(yourParameter);
        
        Debug.Log($"[YourCustomEvent] SCAFFOLDING: {yourParameter}");
    }
    
    public override string GetDisplayName()
    {
        return $"Your Event: {yourParameter}";
    }
    
    public override bool IsValid()
    {
        return base.IsValid() && !string.IsNullOrEmpty(yourParameter);
    }
}
```

### Steps:
1. Add to `GameSystemDialogEvents.cs`
2. Inherit from `DialogEvent`
3. Add `[System.Serializable]` attribute
4. Override `OnExecute()` with scaffolding
5. Add validation in `IsValid()`
6. Provide meaningful `GetDisplayName()`

---

## Troubleshooting

### Events Not Showing in Inspector

**Problem**: DialogEvents list doesn't appear

**Solution**:
- Make sure you're editing a **DialogChoice** (not just any object)
- Check that file `GameSystemDialogEvents.cs` compiled successfully
- Restart Unity Editor to refresh serialization

---

### Events Not Executing

**Problem**: No console logs when choice selected

**Solution**:
- Check event `IsEnabled` is true
- Verify `IsValid()` returns true
- Check DialogNavigator is executing choice events:
  ```csharp
  // In DialogNavigator.SelectChoice():
  ExecuteChoiceEvents(selectedChoice);
  ```

---

### Can't See Custom Event Type

**Problem**: New event type doesn't appear in dropdown

**Solution**:
- Must inherit from `DialogEvent`
- Must have `[System.Serializable]` attribute
- Must be in `Assembly-CSharp` (not `Assembly-CSharp-Editor`)
- Recompile and restart Unity

---

## Files Reference

| File | Purpose |
|------|---------|
| `DialogEvents.cs` | Base DialogEvent class and core events |
| `GameSystemDialogEvents.cs` | Pre-built game system events (NEW) |
| `Dialog Choice.cs` | DialogChoice with ChoiceEvents support |
| `DialogChoicePropertyDrawer.cs` | Inspector display for choices with events |
| `DialogNavigator.cs` | Executes choice events at runtime |

---

## Next Steps

1. **Plan Game Systems** (Current phase)
   - Design CrowdManager behavior
   - Design ApplauseMeter UI
   - Design PerformanceManager stats

2. **Implement One System** (Test phase)
   - Start with simplest (e.g., CrowdReactionEvent)
   - Implement manager class
   - Update event OnExecute()
   - Test thoroughly

3. **Expand to Other Systems** (Rollout phase)
   - Implement remaining managers
   - Update remaining events
   - Test integration

4. **Polish & Balance** (Final phase)
   - Tune point values
   - Balance event timings
   - Add juice/feedback
   - Playtest!

---

## Summary

✅ **Scaffolding Complete**: DialogEvents infrastructure ready  
✅ **10+ Event Types**: Pre-built for common game systems  
✅ **Editor Support**: Full inspector integration  
✅ **Documentation**: Complete usage guide  
⏳ **Pending**: Actual game system implementations  

**Status**: Ready for game system development! 🎉

---

**End of DialogEvents Scaffolding Guide**

**Related Documentation**:
- [DialogTree Editing Workflow Guide](./DialogTree-Editing-Workflow-Guide.md)
- [Class Hierarchy](./Class Hierarchy.md)
- [Troubleshooting](./TROUBLESHOOTING.MD)
