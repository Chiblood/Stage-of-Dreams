# Unity Debug Logging Best Practices

**Last Updated**: January 2025  
**Status**: ✅ ACTIVE STANDARD  
**Related**: `.github\copilot-instructions.md`, `TROUBLESHOOTING.MD`

---

## Overview

This document establishes standards for debug logging in the **Stage of Dreams** Unity project. Following these practices ensures console output is readable, consistent, and properly displayed across all development environments.

---

## The Problem: Unity Console Unicode Limitations

### What Doesn't Work

Unity's Console window uses a **basic monospace font** that doesn't support:
- ✅❌⚠️ Emoji characters
- 📊🎭🔧 Unicode symbols
- ➜→⬆⬇ Directional arrows
- Most characters beyond basic ASCII (32-126)

### What Happens

When you use unsupported characters:
```csharp
Debug.Log("✅ Success!");  // Displays: "? Success!"
Debug.Log("⚠️ Warning");   // Displays: "? Warning"
Debug.Log("🎭 Dialog");    // Displays: "? Dialog"
```

**Result**: Unclear, confusing console output with `?` symbols everywhere.

---

## The Solution: ASCII-Safe Logging

Use **text-based indicators** instead of emojis/symbols:

```csharp
Debug.Log("[OK] Success!");      // ✅ Works perfectly
Debug.LogWarning("[WARNING] Issue");  // ✅ Clear and readable
Debug.Log("[DIALOG] Started");   // ✅ Descriptive
```

---

## Standard Prefixes

### Status Indicators

| Prefix | Meaning | Unity Method | Example |
|--------|---------|--------------|---------|
| `[OK]` | Success/Valid | `Debug.Log()` | `[OK] Tree validated` |
| `[SUCCESS]` | Operation succeeded | `Debug.Log()` | `[SUCCESS] File saved` |
| `[INFO]` | Information | `Debug.Log()` | `[INFO] Loading assets` |
| `[COMPLETE]` | Task finished | `Debug.Log()` | `[COMPLETE] Build done` |

### Warning/Error Indicators

| Prefix | Meaning | Unity Method | Example |
|--------|---------|--------------|---------|
| `[WARNING]` | Potential issue | `Debug.LogWarning()` | `[WARNING] Null reference` |
| `[!]` | Alert/Caution | `Debug.LogWarning()` | `[!] Missing component` |
| `[ERROR]` | Operation failed | `Debug.LogError()` | `[ERROR] Load failed` |

### Action Indicators

| Prefix | Meaning | Unity Method | Example |
|--------|---------|--------------|---------|
| `[START]` | Beginning operation | `Debug.Log()` | `[START] Initializing` |
| `[LOADING]` | Loading resource | `Debug.Log()` | `[LOADING] Scene assets` |
| `[FIXED]` | Issue repaired | `Debug.Log()` | `[FIXED] Reference restored` |
| `[CREATED]` | New instance made | `Debug.Log()` | `[CREATED] New node` |

### System Indicators

| Prefix | Meaning | Unity Method | Example |
|--------|---------|--------------|---------|
| `[DIALOG]` | Dialog system | `Debug.Log()` | `[DIALOG] Node changed` |
| `[STAGE]` | Stage systems | `Debug.Log()` | `[STAGE] Lights dimmed` |
| `[AUDIO]` | Audio system | `Debug.Log()` | `[AUDIO] Music started` |
| `[UI]` | UI system | `Debug.Log()` | `[UI] Menu opened` |

### Statistics Indicators

| Prefix | Meaning | Unity Method | Example |
|--------|---------|--------------|---------|
| `[#]` | Count/Number | `Debug.Log()` | `[#] 5 nodes found` |
| `[STATS]` | Statistics | `Debug.Log()` | `[STATS] FPS: 60` |
| `[TIME]` | Timing info | `Debug.Log()` | `[TIME] Took 0.5s` |

---

## Complete Emoji Replacement Table

| Emoji | ASCII Replacement | Use Case | Example |
|-------|------------------|----------|---------|
| ✅ | `[OK]` | Success/Valid | `[OK] DialogTree valid` |
| ❌ | `[ERROR]` or `[!]` | Error/Invalid | `[ERROR] Load failed` |
| ⚠️ | `[WARNING]` | Warning | `[WARNING] Null ref` |
| 🔧 | `[FIXED]` | Fixed/Repaired | `[FIXED] Node repaired` |
| 📊 | `[STATS]` or `[#]` | Statistics | `[#] 10 items` |
| ✨ | `[CREATED]` | Created | `[CREATED] New tree` |
| 🎯 | `[TARGET]` | Goal/Objective | `[TARGET] Node reached` |
| 💡 | `[INFO]` or `[TIP]` | Information | `[INFO] Use hotkey` |
| 🚀 | `[START]` | Started | `[START] Dialog began` |
| 🎉 | `[COMPLETE]` | Completed | `[COMPLETE] All done` |
| 🐛 | `[BUG]` | Bug/Issue | `[BUG] Render issue` |
| 🔍 | `[SEARCH]` | Searching | `[SEARCH] Finding nodes` |
| ⏱️ | `[TIME]` | Timing/Duration | `[TIME] 1.5s elapsed` |
| 🎭 | `[DIALOG]` | Stage/Dialog | `[DIALOG] Tree loaded` |
| 🎮 | `[GAME]` | Game system | `[GAME] State changed` |
| 🎬 | `[SCENE]` | Scene management | `[SCENE] Loading level` |
| 🎨 | `[RENDER]` | Graphics/Rendering | `[RENDER] Texture loaded` |
| 💾 | `[SAVE]` | Save operation | `[SAVE] Data written` |
| 📁 | `[FILE]` | File operation | `[FILE] Asset loaded` |
| ➜ → | `->` | Direction/Flow | `Node1 -> Node2` |
| ⬆️ | `^` or `UP` | Upward | `[UP] Parent node` |
| ⬇️ | `v` or `DOWN` | Downward | `[DOWN] Child node` |

