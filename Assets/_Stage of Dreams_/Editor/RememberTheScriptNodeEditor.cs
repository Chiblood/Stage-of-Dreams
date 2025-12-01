/* RememberTheScriptNodeEditor.cs
 * Dedicated editor window for editing RememberTheScript minigame nodes.
 * Provides focused editing environment for typing minigame configuration.
 * 
 * How to use:
 * 1. Window opens automatically when editing a RememberTheScript node
 * 2. Configure minigame settings (target phrase, difficulty, scoring)
 * 3. Set success/failure navigation
 * 4. Test minigame directly from editor
 * 5. Multiple windows can be open simultaneously
 */

using UnityEngine;
using UnityEditor;

/// <summary>
/// Dedicated editor window for RememberTheScript minigame nodes
/// </summary>
public class RememberTheScriptNodeEditor : EditorWindow
{
    // Core references
    private SerializedObject serializedObject;
    private SerializedProperty nodeProperty;
    private DialogNode currentNode;
    private DialogTree parentTree;
    
    // UI State
    private Vector2 scrollPosition;
    private bool showTargetPhrasePreview = true;
    private bool showDifficultySettings = true;
    private bool showScoreSettings = true;
    private bool showNavigationSettings = true;
    
    // Cached properties - Core Dialog
    private SerializedProperty nodeTypeProp;
    private SerializedProperty nodeIdProp;
    private SerializedProperty characterNameProp;
    private SerializedProperty dialogTextProp;
    private SerializedProperty isPlayerSpeakingProp;
    
    // Cached properties - RememberTheScript
    private SerializedProperty isRememberScriptNodeProp;
    private SerializedProperty targetPhraseProp;
    private SerializedProperty maxMistakesProp;
    private SerializedProperty timeLimitProp;
    private SerializedProperty scorePerMistakeProp;
    private SerializedProperty scoreOnSuccessProp;
    private SerializedProperty caseSensitiveProp;
    private SerializedProperty failureNodeProp;  // NEW: Direct reference
    private SerializedProperty failureNodeNameProp;  // LEGACY
    
    // Cached properties - Tree Structure
    private SerializedProperty parentNodesProp;
    private SerializedProperty childNodeProp;
    
    /// <summary>
    /// Open window for a specific RememberTheScript node
    /// </summary>
    public static void OpenWindow(SerializedProperty nodeProperty, DialogTree tree)
    {
        RememberTheScriptNodeEditor window = CreateInstance<RememberTheScriptNodeEditor>();
        window.minSize = new Vector2(550, 750);
        window.titleContent = new GUIContent("RememberTheScript Minigame Editor");
        window.Initialize(nodeProperty, tree);
        window.Show();
    }
    
    private void Initialize(SerializedProperty property, DialogTree tree)
    {
        nodeProperty = property;
        parentTree = tree;
        serializedObject = property.serializedObject;
        
        // Try to get the actual node object
        if (property.propertyType == SerializedPropertyType.ManagedReference)
        {
            currentNode = property.managedReferenceValue as DialogNode;
        }
        
        RefreshPropertyReferences();
        UpdateWindowTitle();
    }
    
