# Unity Editor Troubleshooting

**Last Updated**: December 2025  
**Unity Version**: 6000.2.6f2

---

## Unity Won't Open Project

### Symptoms:
- Unity stuck on "Opening Project"
- Unity crashes on startup
- "Hold On" dialog never finishes

### Solutions:

**1. Delete Library Folder**
```bash
# Close Unity first!
rm -r Library/
# Reopen project - Unity will regenerate
```

**2. Check Unity Version**
- Verify you're using Unity 6000.2.6f2
- Install correct version via Unity Hub

**3. Check Disk Space**
- Library folder can be large (1-10GB)
- Ensure 20GB+ free space

**4. Antivirus Interference**
- Add Unity and project folder to antivirus exceptions
- Temporarily disable antivirus to test

---

## Script Compilation Errors

### Errors Won't Clear

**Solution**:
```
1. Assets → Reimport All
2. Edit → Preferences → External Tools → Regenerate project files
3. Close Visual Studio
4. Restart Unity
5. Reopen solution in Visual Studio
```

### "Assembly has reference to non-existent assembly"

**Cause**: Missing package or incorrect reference

**Fix**:
1. Window → Package Manager
2. Check if all required packages are installed
3. Verify package versions are compatible
4. Re-install problematic package

---

## Inspector Issues

### Script Missing / "Can't add script"

**Causes**:
1. Script has compilation errors
2. Class name doesn't match filename
3. Script not in `Assets/` folder
4. Namespace issues

**Fix**:
```csharp
// Filename: MyScript.cs
public class MyScript : MonoBehaviour // Name must match!
{
    // ...
}
```

### Inspector Values Reverting

**Cause**: Prefab overrides or Git conflicts

**Fix**:
1. Check if GameObject is prefab instance
2. GameObject → Prefab → Unpack Completely (if not intentional)
3. Or: Prefab → Apply All (to save changes to prefab)

---

## Performance Issues

### Unity Editor Slow / Laggy

**Solutions**:

**1. Reduce Auto-Refresh**:
- Edit → Preferences → Asset Pipeline
- Disable "Auto Refresh"
- Manually refresh: Ctrl+R when needed

**2. Close Unnecessary Windows**:
- Close Profiler, Frame Debugger when not in use
- Close Scene/Game view if only using one

**3. Reduce Scene Complexity**:
- Use Scene view culling
- Disable Gizmos in Scene view

**4. Check Background Processes**:
- Task Manager → End Unity-related processes
- Close other resource-heavy applications

---

## Asset Import Issues

### Assets Not Importing

**Check**:
1. File format supported by Unity
2. File not corrupted
3. File size reasonable
4. Unity not still importing (check progress bar)

**Force Reimport**:
```
Right-click asset → Reimport
OR
Assets → Reimport All
```

### Import Takes Forever

**Speed up**:
1. Window → Asset Management → Version Control → Work Offline
2. Edit → Project Settings → Editor → Sprite Packer → Disabled
3. Close unused editor windows

---

## Play Mode Issues

### Can't Enter Play Mode

**Error**: "All compiler errors must be fixed before entering Play Mode"

**Solution**:
1. Check Console (Ctrl+Shift+C) for errors
2. Fix all red errors
3. Warnings (yellow) are okay

### Play Mode Crashes

**Debug**:
1. Check Console for errors before crash
2. Check Editor.log:
   - Windows: `%LocalAppData%\Unity\Editor\Editor.log`
3. Run in Debug mode with breakpoints

### Changes in Play Mode Lost

**This is normal behavior!**
- Changes in Play Mode are temporary
- Exit Play Mode before making persistent changes
- Or: Right-click component → Copy Component
- Exit Play Mode → Paste Component Values

---

## UI Toolkit Issues

### UI Builder Won't Open

**Solution**:
```
1. Window → UI Toolkit → UI Builder
2. If still broken:
   - Delete Library/UIElements/ folder
   - Assets → Reimport All
```

### UXML Changes Not Reflecting

**Causes**:
1. UIDocument not reassigned after changes
2. Unity caching old version

**Fix**:
1. Reimport UXML file
2. Reassign in UIDocument component
3. Enter/Exit Play Mode

---

## Input System Issues

### Input Not Working

**Check**:
1. Input System package installed
2. Input Actions asset created and assigned
3. Player Input component present
4. Correct Action Map selected
5. Actions enabled in script

**Test**:
```csharp
// Check if action is triggered
void Update()
{
    if (playerInput.actions["Interact"].triggered)
    {
        Debug.Log("Interact triggered!");
    }
}
```

---

## Quick Fixes

### "Unity has stopped working"

1. Save project (Ctrl+S)
2. Close Unity
3. Delete `Library/ShaderCache/`
4. Reopen project

### Scene Won't Save

1. File → Save Project (Ctrl+S)
2. Check Console for errors
3. Verify scene file not read-only
4. Check Git status - may be conflict

### Missing Prefab Reference

1. Drag prefab from Project to Inspector field
2. Or: Click circle icon → Select prefab

---

## Diagnostic Tools

**Console Filters**:
- Clear On Play - Disable to see errors that caused crash
- Error Pause - Enable to pause on errors
- Collapse - Group similar messages

**Profiler** (Window → Analysis → Profiler):
- CPU Usage - Find performance bottlenecks
- Memory - Check for memory leaks
- Rendering - Check draw calls

**Frame Debugger** (Window → Analysis → Frame Debugger):
- See exactly what's being rendered
- Check if UI elements are being drawn
