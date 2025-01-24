using UnityEngine;

public class BubbleAI : MonoBehaviour
{
    [Header("Bubble Settings")]
    public float oxygenBoost = 25f; 
    public float lifetime = 10f;    

    private void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            WaterSurfaceDetector playerOxygenSystem = collision.GetComponent<WaterSurfaceDetector>();
            if (playerOxygenSystem != null)
            {
                playerOxygenSystem.AddOxygen(oxygenBoost); 
                Debug.Log($"Player mendapat tambahan oksigen: {oxygenBoost}");
            }

            Destroy(gameObject); 
        }
    }
}
