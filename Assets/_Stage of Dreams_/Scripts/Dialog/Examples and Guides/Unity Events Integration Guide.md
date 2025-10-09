# Unity Events Integration Guide for Dialog System

## ? **Unity Events CAN Call Methods with Parameters!**

Yes, Unity's UnityEvent system has excellent capability for calling methods with parameters from your dialog events. Here's the complete guide:

## ?? **What Unity Events Can Do**

### **1. Call Methods with Parameters**
- `UnityEvent<int>` - Call methods with integer parameters
- `UnityEvent<string>` - Call methods with string parameters  
- `UnityEvent<float>` - Call methods with float parameters
- `UnityEvent<bool>` - Call methods with boolean parameters
- Up to 4 parameters: `UnityEvent<int, string, float, bool>`

### **2. Call Different Method Types**
- **Instance Methods**: On any MonoBehaviour component
- **Static Methods**: From any class
- **Multiple Methods**: One event can trigger multiple method calls
- **Cross-Script**: Call methods in completely different scripts

## ?? **Theater Game Examples**

### **Using AudienceManager with Dialog Events**

```csharp
// In your dialog events, you can call:
AudienceManager.AudienceApplause(10);        // Heavy applause
AudienceManager.AudienceReaction("gasp");    // Audience gasps
AudienceManager.SetAudienceMood(0.8f);       // High energy
AudienceManager.CreateSilence();             // Dramatic pause
```

### **Setting Up in Unity Inspector**

1. **Add AudienceManager to Scene**:
   ```
   Create Empty GameObject ? "AudienceManager" 
   ? Add Component ? AudienceManager
   ```

2. **In Dialog Node Events**:
   ```
   Expand "On Dialog Start" UnityEvent
   ? Click "+" to add event
   ? Drag AudienceManager GameObject to Object field
   ? Select "AudienceManager" ? "AudienceApplause(int)"
   ? Set intensity parameter (e.g., 5 for medium applause)
   ```

3. **Multiple Event Example**:
   ```
   On Dialog End:
   ?? AudienceManager.AudienceApplause(8)
   ?? AudienceManager.BoostEngagement(0.1)
   ?? PerformanceManager.TrackDialogCompletion()
   ```

## ??? **Creating Your Own Event-Driven Classes**

### **Example: PerformanceManager**

```csharp
public class PerformanceManager : MonoBehaviour
{
    [Header("Performance Tracking")]
    public int totalScore = 0;
    public float confidenceLevel = 0.5f;
    
    /// <summary>
    /// Add points to performance score
    /// Called from dialog choice events
    /// </summary>
    public void AddPerformancePoints(int points)
    {
        totalScore += points;
        Debug.Log($"Performance score: {totalScore}");
    }
    
    /// <summary>
    /// Modify player confidence
    /// Called from encouraging/discouraging dialog
    /// </summary>
    public void ModifyConfidence(float amount)
    {
        confidenceLevel = Mathf.Clamp01(confidenceLevel + amount);
        Debug.Log($"Confidence: {confidenceLevel:P0}");
    }
    
    /// <summary>
    /// Trigger specific performance events
    /// Called from story-critical dialog moments
    /// </summary>
    public void TriggerPerformanceEvent(string eventType)
    {
        switch (eventType)
        {
            case "curtain_call":
                // Handle curtain call logic
                break;
            case "intermission":
                // Handle intermission logic
                break;
            case "finale":
                // Handle finale logic
                break;
        }
    }
}
```

### **Example: LightingManager**

```csharp
public class LightingManager : MonoBehaviour
{
    /// <summary>
    /// Change stage lighting for dramatic effect
    /// Called from dialog events for atmosphere
    /// </summary>
    public void SetLighting(string lightingMode)
    {
        switch (lightingMode.ToLower())
        {
            case "dramatic":
                // Dim lights, spotlight on speaker
                break;
            case "bright":
                // Full stage lighting
                break;
            case "romantic":
                // Warm, soft lighting
                break;
            case "tense":
                // Cold, harsh lighting
                break;
        }
    }
    
    /// <summary>
    /// Fade lights to create transitions
    /// </summary>
    public void FadeLights(float duration)
    {
        // Implement fade logic
    }
}
```

