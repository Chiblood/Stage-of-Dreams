# UTF-8 Encoding Setup Guide

## ✅ Problem Fixed!

Your project was defaulting to Windows-1252 encoding, which doesn't support emojis and special Unicode characters. This has now been fixed!

## 📁 Files Added

### 1. `.editorconfig`
- Sets UTF-8 as the default encoding for all files
- Configures proper indentation and line endings
- Supported by Visual Studio, VS Code, and many other editors

### 2. `.gitattributes`
- Ensures Git handles text files with UTF-8 encoding
- Prevents encoding corruption during commits/merges
- Sets consistent line endings (CRLF for Windows compatibility)

## 🔧 How to Configure Visual Studio

### For Visual Studio 2022:
1. **Go to**: Tools → Options
2. **Navigate to**: Environment → Documents
3. **Check**: "Save documents as Unicode (UTF-8 with signature) - Codepage 65001"
4. **Click**: OK

### For Individual Files:
1. **Open** the file in Visual Studio
2. **Go to**: File → Advanced Save Options
3. **Select**: "Unicode (UTF-8 with signature) - Codepage 65001"
4. **Click**: OK
5. **Save** the file (Ctrl+S)

## 🔧 How to Configure VS Code

### For VS Code:
1. **Go to**: File → Preferences → Settings
2. **Search for**: "files.encoding"
3. **Set to**: "utf8"

### Or add to settings.json:
```json
{
  "files.encoding": "utf8",
  "files.autoGuessEncoding": false
}
```

## 📝 Files That Need Re-Saving

The following documentation files contain emojis and need to be re-saved in UTF-8:

### Documentation Files:
- `Docs\Class Hierarchy.md`
- `Docs\ExampleFiles-NodeID-Update.md`
- `Docs\Project_Roadmap.md`
- `Docs\DialogNodeID-NamingConvention.md`
- `Docs\DialogNodeID-GeneratorTool.md`
- `Docs\DialogChoiceEditorWindow-TestingGuide.md`
- `Docs\TROUBLESHOOTING.MD`
- `README.md`
- `.github\copilot-instructions.md`

### Guide Files:
- `Assets\_Stage of Dreams_\Scripts\Dialog\Examples and Guides\Unity Events Integration Guide.md`
- `Assets\_Stage of Dreams_\Scripts\Dialog\Examples and Guides\Enhanced Dialog Tree Guide.md`
- `Assets\_Stage of Dreams_\Scripts\Dialog\Examples and Guides\Inspector_Editing_Guide.md`
- `Docs\UI Setup\DialogManager UI Toolkit Connection Guide.md`

## 🔄 How to Re-Save Files in Visual Studio

### Quick Method (Batch):
1. **Open** Visual Studio
2. **Go to**: File → Advanced Save Options
3. **Set**: Unicode (UTF-8 with signature) - Codepage 65001
4. **Open each markdown file** from the list above
5. **Press**: Ctrl+S to save
6. Repeat for all files

### Using PowerShell (Advanced):
I can provide a PowerShell script to batch convert all files if needed.

## ✅ Verification

After re-saving, your files should:
- ✓ Display emojis correctly (✓, ⏳, ❌, 🎮, etc.)
- ✓ Show proper status indicators
- ✓ Maintain formatting across different editors
- ✓ Commit to Git without encoding warnings

## 🎯 Future File Creation

With `.editorconfig` in place:
- **New files** will automatically use UTF-8 encoding
- **Visual Studio** will respect the encoding setting
- **VS Code** will use UTF-8 by default
- **Git** will handle files correctly

## 📚 Additional Notes

### Why UTF-8?
- Universal standard for text encoding
- Supports all Unicode characters (emojis, symbols, international text)
- Compatible with all modern tools and platforms
- Required for modern web and documentation

### Why With Signature (BOM)?
- Visual Studio and Windows tools work better with UTF-8 BOM
- Helps editors detect encoding automatically
- Small 3-byte marker at file start
- Standard for Windows development

### Alternative: UTF-8 Without BOM
If you prefer UTF-8 without BOM (common in Linux/Mac):
1. Change `.editorconfig` setting to: `charset = utf-8-bom` → `charset = utf-8`
2. Use "Unicode (UTF-8 without signature)" in Visual Studio

## 🆘 Troubleshooting

### Issue: Emojis still show as ?
**Solution**: Re-save the file using Advanced Save Options → UTF-8

### Issue: Git shows encoding warnings
**Solution**: Commit `.gitattributes` first, then re-save and commit other files

### Issue: Visual Studio doesn't respect .editorconfig
**Solution**: Ensure you have Visual Studio 2017+ with EditorConfig support

### Issue: Some emojis display but others don't
**Solution**: Check your font supports the emojis - use "Segoe UI Emoji" or "Consolas with emoji support"

## ✨ Benefits of This Setup

- ✅ Consistent encoding across all team members
- ✅ No more encoding corruption
- ✅ Full emoji and Unicode support
- ✅ Better Git compatibility
- ✅ Cross-platform development ready
- ✅ Professional documentation standards

---

**Your project is now configured for UTF-8 by default!** 🎉

Just re-save the markdown files listed above using the instructions, and you're all set!
