using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[System.Serializable]
public class DialogueData
{
    public string characterName;
    public string dialogueText;
    public Sprite characterSprite;
    public bool isPlayer;
    public float displayDuration = 3f; // How long to show dialogue, 0 = wait for input
}

public class DialoguePopupSystem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePopupPrefab;
    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private Transform dialogueParent;
    
    [Header("Settings")]
    [SerializeField] private float animationDuration = 0.3f;
    [SerializeField] private float fadeInAlpha = 0.9f;
    
    [Header("Font Settings")]
    [SerializeField] private TMP_FontAsset defaultFont; // Add this to assign a font asset
    
    private GameObject currentDialoguePopup;
    private bool isDialogueActive = false;
    
    private void Awake()
    {
        // Create dialogue canvas if not assigned
        if (dialogueCanvas == null)
            CreateDialogueCanvas();
    }
    
    private void CreateDialogueCanvas()
    {
        GameObject canvasObj = new GameObject("DialogueCanvas");
        dialogueCanvas = canvasObj.AddComponent<Canvas>();
        dialogueCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        dialogueCanvas.sortingOrder = 10; // In front of characters, behind audience
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        dialogueParent = dialogueCanvas.transform;
    }
    
    public void ShowDialogue(DialogueData dialogueData)
    {
        if (isDialogueActive)
            HideDialogue();
            
        // Ensure we're on the main thread and delay to next frame
        StartCoroutine(ShowDialogueDelayed(dialogueData));
    }
    
    private IEnumerator ShowDialogueDelayed(DialogueData dialogueData)
    {
        // Wait one frame to ensure we're properly on the main thread
        yield return null;
        
        yield return StartCoroutine(DisplayDialogue(dialogueData));
    }
    
    private IEnumerator DisplayDialogue(DialogueData dialogueData)
    {
        isDialogueActive = true;
        
        // Create popup from prefab or dynamically
        if (dialoguePopupPrefab != null)
        {
            currentDialoguePopup = Instantiate(dialoguePopupPrefab, dialogueParent);
        }
        else
        {
            currentDialoguePopup = CreateDialoguePopupDynamic();
        }
        
        // Wait one frame after creation before configuration
        yield return null;
        
        // Configure the popup
        ConfigureDialoguePopup(currentDialoguePopup, dialogueData);
        
        // Animate in
        yield return StartCoroutine(AnimateDialogueIn(currentDialoguePopup));
        
        // Wait for duration or input
        if (dialogueData.displayDuration > 0)
        {
            yield return new WaitForSeconds(dialogueData.displayDuration);
            HideDialogue();
        }
        // If displayDuration is 0, dialogue stays until manually hidden
    }
    
    private GameObject CreateDialoguePopupDynamic()
    {
        GameObject popup = new GameObject("DialoguePopup");
        popup.transform.SetParent(dialogueParent, false);
        
        // Add main panel
        Image panelImage = popup.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        RectTransform popupRect = popup.GetComponent<RectTransform>();
        popupRect.anchorMin = new Vector2(0f, 0f);
        popupRect.anchorMax = new Vector2(1f, 0.3f);
        popupRect.offsetMin = Vector2.zero;
        popupRect.offsetMax = Vector2.zero;
        
        // Character sprite
        GameObject spriteObj = new GameObject("CharacterSprite");
        spriteObj.transform.SetParent(popup.transform, false);
        Image spriteImage = spriteObj.AddComponent<Image>();
        spriteImage.preserveAspect = true;
        
        RectTransform spriteRect = spriteImage.GetComponent<RectTransform>();
        spriteRect.sizeDelta = new Vector2(200, 200);
        
        // Dialogue text - configure more carefully
        GameObject textObj = new GameObject("DialogueText");
        textObj.transform.SetParent(popup.transform, false);
        TextMeshProUGUI dialogueText = textObj.AddComponent<TextMeshProUGUI>();
        
        // Set font asset if available to prevent font loading issues
        if (defaultFont != null)
        {
            dialogueText.font = defaultFont;
        }
        
        dialogueText.fontSize = 24;
        dialogueText.color = Color.white;
        dialogueText.alignment = TextAlignmentOptions.TopLeft;
        dialogueText.text = ""; // Initialize with empty text
        
        RectTransform textRect = dialogueText.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.2f, 0.1f);
        textRect.anchorMax = new Vector2(0.9f, 0.9f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Character name - configure more carefully
        GameObject nameObj = new GameObject("CharacterName");
        nameObj.transform.SetParent(popup.transform, false);
        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        
        // Set font asset if available
        if (defaultFont != null)
        {
            nameText.font = defaultFont;
        }
        
        nameText.fontSize = 18;
        nameText.color = Color.yellow;
        nameText.fontStyle = FontStyles.Bold;
        nameText.text = ""; // Initialize with empty text
        
        RectTransform nameRect = nameText.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.2f, 0.8f);
        nameRect.anchorMax = new Vector2(0.9f, 1f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        return popup;
    }
    
    private void ConfigureDialoguePopup(GameObject popup, DialogueData dialogueData)
    {
        // Get components with null checks
        Transform characterSpriteTransform = popup.transform.Find("CharacterSprite");
        Transform dialogueTextTransform = popup.transform.Find("DialogueText");
        Transform nameTextTransform = popup.transform.Find("CharacterName");
        
        if (characterSpriteTransform == null || dialogueTextTransform == null || nameTextTransform == null)
        {
            Debug.LogError("DialoguePopupSystem: Missing required child components in popup");
            return;
        }
        
        Image characterSprite = characterSpriteTransform.GetComponent<Image>();
        TextMeshProUGUI dialogueText = dialogueTextTransform.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameText = nameTextTransform.GetComponent<TextMeshProUGUI>();
        
        if (characterSprite == null || dialogueText == null || nameText == null)
        {
            Debug.LogError("DialoguePopupSystem: Missing required components");
            return;
        }
        
        // Set content safely
        characterSprite.sprite = dialogueData.characterSprite;
        
        // Set text content with null checks
        if (!string.IsNullOrEmpty(dialogueData.dialogueText))
            dialogueText.text = dialogueData.dialogueText;
        
        if (!string.IsNullOrEmpty(dialogueData.characterName))
            nameText.text = dialogueData.characterName;
        
        // Position based on player vs NPC
        RectTransform spriteRect = characterSprite.GetComponent<RectTransform>();
        RectTransform textRect = dialogueText.GetComponent<RectTransform>();
        RectTransform nameRect = nameText.GetComponent<RectTransform>();
        
        if (dialogueData.isPlayer)
        {
            // Left aligned for player
            spriteRect.anchorMin = new Vector2(0.05f, 0.2f);
            spriteRect.anchorMax = new Vector2(0.05f, 0.2f);
            spriteRect.anchoredPosition = Vector2.zero;
            
            textRect.anchorMin = new Vector2(0.25f, 0.1f);
            textRect.anchorMax = new Vector2(0.9f, 0.8f);
            
            nameRect.anchorMin = new Vector2(0.25f, 0.8f);
            nameRect.anchorMax = new Vector2(0.9f, 1f);
            
            dialogueText.alignment = TextAlignmentOptions.TopLeft;
            nameText.alignment = TextAlignmentOptions.TopLeft;
        }
        else
        {
            // Right aligned for NPCs/enemies
            spriteRect.anchorMin = new Vector2(0.95f, 0.2f);
            spriteRect.anchorMax = new Vector2(0.95f, 0.2f);
            spriteRect.anchoredPosition = Vector2.zero;
            
            textRect.anchorMin = new Vector2(0.1f, 0.1f);
            textRect.anchorMax = new Vector2(0.75f, 0.8f);
            
            nameRect.anchorMin = new Vector2(0.1f, 0.8f);
            nameRect.anchorMax = new Vector2(0.75f, 1f);
            
            dialogueText.alignment = TextAlignmentOptions.TopRight;
            nameText.alignment = TextAlignmentOptions.TopRight;
        }
    }
    
    private IEnumerator AnimateDialogueIn(GameObject popup)
    {
        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = popup.AddComponent<CanvasGroup>();
            
        canvasGroup.alpha = 0f;
        popup.transform.localScale = Vector3.zero;
        
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float progress = elapsedTime / animationDuration;
            canvasGroup.alpha = Mathf.Lerp(0f, fadeInAlpha, progress);
            popup.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, progress);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        canvasGroup.alpha = fadeInAlpha;
        popup.transform.localScale = Vector3.one;
    }
    
    public void HideDialogue()
    {
        if (currentDialoguePopup != null && isDialogueActive)
        {
            StartCoroutine(AnimateDialogueOut());
        }
    }
    
    private IEnumerator AnimateDialogueOut()
    {
        CanvasGroup canvasGroup = currentDialoguePopup.GetComponent<CanvasGroup>();
        
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            float progress = elapsedTime / animationDuration;
            canvasGroup.alpha = Mathf.Lerp(fadeInAlpha, 0f, progress);
            currentDialoguePopup.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progress);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        Destroy(currentDialoguePopup);
        currentDialoguePopup = null;
        isDialogueActive = false;
    }
    
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
    
    // Quick test methods
    public void TestPlayerDialogue()
    {
        DialogueData testData = new DialogueData
        {
            characterName = "Actor",
            dialogueText = "The stage lights are bright tonight...",
            isPlayer = true,
            displayDuration = 3f
        };
        ShowDialogue(testData);
    }
    
    public void TestNPCDialogue()
    {
        DialogueData testData = new DialogueData
        {
            characterName = "Audience Member",
            dialogueText = "What an interesting performance!",
            isPlayer = false,
            displayDuration = 3f
        };
        ShowDialogue(testData);
    }
}