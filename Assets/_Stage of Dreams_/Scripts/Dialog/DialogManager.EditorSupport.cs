/* DialogManager.EditorSupport.cs
 * 
 * Partial class containing Unity Editor debug tools and context menu commands.
 * Only included in editor builds.
 */

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Editor support and debugging portion of DialogManager.
/// Contains context menu commands and validation tools.
/// </summary>
public partial class DialogManager
{
    #region Context Menu Commands

    [ContextMenu("Validate Setup")]
    private void EditorValidateSetup()
    {
        ValidateSetup();
    }

    [ContextMenu("Print Current State")]
    private void EditorPrintCurrentState()
    {
        Debug.Log($"=== DialogManager State ===");
        Debug.Log($"Initialized: {isInitialized}");
        Debug.Log($"UI Active: {isUIActive}");
        Debug.Log($"Current NPC: {currentNPC?.npcName ?? "None"}");
        Debug.Log($"Current Tree: {currentTree?.treeName ?? "None"}");
        Debug.Log($"Session Count: {dialogSessionCount}");
        Debug.Log($"Navigator Active: {navigator?.IsActive ?? false}");

        if (navigator != null)
        {
            var state = navigator.GetCurrentState();
            Debug.Log($"Current Node: {state.currentNode?.DialogText ?? "None"}");
            Debug.Log($"Has Choices: {state.hasChoices}");
            Debug.Log($"Should Auto-Advance: {state.shouldAutoAdvance}");
        }
    }

    [ContextMenu("Test UI Elements")]
    private void EditorTestUIElements()
    {
        Debug.Log("=== UI Elements Test ===");

        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is null");
            return;
        }

        if (rootElement == null)
        {
            Debug.LogError("Root element is null");
            return;
        }

        Debug.Log($"Root element children count: {rootElement.childCount}");

        // Test finding elements
        var testDialogBox = rootElement.Q<GroupBox>("DialogBox");
        var testLabel = rootElement.Q<Label>("GivenDialogLabel");
        var testButton1 = rootElement.Q<Button>("DialogOption1Btn");

        Debug.Log($"DialogBox found: {testDialogBox != null}");
        Debug.Log($"Label found: {testLabel != null}");
        Debug.Log($"Button1 found: {testButton1 != null}");

        if (testDialogBox != null)
        {
            Debug.Log($"DialogBox children count: {testDialogBox.childCount}");
            Debug.Log($"DialogBox style display: {testDialogBox.style.display.value}");
        }

