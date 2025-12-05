# Quick Recovery Commands

## ✅ Current Status (As of 2025-12-01 3:27 PM)

- **Duplicate files:** ✅ Removed (stashed)
- **Orphaned meta files:** ✅ Deleted
- **Docs folder:** ✅ Intact (28 files safe)
- **Stash:** Contains your work-in-progress changes
- **Branch:** Feature/Minigames (diverged from origin)

---

## 🚀 Quick Recovery (Option A - RECOMMENDED)

Close Unity and Visual Studio, then run:

```bash
# Sync with origin
git pull origin Feature/Minigames --rebase

# Apply your stashed changes
git stash pop

# If you have conflicts, resolve them, then:
git add .
git commit -m "Resolve OneDrive conflict merge"
git push origin Feature/Minigames

# Cleanup the OneDrive conflict branch
git branch -D Feature/Minigames-JacksDesktop
git push origin --delete Feature/Minigames-JacksDesktop
```

---

## 📊 Before You Start - Check What You'll Lose/Gain

### See your local changes:
```bash
git show HEAD --stat
git stash show stash@{0} --stat
```

### See what's on origin (4 commits):
```bash
git fetch origin
git log HEAD..origin/Feature/Minigames --oneline --stat
```

---

## 🆘 Emergency Revert

If anything goes wrong:

```bash
# Go back to your current state
git reset --hard a5b869e
git stash list  # Your changes are still safe here
```

---

## 🔍 Visual Studio - Show Docs Folder

Your Docs folder is fine! To see it in Visual Studio:

1. **Option 1:** Solution Explorer → Click "Show All Files" button
2. **Option 2:** Right-click solution → Add → Existing Folder → Select "Docs"
3. **Option 3:** Use File Explorer to access the files directly

---

## ⚙️ OneDrive Prevention

To prevent future conflicts:

1. **Pause OneDrive during Git operations:**
   - Right-click OneDrive tray icon
   - Click "Pause syncing" → Choose duration

2. **Exclude Git folders from OneDrive:**
   - Add `.git` folder to OneDrive's excluded folders list

3. **Use .gitignore properly:**
   - Your `.gitignore` should exclude Unity's temp files
   - OneDrive should not sync the Library, Temp, or obj folders

---

## 📞 Need Help?

If you're unsure which option to choose:

1. Check what changed: `git diff a5b869e origin/Feature/Minigames`
2. Review commit messages: `git log origin/Feature/Minigames --oneline -5`
3. Ask: "Do I want the 4 commits from origin, or should I keep my single local commit?"

---

## ✨ After Recovery

Once complete:

```bash
# Verify clean state
git status

# Check branch is in sync
git log --oneline --graph origin/Feature/Minigames..HEAD

# Should show nothing if in sync
```

Then:
1. Open Unity → Let it reimport
2. Open Visual Studio → Rebuild solution
3. Verify no errors in Editor folder
