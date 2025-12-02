# Git & Version Control Troubleshooting

**Last Updated**: December 2025  
**Git LFS**: Enabled  
**Current Branch**: Feature/Minigames

---

## Submodule Issues

### Error: "No submodule mapping found in .gitmodules"

**Cause**: Git is tracking a directory as a submodule when it shouldn't be.

**Solution**:
```bash
# 1. Remove the submodule cache
git rm --cached path/to/directory

# 2. Remove .git file inside the directory
rm path/to/directory/.git

# 3. Add as normal directory
git add path/to/directory

# 4. Commit
git commit -m "Convert submodule to normal directory"
```

---

## Merge Conflicts

### Unity Scene/Prefab Conflicts

**Don't manually edit these files!**

**Solution**:
```bash
# Option 1: Accept theirs (remote version)
git checkout --theirs path/to/file.unity
git add path/to/file.unity

# Option 2: Accept ours (local version)
git checkout --ours path/to/file.unity
git add path/to/file.unity

# Option 3: Use Unity's Smart Merge (recommended)
# Unity → Edit → Project Settings → Editor → Version Control → Smart Merge
```

### Meta File Conflicts

**Always keep meta files in sync with their assets!**

**Solution**:
```bash
# If you kept the asset, keep its meta file too
git checkout --ours Assets/MyAsset.meta

# If you deleted the asset, delete its meta file
git rm Assets/MyAsset.meta
```

---

## Large File Issues (LFS)

### Git LFS Not Tracking Files

**Check tracking**:
```bash
git lfs track
```

**Add file type to LFS**:
```bash
git lfs track "*.psd"
git lfs track "*.fbx"
git add .gitattributes
git commit -m "Track PSD and FBX with LFS"
```

### LFS Files Not Downloading

```bash
# Fetch all LFS files
git lfs fetch --all

# Pull LFS files
git lfs pull
```

---

## Branch Issues

### Switching Branches with Uncommitted Changes

**Option 1: Commit changes**
```bash
git add .
git commit -m "WIP: Description"
git checkout other-branch
```

**Option 2: Stash changes**
```bash
git stash save "WIP: Description"
git checkout other-branch

# Later, restore:
git stash pop
```

**Option 3: Create temporary branch**
```bash
git checkout -b temp-branch
git add .
git commit -m "Temp work"
git checkout main
```

---

## Common Mistakes

### Committed Unity Library Files

**These should NEVER be committed**:
- `Library/`
- `Temp/`
- `Obj/`
- `Build/`

**Fix**:
```bash
# Remove from Git but keep locally
git rm -r --cached Library/
git rm -r --cached Temp/
git rm -r --cached Obj/

# Add to .gitignore if not already there
echo "Library/" >> .gitignore
echo "Temp/" >> .gitignore
echo "Obj/" >> .gitignore

git add .gitignore
git commit -m "Remove Unity generated files from Git"
```

---

## Sync Issues

### Remote Rejected Push

**Error**: `Updates were rejected because the remote contains work that you do not have locally`

**Solution**:
```bash
# Pull and merge
git pull origin Feature/Minigames

# Resolve conflicts if any
# Then push
git push origin Feature/Minigames
```

### Diverged Branches

```bash
# See divergence
git status

# Option 1: Rebase (cleaner history)
git pull --rebase origin Feature/Minigames

# Option 2: Merge (preserves history)
git pull origin Feature/Minigames
```

---

## Best Practices

### Before Committing

✓ Test in Unity  
✓ Check for errors in Console  
✓ Run build if changing code  
✓ Review changes: `git status` and `git diff`  
✓ Write descriptive commit message

### Commit Message Format

```
Type: Short description (50 chars or less)

Longer explanation if needed (wrap at 72 chars)
- Bullet points for details
- What changed and why
```

**Types**: `Feature`, `Fix`, `Refactor`, `Docs`, `Test`, `Chore`

### Before Pulling

✓ Commit or stash local changes  
✓ Check current branch: `git branch`  
✓ Pull with `--rebase` for cleaner history

---

## Quick Reference

```bash
# Check status
git status

# View commit history
git log --oneline --graph --decorate --all

# Undo last commit (keep changes)
git reset --soft HEAD~1

# Discard local changes
git checkout -- path/to/file

# Force pull (DANGEROUS - loses local changes)
git fetch origin
git reset --hard origin/Feature/Minigames
