using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiChes : MonoBehaviour
{
    [Header("Movement Settings")]
    public float wanderSpeed = 2f; 
    public float chaseSpeed = 3.5f; 
    public float fleeSpeed = 4f; 
    public float wanderRadius = 5f; 
    public float directionChangeInterval = 2f; 

    [Header("Attack Settings")]
    public float attackRadius = 3f; 
    public float fleeDistance = 5f; 
    public float stunDuration = 2f;

    private Vector2 wanderDirection; 
    private Transform playerTransform; 
    private Rigidbody2D rb; 
    private bool isFleeing = false; 
    private bool isAttacking = false; 

    [Header("Boundary Settings")]
    public BoxCollider2D boundaryCollider;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        StartCoroutine(ChangeWanderDirection());
    }

    private void Update()
    {
        if (playerTransform == null || isFleeing || isAttacking) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < attackRadius)
        {
            StartCoroutine(AttackPlayer());
        }
        else if (distanceToPlayer < wanderRadius)
        {
            ChasePlayer();
        }
        else
        {
            Wander();
        }
        RestrictMovementWithinBoundary();
    }

    private void Wander()
    {
        rb.velocity = wanderDirection * wanderSpeed;

        float angle = Mathf.Atan2(wanderDirection.y, wanderDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator ChangeWanderDirection()
    {
        while (!isFleeing && !isAttacking)
        {
            wanderDirection = Random.insideUnitCircle.normalized;
            yield return new WaitForSeconds(directionChangeInterval);
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * chaseSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator AttackPlayer()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        Debug.Log("Jellyfish attacking the player!");
        PlayerController2 playerController = playerTransform.GetComponent<PlayerController2>();
        if (playerController != null)
        {
            playerController.Stun(stunDuration);
        }

        yield return new WaitForSeconds(stunDuration / 2);

        StartFleeing();
    }

    private void StartFleeing()
    {
        isAttacking = false;
        isFleeing = true;

        Vector2 fleeDirection = (transform.position - playerTransform.position).normalized;
        rb.velocity = fleeDirection * fleeSpeed;

        StartCoroutine(StopFleeing());
    }

    private IEnumerator StopFleeing()
    {

        while (Vector2.Distance(transform.position, playerTransform.position) < fleeDistance)
        {
            yield return null;
        }

        isFleeing = false;
        rb.velocity = Vector2.zero;
    }
    void RestrictMovementWithinBoundary()
    {
        if (boundaryCollider != null)
        {
            Bounds bounds = boundaryCollider.bounds;

            Vector3 currentPosition = transform.position;

            float clampedX = Mathf.Clamp(currentPosition.x, bounds.min.x, bounds.max.x);
            float clampedY = Mathf.Clamp(currentPosition.y, bounds.min.y, bounds.max.y);

            transform.position = new Vector3(clampedX, clampedY, currentPosition.z);
        }
        else
        {
            Debug.LogWarning("Boundary Collider belum diatur! Ikan tidak akan dibatasi pergerakannya.");
        }
    }
}