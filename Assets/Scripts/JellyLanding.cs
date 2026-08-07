using UnityEngine;

public class JellyLanding : MonoBehaviour
{
    private JellyManager jellyManager;


    void Start()
    {
        jellyManager = GetComponentInParent<JellyManager>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            jellyManager.Landed();
        }
    }
}