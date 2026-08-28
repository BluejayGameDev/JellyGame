using UnityEngine;

public class JellyBone : MonoBehaviour
{
    private JellyDamage jellyDamage;


    // ============================================================
    // AWAKE
    // ============================================================

    private void Awake()
    {
        jellyDamage = GetComponentInParent<JellyDamage>();
    }


    // ============================================================
    // COLLISION
    // ============================================================

    private void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (jellyDamage == null)
            return;


        BlockBone blockBone = collision.collider.GetComponent<BlockBone>();


        if (blockBone == null) return;


        BlockHealth blockHealth = blockBone.GetBlockHealth();


        if (blockHealth == null) return;


        jellyDamage.TryDamageBlock(blockHealth);
    }
}