/* NodeIdGeneratorWindow.cs
 * Automated tool for generating Dialog Node IDs following project naming conventions.
 * Provides validation, auto-complete, and batch generation features.
 * 
 * Access via: Tools ? Dialog System ? Generate Node ID
 * 
 * Features:
 * - Real-time validation
 * - Auto-suggestions based on project patterns
 * - Copy to clipboard
 * - Batch mode for multiple sequential IDs
 * - Integration with DialogNode inspector
 */

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor window for generating standardized Dialog Node IDs
/// </summary>
public class NodeIdGeneratorWindow : EditorWindow
{
    // Input fields
    private string context = "act1";
    private string speaker = "director";
    private string action = "intro";
    private int sequence = 1;

    // Generated result
    private string generatedId = "";
    private bool isValid = true;
    private string validationMessage = "";

    // Suggestions (populated from existing trees)
    private string[] contextSuggestions = new string[0];
    private string[] speakerSuggestions = new string[0];
    private string[] actionSuggestions = new string[0];

    // UI state
    private int selectedContextIndex = 0;
    private int selectedSpeakerIndex = 0;
    private int selectedActionIndex = 0;
    private bool showBatchMode = false;
    private int batchCount = 5;
    private Vector2 scrollPosition;

    // Common presets
    private static readonly string[] ContextPresets = {
        "act1", "act2", "act3",
        "dream1", "dream2", "dream3",
        "npc", "tutorial", "perf", "climax", "side", "cond"
    };

    private static readonly string[] SpeakerPresets = {
        "player", "director", "stagemgr", "narrator", "audience"
    };

    private static readonly string[] ActionPresets = {
        "intro", "outro", "branch", "choice", "response", "convergence",
        "advice", "pep", "challenge", "question", "explain",
        "prep", "perform", "react", "success", "fail"
    };

    [MenuItem("Tools/Dialog System/Generate Node ID")]
    public static NodeIdGeneratorWindow ShowWindow()
    {
        var window = GetWindow<NodeIdGeneratorWindow>("Node ID Generator");
        window.minSize = new Vector2(450, 600);
        window.LoadSuggestions();
        window.Show();
        return window;
    }

    /// <summary>
    /// Open window with pre-filled context from a dialog tree
    /// </summary>
    public static void ShowWindowWithContext(string contextPrefix)
    {
        var window = ShowWindow();

        // Try to parse existing context
        var parts = contextPrefix.Split('_');
        if (parts.Length > 0)
        {
            window.context = parts[0];
            if (parts.Length > 1) window.speaker = parts[1];
            if (parts.Length > 2) window.action = parts[2];
        }

        window.RegenerateId();
    }

    private void LoadSuggestions()
    {
        // Find all DialogTree assets and extract unique patterns
        var allTrees = AssetDatabase.FindAssets("t:DialogTree")
            .Select(guid => AssetDatabase.LoadAssetAtPath<DialogTree>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(tree => tree != null)
            .ToList();

        HashSet<string> contexts = new HashSet<string>();
        HashSet<string> speakers = new HashSet<string>();
        HashSet<string> actions = new HashSet<string>();

        foreach (var tree in allTrees)
        {
            var nodes = tree.GetAllNodes();
            foreach (var node in nodes)
            {
                if (string.IsNullOrEmpty(node.NodeName)) continue;

                var parts = node.NodeName.Split('_');
                if (parts.Length >= 4)
                {
                    contexts.Add(parts[0]);
                    speakers.Add(parts[1]);
                    actions.Add(parts[2]);
                }
            }
        }

        // Combine with presets
        contextSuggestions = ContextPresets.Union(contexts).OrderBy(x => x).ToArray();
        speakerSuggestions = SpeakerPresets.Union(speakers).OrderBy(x => x).ToArray();
        actionSuggestions = ActionPresets.Union(actions).OrderBy(x => x).ToArray();
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawHeader();
        EditorGUILayout.Space(10);

        DrawInputFields();
        EditorGUILayout.Space(10);

        DrawGeneratedResult();
        EditorGUILayout.Space(10);

        DrawQuickActions();
        EditorGUILayout.Space(10);

        DrawBatchMode();
        EditorGUILayout.Space(10);

        DrawHelp();

        EditorGUILayout.EndScrollView();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };

        EditorGUILayout.LabelField("Dialog Node ID Generator", headerStyle);
        EditorGUILayout.LabelField("Format: [context]_[speaker]_[action]_[sequence]", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndVertical();
    }

    private void DrawInputFields()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Node ID Components", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Context field with dropdown
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Context", GUILayout.Width(80));

        EditorGUI.BeginChangeCheck();
        context = EditorGUILayout.TextField(context);

        if (contextSuggestions.Length > 0)
        {
            selectedContextIndex = EditorGUILayout.Popup(selectedContextIndex, contextSuggestions, GUILayout.Width(100));
            if (GUI.changed)
            {
                context = contextSuggestions[selectedContextIndex];
                EditorGUI.EndChangeCheck();
                RegenerateId();
            }
        }

        if (EditorGUI.EndChangeCheck())
            RegenerateId();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.HelpBox("Where/when: act1, dream1, npc, tutorial, perf", MessageType.None);

        // Speaker field with dropdown
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Speaker", GUILayout.Width(80));

        EditorGUI.BeginChangeCheck();
        speaker = EditorGUILayout.TextField(speaker);

        if (speakerSuggestions.Length > 0)
        {
            selectedSpeakerIndex = EditorGUILayout.Popup(selectedSpeakerIndex, speakerSuggestions, GUILayout.Width(100));
            if (GUI.changed)
            {
                speaker = speakerSuggestions[selectedSpeakerIndex];
                EditorGUI.EndChangeCheck();
                RegenerateId();
            }
        }

        if (EditorGUI.EndChangeCheck())
            RegenerateId();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.HelpBox("Who's speaking: player, director, narrator", MessageType.None);

        // Action field with dropdown
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Action", GUILayout.Width(80));

        EditorGUI.BeginChangeCheck();
        action = EditorGUILayout.TextField(action);

        if (actionSuggestions.Length > 0)
        {
            selectedActionIndex = EditorGUILayout.Popup(selectedActionIndex, actionSuggestions, GUILayout.Width(100));
            if (GUI.changed)
            {
                action = actionSuggestions[selectedActionIndex];
                EditorGUI.EndChangeCheck();
                RegenerateId();
            }
        }

        if (EditorGUI.EndChangeCheck())
            RegenerateId();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.HelpBox("What's happening: intro, branch, choice, convergence", MessageType.None);

        // Sequence number
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Sequence", GUILayout.Width(80));

        EditorGUI.BeginChangeCheck();
        sequence = EditorGUILayout.IntField(sequence);
        sequence = Mathf.Clamp(sequence, 1, 99);

        if (EditorGUI.EndChangeCheck())
            RegenerateId();

        if (GUILayout.Button("?", GUILayout.Width(30)))
        {
            sequence++;
            RegenerateId();
        }

        if (GUILayout.Button("?", GUILayout.Width(30)))
        {
            sequence = Mathf.Max(1, sequence - 1);
            RegenerateId();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.HelpBox("Two-digit number: 01-99", MessageType.None);

        EditorGUILayout.EndVertical();
    }

    private void DrawGeneratedResult()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Generated Node ID", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Display generated ID with visual feedback
        GUIStyle resultStyle = new GUIStyle(EditorStyles.textField)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter
        };

