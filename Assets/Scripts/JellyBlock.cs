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

    [Header("Fall Damage")]
    [SerializeField] private float fallDamageThreshold = 3f;
    [SerializeField] private float maxFallDamageVelocity = 12f;
    [SerializeField] private float maxFallDamage = 40f;

    [Header("Soft Body")]
    [SerializeField] private bool activateSoftBodyOnImpact = true;

    private bool isJelly = false;

    private void Awake()
    {
        // ============================================================
        // NORMAL BLOCK STATE
        // ============================================================

        if (normalCollider != null)
        {
            normalCollider.enabled = true;
        }

        foreach (CircleCollider2D collider in jellyColliders)
        {
            if (collider != null)
            {
                collider.enabled = false;
            }
        }

        foreach (Rigidbody2D jellyRb in jellyRigidbodies)
        {
            if (jellyRb != null)
            {
                jellyRb.simulated = true;
                jellyRb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }


    // ============================================================
    // IMPACT DETECTION
    // ============================================================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float impactForce = collision.relativeVelocity.magnitude;


        // ========================================================
        // ACTIVATE SOFT BODY
        // ========================================================

        if (!isJelly && activateSoftBodyOnImpact && impactForce >= impactThreshold)
        {
            ActivateJelly(collision.relativeVelocity);
        }


        // ========================================================
        // FALL / IMPACT DAMAGE
        // ========================================================

        if (impactForce >= fallDamageThreshold)
        {
            ApplyImpactDamage(impactForce);
        }
    }


    // ============================================================
    // ACTIVATE SOFT BODY
    // ============================================================

    public void ActivateJelly(Vector2 impactVelocity)
    {
        // Don't activate twice
        if (isJelly) return;


        isJelly = true;


        // ========================================================
        // DISABLE NORMAL BLOCK
        // ========================================================

        if (normalCollider != null)
        {
            normalCollider.enabled = false;
        }


        // ========================================================
        // ENABLE JELLY COLLIDERS
        // ========================================================

        foreach (CircleCollider2D collider in jellyColliders)
        {
            if (collider != null)
            {
                collider.enabled = true;
            }
        }


        // ========================================================
        // ENABLE JELLY PHYSICS
        // ========================================================

        foreach (Rigidbody2D jellyRb in jellyRigidbodies)
        {
            if (jellyRb != null)
            {
                jellyRb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

    }


    // ============================================================
    // IMPACT DAMAGE
    // ============================================================

    private void ApplyImpactDamage(float impactVelocity)
    {
        BlockHealth health =
            GetComponent<BlockHealth>();


        if (health == null)
            return;


        // Don't damage the block from tiny impacts
        if (impactVelocity < fallDamageThreshold)
            return;


        // Convert velocity into 0-1 range
        float damagePercent = Mathf.InverseLerp(fallDamageThreshold, maxFallDamageVelocity, impactVelocity);


        float damage = Mathf.Lerp(0f, maxFallDamage, damagePercent);


        if (damage <= 0f) return;


        health.TakeDamage(damage);
    }

    // ============================================================
    // CHECK STATE
    // ============================================================

    public bool IsJellyActive()
    {
        return isJelly;
    }
}