using System.Collections;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    [Header("Bubble Spawn Settings")]
    public GameObject bubblePrefab; 
    public Transform waterSurface; 
    public int maxBubbles = 10; 
    public float spawnInterval = 3f; 
    public Vector2 spawnOffsetRange = new Vector2(-10f, 10f); 

    private void Start()
    {
        StartCoroutine(SpawnBubblesRoutine());
    }

    private IEnumerator SpawnBubblesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            int currentBubbleCount = GameObject.FindGameObjectsWithTag("Bubble").Length;
            if (currentBubbleCount < maxBubbles)
            {
                SpawnBubble();
            }
        }
    }

    private void SpawnBubble()
    {
        if (bubblePrefab != null && waterSurface != null)
        {
            Vector3 randomPosition = waterSurface.position + new Vector3(
                Random.Range(spawnOffsetRange.x, spawnOffsetRange.y), 
                Random.Range(0, spawnOffsetRange.y),                 
                0
            );

            GameObject newBubble = Instantiate(bubblePrefab, randomPosition, Quaternion.identity);
            newBubble.tag = "Bubble"; 
        }
    }
}
