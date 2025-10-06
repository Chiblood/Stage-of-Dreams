using UnityEngine;

public class Spotlight : MonoBehaviour
{
    [Header("Spotlight Settings")]
    public float radius = 1.5f;
    
    [Header("Dependencies")]
    [SerializeField] private PlayerScript _player;
    
    private bool _wasInSpotlight = false;

    private void Awake()
    {
        // Find player if not assigned
        if (_player == null)
            _player = FindFirstObjectByType<PlayerScript>();
            
        // Set center position to this object's position
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    private void Update()
    {
        if (_player != null)
        {
            CheckPlayerInSpotlight(_player);
        }
    }

    public void CheckPlayerInSpotlight(PlayerScript player)
    {
        if (player == null) return;
        
        Vector2 playerPos = player.transform.position;
        Vector2 centerPosition = transform.position;
        bool isInSpotlight = Vector2.Distance(centerPosition, playerPos) <= radius;
        
        // Only update if state changed
        if (isInSpotlight != _wasInSpotlight)
        {
            player.inSpotlight = isInSpotlight;
            _wasInSpotlight = isInSpotlight;
            
            if (isInSpotlight)
            {
                OnPlayerEnteredSpotlight(player);
            }
            else
            {
                OnPlayerExitedSpotlight(player);
            }
        }
    }
    
    private void OnPlayerEnteredSpotlight(PlayerScript player)
    {
        Debug.Log("Player entered spotlight - Begin turn-based mode!");
        // TODO: Trigger turn-based gameplay start
        // You could send an event, call a game manager, etc.
    }
    
    private void OnPlayerExitedSpotlight(PlayerScript player)
    {
        Debug.Log("Player exited spotlight - Return to exploration mode!");
        // TODO: Return to normal gameplay
    }
    
    // Optional: Draw the spotlight radius in the scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
