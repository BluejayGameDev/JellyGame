using UnityEngine;

public class BlockBone : MonoBehaviour
{
    private BlockHealth blockHealth;

    private void Awake()
    {
        blockHealth = GetComponentInParent<BlockHealth>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        JellyDamage jelly = collision.collider.GetComponentInParent<JellyDamage>();

        if (jelly == null)
            return;

        if (blockHealth == null)
            return;

        jelly.TryDamageBlock(blockHealth);
    }
}