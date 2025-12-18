using UnityEngine;

/// <summary>
/// Test helper for RememberTheScript minigame.
/// Attach to any GameObject in your scene to quickly test the minigame functionality.
/// 
/// Usage in Unity:
/// 1. Attach this script to a GameObject
/// 2. Create a DialogTree asset with at least one node
/// 3. Configure that node with RememberTheScript settings:
///    - Enable "Is Remember Script Node"
///    - Set Target Phrase (e.g., "Break a leg")
///    - Set Max Mistakes (e.g., 3)
///    - Set Time Limit (e.g., 30)
/// 4. Assign the tree to this script's "Test Tree" field
/// 5. Press Play and press T to trigger the test
/// </summary>
public class RememberTheScriptTest : MonoBehaviour
{
    [Header("Test Configuration")]
    [SerializeField] private DialogTree testTree;
    [SerializeField] private KeyCode testKey = KeyCode.T;
    [SerializeField] private bool enableDebugLogs = true;

    private DialogNavigator testNavigator;
    private bool isTestActive = false;

    private void Start()
    {
        Log("RememberTheScript Test Helper initialized");
        Log($"Press '{testKey}' to start test");

        if (testTree == null)
        {
            LogError("No test tree assigned! Please assign a DialogTree with a RememberTheScript node.");
        }
    }

    private void Update()
    {
        // Start test
        if (Input.GetKeyDown(testKey) && !isTestActive)
        {
            StartTest();
        }

        // Process typing input if test is active
        if (isTestActive && testNavigator != null && testNavigator.IsRememberScriptActive)
        {
            // Process any key press
            if (Input.anyKeyDown)
            {
                foreach (char c in Input.inputString)
                {
                    // Ignore control characters
                    if (c >= 32 && c <= 126) // Printable ASCII
                    {
                        bool correct = testNavigator.ProcessRememberScriptInput(c);
                        LogInput(c, correct);
                    }
                }
            }

            // Update timer
            testNavigator.UpdateRememberScriptTimer(Time.deltaTime);
        }

        // End test
        if (Input.GetKeyDown(KeyCode.Escape) && isTestActive)
        {
            EndTest();
        }
    }

    private void StartTest()
    {
        if (testTree == null)
        {
            LogError("Cannot start test - no tree assigned");
            return;
        }

        // Validate tree has RememberTheScript node
        var startNode = testTree.GetStartingNode();
        if (startNode == null || !startNode.IsRememberScriptNode)
        {
            LogError("Test tree's starting node is not configured for RememberTheScript minigame!");
            LogError("Please enable 'Is Remember Script Node' and set a Target Phrase in the Inspector.");
            return;
        }

        Log($"=== Starting RememberTheScript Test ===");
        Log($"Target Phrase: '{startNode.TargetPhrase}'");
        Log($"Max Mistakes: {startNode.MaxMistakes}");
        Log($"Time Limit: {startNode.TimeLimit}s");
        Log($"Case Sensitive: {startNode.CaseSensitive}");
        Log("Start typing!");

        // Create test navigator
        testNavigator = new DialogNavigator();

        // Subscribe to events for debugging
        testNavigator.OnRememberScriptStarted += HandleStart;
        testNavigator.OnRememberScriptProgress += HandleProgress;
        testNavigator.OnRememberScriptMistake += HandleMistake;
        testNavigator.OnRememberScriptReset += HandleReset;
        testNavigator.OnRememberScriptSuccess += HandleSuccess;
        testNavigator.OnRememberScriptFailure += HandleFailure;
        testNavigator.OnRememberScriptEnded += HandleEnded;

        // Create minimal NPC GameObject for test (NPCContent is MonoBehaviour, not ScriptableObject)
        var testNPCGO = new GameObject("Test NPC");
        testNPCGO.hideFlags = HideFlags.HideAndDontSave; // Hide from hierarchy
        var testNPC = testNPCGO.AddComponent<NPCContent>();
        testNPC.npcName = "Test NPC";
        testNPC.mainDialogTree = testTree;

        // Start navigation
        isTestActive = true;

        // Start dialog directly with the tree (DialogNavigator needs NPCContent but we bypass validation)
        bool success = testNavigator.StartDialog(testNPC, null);

        if (!success)
        {
            LogError("Failed to start dialog navigation - check tree validity");
            EndTest();

            // Clean up test NPC
            if (testNPCGO != null)
            {
                Destroy(testNPCGO);
            }
        }
    }

