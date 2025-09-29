using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private DialogueData[] dialogueSequence;
    [SerializeField] private bool triggerOnSpotlight = true;
    [SerializeField] private bool triggerOnInteraction = false;
    
    private DialoguePopupSystem dialogueSystem;
    private PlayerScript player;
    private int currentDialogueIndex = 0;
    private bool hasTriggered = false;
    
    private void Start()
    {
        dialogueSystem = FindObjectOfType<DialoguePopupSystem>();
        if (dialogueSystem == null)
        {
            // Create one if it doesn't exist
            GameObject dialogueObj = new GameObject("DialogueSystem");
            dialogueSystem = dialogueObj.AddComponent<DialoguePopupSystem>();
        }
        
        player = FindObjectOfType<PlayerScript>();
    }
    
    private void Update()
    {
        if (triggerOnSpotlight && player != null && player.inSpotlight && !hasTriggered)
        {
            TriggerDialogue();
        }
        
        if (triggerOnInteraction && Input.GetKeyDown(KeyCode.E) && IsPlayerNearby())
        {
            TriggerDialogue();
        }
    }
    
    private void TriggerDialogue()
    {
        if (dialogueSequence.Length > 0 && currentDialogueIndex < dialogueSequence.Length)
        {
            dialogueSystem.ShowDialogue(dialogueSequence[currentDialogueIndex]);
            currentDialogueIndex++;
            
            if (currentDialogueIndex >= dialogueSequence.Length)
            {
                hasTriggered = true; // Prevent re-triggering
            }
        }
    }
    
    private bool IsPlayerNearby()
    {
        if (player == null) return false;
        return Vector2.Distance(transform.position, player.transform.position) <= 2f;
    }
    
    public void ResetDialogue()
    {
        currentDialogueIndex = 0;
        hasTriggered = false;
    }
}