        // List all elements for debugging
        Debug.Log("All elements in root:");
        ListAllElements(rootElement, 0);
    }

    [ContextMenu("Test Show Dialog")]
    private void EditorTestShowDialog()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Dialog test only works in play mode");
            return;
        }

        if (!isInitialized)
        {
            Debug.LogError("DialogManager not initialized");
            return;
        }

        // Create a test dialog node
        var testNode = new DialogNode("Test Speaker", "This is a test dialog message to verify UI connectivity.", false);
        testNode.AddChoice("Test Choice 1", null, "choice1");
        testNode.AddChoice("Test Choice 2", null, "choice2");

        DisplayNode(testNode);
        Debug.Log("Test dialog displayed");
    }

    [ContextMenu("Test Hide Dialog")]
    private void EditorTestHideDialog()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Dialog test only works in play mode");
            return;
        }

        HideDialog();
        Debug.Log("Dialog hidden");
    }

    [ContextMenu("Debug UI Document Settings")]
    private void EditorDebugUIDocumentSettings()
    {
        Debug.Log("=== UI Document Debug Info ===");

        if (uiDocument == null)
        {
            Debug.LogError("UIDocument is null");
            return;
        }

        Debug.Log($"UIDocument enabled: {uiDocument.enabled}");
        Debug.Log($"UIDocument gameObject active: {uiDocument.gameObject.activeInHierarchy}");
        Debug.Log($"Sort order: {uiDocument.sortingOrder}");
        Debug.Log($"Panel settings: {uiDocument.panelSettings}");

        if (uiDocument.panelSettings != null)
        {
            Debug.Log($"Panel scale: {uiDocument.panelSettings.scale}");
            Debug.Log($"Panel reference resolution: {uiDocument.panelSettings.referenceResolution}");
        }

        if (rootElement != null)
        {
            Debug.Log($"Root element visible: {rootElement.visible}");
            Debug.Log($"Root element style display: {rootElement.style.display.value}");
            Debug.Log($"Root element resolvedStyle display: {rootElement.resolvedStyle.display}");
            Debug.Log($"Root element parent: {rootElement.parent}");
        }

        if (dialogBox != null)
        {
            Debug.Log($"DialogBox parent: {dialogBox.parent}");
            Debug.Log($"DialogBox childCount: {dialogBox.childCount}");
            Debug.Log($"DialogBox world bound: {dialogBox.worldBound}");
            Debug.Log($"DialogBox layout: {dialogBox.layout}");
        }
    }

    [ContextMenu("Force Refresh UI")]
    private void EditorForceRefreshUI()
    {
        if (Application.isPlaying)
        {
            InitializeUI();
            Debug.Log("UI connection refreshed");
        }
        else
        {
            Debug.LogWarning("UI refresh only works in play mode");
        }
    }

    [ContextMenu("Debug Dialog Layout")]
    private void EditorDebugDialogLayout()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("Layout debug only works in play mode");
            return;
        }

        if (dialogBox == null)
        {
            Debug.LogError("DialogBox is null - cannot debug layout");
            return;
        }

        Debug.Log("=== Dialog Layout Debug ===");
        Debug.Log($"DialogBox world bounds: {dialogBox.worldBound}");
        Debug.Log($"DialogBox layout: {dialogBox.layout}");
        Debug.Log($"DialogBox computed style width: {dialogBox.resolvedStyle.width}");
        Debug.Log($"DialogBox computed style height: {dialogBox.resolvedStyle.height}");
        Debug.Log($"DialogBox style position: {dialogBox.style.position.value}");
        Debug.Log($"DialogBox style left: {dialogBox.style.left.value}");
        Debug.Log($"DialogBox style right: {dialogBox.style.right.value}");
        Debug.Log($"DialogBox style bottom: {dialogBox.style.bottom.value}");
        Debug.Log($"DialogBox style width: {dialogBox.style.width.value}");
        Debug.Log($"DialogBox style max-width: {dialogBox.style.maxWidth.value}");

        if (rootElement != null)
        {
            Debug.Log($"Root element bounds: {rootElement.worldBound}");
            Debug.Log($"Root element layout: {rootElement.layout}");
            Debug.Log($"Root element computed width: {rootElement.resolvedStyle.width}");
            Debug.Log($"Root element computed height: {rootElement.resolvedStyle.height}");
        }

        if (uiDocument != null && uiDocument.panelSettings != null)
        {
            Debug.Log($"Panel reference resolution: {uiDocument.panelSettings.referenceResolution}");
            Debug.Log($"Panel scale: {uiDocument.panelSettings.scale}");
        }

        Debug.Log($"Screen width: {Screen.width}, Screen height: {Screen.height}");
    }

    [ContextMenu("Force Show Dialog UI")]
    private void EditorForceShowDialogUI()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("UI test only works in play mode");
            return;
        }

        if (dialogBox == null)
        {
            Debug.LogError("DialogBox is null - cannot force show");
            return;
        }

        Debug.Log("Force showing dialog UI...");

        dialogBox.style.display = DisplayStyle.Flex;
        dialogBox.style.opacity = 1f;
        dialogBox.style.scale = new Scale(Vector2.one);
        dialogBox.style.visibility = Visibility.Visible;
        dialogBox.AddToClassList("dialog-visible");

        if (dialogLabel != null)
        {
            dialogLabel.text = "FORCE TEST: This dialog is being force-displayed for testing.";
        }

        dialogBox.MarkDirtyRepaint();
        rootElement?.MarkDirtyRepaint();
        uiDocument?.rootVisualElement?.MarkDirtyRepaint();

        Debug.Log($"After force show - Display: {dialogBox.style.display.value}, Opacity: {dialogBox.style.opacity.value}");
        Debug.Log($"Resolved styles - Display: {dialogBox.resolvedStyle.display}, Opacity: {dialogBox.resolvedStyle.opacity}");
        Debug.Log($"World bound: {dialogBox.worldBound}");
        Debug.Log($"Visible: {dialogBox.visible}");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Recursively list all UI elements for debugging
    /// </summary>
    private void ListAllElements(VisualElement element, int depth)
    {
        string indent = new string(' ', depth * 2);
        string name = string.IsNullOrEmpty(element.name) ? "unnamed" : element.name;
        Debug.Log($"{indent}{element.GetType().Name} - '{name}'");

        foreach (var child in element.Children())
        {
            ListAllElements(child, depth + 1);
        }
    }

    #endregion
}
#endif
