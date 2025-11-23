# Convert Markdown Files to UTF-8 Encoding
# This script converts all markdown files in the project to UTF-8 with BOM

param(
    [switch]$WhatIf,
    [switch]$NoBOM
)

Write-Host "Stage of Dreams - UTF-8 Encoding Converter" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# Determine encoding type
$encoding = if ($NoBOM) { 
    New-Object System.Text.UTF8Encoding $false
    Write-Host "Using: UTF-8 without BOM" -ForegroundColor Yellow
} else { 
    New-Object System.Text.UTF8Encoding $true
    Write-Host "Using: UTF-8 with BOM (recommended for Windows)" -ForegroundColor Green
}

Write-Host ""

# List of files to convert
$filesToConvert = @(
    "Docs\Class Hierarchy.md",
    "Docs\ExampleFiles-NodeID-Update.md",
    "Docs\Project_Roadmap.md",
    "Docs\DialogNodeID-NamingConvention.md",
    "Docs\DialogNodeID-GeneratorTool.md",
    "Docs\DialogChoiceEditorWindow-TestingGuide.md",
    "Docs\TROUBLESHOOTING.MD",
    "Docs\Requirements.md",
    "README.md",
    ".github\copilot-instructions.md",
    "Assets\_Stage of Dreams_\Scripts\Dialog\Examples and Guides\Unity Events Integration Guide.md",
    "Assets\_Stage of Dreams_\Scripts\Dialog\Examples and Guides\Enhanced Dialog Tree Guide.md",
    "Assets\_Stage of Dreams_\Scripts\Dialog\Examples and Guides\Inspector_Editing_Guide.md",
    "Docs\UI Setup\DialogManager UI Toolkit Connection Guide.md"
)

$successCount = 0
$failCount = 0
$skippedCount = 0

foreach ($file in $filesToConvert) {
    $fullPath = Join-Path $PSScriptRoot $file
    
    if (-not (Test-Path $fullPath)) {
        Write-Host "SKIPPED: $file (file not found)" -ForegroundColor Yellow
        $skippedCount++
        continue
    }
    
    try {
        if ($WhatIf) {
            Write-Host "WOULD CONVERT: $file" -ForegroundColor Cyan
            $successCount++
        } else {
            # Read file with automatic encoding detection
            $content = Get-Content -Path $fullPath -Raw -Encoding Default
            
            # Write file with UTF-8 encoding
            [System.IO.File]::WriteAllText($fullPath, $content, $encoding)
            
            Write-Host "CONVERTED: $file" -ForegroundColor Green
            $successCount++
        }
    } catch {
        Write-Host "FAILED: $file" -ForegroundColor Red
        Write-Host "  Error: $_" -ForegroundColor Red
        $failCount++
    }
}

Write-Host ""
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host "Conversion Complete!" -ForegroundColor Cyan
Write-Host ""
Write-Host "Successful: $successCount" -ForegroundColor Green
if ($failCount -gt 0) {
    Write-Host "Failed: $failCount" -ForegroundColor Red
}
if ($skippedCount -gt 0) {
    Write-Host "Skipped: $skippedCount" -ForegroundColor Yellow
}
Write-Host ""

if ($WhatIf) {
    Write-Host "This was a dry run. Use without -WhatIf to actually convert files." -ForegroundColor Yellow
} else {
    Write-Host "All files have been converted to UTF-8!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Cyan
    Write-Host "1. Review the changes in your files" -ForegroundColor White
    Write-Host "2. Verify emojis display correctly" -ForegroundColor White
    Write-Host "3. Commit the changes to Git" -ForegroundColor White
}

Write-Host ""
