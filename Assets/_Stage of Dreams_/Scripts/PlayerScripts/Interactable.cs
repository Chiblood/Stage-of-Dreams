using UnityEngine;

/// <summary>
/// Base class for objects that can be interacted with by the player.
/// This provides a simple interface for 2D interactables in your game.
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    [Header("Interactable Settings")]
    [SerializeField] protected string interactionText = "Interact";
    [SerializeField] protected bool canInteract = true;
    [SerializeField] protected float cooldownTime = 0f;
    
    // Internal state
    private float lastInteractionTime = 0f;
    
    /// <summary>
    /// The text to display in interaction prompts
    /// </summary>
    public virtual string InteractionText => interactionText;
    
    /// <summary>
    /// Whether this object can currently be interacted with
    /// </summary>
    public virtual bool CanInteract => canInteract && (Time.time - lastInteractionTime >= cooldownTime);
    
    /// <summary>
    /// Called when the player interacts with this object
    /// </summary>
    public void Interact()
    {
        if (!CanInteract) return;
        
        lastInteractionTime = Time.time;
        OnInteract();
    }
    
    /// <summary>
    /// Override this method to implement specific interaction behavior
    /// </summary>
    protected abstract void OnInteract();
    
    /// <summary>
    /// Called when the player enters interaction range (optional override)
    /// </summary>
    public virtual void OnPlayerEnterRange()
    {
        // Override in derived classes for custom behavior
    }
    
    /// <summary>
    /// Called when the player leaves interaction range (optional override)
    /// </summary>
    public virtual void OnPlayerExitRange()
    {
        // Override in derived classes for custom behavior
    }
    
    /// <summary>
    /// Enable or disable interaction capability
    /// </summary>
    public virtual void SetInteractionEnabled(bool enabled)
    {
        canInteract = enabled;
    }
}

/// <summary>
/// Example implementation of an interactable object
/// </summary>
public class SimpleInteractable : Interactable
{
    [Header("Simple Interactable")]
    [SerializeField] private string debugMessage = "Hello from Simple Interactable!";
    [SerializeField] private UnityEngine.Events.UnityEvent onInteracted;
    
    protected override void OnInteract()
    {
        Debug.Log($"{gameObject.name}: {debugMessage}");
        onInteracted?.Invoke();
    }
    
    public override void OnPlayerEnterRange()
    {
        Debug.Log($"Player entered range of {gameObject.name}");
    }
    
    public override void OnPlayerExitRange()
    {
        Debug.Log($"Player left range of {gameObject.name}");
    }
}

/// <summary>
/// An interactable that can toggle something on/off
/// </summary>
public class ToggleInteractable : Interactable
{
    [Header("Toggle Settings")]
    [SerializeField] private bool isOn = false;
    [SerializeField] private string onText = "Turn Off";
    [SerializeField] private string offText = "Turn On";
    [SerializeField] private UnityEngine.Events.UnityEvent onToggleOn;
    [SerializeField] private UnityEngine.Events.UnityEvent onToggleOff;
    
    public override string InteractionText => isOn ? onText : offText;
    
    protected override void OnInteract()
    {
        isOn = !isOn;
        
        if (isOn)
        {
            Debug.Log($"{gameObject.name} turned ON");
            onToggleOn?.Invoke();
        }
        else
        {
            Debug.Log($"{gameObject.name} turned OFF");
            onToggleOff?.Invoke();
        }
    }
    
    public bool IsOn => isOn;
    
    public void SetState(bool state)
    {
        isOn = state;
    }
}