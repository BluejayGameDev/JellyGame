using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyDamage : MonoBehaviour
{
    // ============================================================
    // DAMAGE
    // ============================================================

    [Header("Damage")]

    public float damage = 10f;


    // ============================================================
    // DESTRUCTION
    // ============================================================

    [Header("Jelly Destruction")]

    public float destroyDelay = 2f;


    // ============================================================
    // DAMAGED BLOCKS
    // ============================================================

    private HashSet<BlockHealth> damagedBlocks = new HashSet<BlockHealth>();

    // ============================================================
    // STATE
    // ============================================================

    private bool hasHitBlock = false;

    private bool isDestroying = false;


    // ============================================================
    // DAMAGE BLOCK
    // ============================================================

    public void TryDamageBlock(BlockHealth block)
    {
        if (block == null) return;

        if (block.IsDestroyed()) return;

        // This jelly has already damaged this block
        if (damagedBlocks.Contains(block)) return;


        // ========================================================
        // DAMAGE
        // ========================================================

        block.TakeDamage(damage);

        damagedBlocks.Add(block);


        // ========================================================
        // ACTIVATE BLOCK SOFT BODY
        // ========================================================

        JellyBlock jellyBlock = block.GetComponent<JellyBlock>();

        if (jellyBlock != null)
        {
            jellyBlock.ActivateJelly(Vector2.zero);
        }


        // ========================================================
        // FIRST BLOCK HIT
        // ========================================================

        if (!hasHitBlock)
        {
            hasHitBlock = true;

            StartCoroutine(DestroyJellyAfterDelay());
        }
    }


    // ============================================================
    // DESTROY JELLY AFTER DELAY
    // ============================================================

    private IEnumerator DestroyJellyAfterDelay()
    {
        if (isDestroying)
            yield break;


        isDestroying = true;


        yield return new WaitForSeconds(
            destroyDelay
        );


        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}