    private void OnDisable()
    {
        // Apply any pending changes before closing
        // Add extra null checks to prevent NullReferenceException
        if (serializedObject != null)
        {
            try
            {
                // Check if target object is still valid (may be destroyed when window closes)
                if (serializedObject.targetObject != null)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
            catch (System.Exception ex)
            {
                // Silently catch exceptions during window close - object may already be disposed
                // Only log if it's not a common disposal exception
                if (!(ex is System.NullReferenceException || ex is UnityEngine.MissingReferenceException))
                {
                    Debug.LogWarning($"[RememberTheScriptNodeEditor] Error applying properties on disable: {ex.Message}");
                }
            }
        }
    }
    
    private void RefreshPropertyReferences()
    {
        if (nodeProperty == null) return;
        
        // Core Dialog properties
        nodeTypeProp = nodeProperty.FindPropertyRelative("_nodeType");
        nodeIdProp = nodeProperty.FindPropertyRelative("_nodeId");
        characterNameProp = nodeProperty.FindPropertyRelative("_characterName");
        dialogTextProp = nodeProperty.FindPropertyRelative("_dialogText");
        isPlayerSpeakingProp = nodeProperty.FindPropertyRelative("_isPlayerSpeaking");
        
        // RememberTheScript properties
        isRememberScriptNodeProp = nodeProperty.FindPropertyRelative("_isRememberScriptNode");
        targetPhraseProp = nodeProperty.FindPropertyRelative("_targetPhrase");
        maxMistakesProp = nodeProperty.FindPropertyRelative("_maxMistakes");
        timeLimitProp = nodeProperty.FindPropertyRelative("_timeLimit");
        scorePerMistakeProp = nodeProperty.FindPropertyRelative("_scorePerMistake");
        scoreOnSuccessProp = nodeProperty.FindPropertyRelative("_scoreOnSuccess");
        caseSensitiveProp = nodeProperty.FindPropertyRelative("_caseSensitive");
        failureNodeProp = nodeProperty.FindPropertyRelative("_failureNode");  // NEW: Direct reference
        failureNodeNameProp = nodeProperty.FindPropertyRelative("_failureNodeName");  // LEGACY
        
        // Tree structure
        parentNodesProp = nodeProperty.FindPropertyRelative("_parentNodes");
        childNodeProp = nodeProperty.FindPropertyRelative("_childNode");
    }
    
    private void UpdateWindowTitle()
    {
        string nodeId = nodeIdProp?.stringValue ?? "";
        string targetPhrase = targetPhraseProp?.stringValue ?? "";
        
        if (!string.IsNullOrEmpty(nodeId))
        {
            titleContent = new GUIContent($"RememberTheScript: {nodeId}");
        }
        else if (!string.IsNullOrEmpty(targetPhrase))
        {
            titleContent = new GUIContent($"RememberTheScript: \"{targetPhrase}\"");
        }
        else
        {
            titleContent = new GUIContent("RememberTheScript Minigame Editor");
        }
    }
    
    private void OnGUI()
    {
        // Check if critical references are still valid
        // Use more defensive checks to prevent NullReferenceException
        if (serializedObject == null)
        {
            EditorGUILayout.HelpBox("SerializedObject is null. This window can be closed.", MessageType.Warning);
            
            if (GUILayout.Button("Close Window"))
            {
                Close();
            }
            return;
        }
        
        // Check targetObject separately with try-catch to handle Unity's internal null checking
        bool targetObjectValid = false;
        try
        {
            targetObjectValid = serializedObject.targetObject != null;
        }
        catch (System.NullReferenceException)
        {
            // Unity's internal null check threw - object is definitely invalid
            targetObjectValid = false;
        }
        
        if (!targetObjectValid || nodeProperty == null)
        {
            EditorGUILayout.HelpBox("Node reference lost. This window can be closed.", MessageType.Warning);
            
            if (GUILayout.Button("Close Window"))
            {
                Close();
            }
            return;
        }
        
        // Refresh property references if any are null (can happen after recompile)
        if (nodeIdProp == null || targetPhraseProp == null)
        {
            RefreshPropertyReferences();
        }
        
        // CRITICAL: Update at the start of every frame
        try
        {
            serializedObject.Update();
        }
        catch (System.Exception ex)
        {
            EditorGUILayout.HelpBox($"Error updating serialized object: {ex.Message}\nThis window can be closed.", MessageType.Error);
            
            if (GUILayout.Button("Close Window"))
            {
                Close();
            }
            return;
        }
        
        // Sync currentNode reference
        if (currentNode == null && nodeProperty.propertyType == SerializedPropertyType.ManagedReference)
        {
            currentNode = nodeProperty.managedReferenceValue as DialogNode;
        }
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        DrawHeader();
        EditorGUILayout.Space(10);
        
        DrawNodeIdentification();
        EditorGUILayout.Space(10);
        
        DrawTargetPhraseConfiguration();
        EditorGUILayout.Space(10);
        
        DrawDifficultySettings();
        EditorGUILayout.Space(10);
        
        DrawScoreSettings();
        EditorGUILayout.Space(10);
        
        DrawNavigationSettings();
        EditorGUILayout.Space(10);
        
        DrawQuickActions();
        
        EditorGUILayout.EndScrollView();
        
        // CRITICAL: Apply changes at the end
        try
        {
            if (serializedObject.ApplyModifiedProperties())
            {
                // Double-check targetObject is still valid before setting dirty
                try
                {
                    if (serializedObject.targetObject != null)
                    {
                        EditorUtility.SetDirty(serializedObject.targetObject);
                    }
                }
                catch (System.NullReferenceException)
                {
                    // Object became null during ApplyModifiedProperties - just skip SetDirty
                    Debug.LogWarning("[RememberTheScriptNodeEditor] Target object became null during property application");
                }
                
                // Only update window title if serializedObject is still valid
                try
                {
                    if (serializedObject != null && serializedObject.targetObject != null)
                    {
                        UpdateWindowTitle();
                    }
                }
                catch (System.NullReferenceException)
                {
                    // Object disposed - skip title update
                }
                
                Repaint();
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[RememberTheScriptNodeEditor] Error applying properties: {ex.Message}");
        }
    }
    
    #region Drawing Sections
    
    private void DrawHeader()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 16,
            alignment = TextAnchor.MiddleCenter
        };
        
        EditorGUILayout.LabelField("🎭 RememberTheScript Minigame", headerStyle);
        
        // Get preview info
        string nodeId = nodeIdProp?.stringValue ?? "<No ID>";
        string targetPhrase = targetPhraseProp?.stringValue ?? "<No Target Phrase>";
        
        GUIStyle previewStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Italic,
            wordWrap = true
        };
        
        EditorGUILayout.LabelField($"Node: {nodeId}", previewStyle);
        EditorGUILayout.LabelField($"Target: \"{targetPhrase}\"", previewStyle);
        
