using UnityEngine;

public class JellyBlock : MonoBehaviour
{
    [Header("Normal Collider")]
    [SerializeField] private BoxCollider2D normalCollider;

    [Header("Jelly Colliders")]
    [SerializeField] private CircleCollider2D[] jellyColliders;

    [Header("Jelly Rigidbody2Ds")]
    [SerializeField] private Rigidbody2D[] jellyRigidbodies;

    [Header("Main Physics")]
    [SerializeField] private Rigidbody2D rb;

    [Header("Impact Settings")]
    [SerializeField] private float impactThreshold = 5f;

    private bool isJelly = false;

    private void Awake()
    {
        // ============================================================
        // NORMAL BLOCK STATE
        // ============================================================

        // Enable the normal BoxCollider
        if (normalCollider != null)
        {
            normalCollider.enabled = true;
        }

        // Disable all jelly colliders
        foreach (CircleCollider2D collider in jellyColliders)
        {
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        // Keep the main block standing upright
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        // Freeze all the jelly bones
        foreach (Rigidbody2D jellyRb in jellyRigidbodies)
        {
            if (jellyRb != null)
            {
                jellyRb.simulated = true;
                jellyRb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Don't activate jelly more than once
        if (isJelly)
            return;

        // Get the strength of the impact
        float impactForce = collision.relativeVelocity.magnitude;

        // Check if the impact was strong enough
        if (impactForce >= impactThreshold)
        {
            ActivateJelly(collision.relativeVelocity);
        }
    }

    private void ActivateJelly(Vector2 impactVelocity)
    {
        isJelly = true;

        // ============================================================
        // DISABLE NORMAL BLOCK
        // ============================================================

        if (normalCollider != null)
        {
            normalCollider.enabled = false;
        }

        // ============================================================
        // ENABLE JELLY COLLIDERS
        // ============================================================

        foreach (CircleCollider2D collider in jellyColliders)
        {
            if (collider != null)
            {
                collider.enabled = true;
            }
        }

        // ============================================================
        // ENABLE JELLY PHYSICS
        // ============================================================

        foreach (Rigidbody2D jellyRb in jellyRigidbodies)
        {
            if (jellyRb != null)
            {
                jellyRb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        // Allow the main Rigidbody to rotate
        if (rb != null)
        {
            rb.freezeRotation = false;
        }
    }
}