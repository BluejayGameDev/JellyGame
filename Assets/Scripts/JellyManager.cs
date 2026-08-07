using UnityEngine;

public class JellyManager : MonoBehaviour
{
    public bool hasLanded = false;

    private CameraFollow cameraFollow;
    private Rigidbody2D[] bodies;

    public float stopSpeed = 0.2f;
    public float stopTime = 1f;

    private float stoppedTimer;

    public float destroyDelay = 5f;


    void Start()
    {
        cameraFollow = FindAnyObjectByType<CameraFollow>();

        bodies = GetComponentsInChildren<Rigidbody2D>();
    }


    void Update()
    {
        if (hasLanded)
            return;


        float totalSpeed = 0f;

        foreach (Rigidbody2D rb in bodies)
        {
            totalSpeed += rb.linearVelocity.magnitude;
        }


        float averageSpeed = totalSpeed / bodies.Length;


        if (averageSpeed < stopSpeed)
        {
            stoppedTimer += Time.deltaTime;

            if (stoppedTimer >= stopTime)
            {
                Landed();
            }
        }
        else
        {
            stoppedTimer = 0;
        }
    }


    public void Landed()
    {
        if (hasLanded) return;

        hasLanded = true;

        cameraFollow.JellyLanded();

        Debug.Log("Jelly stopped");
    }
}