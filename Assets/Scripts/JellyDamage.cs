using System.Collections.Generic;
using UnityEngine;

public class JellyDamage : MonoBehaviour
{
    [Header("Damage")]
    public float damage = 10f;

    private HashSet<BlockHealth> damagedBlocks = new HashSet<BlockHealth>();

    public void TryDamageBlock(BlockHealth block)
    {
        if (block == null)
            return;

        // This Jelly already damaged this block
        if (damagedBlocks.Contains(block))
            return;

        block.TakeDamage(damage);

        damagedBlocks.Add(block);
    }
}