    private void EndTest()
    {
        Log("=== Ending Test ===");

        if (testNavigator != null)
        {
            testNavigator.EndDialog();

            // Unsubscribe from events
            testNavigator.OnRememberScriptStarted -= HandleStart;
            testNavigator.OnRememberScriptProgress -= HandleProgress;
            testNavigator.OnRememberScriptMistake -= HandleMistake;
            testNavigator.OnRememberScriptReset -= HandleReset;
            testNavigator.OnRememberScriptSuccess -= HandleSuccess;
            testNavigator.OnRememberScriptFailure -= HandleFailure;
            testNavigator.OnRememberScriptEnded -= HandleEnded;
        }

        testNavigator = null;
        isTestActive = false;

        Log($"Press '{testKey}' to start a new test");
    }

    #region Event Handlers

    private void HandleStart(DialogNode node)
    {
        Log($"[Event] Minigame Started - Target: '{node.TargetPhrase}'");
    }

    private void HandleProgress(string currentText, string targetPhrase)
    {
        Log($"[Event] Progress: {currentText.Length}/{targetPhrase.Length} - '{currentText}'");
    }

    private void HandleMistake(char typed, char expected, int mistakeCount)
    {
        LogError($"[Event] Mistake! Typed '{typed}' expected '{expected}'. Total mistakes: {mistakeCount}");
    }

    private void HandleReset()
    {
        Log("[Event] Text reset - start over!");
    }

    private void HandleSuccess(string completedText, int mistakes, float timeRemaining)
    {
        Log($"[Event] SUCCESS! Completed '{completedText}' with {mistakes} mistakes. Time left: {timeRemaining:F1}s");
        EndTest();
    }

    private void HandleFailure(int mistakes)
    {
        LogError($"[Event] FAILED! Too many mistakes: {mistakes}");
    }

    private void HandleEnded()
    {
        Log("[Event] Minigame Ended");
    }

    #endregion

    #region Logging

    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[RememberTheScript Test] {message}");
        }
    }

    private void LogError(string message)
    {
        Debug.LogError($"[RememberTheScript Test] {message}");
    }

    private void LogInput(char c, bool correct)
    {
        if (enableDebugLogs)
        {
            string status = correct ? "<color=green>✓</color>" : "<color=red>✗</color>";
            Debug.Log($"[RememberTheScript Test] {status} Typed: '{c}'");
        }
    }

    #endregion

    [ContextMenu("Create Test Dialog Tree")]
    private void CreateTestTree()
    {
#if UNITY_EDITOR
        if (testTree != null)
        {
            Debug.LogWarning("Test tree already assigned. Remove it first if you want to create a new one.");
            return;
        }

        Debug.Log("Creating test dialog tree...");
        Debug.Log("This feature requires Unity Editor. Please create a DialogTree asset manually and configure it for RememberTheScript.");
        Debug.Log("Steps:");
        Debug.Log("1. Right-click in Project window → Create → Dialog System → Dialog Tree");
        Debug.Log("2. Name it 'RememberTheScript Test Tree'");
        Debug.Log("3. In Inspector, create a starting node");
        Debug.Log("4. Enable 'Is Remember Script Node'");
        Debug.Log("5. Set Target Phrase to something simple like 'test'");
        Debug.Log("6. Set Max Mistakes to 3");
        Debug.Log("7. Assign the tree to this script's Test Tree field");
#endif
    }
}