        // Quick status indicators
        bool isValid = ValidateMinigameConfiguration();
        GUIStyle statusStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold,
            normal = { textColor = isValid ? Color.green : Color.red }
        };
        EditorGUILayout.LabelField(isValid ? "✓ Configuration Valid" : "⚠ Configuration Incomplete", statusStyle);
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawNodeIdentification()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Node Identification", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);
        
        // Node Type Display with Conversion Warning
        if (nodeTypeProp != null && currentNode != null)
        {
            EditorGUI.BeginChangeCheck();
            DialogNodeType currentType = (DialogNodeType)nodeTypeProp.enumValueIndex;
            DialogNodeType newType = (DialogNodeType)EditorGUILayout.EnumPopup(
                new GUIContent("Node Type", "Type of node - determines editor and behavior"),
                currentType);
            
            if (EditorGUI.EndChangeCheck() && newType != currentType)
            {
                // Warn about converting away from minigame
                if (newType != DialogNodeType.RememberTheScript)
                {
                    string warning = $"Converting from RememberTheScript to {newType} will lose all minigame settings:\n\n" +
                                   "• Target phrase\n" +
                                   "• Difficulty settings\n" +
                                   "• Score configuration\n\n" +
                                   "Connections (parent/child) will be preserved.\n\n" +
                                   "Continue with conversion?";
                    
                    if (EditorUtility.DisplayDialog("Confirm Node Type Conversion", warning, "Convert", "Cancel"))
                    {
                        currentNode.ConvertToType(newType);
                        nodeTypeProp.enumValueIndex = (int)newType;
                        serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(serializedObject.targetObject);
                        
                        // Close this window - user can reopen from DialogTree inspector
                        EditorUtility.DisplayDialog("Conversion Complete", 
                            "This node is now a standard dialog node.\n" +
                            "Close and reopen from DialogTree inspector to edit.", "OK");
                        
                        Close();
                        return;
                    }
                    else
                    {
                        // Revert the change
                        nodeTypeProp.enumValueIndex = (int)currentType;
                    }
                }
            }
            
            EditorGUILayout.HelpBox("🎭 RememberTheScript Typing Minigame", MessageType.Info);
        }
        
        EditorGUILayout.Space(5);
        
        // Node Name/ID
        if (nodeIdProp != null)
        {
            EditorGUI.BeginChangeCheck();
            string newId = EditorGUILayout.TextField(
                new GUIContent("Node Name/ID", "Unique identifier for this minigame node"), 
                nodeIdProp.stringValue ?? "");
            if (EditorGUI.EndChangeCheck())
            {
                nodeIdProp.stringValue = newId;
                GUI.changed = true;
            }
        }
        
        // Speaker Name (optional for minigames)
        if (characterNameProp != null)
        {
            EditorGUI.BeginChangeCheck();
            string newSpeaker = EditorGUILayout.TextField(
                new GUIContent("Speaker Name (Optional)", "Character introducing the minigame"), 
                characterNameProp.stringValue ?? "");
            if (EditorGUI.EndChangeCheck())
            {
                characterNameProp.stringValue = newSpeaker;
                GUI.changed = true;
            }
        }
        
        // Intro Dialog Text (optional)
        if (dialogTextProp != null)
        {
            EditorGUILayout.LabelField("Intro Dialog (Optional)", EditorStyles.miniBoldLabel);
            EditorGUI.BeginChangeCheck();
            string newText = EditorGUILayout.TextArea(
                dialogTextProp.stringValue ?? "", 
                GUILayout.Height(60)
            );
            if (EditorGUI.EndChangeCheck())
            {
                dialogTextProp.stringValue = newText;
                GUI.changed = true;
            }
            EditorGUILayout.HelpBox("Optional intro text shown before minigame starts", MessageType.Info);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawTargetPhraseConfiguration()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        showTargetPhrasePreview = EditorGUILayout.Foldout(showTargetPhrasePreview, "Target Phrase Configuration", true, EditorStyles.foldoutHeader);
        
        if (showTargetPhrasePreview)
        {
            EditorGUILayout.Space(5);
            
            // Enable/Disable minigame
            if (isRememberScriptNodeProp != null)
            {
                EditorGUI.BeginChangeCheck();
                bool isEnabled = EditorGUILayout.Toggle(
                    new GUIContent("Enable RememberTheScript Minigame", "Turn on typing minigame for this node"),
                    isRememberScriptNodeProp.boolValue);
                if (EditorGUI.EndChangeCheck())
                {
                    isRememberScriptNodeProp.boolValue = isEnabled;
                    GUI.changed = true;
                }
            }
            
            if (isRememberScriptNodeProp.boolValue)
            {
                EditorGUILayout.Space(5);
                
                // Target Phrase
                if (targetPhraseProp != null)
                {
                    EditorGUILayout.LabelField("Target Phrase", EditorStyles.miniBoldLabel);
                    EditorGUI.BeginChangeCheck();
                    string newPhrase = EditorGUILayout.TextField(
                        new GUIContent("Phrase to Type", "The exact phrase the player must type"),
                        targetPhraseProp.stringValue ?? "");
                    if (EditorGUI.EndChangeCheck())
                    {
                        targetPhraseProp.stringValue = newPhrase;
                        GUI.changed = true;
                    }
                    
                    // Character count
                    int charCount = string.IsNullOrEmpty(targetPhraseProp.stringValue) ? 0 : targetPhraseProp.stringValue.Length;
                    EditorGUILayout.LabelField($"Character Count: {charCount}", EditorStyles.miniLabel);
                    
                    // Visual preview
                    if (!string.IsNullOrEmpty(targetPhraseProp.stringValue))
                    {
                        EditorGUILayout.Space(5);
                        EditorGUILayout.LabelField("Preview:", EditorStyles.miniBoldLabel);
                        
                        GUIStyle previewStyle = new GUIStyle(EditorStyles.textArea)
                        {
                            fontSize = 16,
                            fontStyle = FontStyle.Bold,
                            alignment = TextAnchor.MiddleCenter,
                            wordWrap = true
                        };
                        
                        EditorGUILayout.TextArea(targetPhraseProp.stringValue, previewStyle, GUILayout.Height(50));
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Minigame is disabled. Enable it to configure settings.", MessageType.Info);
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawDifficultySettings()
    {
        if (!isRememberScriptNodeProp.boolValue) return;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        showDifficultySettings = EditorGUILayout.Foldout(showDifficultySettings, "Difficulty Settings", true, EditorStyles.foldoutHeader);
        
        if (showDifficultySettings)
        {
            EditorGUILayout.Space(5);
            
            // Max Mistakes
            if (maxMistakesProp != null)
            {
                EditorGUI.BeginChangeCheck();
                int newMaxMistakes = EditorGUILayout.IntSlider(
                    new GUIContent("Max Mistakes", "Maximum number of mistakes before failure"),
                    maxMistakesProp.intValue, 1, 10);
                if (EditorGUI.EndChangeCheck())
                {
                    maxMistakesProp.intValue = newMaxMistakes;
                    GUI.changed = true;
                }
            }
            
            // Time Limit
            if (timeLimitProp != null)
            {
                EditorGUI.BeginChangeCheck();
                float newTimeLimit = EditorGUILayout.FloatField(
                    new GUIContent("Time Limit (seconds)", "Time limit in seconds (0 = no limit)"),
                    timeLimitProp.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    timeLimitProp.floatValue = Mathf.Max(0f, newTimeLimit);
                    GUI.changed = true;
                }
                
                if (timeLimitProp.floatValue > 0f)
                {
                    EditorGUILayout.LabelField($"⏱ {timeLimitProp.floatValue:F1} seconds", EditorStyles.miniLabel);
                }
                else
                {
                    EditorGUILayout.LabelField("⏱ No time limit", EditorStyles.miniLabel);
                }
            }
            
            // Case Sensitive
            if (caseSensitiveProp != null)
            {
                EditorGUI.BeginChangeCheck();
                bool newCaseSensitive = EditorGUILayout.Toggle(
                    new GUIContent("Case Sensitive", "Require exact case matching"),
                    caseSensitiveProp.boolValue);
                if (EditorGUI.EndChangeCheck())
                {
                    caseSensitiveProp.boolValue = newCaseSensitive;
                    GUI.changed = true;
                }
            }
            
            // Difficulty preview
            EditorGUILayout.Space(5);
            string difficulty = CalculateDifficultyRating();
            GUIStyle difficultyStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold
            };
            EditorGUILayout.LabelField($"Estimated Difficulty: {difficulty}", difficultyStyle);
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawScoreSettings()
    {
        if (!isRememberScriptNodeProp.boolValue) return;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        showScoreSettings = EditorGUILayout.Foldout(showScoreSettings, "Score Settings", true, EditorStyles.foldoutHeader);
        
        if (showScoreSettings)
        {
            EditorGUILayout.Space(5);
            
            // Score on Success
            if (scoreOnSuccessProp != null)
            {
                EditorGUI.BeginChangeCheck();
                float newScoreOnSuccess = EditorGUILayout.FloatField(
                    new GUIContent("Score on Success", "Audience score reward for completing the phrase"),
                    scoreOnSuccessProp.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    scoreOnSuccessProp.floatValue = newScoreOnSuccess;
                    GUI.changed = true;
                }
            }
            
            // Score per Mistake
            if (scorePerMistakeProp != null)
            {
                EditorGUI.BeginChangeCheck();
                float newScorePerMistake = EditorGUILayout.FloatField(
                    new GUIContent("Score per Mistake", "Audience score penalty for each mistake (usually negative)"),
                    scorePerMistakeProp.floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    scorePerMistakeProp.floatValue = newScorePerMistake;
                    GUI.changed = true;
                }
            }
            
            // Score preview
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Score Preview:", EditorStyles.miniBoldLabel);
            
            float successScore = scoreOnSuccessProp?.floatValue ?? 0f;
            float mistakeScore = scorePerMistakeProp?.floatValue ?? 0f;
            int maxMistakes = maxMistakesProp?.intValue ?? 3;
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Perfect run:", GUILayout.Width(150));
            EditorGUILayout.LabelField($"+{successScore:F1} points", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"With max mistakes ({maxMistakes}):", GUILayout.Width(150));
            float worstScore = successScore + (mistakeScore * maxMistakes);
            EditorGUILayout.LabelField($"{worstScore:+0.0;-0.0;0} points", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawNavigationSettings()
    {
        if (!isRememberScriptNodeProp.boolValue) return;
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        showNavigationSettings = EditorGUILayout.Foldout(showNavigationSettings, "Navigation Settings", true, EditorStyles.foldoutHeader);
        
        if (showNavigationSettings)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.HelpBox("Configure what happens on success or failure", MessageType.Info);
            
            // Success Navigation (Child Node)
            EditorGUILayout.LabelField("On Success:", EditorStyles.miniBoldLabel);
            
            if (childNodeProp != null && childNodeProp.managedReferenceValue != null)
            {
                DialogNode successNode = childNodeProp.managedReferenceValue as DialogNode;
                string successLabel = GetNodeButtonLabel(successNode, "Success Node");
                
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"✓ Success Node: {successLabel}", EditorStyles.wordWrappedLabel);
                
                // Edit button
                GUI.backgroundColor = new Color(0.7f, 0.9f, 1f); // Light blue
                if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    OpenChildNodeForEditing(successNode);
                }
                
                // Clear button
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("Clear Success Node?", 
                        "This will remove the success node connection. Continue?", "Clear", "Cancel"))
                    {
                        childNodeProp.managedReferenceValue = null;
                        serializedObject.ApplyModifiedProperties();
                        
                        try
                        {
                            if (serializedObject.targetObject != null)
                            {
                                EditorUtility.SetDirty(serializedObject.targetObject);
                            }
                        }
                        catch (System.NullReferenceException)
                        {
                            // Object became null - ignore
                        }
                    }
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("No success node set.", MessageType.Warning);
                
                EditorGUILayout.BeginHorizontal();
                
                // Create new node
                GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
                if (GUILayout.Button("+ Create New Success Node", GUILayout.Height(25)))
                {
                    CreateSuccessNode();
                }
                
                // Link to existing node
                GUI.backgroundColor = new Color(0.7f, 0.9f, 1f);
                if (GUILayout.Button("🔗 Link to Existing Node", GUILayout.Height(25)))
                {
                    ShowNodePicker(true); // true = success node
                }
                GUI.backgroundColor = Color.white;
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space(5);
            
            // Failure Navigation - NOW USES DIRECT OBJECT REFERENCE (same as success node)
            EditorGUILayout.LabelField("On Failure:", EditorStyles.miniBoldLabel);
            
            if (failureNodeProp != null && failureNodeProp.managedReferenceValue != null)
            {
                DialogNode failureNode = failureNodeProp.managedReferenceValue as DialogNode;
                string failureLabel = GetNodeButtonLabel(failureNode, "Failure Node");
                
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField($"✗ Failure Node: {failureLabel}", EditorStyles.wordWrappedLabel);
                
                // Edit button - NOW WORKS LIKE SUCCESS NODE!
                GUI.backgroundColor = new Color(0.7f, 0.9f, 1f); // Light blue
                if (GUILayout.Button("Edit", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    OpenFailureNodeForEditing(failureNode);
                }
                
                // Clear button
                GUI.backgroundColor = Color.yellow;
                if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(50)))
                {
                    if (EditorUtility.DisplayDialog("Clear Failure Node?", 
                        "Player will be able to retry the minigame. Continue?", "Clear", "Cancel"))
                    {
                        failureNodeProp.managedReferenceValue = null;
                        serializedObject.ApplyModifiedProperties();
                        
                        try
                        {
                            if (serializedObject.targetObject != null)
                            {
                                EditorUtility.SetDirty(serializedObject.targetObject);
                            }
                        }
                        catch (System.NullReferenceException)
                        {
                            // Object became null - ignore
                        }
                    }
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("⟳ No failure node set - player can retry minigame", MessageType.Info);
                
                EditorGUILayout.BeginHorizontal();
                
                // Create new failure node
                GUI.backgroundColor = new Color(1f, 0.7f, 0.7f);
                if (GUILayout.Button("+ Create New Failure Node", GUILayout.Height(25)))
                {
                    CreateFailureNode();
                }
                
                // Link to existing node
                GUI.backgroundColor = new Color(0.7f, 0.9f, 1f);
                if (GUILayout.Button("🔗 Link to Existing Node", GUILayout.Height(25)))
                {
                    ShowNodePicker(false); // false = failure node
                }
                GUI.backgroundColor = Color.white;
                
                EditorGUILayout.EndHorizontal();
            }
        }
        
        EditorGUILayout.EndVertical();
    }
    
    private void DrawQuickActions()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
        
        EditorGUILayout.Space(5);
        
        EditorGUILayout.BeginHorizontal();
        
        // Validate Configuration
        if (GUILayout.Button("Validate Configuration", GUILayout.Height(30)))
        {
            ValidateAndReport();
        }
        
        // Test Minigame
        GUI.backgroundColor = new Color(0.7f, 1f, 0.7f);
        if (GUILayout.Button("🎮 Test Minigame", GUILayout.Height(30)))
        {
            TestMinigame();
        }
        GUI.backgroundColor = Color.white;
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(5);
        
        // Status display - wrap all SerializedProperty accesses in try-catch
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Configuration Status", EditorStyles.miniBoldLabel);
        
        try
        {
            // Safely access all properties with defensive checks
            bool hasId = false;
            bool isEnabled = false;
            bool hasTargetPhrase = false;
            bool hasValidDifficulty = false;
            bool hasSuccessNode = false;
            
            // Check each property individually with try-catch
            if (nodeIdProp != null)
            {
                try { hasId = !string.IsNullOrEmpty(nodeIdProp.stringValue); }
                catch (System.NullReferenceException) { /* Property disposed */ }
            }
            
            if (isRememberScriptNodeProp != null)
            {
                try { isEnabled = isRememberScriptNodeProp.boolValue; }
                catch (System.NullReferenceException) { /* Property disposed */ }
            }
            
            if (targetPhraseProp != null)
            {
                try { hasTargetPhrase = !string.IsNullOrEmpty(targetPhraseProp.stringValue); }
                catch (System.NullReferenceException) { /* Property disposed */ }
            }
            
            if (maxMistakesProp != null)
            {
                try { hasValidDifficulty = maxMistakesProp.intValue > 0; }
                catch (System.NullReferenceException) { /* Property disposed */ }
            }
            
            if (childNodeProp != null)
            {
                try { hasSuccessNode = childNodeProp.managedReferenceValue != null; }
                catch (System.NullReferenceException) { /* Property disposed */ }
            }
            
            bool isValid = isEnabled && hasTargetPhrase && hasValidDifficulty;
            
            DrawStatusLine("Has ID/Name", hasId);
            DrawStatusLine("Minigame Enabled", isEnabled);
            DrawStatusLine("Has Target Phrase", hasTargetPhrase);
            DrawStatusLine("Valid Difficulty", hasValidDifficulty);
            DrawStatusLine("Has Success Node", hasSuccessNode);
            DrawStatusLine("Ready to Use", isValid);
        }
        catch (System.NullReferenceException)
        {
            // SerializedObject disposed during property access
            EditorGUILayout.HelpBox("SerializedObject disposed. Status unavailable.", MessageType.Warning);
        }
        
        EditorGUILayout.EndVertical();
        
        EditorGUILayout.EndVertical();
    }
    
    #endregion
    
    #region Helper Methods
    
    private string GetNodeButtonLabel(DialogNode node, string fallback)
    {
        if (node == null) return fallback;
        
        string nodeId = !string.IsNullOrEmpty(node.NodeName) ? $"[{node.NodeName}]" : "[No ID]";
        
        // Check if it's a RememberTheScript node
        if (node.IsRememberScriptNode)
        {
            string phrase = !string.IsNullOrEmpty(node.TargetPhrase) ? node.TargetPhrase : "<No phrase>";
            return $"{nodeId} RememberTheScript: \"{phrase}\"";
        }
        
        string preview = !string.IsNullOrEmpty(node.DialogText) 
            ? (node.DialogText.Length > 30 ? node.DialogText.Substring(0, 30) + "..." : node.DialogText)
            : "<No text>";
        
        return $"{nodeId} \"{preview}\"";
    }
    
    private bool ValidateMinigameConfiguration()
    {
        bool isEnabled = isRememberScriptNodeProp != null && isRememberScriptNodeProp.boolValue;
        bool hasTargetPhrase = targetPhraseProp != null && !string.IsNullOrEmpty(targetPhraseProp.stringValue);
        bool hasValidDifficulty = maxMistakesProp != null && maxMistakesProp.intValue > 0;
        
        return isEnabled && hasTargetPhrase && hasValidDifficulty;
    }
    
    private void DrawStatusLine(string label, bool status)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(label, GUILayout.Width(150));
        
        GUIStyle statusStyle = new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold
        };
        
        if (status)
        {
            statusStyle.normal.textColor = Color.green;
            EditorGUILayout.LabelField("✅", statusStyle);
        }
        else
        {
            statusStyle.normal.textColor = Color.red;
            EditorGUILayout.LabelField("❌", statusStyle);
        }
        
        EditorGUILayout.EndHorizontal();
    }
    
    private string CalculateDifficultyRating()
    {
        if (targetPhraseProp == null || maxMistakesProp == null || timeLimitProp == null)
            return "Unknown";
        
        string phrase = targetPhraseProp.stringValue ?? "";
        int phraseLength = phrase.Length;
        int maxMistakes = maxMistakesProp.intValue;
        float timeLimit = timeLimitProp.floatValue;
        bool caseSensitive = caseSensitiveProp?.boolValue ?? false;
        
        int difficultyScore = 0;
        
        // Length factor
        if (phraseLength > 50) difficultyScore += 3;
        else if (phraseLength > 30) difficultyScore += 2;
        else if (phraseLength > 15) difficultyScore += 1;
        
        // Mistakes factor
        if (maxMistakes <= 2) difficultyScore += 2;
        else if (maxMistakes <= 3) difficultyScore += 1;
        
        // Time limit factor
        if (timeLimit > 0f)
        {
            float charsPerSecond = phraseLength / timeLimit;
            if (charsPerSecond > 2f) difficultyScore += 3;
            else if (charsPerSecond > 1f) difficultyScore += 2;
            else if (charsPerSecond > 0.5f) difficultyScore += 1;
        }
        
        // Case sensitive factor
        if (caseSensitive) difficultyScore += 1;
        
        // Rate difficulty
        if (difficultyScore >= 7) return "⚠ VERY HARD";
        if (difficultyScore >= 5) return "🔥 Hard";
        if (difficultyScore >= 3) return "⚡ Medium";
        if (difficultyScore >= 1) return "✓ Easy";
        return "😎 Very Easy";
    }
    
    private void CreateSuccessNode()
    {
        if (childNodeProp == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot create success node - property reference is null.", "OK");
            return;
        }
        
        if (serializedObject == null || serializedObject.targetObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot create success node - serialized object is invalid.", "OK");
            return;
        }
        
        try
        {
            // Create new DialogNode
            var newNode = new DialogNode("Speaker", "Success! Well done!", false);
            
            childNodeProp.managedReferenceValue = newNode;
            
            serializedObject.ApplyModifiedProperties();
            
            if (serializedObject.targetObject != null)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
            
            // Refresh tree
            if (parentTree != null)
            {
                parentTree.RefreshNodeList();
                EditorUtility.SetDirty(parentTree);
            }
            
            EditorUtility.DisplayDialog("Success Node Created", 
                "Success node created! You can edit it through the DialogTree inspector.", "OK");
            
            // Force repaint
            Repaint();
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Error Creating Node", 
                $"Failed to create success node:\n{ex.Message}", "OK");
            Debug.LogError($"[RememberTheScriptNodeEditor] Error creating success node: {ex}");
        }
    }

    private void CreateFailureNode()
    {
        if (failureNodeProp == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot create failure node - property reference is null.", "OK");
            return;
        }
        
        if (serializedObject == null || serializedObject.targetObject == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot create failure node - serialized object is invalid.", "OK");
            return;
        }
        
        try
        {
            // Create failure node ID
            string failureNodeId = string.IsNullOrEmpty(currentNode.NodeName) 
                ? "failure_node" 
                : $"{currentNode.NodeName}_failure";
            
            // Create new DialogNode for failure (SAME AS SUCCESS NODE PATTERN)
            var failureNode = new DialogNode(
                "Director",
                "Let's try that again. Remember your lines!",
                false,
                failureNodeId
            );
            
            // Assign directly to property (SAME AS SUCCESS NODE)
            failureNodeProp.managedReferenceValue = failureNode;
            
            serializedObject.ApplyModifiedProperties();
            
            if (serializedObject.targetObject != null)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
            
            // Refresh tree
            if (parentTree != null)
            {
                parentTree.RefreshNodeList();
                EditorUtility.SetDirty(parentTree);
            }
            
            EditorUtility.DisplayDialog("Failure Node Created!", 
                "Failure node created! Edit button is now available.", "OK");
            
            // Force repaint
            Repaint();
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Error Creating Node", 
                $"Failed to create failure node:\n{ex.Message}", "OK");
            Debug.LogError($"[RememberTheScriptNodeEditor] Error creating failure node: {ex}");
        }
    }
    
    private void ValidateAndReport()
    {
        bool isEnabled = isRememberScriptNodeProp != null && isRememberScriptNodeProp.boolValue;
        bool hasTargetPhrase = targetPhraseProp != null && !string.IsNullOrEmpty(targetPhraseProp.stringValue);
        bool hasValidDifficulty = maxMistakesProp != null && maxMistakesProp.intValue > 0;
        bool hasSuccessNode = childNodeProp != null && childNodeProp.managedReferenceValue != null;
        bool isValid = isEnabled && hasTargetPhrase && hasValidDifficulty;
        
        if (isValid)
        {
            string report = "✅ RememberTheScript Minigame Configuration Valid!\n\n";
            report += $"Target Phrase: \"{targetPhraseProp.stringValue}\"\n";
            report += $"Max Mistakes: {maxMistakesProp.intValue}\n";
            report += $"Time Limit: {(timeLimitProp.floatValue > 0 ? timeLimitProp.floatValue + "s" : "None")}\n";
            report += $"Difficulty: {CalculateDifficultyRating()}\n";
            report += $"Score on Success: +{scoreOnSuccessProp.floatValue}\n";
            report += $"Score per Mistake: {scorePerMistakeProp.floatValue}\n";
            
            if (hasSuccessNode)
                report += "\n✓ Success node configured";
            else
                report += "\n⚠ Warning: No success node (will need manual navigation)";
            
            EditorUtility.DisplayDialog("Validation Success", report, "OK");
        }
        else
        {
            string issues = "❌ Configuration Issues:\n\n";
            
            if (!isEnabled)
                issues += "• Minigame is not enabled\n";
            
            if (!hasTargetPhrase)
                issues += "• Missing target phrase\n";
            
            if (!hasValidDifficulty)
                issues += "• Invalid difficulty settings (max mistakes must be > 0)\n";
            
            if (!hasSuccessNode)
                issues += "• Warning: No success node configured\n";
            
            EditorUtility.DisplayDialog("Validation Failed", issues, "OK");
        }
    }
    
    private void TestMinigame()
    {
        if (!ValidateMinigameConfiguration())
        {
            EditorUtility.DisplayDialog("Cannot Test", 
                "Minigame configuration is incomplete. Please fix issues first.", "OK");
            return;
        }
        
        EditorUtility.DisplayDialog("Test Minigame", 
            "To test this minigame:\n\n" +
            "1. Create a test scene with RememberTheScriptTest component\n" +
            "2. Assign the parent DialogTree to the test component\n" +
            "3. Press Play and press 'T' to start test\n" +
            "4. Type the phrase to test validation\n\n" +
            "See RememberTheScript-QuickStart.md for detailed instructions.", "OK");
    }
    
    private void ShowNodePicker(bool isSuccessNode)
    {
        if (parentTree == null)
        {
            EditorUtility.DisplayDialog("Error", "Parent tree reference is missing.", "OK");
            return;
        }
        
        var allNodes = parentTree.GetAllNodes();
        
        if (allNodes.Count == 0)
        {
            EditorUtility.DisplayDialog("No Nodes Available", 
                "There are no other nodes in this tree to link to.\n\n" +
                "Create nodes first, then link them.", "OK");
            return;
        }
        
        // Create menu with all available nodes
        GenericMenu menu = new GenericMenu();
        
        foreach (var node in allNodes)
        {
            // Skip the current node
            if (node == currentNode) continue;
            
            string nodeLabel = GetNodeButtonLabel(node, "Node");
            string menuPath = $"{nodeLabel}";
            
            // Add to menu
            if (isSuccessNode)
            {
                menu.AddItem(new GUIContent(menuPath), false, () => LinkSuccessNode(node));
            }
            else
            {
                menu.AddItem(new GUIContent(menuPath), false, () => LinkFailureNode(node));
            }
        }
        
        if (menu.GetItemCount() == 0)
        {
            EditorUtility.DisplayDialog("No Nodes Available", 
                "There are no other nodes in this tree to link to.", "OK");
            return;
        }
        
        menu.ShowAsContext();
    }
    
    private void LinkSuccessNode(DialogNode targetNode)
    {
        if (childNodeProp == null || targetNode == null) return;
        
        try
        {
            childNodeProp.managedReferenceValue = targetNode;
            serializedObject.ApplyModifiedProperties();
            
            if (serializedObject.targetObject != null)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
            
            // Refresh tree
            if (parentTree != null)
            {
                parentTree.RefreshNodeList();
                EditorUtility.SetDirty(parentTree);
            }
            
            EditorUtility.DisplayDialog("Success Node Linked", 
                $"Linked to existing node: {targetNode.NodeName}", "OK");
            
            Repaint();
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Error Linking Node", 
                $"Failed to link success node:\n{ex.Message}", "OK");
            Debug.LogError($"[RememberTheScriptNodeEditor] Error linking success node: {ex}");
        }
    }
    
    private void LinkFailureNode(DialogNode targetNode)
    {
        if (failureNodeProp == null || targetNode == null) return;
        
        try
        {
            // Direct object assignment (SAME AS SUCCESS NODE)
            failureNodeProp.managedReferenceValue = targetNode;
            serializedObject.ApplyModifiedProperties();
            
            if (serializedObject.targetObject != null)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
            
            // Refresh tree
            if (parentTree != null)
            {
                parentTree.RefreshNodeList();
                EditorUtility.SetDirty(parentTree);
            }
            
            EditorUtility.DisplayDialog("Failure Node Linked", 
                $"Linked to existing node: {targetNode.NodeName}", "OK");
            
            Repaint();
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Error Linking Node", 
                $"Failed to link failure node:\n{ex.Message}", "OK");
            Debug.LogError($"[RememberTheScriptNodeEditor] Error linking failure node: {ex}");
        }
    }
    
    private void AutoCreateOutcomeNodes()
    {
        if (currentNode == null || parentTree == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot auto-create nodes - missing references.", "OK");
            return;
        }
        
        try
        {
            bool createdSuccess = false;
            bool createdFailure = false;
            string successNodeName = "";
            string failureNodeName = "";
            
            // Create success node if missing
            if (childNodeProp.managedReferenceValue == null)
            {
                var successNode = new DialogNode(
                    "Director", 
                    "Excellent work! Your performance was flawless!", 
                    false,
                    $"{currentNode.NodeName}_success"
                );
                
                childNodeProp.managedReferenceValue = successNode;
                createdSuccess = true;
                successNodeName = successNode.NodeName;
            }
            
            // Create failure node if missing (NOW USES DIRECT REFERENCE)
            if (failureNodeProp.managedReferenceValue == null)
            {
                string failureNodeId = string.IsNullOrEmpty(currentNode.NodeName)
                    ? "failure_node"
                    : $"{currentNode.NodeName}_failure";
                
                var failureNode = new DialogNode(
                    "Director",
                    "Let's try that again. Remember your lines!",
                    false,
                    failureNodeId
                );
                
                // Direct assignment (SAME AS SUCCESS NODE)
                failureNodeProp.managedReferenceValue = failureNode;
                createdFailure = true;
                failureNodeName = failureNodeId;
            }
            
            serializedObject.ApplyModifiedProperties();
            
            if (serializedObject.targetObject != null)
            {
                EditorUtility.SetDirty(serializedObject.targetObject);
            }
            
            // Refresh tree
            if (parentTree != null)
            {
                parentTree.RefreshNodeList();
                EditorUtility.SetDirty(parentTree);
            }
            
            // Build result message
            string message = "Created:\n";
            if (createdSuccess)
                message += $"✓ Success node: {successNodeName}\n";
            if (createdFailure)
                message += $"✗ Failure node: {failureNodeName}\n";
            
            if (!createdSuccess && !createdFailure)
                message = "Both outcome nodes already exist!";
            else
                message += "\nBoth nodes are now in the tree and ready to use!";
            
            EditorUtility.DisplayDialog("Outcome Nodes Created", message, "OK");
            
            Repaint();
        }
        catch (System.Exception ex)
        {
            EditorUtility.DisplayDialog("Error Creating Nodes", 
                $"Failed to auto-create outcome nodes:\n{ex.Message}", "OK");
            Debug.LogError($"[RememberTheScriptNodeEditor] Error auto-creating outcome nodes: {ex}");
        }
    }
    
    /// <summary>
    /// Open child node for editing in appropriate editor window
    /// </summary>
    private void OpenChildNodeForEditing(DialogNode childNode)
    {
        if (childNode == null || parentTree == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot open child node - reference is missing.", "OK");
            return;
        }
        
        // Find the child node in the tree's allNodes list
        SerializedObject treeObject = new SerializedObject(parentTree);
        SerializedProperty allNodesProp = treeObject.FindProperty("allNodes");
        
        if (allNodesProp != null)
        {
            for (int i = 0; i < allNodesProp.arraySize; i++)
            {
                SerializedProperty nodeProp = allNodesProp.GetArrayElementAtIndex(i);
                DialogNode node = nodeProp.managedReferenceValue as DialogNode;
                
                if (node == childNode)
                {
                    // Open appropriate editor based on node type
                    RememberTheScriptNodeEditor_OpenWindow(nodeProp, parentTree);
                    return;
                }
            }
        }
        
        EditorUtility.DisplayDialog("Error", 
            "Could not find child node in tree.\nTry refreshing the tree from DialogTree inspector.", "OK");
    }
    
    /// <summary>
    /// Open failure node for editing in appropriate editor window
    /// </summary>
    private void OpenFailureNodeForEditing(DialogNode failureNode)
    {
        if (failureNode == null || parentTree == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot open failure node - reference is missing.", "OK");
            return;
        }
        
        // Find the failure node in the tree's allNodes list
        SerializedObject treeObject = new SerializedObject(parentTree);
        SerializedProperty allNodesProp = treeObject.FindProperty("allNodes");
        
        if (allNodesProp != null)
        {
            for (int i = 0; i < allNodesProp.arraySize; i++)
            {
                SerializedProperty nodeProp = allNodesProp.GetArrayElementAtIndex(i);
                DialogNode node = nodeProp.managedReferenceValue as DialogNode;
                
                if (node == failureNode)
                {
                    // Open appropriate editor based on node type
                    RememberTheScriptNodeEditor_OpenWindow(nodeProp, parentTree);
                    return;
                }
            }
        }
        
        EditorUtility.DisplayDialog("Error", 
            "Could not find failure node in tree.\nTry refreshing the tree from DialogTree inspector.", "OK");
    }
    
    /// <summary>
    /// Open parent node for editing in appropriate editor window
    /// </summary>
    private void OpenParentNodeForEditing(DialogNode parentNode, int parentIndex)
    {
        if (parentNode == null || parentTree == null)
        {
            EditorUtility.DisplayDialog("Error", "Cannot open parent node - reference is missing.", "OK");
            return;
        }
        
        // Find the parent node in the tree's allNodes list
        SerializedObject treeObject = new SerializedObject(parentTree);
        SerializedProperty allNodesProp = treeObject.FindProperty("allNodes");
        
        if (allNodesProp != null)
        {
            for (int i = 0; i < allNodesProp.arraySize; i++)
            {
                SerializedProperty nodeProp = allNodesProp.GetArrayElementAtIndex(i);
                DialogNode node = nodeProp.managedReferenceValue as DialogNode;
                
                if (node == parentNode)
                {
                    // Open appropriate editor based on node type
                    RememberTheScriptNodeEditor_OpenWindow(nodeProp, parentTree);
                    return;
                }
            }
        }
        
        EditorUtility.DisplayDialog("Error", 
            "Could not find parent node in tree.\nTry refreshing the tree from DialogTree inspector.", "OK");
    }
    
    /// <summary>
    /// Helper method to open any node in the appropriate editor window
    /// Routes to RememberTheScriptNodeEditor or DialogNodeEditorWindow based on node type
    /// </summary>
    private static void RememberTheScriptNodeEditor_OpenWindow(SerializedProperty nodeProp, DialogTree tree)
    {
        // Check if this is a RememberTheScript node
        SerializedProperty nodeTypeProp = nodeProp.FindPropertyRelative("_nodeType");
        if (nodeTypeProp != null && (DialogNodeType)nodeTypeProp.enumValueIndex == DialogNodeType.RememberTheScript)
        {
            RememberTheScriptNodeEditor.OpenWindow(nodeProp, tree);
        }
        else
        {
            // Create a standard dialog editor window (same pattern as DialogNodeEditorWindow.OpenWindow)
            var window = EditorWindow.CreateInstance(System.Type.GetType("DialogNodeEditorWindow")) as EditorWindow;
            if (window != null)
            {
                window.minSize = new Vector2(500, 650);
                
                // Use reflection to call Initialize
                var initMethod = window.GetType().GetMethod("Initialize", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (initMethod != null)
                {
                    initMethod.Invoke(window, new object[] { nodeProp, tree });
                    window.Show();
                }
            }
        }
    }
    #endregion
}