## ?? **Common Dialog Event Patterns**

### **1. On Dialog Start Events**
```
Perfect for:
?? Setting scene atmosphere
?? Playing entrance music  
?? Triggering character animations
?? Adjusting lighting
?? Starting sound effects
```

### **2. On Dialog End Events**
```
Perfect for:
?? Audience reactions
?? Score tracking
?? Triggering next scene events
?? Saving progress
?? Playing exit animations
```

### **3. On Choice Selected Events**
```
Perfect for:
?? Different audience reactions per choice
?? Confidence/score modifications
?? Branching story tracking
?? Character relationship changes
?? Performance style adjustments
```

## ?? **Advanced Event Techniques**

### **1. Parameterized Events with Validation**

```csharp
public class DialogEventHandler : MonoBehaviour
{
    /// <summary>
    /// Smart applause that adjusts based on current performance quality
    /// </summary>
    public void SmartApplause(int baseIntensity)
    {
        var performance = FindObjectOfType<PerformanceManager>();
        float multiplier = performance.confidenceLevel;
        int adjustedIntensity = Mathf.RoundToInt(baseIntensity * multiplier);
        
        var audience = FindObjectOfType<AudienceManager>();
        audience.AudienceApplause(adjustedIntensity);
    }
}
```

### **2. Event Chains**

```csharp
/// <summary>
/// Trigger a sequence of events with delays
/// </summary>
public void TriggerEventSequence(string sequenceName)
{
    StartCoroutine(ExecuteSequence(sequenceName));
}

private IEnumerator ExecuteSequence(string sequenceName)
{
    switch (sequenceName)
    {
        case "dramatic_entrance":
            SetLighting("dramatic");
            yield return new WaitForSeconds(1f);
            PlaySound("dramatic_music");
            yield return new WaitForSeconds(2f);
            AudienceReaction("anticipation");
            break;
    }
}
```

### **3. Conditional Events**

```csharp
/// <summary>
/// Different reactions based on game state
/// </summary>
public void ConditionalAudienceReaction(string choiceType)
{
    var audience = FindObjectOfType<AudienceManager>();
    var performance = FindObjectOfType<PerformanceManager>();
    
    if (performance.confidenceLevel > 0.7f)
    {
        audience.AudienceReaction("supportive");
    }
    else
    {
        audience.AudienceReaction("uncertain");
    }
}
```

## ?? **Setup Checklist**

### **For Each Event-Driven Class:**
- [ ] Create MonoBehaviour class with public methods
- [ ] Add class to scene as component on GameObject
- [ ] Configure parameters in inspector if needed
- [ ] Test methods work independently

### **For Each Dialog Node:**
- [ ] Expand desired UnityEvent (OnDialogStart/OnDialogEnd)
- [ ] Click "+" to add new event
- [ ] Drag target GameObject to Object field
- [ ] Select Component ? Method from dropdown
- [ ] Configure parameters if method requires them
- [ ] Test in play mode

## ?? **Best Practices**

### **Method Design:**
- Keep methods focused on single responsibility
- Use clear, descriptive parameter names
- Add validation/clamping for numeric parameters
- Include debug logging for testing

### **Event Organization:**
- Group related methods in dedicated Manager classes
- Use consistent naming conventions
- Document what each method does
- Keep parameter counts reasonable (1-3 parameters max)

### **Performance:**
- Avoid expensive operations in event methods
- Use coroutines for delayed/complex sequences
- Cache component references when possible
- Consider pooling for frequently called events

## ?? **Example Usage in Your Theater Game**

```
Dialog: "The crowd holds its breath as you approach center stage..."

OnDialogStart Events:
?? AudienceManager.CreateSilence()
?? LightingManager.SetLighting("dramatic")  
?? CameraManager.FocusOnPlayer()
?? MusicManager.PlayAmbientTension()

Choice: "Deliver your opening line with confidence"

OnChoiceSelected Events:
?? PerformanceManager.AddPerformancePoints(10)
?? PerformanceManager.ModifyConfidence(0.2)
?? AudienceManager.AudienceReaction("anticipation")
?? DialogEventHandler.TriggerEventSequence("confident_opening")
```

**This system gives you incredibly powerful and flexible event integration with your dialog system!** ??