# Documentation Visibility in Visual Studio

This document explains how your documentation files are automatically added to the Visual Studio solution.

## 📋 Overview

The `SolutionFileModifier.cs` script automatically adds important documentation files to your Visual Studio solution every time Unity regenerates the project files.

## 📁 Files Automatically Included

### Core Documentation
- `Docs/Class Hierarchy.md` - Complete project architecture
- `Docs/Project_Roadmap.md` - Project milestones and planning

### Troubleshooting
- `Docs/Troubleshooting/README.md` - Troubleshooting index
- `Docs/Troubleshooting/Dialog-System.md` - Dialog system issues
- `Docs/Troubleshooting/Git-VersionControl.md` - Git issues
- `Docs/Troubleshooting/UI-Toolkit.md` - UI Toolkit issues
- `Docs/Troubleshooting/Unity-Editor.md` - Unity Editor issues

### System Guides
- `Docs/Dialog-System-Complete-Guide.md`
- `Docs/Dialog-System-Quick-Reference.md`
- `Docs/Dialog-System-Technical-Reference.md`
- `Docs/GameState-Implementation-Summary.md`
- `Docs/RememberTheScript-QuickStart.md`

### Git & Project Files
- `.gitignore`
- `.gitattributes`
- `.github/copilot-instructions.md`
- `README.md`

## 🔧 How It Works

1. **Automatic**: Runs every time Unity regenerates solution files
2. **Non-intrusive**: Doesn't modify Unity project structure
3. **Version Control Safe**: Only modifies `.slnx` file (which is in `.gitignore`)

## ➕ Adding More Files

To add more documentation files to the solution:

1. Open `Assets\_Stage of Dreams_\Editor\SolutionFileModifier.cs`
2. Find the `documentationFiles` array
3. Add your file path (relative to project root)
4. Save and let Unity regenerate the solution (or use `Assets > Open C# Project`)

Example:
```csharp
string[] documentationFiles = new string[]
{
    // ... existing files ...
    @"Docs\MyNewDocument.md",  // Add your new file here
};
```

## 🔄 Force Regeneration

If you don't see your files in Visual Studio:

1. **Close Visual Studio**
2. In Unity: `Assets > Open C# Project`
3. This forces Unity to regenerate solution files with your documentation

## 📂 Folder Organization in Solution Explorer

Files appear organized in the Solution Explorer under:
```
📚 Documentation/
  ├── 📖 Guides/
  ├── ⚠️ Troubleshooting/
  ├── 🤖 GitHub Config/
  └── Root files (README, .gitignore, etc.)
```

## ⚙️ Troubleshooting

### Files not appearing?
- Close Visual Studio
- Delete `Stage of Dreams.slnx` 
- Reopen from Unity (`Assets > Open C# Project`)

### Wrong files showing?
- Edit `SolutionFileModifier.cs` 
- Remove unwanted paths from `documentationFiles` array
- Regenerate solution

### Script not running?
- Check Unity Console for errors
- Script must be in an `Editor` folder
- Script must have `[InitializeOnLoad]` attribute

## 🎓 Learn More

This uses Unity's `AssetPostprocessor.OnGeneratedSlnSolution` callback to customize the generated solution file.

**Unity Documentation**: 
https://docs.unity3d.com/ScriptReference/AssetPostprocessor.OnGeneratedSlnSolution.html
