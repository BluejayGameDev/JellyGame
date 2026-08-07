using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform launcherPosition;

    public float followSpeed = 5f;
    public float returnSpeed = 2f;

    public float groundWaitTime = 5f;

    private Rigidbody2D[] targetbodies;
    private bool returning = false;

    private float lockedY;

    private float velocityX;

    public Transform launcherIdlePosition;
    public float followStartDistance = 0.5f;

    private bool waitingForLaunch = false;

    private Coroutine returnCoroutine;

    void Start()
    {
        lockedY = transform.position.y;
    }

    void LateUpdate()
    {
        if (targetbodies == null) return;

        Vector2 average = Vector2.zero;

        foreach (Rigidbody2D rb in targetbodies)
        {
            average += rb.position;
        }

        average /= targetbodies.Length;

        // Wait until jelly passes the launcher
        if (waitingForLaunch)
        {
            if (average.x > launcherIdlePosition.position.x + followStartDistance)
            {
                waitingForLaunch = false;
            }
            else
            {
                return;
            }
        }

        float x = Mathf.SmoothDamp(transform.position.x, average.x, ref velocityX, 0.05f);

        transform.position = new Vector3(x, lockedY, transform.position.z);
    }

    public void FollowJelly(Rigidbody2D[] bodies)
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        targetbodies = bodies;

        waitingForLaunch = true;
        returning = false;
        velocityX = 0;
    }   

    public void JellyLanded()
    {
        if (returning) return;
        
        returnCoroutine = StartCoroutine(ReturnToLauncher());
    }

    IEnumerator ReturnToLauncher()
    {
        returning = true;

        yield return new WaitForSeconds(groundWaitTime);

        targetbodies = null;

        while (Vector3.Distance(transform.position, launcherPosition.position) > 0.05f)
        {
            float x = Mathf.Lerp(transform.position.x, launcherPosition.position.x, Time.deltaTime * returnSpeed);

            transform.position = new Vector3(x, lockedY, transform.position.z);

            yield return null;
        }

        transform.position = new Vector3(launcherPosition.position.x, lockedY, transform.position.z);

        returning = false;

        StopFollowing();
    }

    public void StopFollowing()
    {
        targetbodies = null;
    }
}