        if (isValid)
        {
            resultStyle.normal.textColor = Color.green;
        }
        else
        {
            resultStyle.normal.textColor = Color.red;
        }

        EditorGUILayout.SelectableLabel(generatedId, resultStyle, GUILayout.Height(30));

        // Validation message
        if (!string.IsNullOrEmpty(validationMessage))
        {
            MessageType msgType = isValid ? MessageType.Info : MessageType.Warning;
            EditorGUILayout.HelpBox(validationMessage, msgType);
        }

        // Copy button
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("?? Copy to Clipboard", GUILayout.Height(35)))
        {
            GUIUtility.systemCopyBuffer = generatedId;
            EditorUtility.DisplayDialog("Copied!", $"Node ID copied to clipboard:\n{generatedId}", "OK");
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.EndVertical();
    }

    private void DrawQuickActions()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Reset", GUILayout.Height(30)))
        {
            context = "act1";
            speaker = "director";
            action = "intro";
            sequence = 1;
            RegenerateId();
        }

        if (GUILayout.Button("Load from Selection", GUILayout.Height(30)))
        {
            LoadFromSelectedNode();
        }

        if (GUILayout.Button("Suggest Next", GUILayout.Height(30)))
        {
            SuggestNextSequence();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private void DrawBatchMode()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        showBatchMode = EditorGUILayout.Foldout(showBatchMode, "Batch Mode (Generate Multiple IDs)", true, EditorStyles.foldoutHeader);

        if (showBatchMode)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Generate multiple sequential Node IDs at once.", MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Generate Count", GUILayout.Width(120));
            batchCount = EditorGUILayout.IntSlider(batchCount, 2, 20);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button($"Generate {batchCount} Sequential IDs", GUILayout.Height(35)))
            {
                GenerateBatchIds();
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawHelp()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("?? Quick Reference", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Common Contexts:", EditorStyles.miniBoldLabel);
        EditorGUILayout.LabelField("• act1-3: Story acts");
        EditorGUILayout.LabelField("• dream1-3: Specific levels");
        EditorGUILayout.LabelField("• npc: NPC conversations");
        EditorGUILayout.LabelField("• tutorial: Tutorial sequences");
        EditorGUILayout.LabelField("• perf: Performance/minigames");

        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField("Special Actions:", EditorStyles.miniBoldLabel);
        EditorGUILayout.LabelField("• branch: Choice point");
        EditorGUILayout.LabelField("• choice: Choice outcome");
        EditorGUILayout.LabelField("• convergence: Paths merge");

        EditorGUILayout.Space(5);

        if (GUILayout.Button("?? Open Full Documentation"))
        {
            var docPath = System.IO.Path.Combine(Application.dataPath, "../Docs/DialogNodeID-NamingConvention.md");
            if (System.IO.File.Exists(docPath))
            {
                System.Diagnostics.Process.Start(docPath);
            }
            else
            {
                EditorUtility.DisplayDialog("Documentation",
                    "Full documentation available at:\nDocs/DialogNodeID-NamingConvention.md", "OK");
            }
        }

        EditorGUILayout.EndVertical();
    }

    private void RegenerateId()
    {
        // Clean inputs
        context = CleanInput(context);
        speaker = CleanInput(speaker);
        action = CleanInput(action);

        // Generate ID
        generatedId = $"{context}_{speaker}_{action}_{sequence:D2}";

        // Validate
        ValidateId();
    }

    private string CleanInput(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;

        // Convert to lowercase
        input = input.ToLower();

        // Remove invalid characters
        input = Regex.Replace(input, @"[^a-z0-9]", "");

        // Limit length
        if (input.Length > 12)
            input = input.Substring(0, 12);

        return input;
    }

    private void ValidateId()
    {
        isValid = true;
        validationMessage = "";

        // Check empty fields
        if (string.IsNullOrEmpty(context) || string.IsNullOrEmpty(speaker) || string.IsNullOrEmpty(action))
        {
            isValid = false;
            validationMessage = "?? All fields must be filled";
            return;
        }

        // Check length
        if (generatedId.Length > 40)
        {
            isValid = false;
            validationMessage = $"?? ID too long ({generatedId.Length}/40 characters). Use shorter components.";
            return;
        }

        // Check for duplicates in existing trees
        var allTrees = AssetDatabase.FindAssets("t:DialogTree")
            .Select(guid => AssetDatabase.LoadAssetAtPath<DialogTree>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(tree => tree != null);

        foreach (var tree in allTrees)
        {
            if (tree.FindNodeByName(generatedId) != null)
            {
                isValid = false;
                validationMessage = $"?? Duplicate! ID already exists in tree: {tree.treeName}";
                return;
            }
        }

        // All good!
        isValid = true;
        validationMessage = $"? Valid ID ({generatedId.Length} characters)";
    }

    private void LoadFromSelectedNode()
    {
        var selected = Selection.activeObject as DialogTree;
        if (selected == null)
        {
            EditorUtility.DisplayDialog("No Selection",
                "Please select a DialogTree asset first.", "OK");
            return;
        }

        // Find highest sequence number with matching pattern
        var nodes = selected.GetAllNodes();
        int maxSeq = 0;

        foreach (var node in nodes)
        {
            if (string.IsNullOrEmpty(node.NodeName)) continue;

            var parts = node.NodeName.Split('_');
            if (parts.Length >= 4)
            {
                // Check if first 3 parts match
                if (parts[0] == context && parts[1] == speaker && parts[2] == action)
                {
                    if (int.TryParse(parts[3], out int seq))
                    {
                        maxSeq = Mathf.Max(maxSeq, seq);
                    }
                }
            }
        }

        if (maxSeq > 0)
        {
            sequence = maxSeq + 1;
            EditorUtility.DisplayDialog("Loaded",
                $"Found {maxSeq} existing nodes with this pattern.\nNext sequence: {sequence:D2}", "OK");
        }
        else
        {
            sequence = 1;
            EditorUtility.DisplayDialog("No Match",
                "No matching nodes found. Starting from 01.", "OK");
        }

        RegenerateId();
    }

    private void SuggestNextSequence()
    {
        var allTrees = AssetDatabase.FindAssets("t:DialogTree")
            .Select(guid => AssetDatabase.LoadAssetAtPath<DialogTree>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(tree => tree != null);

        int maxSeq = 0;
        int matchCount = 0;

        foreach (var tree in allTrees)
        {
            var nodes = tree.GetAllNodes();
            foreach (var node in nodes)
            {
                if (string.IsNullOrEmpty(node.NodeName)) continue;

                var parts = node.NodeName.Split('_');
                if (parts.Length >= 4 && parts[0] == context && parts[1] == speaker && parts[2] == action)
                {
                    if (int.TryParse(parts[3], out int seq))
                    {
                        maxSeq = Mathf.Max(maxSeq, seq);
                        matchCount++;
                    }
                }
            }
        }

        if (maxSeq > 0)
        {
            sequence = maxSeq + 1;
            RegenerateId();
            EditorUtility.DisplayDialog("Suggested",
                $"Found {matchCount} existing nodes with this pattern.\nSuggested next: {sequence:D2}", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("No Match",
                "No existing nodes with this pattern found.", "OK");
        }
    }

    private void GenerateBatchIds()
    {
        string result = "Generated Node IDs:\n\n";

        for (int i = 0; i < batchCount; i++)
        {
            string batchId = $"{context}_{speaker}_{action}_{(sequence + i):D2}";
            result += $"{batchId}\n";
        }

        GUIUtility.systemCopyBuffer = result;

        EditorUtility.DisplayDialog("Batch Generated",
            $"{batchCount} Node IDs generated and copied to clipboard!\n\n" +
            $"First: {context}_{speaker}_{action}_{sequence:D2}\n" +
            $"Last: {context}_{speaker}_{action}_{(sequence + batchCount - 1):D2}", "OK");
    }
}
