# Light2D Spotlight System & Darkness Control

## Overview
Your Spotlight system has been updated to work with Unity's Light2D components instead of sprite renderers, providing more realistic and flexible lighting for your theatrical game. Additionally, a new LightingManager has been created to control global lighting and darkness.

## Key Changes Made

### 1. Updated Spotlight.cs
- **Light2D Integration**: Now works with Light2D components as the primary lighting source
- **Automatic Detection**: Automatically finds and uses Light2D components in the spotlight hierarchy
- **Radius Syncing**: Can automatically sync detection radius with Light2D outer radius
- **Intensity Control**: Controls Light2D intensity for fade in/out effects
- **Color Control**: Can change Light2D color dynamically
- **Fallback Support**: Still supports SpriteRenderer for visual representation (optional)

### 2. New LightingManager.cs
- **Global Lighting Control**: Manages scene-wide lighting and darkness
- **Dark Mode**: Toggle between normal and dramatic/dark lighting
- **Smooth Transitions**: Animated transitions between lighting states
- **Background Control**: Manages camera background color
- **Dialog Integration**: Methods that can be called from Dialog Nodes

### 3. Enhanced SpotlightController.cs
- **Light2D Methods**: New methods for controlling Light2D properties
- **Lighting Integration**: Controls both spotlights and global lighting
- **Extended Dialog Support**: More methods for Dialog Node integration
- **Test Controls**: Debug keys for testing lighting effects

## Setup Instructions

### Setting Up Light2D Spotlights

1. **Add Light2D Components to Your Spotlights**:
   - Select your spotlight GameObject
   - Add Component ? Rendering ? Light 2D
   - Set Light Type to "Point" or "Spot" as needed
   - Configure outer radius, intensity, and color

2. **Configure Spotlight Script**:
   - The script will automatically find the Light2D component
   - Set `Use Outer Radius` to true to sync detection with Light2D radius
   - Adjust `Base Intensity` if needed (defaults to Light2D intensity)

### Setting Up Global Lighting Control

1. **Create a Global Light2D**:
   - Create an empty GameObject named "Global Light"
   - Add Component ? Rendering ? Light 2D
   - Set Light Type to "Global"
   - This will control overall scene brightness

2. **Add LightingManager**:
   - Create an empty GameObject named "Lighting Manager"
   - Add the LightingManager script
   - Assign the Global Light2D in the inspector
   - Configure normal and dark lighting intensities

3. **Configure SpotlightController**:
   - Assign the LightingManager in SpotlightController
   - Enable "Enable Lighting Effects" checkbox

## Controlling Darkness

### From Dialog Nodes/Choices
You can now call these methods from Dialog Node events:

**Lighting Control:**
- `EnableDarkLighting()` - Enable dramatic/dark mode
- `DisableDarkLighting()` - Return to normal lighting
- `ToggleDarkLighting()` - Toggle between modes
- `SetAmbientLighting(float intensity)` - Set custom intensity

**Spotlight Control:**
- `ShowSpotlightByName(string name)` - Show specific spotlight
- `HideSpotlightByName(string name)` - Hide specific spotlight
- `SetSpotlightColor(string name, Color color)` - Change spotlight color
- `SetSpotlightIntensity(string name, float intensity)` - Change spotlight intensity

### From Code
```csharp
// Enable dark mode
lightingManager.EnableDarkMode();

// Disable dark mode
lightingManager.DisableDarkMode();

// Set custom lighting
lightingManager.SetAmbientIntensity(0.3f);

// Change spotlight properties
spotlight.SetColor(Color.red);
spotlight.SetIntensity(15f);
```

### Testing Controls
**Keyboard shortcuts for testing:**
- **L** - Toggle dark lighting mode
- **Shift+L** - Instant toggle (no animation)
- **1-5** - Spotlight movement tests
- **6-8** - Spotlight visibility tests
- **9** - Toggle dark lighting
- **0** - Instant lighting toggle

## Scene Setup Recommendations

### For Dramatic Theater Lighting:

1. **Global Light Setup**:
   - Global Light2D intensity: 0.1-0.3 for dark mode
   - Global Light2D intensity: 1.0 for normal mode
   - Camera background: Black

2. **Spotlight Setup**:
   - Point Light2D with high intensity (15-25)
   - Warm colors (yellow/orange) for dramatic effect
   - Outer radius matching your detection needs

3. **Layering**:
   - Use different Light2D blend modes for effects
   - Consider using multiple lights for complex scenes
   - Use Light2D cookies for spotlight patterns

### Preventing Total Darkness:

1. **Minimum Ambient**: Set dark mode intensity to 0.1-0.2 (not 0)
2. **Character Visibility**: Add small lights to important characters
3. **UI Lighting**: Use separate UI elements that aren't affected by 2D lighting
4. **Fallback Sprites**: Keep optional sprite renderers for visibility fallback

## Integration with Dialog System

### Example Dialog Node Setup:
1. Create a Dialog Node
2. In `OnDialogStart` event, add a new entry
3. Select your SpotlightController
4. Choose a method like `EnableDarkLighting()` or `ShowSpotlightByName()`
5. For methods with parameters, you can pass spotlight names or values

### Example Dramatic Scene Sequence:
1. **Start**: `EnableDarkLighting()` - Set dramatic atmosphere
2. **Spotlight Character**: `ShowSpotlightByName("MainSpotlight")` - Highlight speaker
3. **Color Change**: `SetSpotlightColor("MainSpotlight", Color.red)` - Add tension
4. **End Scene**: `DisableDarkLighting()` - Return to normal

## Troubleshooting

### Common Issues:

1. **"No Light2D component found"**: Ensure Light2D is in spotlight hierarchy
2. **Spotlight not visible**: Check Light2D is enabled and has intensity > 0
3. **No darkness effect**: Ensure Global Light2D is set to "Global" type
4. **Jerky transitions**: Adjust fade speed in Spotlight or transition speed in LightingManager

### Performance Considerations:
- Limit number of active Light2D components (Unity recommendation: <50)
- Use Light2D cookies instead of multiple small lights where possible
- Consider using Light2D culling for off-screen lights

## Future Enhancements

The system is designed to be extensible. You could add:
- Light2D cookie support for spotlight shapes
- Color temperature controls
- Flickering effects
- Light following animations
- Multiple lighting presets
- Integration with post-processing effects

This system gives you full control over theatrical lighting while maintaining good performance and easy Dialog Node integration.