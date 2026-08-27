using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    // ============================================================
    // CAMERA VIEW POINTS
    // ============================================================

    [Header("Camera View Points")]

    public Transform launcherViewPoint;

    public Transform levelViewPoint;


    // ============================================================
    // CAMERA ZOOM
    // ============================================================

    [Header("Camera Zoom")]

    public float launcherZoomSize = 5f;

    public float levelViewZoomSize = 12f;

    public float levelViewSpeed = 2f;


    // ============================================================
    // JELLY FOLLOWING
    // ============================================================

    [Header("Jelly Following")]

    public float followSpeed = 5f;

    public Transform launcherIdlePosition;

    public float followStartDistance = 0.5f;


    // ============================================================
    // RETURNING
    // ============================================================

    [Header("Returning")]

    public float returnSpeed = 2f;

    public float groundWaitTime = 5f;


    // ============================================================
    // REFERENCES
    // ============================================================

    [Header("References")]

    public Launcher launcher;


    // ============================================================
    // CAMERA VARIABLES
    // ============================================================

    private Rigidbody2D[] targetbodies;

    private bool returning = false;

    private bool waitingForLaunch = false;

    private bool viewingLevel = false;

    private bool changingView = false;

    private float lockedY;

    private float velocityX;


    // ============================================================
    // COROUTINES
    // ============================================================

    private Coroutine returnCoroutine;

    private Coroutine levelViewCoroutine;


    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        lockedY = transform.position.y;


        Camera cam = GetComponent<Camera>();

        if (cam != null)
        {
            cam.orthographicSize = launcherZoomSize;
        }


        // Start at launcher view point
        if (launcherViewPoint != null)
        {
            transform.position = new Vector3(launcherViewPoint.position.x, launcherViewPoint.position.y, transform.position.z);

            lockedY = launcherViewPoint.position.y;
        }
    }


    // ============================================================
    // UPDATE
    // ============================================================

    void Update()
    {
        // ========================================================
        // LEVEL VIEW
        // ========================================================

        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleLevelView();
        }


        // ========================================================
        // SPACE
        // ========================================================

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReturnToLauncherNow();
        }
    }


    // ============================================================
    // LATE UPDATE
    // ============================================================

    void LateUpdate()
    {
        if (viewingLevel || changingView) return;

        if (targetbodies == null) return;

        Vector2 average = Vector2.zero;

        int validBodies = 0;


        // ========================================================
        // FIND AVERAGE JELLY POSITION
        // ========================================================

        foreach (Rigidbody2D rb in targetbodies)
        {
            if (rb != null)
            {
                average += rb.position;

                validBodies++;
            }
        }

        if (validBodies == 0)
        {
            targetbodies = null;

            return;
        }

        average /= validBodies;


        // ========================================================
        // WAIT UNTIL JELLY PASSES LAUNCHER
        // ========================================================

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


        // ========================================================
        // FOLLOW JELLY
        // ========================================================

        float x = Mathf.SmoothDamp(transform.position.x, average.x, ref velocityX, 0.05f);


        transform.position = new Vector3(x, lockedY, transform.position.z);
    }


    // ============================================================
    // CAN USE LEVEL VIEW?
    // ============================================================

    bool CanUseLevelView()
    {
        if (viewingLevel)
        {
            return true;
        }

        if (changingView)
        {
            return false;
        }

        if (returning)
        {
            return false;
        }

        if (targetbodies != null)
        {
            return false;
        }

        if (waitingForLaunch)
        {
            return false;
        }

        if (launcher == null)
        {
            return false;
        }

        if (!launcher.CanInteract())
        {
            return false;
        }

        return true;
    }


    // ============================================================
    // TOGGLE LEVEL VIEW
    // ============================================================

    public void ToggleLevelView()
    {
        if (viewingLevel)
        {
            ReturnFromLevelView();

            return;
        }

        if (!CanUseLevelView())
        {
            return;
        }

        GoToLevelView();
    }


    // ============================================================
    // GO TO LEVEL VIEW
    // ============================================================

    public void GoToLevelView()
    {
        if (levelViewPoint == null)
        {
            return;
        }


        if (changingView) return;

        viewingLevel = true;

        changingView = true;

        targetbodies = null;

        waitingForLaunch = false;

        //Lock Slingshot
        if (launcher != null)
        {
            launcher.DisableInteraction();
        }


        if (levelViewCoroutine != null)
        {
            StopCoroutine(levelViewCoroutine);
        }


        levelViewCoroutine = StartCoroutine(MoveToLevelView());
    }


    // ============================================================
    // MOVE TO LEVEL VIEW
    // ============================================================

    IEnumerator MoveToLevelView()
    {
        Camera cam = GetComponent<Camera>();

        Vector3 startPosition = transform.position;


        Vector3 targetPosition = new Vector3(levelViewPoint.position.x, levelViewPoint.position.y, transform.position.z);


        float startZoom = cam.orthographicSize;

        float elapsed = 0f;

        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * levelViewSpeed;

            float t = Mathf.SmoothStep(0f, 1f, elapsed);

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            cam.orthographicSize = Mathf.Lerp(startZoom, levelViewZoomSize, t);

            yield return null;
        }


        transform.position = targetPosition;

        cam.orthographicSize = levelViewZoomSize;

        changingView = false;
    }


    // ============================================================
    // RETURN FROM LEVEL VIEW
    // ============================================================

    public void ReturnFromLevelView()
    {
        if (changingView) return;

        viewingLevel = false;

        changingView = true;


        if (levelViewCoroutine != null)
        {
            StopCoroutine(levelViewCoroutine);
        }


        levelViewCoroutine = StartCoroutine(ReturnFromLevelViewCoroutine());
    }

    IEnumerator ReturnFromLevelViewCoroutine()
    {
        Camera cam = GetComponent<Camera>();

        Vector3 startPosition = transform.position;

        Vector3 targetPosition = new Vector3(launcherViewPoint.position.x, launcherViewPoint.position.y, transform.position.z);


        float startZoom = cam.orthographicSize;


        float elapsed = 0f;


        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * levelViewSpeed;

            float t =Mathf.SmoothStep(0f, 1f, elapsed);


            transform.position = Vector3.Lerp(startPosition, targetPosition, t);


            cam.orthographicSize = Mathf.Lerp(startZoom, launcherZoomSize, t);


            yield return null;
        }


        transform.position = targetPosition;

        cam.orthographicSize = launcherZoomSize;

        changingView = false;

        // Unlock slingshot
        if (launcher != null)
        {
            launcher.EnableInteraction();
        }
    }


    // ============================================================
    // FOLLOW JELLY
    // ============================================================

    public void FollowJelly(Rigidbody2D[] bodies)
    {
        if (viewingLevel) return;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);

            returnCoroutine = null;
        }

        targetbodies = bodies;

        waitingForLaunch = true;

        returning = false;

        velocityX = 0f;

        // Lock slingshot
        if (launcher != null)
        {
            launcher.DisableInteraction();
        }
    }


    public void JellyLanded()
    {
        if (returning) return;

        returnCoroutine = StartCoroutine(ReturnAfterDelay());
    }


    // ============================================================
    // NATURAL RETURN
    // ============================================================

    IEnumerator ReturnAfterDelay()
    {
        yield return new WaitForSeconds(groundWaitTime);

        StartReturnToLauncher();
    }


    // ============================================================
    // SPACE RETURN
    // ============================================================

    public void ReturnToLauncherNow()
    {
        if (viewingLevel)
        {
            ReturnFromLevelView();

            return;
        }

        // Don't do anything if already returning
        if (returning) return;


        // Don't return if already at launcher
        if (targetbodies == null) return;


        StartReturnToLauncher();
    }


    void StartReturnToLauncher()
    {
        if (returning) return;


        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }


        returnCoroutine = StartCoroutine(ReturnToLauncher());
    }


    IEnumerator ReturnToLauncher()
    {
        returning = true;

        targetbodies = null;

        waitingForLaunch = false;


        if (launcherViewPoint == null)
        {
            returning = false;

            yield break;
        }


        Vector3 targetPosition = new Vector3(launcherViewPoint.position.x, launcherViewPoint.position.y, transform.position.z);

        while (Vector2.Distance(transform.position, targetPosition) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * returnSpeed);

            yield return null;
        }


        transform.position = targetPosition;

        returning = false;

        returnCoroutine = null;

        // Unlock slingshot
        if (launcher != null)
        {
            launcher.EnableInteraction();
        }
    }


    public void StopFollowing()
    {
        targetbodies = null;

        waitingForLaunch = false;
    }
}