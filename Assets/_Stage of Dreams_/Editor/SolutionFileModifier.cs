using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Automatically adds documentation and project files to the Visual Studio solution.
/// This ensures important project files are always visible in Solution Explorer.
/// </summary>
[InitializeOnLoad]
public class SolutionFileModifier : AssetPostprocessor
{
    /// <summary>
    /// Called after Unity generates solution files.
    /// Customizes the solution to include documentation files.
    /// </summary>
    private static string OnGeneratedSlnSolution(string path, string content)
    {
        // Get the project root directory
        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        
        // Define documentation files to include
        string[] documentationFiles = new string[]
        {
            // Core documentation
            @"Docs\Class Hierarchy.md",
            @"Docs\Project_Roadmap.md",
            
            // Troubleshooting documentation
            @"Docs\Troubleshooting\README.md",
            @"Docs\Troubleshooting\Dialog-System.md",
            @"Docs\Troubleshooting\Git-VersionControl.md",
            @"Docs\Troubleshooting\UI-Toolkit.md",
            @"Docs\Troubleshooting\Unity-Editor.md",
            
            // System documentation
            @"Docs\Dialog-System-Complete-Guide.md",
            @"Docs\Dialog-System-Quick-Reference.md",
            @"Docs\Dialog-System-Technical-Reference.md",
            @"Docs\GameState-Implementation-Summary.md",
            @"Docs\RememberTheScript-QuickStart.md",
            
            // Git and project files
            @".gitignore",
            @".gitattributes",
            @".github\copilot-instructions.md",
            @"README.md"
        };
        
        // Create solution folder structure in XML format
        string solutionItemsSection = BuildSolutionItemsSection(documentationFiles, projectRoot);
        
        // Insert solution items before the closing </Solution> tag
        if (content.Contains("</Solution>"))
        {
            content = content.Replace("</Solution>", solutionItemsSection + "\n</Solution>");
        }
        
        return content;
    }
    
    /// <summary>
    /// Builds the solution items folder structure in XML format.
    /// </summary>
    private static string BuildSolutionItemsSection(string[] files, string projectRoot)
    {
        var validFiles = files.Where(f => File.Exists(Path.Combine(projectRoot, f))).ToList();
        
        if (validFiles.Count == 0)
        {
            return string.Empty;
        }
        
        // Group files by folder
        var grouped = validFiles
            .GroupBy(f => Path.GetDirectoryName(f))
            .OrderBy(g => g.Key);
        
        string result = "\n  <!-- Documentation and Project Files -->\n";
        result += "  <Folder Name=\"📚 Documentation\">\n";
        
        foreach (var group in grouped)
        {
            string folderName = string.IsNullOrEmpty(group.Key) ? "Root" : group.Key.Replace('\\', '/');
            
            if (group.Key.Contains("Troubleshooting"))
            {
                result += "    <Folder Name=\"⚠️ Troubleshooting\">\n";
                foreach (var file in group)
                {
                    result += $"      <File Path=\"{file.Replace('\\', '/')}\" />\n";
                }
                result += "    </Folder>\n";
            }
            else if (group.Key.Contains("Docs"))
            {
                result += "    <Folder Name=\"📖 Guides\">\n";
                foreach (var file in group)
                {
                    result += $"      <File Path=\"{file.Replace('\\', '/')}\" />\n";
                }
                result += "    </Folder>\n";
            }
            else if (group.Key.Contains(".github"))
            {
                result += "    <Folder Name=\"🤖 GitHub Config\">\n";
                foreach (var file in group)
                {
                    result += $"      <File Path=\"{file.Replace('\\', '/')}\" />\n";
                }
                result += "    </Folder>\n";
            }
            else
            {
                // Root files
                foreach (var file in group)
                {
                    result += $"    <File Path=\"{file.Replace('\\', '/')}\" />\n";
                }
            }
        }
        
        result += "  </Folder>";
        
        return result;
    }
}
