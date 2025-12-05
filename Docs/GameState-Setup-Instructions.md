# GameState Setup Instructions

## Quick Start Guide

### Step 1: Create Default GameStateData Asset

1. **In Unity Project window**, navigate to: `Assets\_Stage of Dreams_\World\`
2. **Right-click** in the folder
3. **Select**: `Create → Stage of Dreams → Game State Data`
4. **Name it**: `DefaultGameState`

### Step 2: Configure Default State (Optional)

1. **Select** the `DefaultGameState` asset
2. **In Inspector**, you'll see all the default values:
   - Applause Score: 50
   - Boo Score: 0
   - Audience Mood: 0.5 (Neutral)
   - All scores: 0
   - Empty progression lists
3. **Modify** any values you want for your game's starting state
4. **Right-click** on the asset → `Validate Data` to check integrity

### Step 3: Add GameStateManager to Scene

**Option A: Automatic Creation**
- GameStateManager will create itself automatically when first accessed via `GameStateManager.Instance`
- It uses `DontDestroyOnLoad()` so it persists across scenes

**Option B: Manual Setup (Recommended for Development)**
1. **In Hierarchy**, create an empty GameObject: `GameObject → Create Empty`
2. **Name it**: `GameStateManager`
3. **Add component**: `GameStateManager` script
4. **In Inspector**, assign your `DefaultGameState` asset to the `Default State` field
5. **Configure settings**:
   - Enable Debug Logs: ✓ (for development)
   - Audience Decay Rate: 1.0 (points per second)
   - Audience Mood Smooth Time: 2.0 (seconds)
   - Max Score Per Scene: 1000
   - Score Threshold For Next Dream: 500

### Step 4: Test the System

1. **Enter Play Mode**
2. **In Hierarchy**, find the GameStateManager object
3. **Right-click** on GameStateManager component → `Print Current State`
4. **Check Console** - you should see the initial state
5. **Test modifications**:
   - Right-click → `Test Applause Increase` (adds 10 points)
   - Right-click → `Test Boo Increase` (adds 10 points)
   - Right-click → `Test Score Add` (adds 50 points)
6. **Check Console** for [GameStateManager] log messages

---

## Usage in Your Code

### Accessing GameState

```csharp
// Adjust audience scores
GameStateManager.Instance.AdjustApplause(20f);
GameStateManager.Instance.AdjustBoo(-5f);

// Add performance scores
GameStateManager.Instance.AddSceneScore(100);

// Track minigames
GameStateManager.Instance.StartMinigame("calm_dialog");
GameStateManager.Instance.EndMinigame("calm_dialog", success: true);

// Progression
GameStateManager.Instance.CompleteScene("act1_scene1");
GameStateManager.Instance.UnlockAbility("PerfectMemory");

// Check state
if (GameStateManager.Instance.HasAbility("Improvise"))
{
    // Show improvise option
}

// Turn-based
GameStateManager.Instance.StartTurnBasedMode();
if (GameStateManager.Instance.UsePlayerAction())
{
    // Perform action
}
```

### Subscribing to Events

```csharp
private void OnEnable()
{
    GameStateManager.Instance.OnApplauseScoreChanged += HandleApplauseChanged;
    GameStateManager.Instance.OnSceneScoreChanged += HandleScoreChanged;
    GameStateManager.Instance.OnAbilityUnlocked += HandleAbilityUnlocked;
}

private void OnDisable()
{
    GameStateManager.Instance.OnApplauseScoreChanged -= HandleApplauseChanged;
    GameStateManager.Instance.OnSceneScoreChanged -= HandleScoreChanged;
    GameStateManager.Instance.OnAbilityUnlocked -= HandleAbilityUnlocked;
}

private void HandleApplauseChanged(float newScore)
{
    Debug.Log($"Applause changed to: {newScore}");
    // Update UI
}
```

---

## Next Steps

1. ✅ **Created**: GameStateManager.cs
2. ✅ **Created**: GameStateData.cs
3. ⏳ **To Do**: Create DefaultGameState asset (follow Step 1 above)
4. ⏳ **To Do**: Add GameStateManager to your main scene (follow Step 3 above)
5. ⏳ **To Do**: Test the system (follow Step 4 above)
6. ⏳ **To Do**: Integrate with DialogNavigator for minigames
7. ⏳ **To Do**: Integrate with AudienceManager for reactions
8. ⏳ **To Do**: Create UI to display scores and audience metrics

---

## Troubleshooting

### "Instance not found" Error
- **Solution**: GameStateManager creates itself automatically. If you see this error, make sure you're calling `GameStateManager.Instance` and not trying to `Find` it manually.

### No Debug Logs Appearing
- **Solution**: Select GameStateManager in Hierarchy → Check "Enable Debug Logs" is enabled

### State Not Persisting Between Scenes
- **Solution**: GameStateManager uses `DontDestroyOnLoad()` automatically. If state resets, check that you only have ONE GameStateManager instance (the singleton prevents duplicates).

### Default State Not Loading
- **Solution**: Make sure you've assigned the DefaultGameState asset to the GameStateManager's "Default State" field in the Inspector.

---

## File Locations

- **GameStateManager.cs**: `Assets\_Stage of Dreams_\Scripts\GameStateManager.cs`
- **GameStateData.cs**: `Assets\_Stage of Dreams_\World\GameStateData.cs`
- **DefaultGameState.asset**: `Assets\_Stage of Dreams_\World\DefaultGameState.asset` (create this)

---

**Ready to use!** The core system is implemented and ready for integration with your minigames and UI systems.