---

## Code Examples

### ✅ Good Examples

**Dialog System**:
```csharp
Debug.Log($"[DIALOG] Starting dialog with {npcName}");
Debug.Log($"[DIALOG] Node changed: {node.NodeName}");
Debug.Log($"[DIALOG] Choice selected: {choice.ChoiceText}");
```

**Tree Validation**:
```csharp
if (tree.IsValid())
{
    Debug.Log($"[OK] DialogTree '{tree.treeName}' validated: {nodeCount} nodes");
}
else
{
    Debug.LogError($"[ERROR] DialogTree '{tree.treeName}' invalid: {reason}");
}
```

**Operations with Status**:
```csharp
Debug.Log("[START] Loading dialog assets...");
// ... loading code ...
Debug.Log("[COMPLETE] All assets loaded successfully");
```

**Warnings**:
```csharp
if (string.IsNullOrEmpty(node.DialogText))
{
    Debug.LogWarning($"[WARNING] Node '{node.NodeName}' has empty dialog text");
}
```

**Statistics**:
```csharp
Debug.Log($"[STATS] Tree Analysis:");
Debug.Log($"  [#] Total Nodes: {allNodes.Count}");
Debug.Log($"  [#] Choices: {choiceCount}");
Debug.Log($"  [#] End Nodes: {endNodeCount}");
```

### ❌ Bad Examples (Don't Do This)

**Using Emojis**:
```csharp
Debug.Log($"✅ Tree valid");           // Displays: "? Tree valid"
Debug.LogWarning($"⚠️ Warning");      // Displays: "? Warning"
Debug.LogError($"❌ Error");          // Displays: "? Error"
```

**Using Unicode Arrows**:
```csharp
Debug.Log($"Node1 → Node2");           // Displays: "Node1 ? Node2"
Debug.Log($"⬆ Parent: {parent}");     // Displays: "? Parent: ..."
```

**Using Box-Drawing Characters**:
```csharp
Debug.Log("├─ Child node");            // Displays: "?? Child node"
Debug.Log("└─ Last item");             // Displays: "?? Last item"
```

---

## Implementation Guide

### Step 1: Audit Existing Code

Search your codebase for:
- `Debug.Log` statements with emojis
- Unicode characters (arrows, symbols, etc.)
- Box-drawing characters

**Tools**:
- Visual Studio: Search for regex pattern `[^\x00-\x7F]` (non-ASCII)
- Find all: `Ctrl+Shift+F` → Search for emoji patterns

### Step 2: Replace Emojis

Use Find & Replace with these patterns:

| Find | Replace |
|------|---------|
| `✅` | `[OK]` |
| `❌` | `[ERROR]` |
| `⚠️` | `[WARNING]` |
| `🔧` | `[FIXED]` |
| `📊` | `[STATS]` |
| `✨` | `[CREATED]` |

### Step 3: Test Console Output

1. Run your game in Unity Editor
2. Trigger debug logs
3. Check Console window for `?` characters
4. Fix any remaining non-ASCII characters

### Step 4: Add to Code Review

Check for ASCII-safe logging in all new code:
- ✅ Uses `[PREFIX]` format
- ✅ No emojis or Unicode symbols
- ✅ Clear, descriptive messages

---

## Advanced Patterns

### Hierarchical Logging

Use indentation for structure:
```csharp
Debug.Log("[DIALOG] Tree Structure:");
Debug.Log("  Node 1: Director intro");
Debug.Log("    Choice 1: Accept role");
Debug.Log("      -> Node 2: Rehearsal");
Debug.Log("    Choice 2: Decline");
Debug.Log("      -> Node 3: Goodbye");
```

### Multi-Line Logs

For complex data:
```csharp
Debug.Log($"[STATS] Tree Validation Results:\n" +
          $"  Total Nodes: {nodeCount}\n" +
          $"  Valid: {validCount}\n" +
          $"  Invalid: {invalidCount}\n" +
          $"  Convergent: {convergentCount}");
```

### Contextual Prefixes

Combine prefixes for clarity:
```csharp
Debug.Log("[DIALOG][START] Beginning conversation");
Debug.Log("[AUDIO][LOADING] Music track 'theme'");
Debug.LogError("[SAVE][ERROR] Failed to write file");
```

### Conditional Verbose Logging

Use compile-time or runtime flags:
```csharp
#if UNITY_EDITOR
    Debug.Log($"[DEBUG] Detailed node info: {node.GetDebugInfo()}");
#endif
```

Or:
```csharp
if (verboseLogging)
{
    Debug.Log($"[VERBOSE] Step-by-step processing");
}
```

---

## Common Mistakes

### Mistake 1: Copy-Pasting from Documentation

**Problem**: Docs may contain emojis for readability
```markdown
✅ Step 1: Create node
```

**Solution**: Don't copy emoji into code
```csharp
// ✅ Step 1: Create node  ← WRONG (emoji in comment)
// [OK] Step 1: Create node  ← RIGHT
```

### Mistake 2: Using "Smart" Quotes

**Problem**: Word processors convert quotes
```csharp
Debug.Log("✅ Success");  // "Smart quotes" may cause issues
```

**Solution**: Use straight quotes
```csharp
Debug.Log("[OK] Success");  // Straight quotes
```

### Mistake 3: Platform-Specific Characters

**Problem**: Works on Windows, breaks on Mac/Linux
```csharp
Debug.Log("File: C:\Users\...");  // Backslash issues
```

**Solution**: Use `Path.Combine()` or forward slashes
```csharp
Debug.Log($"File: {Path.Combine("Assets", "World", "tree.asset")}");
```

---

## Tools & Resources

### Find Non-ASCII Characters

**Regex Pattern** (Visual Studio):
```
[^\x00-\x7F]
```

**PowerShell Script**:
```powershell
# Find files with non-ASCII characters
Get-ChildItem -Path "Assets" -Recurse -Filter "*.cs" | 
    Where-Object { 
        (Get-Content $_.FullName -Raw) -match '[^\x00-\x7F]' 
    } | 
    Select-Object FullName
```

### Bulk Replace Script

```powershell
# Replace emojis in all .cs files
$files = Get-ChildItem -Path "Assets" -Recurse -Filter "*.cs"
foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $content = $content -replace '✅', '[OK]'
    $content = $content -replace '❌', '[ERROR]'
    $content = $content -replace '⚠️', '[WARNING]'
    Set-Content $file.FullName -Value $content -NoNewline
}
```

---

## Integration with CI/CD

### Pre-Commit Hook (Git)

Check for emojis before commit:
```bash
#!/bin/bash
# .git/hooks/pre-commit

# Check staged .cs files for non-ASCII
if git diff --cached --name-only --diff-filter=ACM | grep '\.cs$' | xargs grep -P '[^\x00-\x7F]' > /dev/null; then
    echo "[ERROR] Non-ASCII characters found in .cs files!"
    echo "[INFO] Please use ASCII-safe prefixes like [OK], [ERROR], [WARNING]"
    exit 1
fi
```

### GitHub Actions (CI)

Add to `.github/workflows/unity-build.yml`:
```yaml
- name: Check for Non-ASCII in Logs
  run: |
    if grep -r -P '[^\x00-\x7F]' Assets/**/*.cs | grep -i 'Debug\.Log'; then
      echo "[ERROR] Non-ASCII characters found in Debug.Log statements"
      exit 1
    fi
```

---

## Summary

### Quick Reference Card

```
✅ USE:
  [OK], [SUCCESS], [ERROR], [WARNING], [INFO]
  [START], [COMPLETE], [FIXED], [CREATED]
  [DIALOG], [STAGE], [AUDIO], [UI]
  [#], [STATS], [TIME]

❌ DON'T USE:
  ✅❌⚠️ (Emojis)
  📊🎭🔧 (Unicode symbols)
  →⬆⬇ (Directional arrows)
  "Smart" quotes
  Non-ASCII characters

✅ EXAMPLE:
  Debug.Log("[OK] Operation successful");
  Debug.LogWarning("[WARNING] Potential issue");
  Debug.LogError("[ERROR] Operation failed");
```

### Enforcement Checklist

- [ ] All Debug.Log statements use ASCII-safe prefixes
- [ ] No emojis in any C# files
- [ ] No Unicode symbols in debug messages
- [ ] Code review checks for ASCII-safe logging
- [ ] New developers trained on logging standards
- [ ] Pre-commit hooks prevent non-ASCII (optional)

---

## Related Documentation

- **Copilot Instructions**: `.github\copilot-instructions.md`
- **Troubleshooting**: `Docs\TROUBLESHOOTING.MD` → "Emojis Displaying as `?`"
- **Encoding Guide**: `Docs\UTF-8-Encoding-Setup-Guide.md`

---

**End of Unity Debug Logging Best Practices**

**Last Reviewed**: January 2025  
**Next Review**: When updating coding standards

---
