using UnityEngine;

public class Launcher : MonoBehaviour
{
    // ============================================================
    // SLINGSHOT
    // ============================================================

    [Header("Slingshot")]

    public LineRenderer[] lineRenderers;

    public Transform[] stripPositions;

    public Transform center;

    public Transform idlePosition;


    // ============================================================
    // JELLY
    // ============================================================

    [Header("Jelly")]

    public GameObject[] JellyPrefabs;

    public float maxLength = 3f;

    public float bottomBoundary = -5f;

    public float jellyPositionOffset = 0.5f;


    // ============================================================
    // FORCE
    // ============================================================

    [Header("Launch Force")]

    public float minForce = 5f;

    public float maxForce = 15f;


    // ============================================================
    // TRAJECTORY
    // ============================================================

    [Header("Trajectory")]

    public GameObject TrajectoryDot;

    public int number;


    // ============================================================
    // CAMERA
    // ============================================================

    [Header("Camera")]

    public CameraFollow cameraFollow;


    // ============================================================
    // CURRENT JELLY
    // ============================================================

    private GameObject currentJelly;

    private Rigidbody2D[] jellyBodies;

    private Collider2D[] jellyColliders;


    // ============================================================
    // AIMING
    // ============================================================

    private Vector3 currentPosition;

    private bool isMouseDown;


    // ============================================================
    // TRAJECTORY DOTS
    // ============================================================

    private GameObject[] TrajectoryDots;


    // ============================================================
    // INTERACTION
    // ============================================================

    private bool canInteract = true;

    private bool waitingForFreshMousePress = false;


    // ============================================================
    // START
    // ============================================================

    void Start()
    {
        lineRenderers[0].positionCount = 2;

        lineRenderers[1].positionCount = 2;


        lineRenderers[0].SetPosition(
            0,
            stripPositions[0].position
        );

        lineRenderers[1].SetPosition(
            0,
            stripPositions[1].position
        );


        // Create trajectory dots
        TrajectoryDots =
            new GameObject[number];


        for (int i = 0; i < number; i++)
        {
            TrajectoryDots[i] =
                Instantiate(
                    TrajectoryDot,
                    transform
                );

            TrajectoryDots[i].SetActive(false);
        }


        CreateJelly();


        canInteract = true;


        // Require fresh click if mouse is already held
        if (Input.GetMouseButton(0))
        {
            waitingForFreshMousePress = true;
        }
    }


    // ============================================================
    // UPDATE
    // ============================================================

    void Update()
    {
        // ========================================================
        // WAIT FOR FRESH MOUSE PRESS
        // ========================================================

        if (waitingForFreshMousePress)
        {
            if (Input.GetMouseButton(0))
            {
                isMouseDown = false;

                HideTrajectory();

                ResetStrips();

                return;
            }


            waitingForFreshMousePress = false;
        }


        // ========================================================
        // LOCKED
        // ========================================================

        if (!canInteract)
        {
            isMouseDown = false;

            HideTrajectory();

            ResetStrips();

            return;
        }


        // ========================================================
        // NO JELLY
        // ========================================================

        if (currentJelly == null)
            return;


        // ========================================================
        // AIM
        // ========================================================

        if (isMouseDown)
        {
            Vector3 mousePosition =
                Input.mousePosition;

            mousePosition.z = 10f;


            currentPosition =
                Camera.main.ScreenToWorldPoint(
                    mousePosition
                );


            currentPosition =
                center.position +
                Vector3.ClampMagnitude(
                    currentPosition -
                    center.position,
                    maxLength
                );


            currentPosition =
                ClampBoundary(
                    currentPosition
                );


            MoveJelly(
                currentPosition
            );


            SetStrips(
                currentPosition
            );


            ShowTrajectory();
        }
        else
        {
            ResetStrips();

            HideTrajectory();
        }
    }


    // ============================================================
    // CREATE JELLY
    // ============================================================

    void CreateJelly()
    {
        if (JellyPrefabs == null ||
            JellyPrefabs.Length == 0)
        {
            Debug.LogError(
                "No Jelly Prefabs assigned!"
            );

            return;
        }


        GameObject selectedJelly =
            JellyPrefabs[
                Random.Range(
                    0,
                    JellyPrefabs.Length
                )
            ];


        currentJelly =
            Instantiate(
                selectedJelly,
                idlePosition.position,
                Quaternion.identity
            );


        jellyBodies =
            currentJelly.GetComponentsInChildren<Rigidbody2D>();


        jellyColliders =
            currentJelly.GetComponentsInChildren<Collider2D>();


        // Disable physics while aiming
        foreach (Rigidbody2D rb in jellyBodies)
        {
            rb.simulated = false;
        }


        // Disable collisions while aiming
        foreach (Collider2D col in jellyColliders)
        {
            col.enabled = false;
        }
    }


    // ============================================================
    // MOVE JELLY
    // ============================================================

    void MoveJelly(
        Vector3 position
    )
    {
        Vector3 direction =
            position -
            center.position;


        Vector3 targetPosition =
            position +
            direction.normalized *
            jellyPositionOffset;


        currentJelly.transform.position =
            targetPosition;


        currentJelly.transform.rotation =
            Quaternion.Euler(
                0f,
                0f,
                -direction.x * 15f
            );
    }


    // ============================================================
    // SHOOT
    // ============================================================

    void Shoot()
    {
        if (!canInteract)
            return;


        if (currentJelly == null)
            return;


        // ========================================================
        // CHECK JELLY LIMIT
        // ========================================================

        if (GameManager.Instance != null)
        {
            bool canUseJelly =
                GameManager.Instance.UseJelly();


            if (!canUseJelly)
            {
                Debug.Log(
                    "Cannot launch! No jellies remaining."
                );

                return;
            }
        }


        HideTrajectory();


        // ========================================================
        // FORCE
        // ========================================================

        float pullDistance =
            Vector2.Distance(
                center.position,
                currentPosition
            );


        float forcePercent =
            Mathf.Clamp01(
                pullDistance /
                maxLength
            );


        float launchForce =
            Mathf.Lerp(
                minForce,
                maxForce,
                forcePercent
            );


        Vector2 launchDirection =
            (
                center.position -
                currentPosition
            ).normalized;


        // ========================================================
        // ENABLE PHYSICS
        // ========================================================

        foreach (Rigidbody2D rb in jellyBodies)
        {
            rb.simulated = true;

            rb.linearVelocity =
                launchDirection *
                launchForce;
        }


        // ========================================================
        // ENABLE COLLIDERS
        // ========================================================

        foreach (Collider2D col in jellyColliders)
        {
            col.enabled = true;
        }


        // ========================================================
        // LOCK
        // ========================================================

        canInteract = false;

        isMouseDown = false;


        // ========================================================
        // CAMERA
        // ========================================================

        if (cameraFollow != null)
        {
            cameraFollow.FollowJelly(
                jellyBodies
            );
        }


        Debug.Log(
            "Shot Jelly: " +
            currentJelly.name
        );


        // ========================================================
        // CLEAR
        // ========================================================

        currentJelly = null;

        jellyBodies = null;

        jellyColliders = null;


        // ========================================================
        // CREATE NEXT JELLY
        // ========================================================

        Invoke(
            nameof(CreateJelly),
            2f
        );
    }


    // ============================================================
    // ENABLE INTERACTION
    // ============================================================

    public void EnableInteraction()
    {
        canInteract = true;


        if (Input.GetMouseButton(0))
        {
            waitingForFreshMousePress = true;
        }
        else
        {
            waitingForFreshMousePress = false;
        }


        Debug.Log(
            "Slingshot unlocked."
        );
    }


    // ============================================================
    // DISABLE INTERACTION
    // ============================================================

    public void DisableInteraction()
    {
        canInteract = false;

        isMouseDown = false;


        HideTrajectory();

        ResetStrips();


        if (Input.GetMouseButton(0))
        {
            waitingForFreshMousePress = true;
        }
    }


    // ============================================================
    // CAN INTERACT
    // ============================================================

    public bool CanInteract()
    {
        return canInteract;
    }


    // ============================================================
    // MOUSE DOWN
    // ============================================================

    private void OnMouseDown()
    {
        if (!canInteract)
            return;


        if (waitingForFreshMousePress)
            return;


        if (currentJelly == null)
            return;


        isMouseDown = true;
    }


    // ============================================================
    // MOUSE UP
    // ============================================================

    private void OnMouseUp()
    {
        if (!canInteract)
            return;


        if (waitingForFreshMousePress)
            return;


        isMouseDown = false;


        if (currentJelly != null)
        {
            Shoot();
        }
    }


    // ============================================================
    // RESET STRIPS
    // ============================================================

    void ResetStrips()
    {
        currentPosition =
            idlePosition.position;


        SetStrips(
            currentPosition
        );
    }


    // ============================================================
    // SET STRIPS
    // ============================================================

    void SetStrips(
        Vector3 position
    )
    {
        lineRenderers[0].SetPosition(
            1,
            position
        );

        lineRenderers[1].SetPosition(
            1,
            position
        );
    }


    // ============================================================
    // CLAMP
    // ============================================================

    Vector3 ClampBoundary(
        Vector3 vector
    )
    {
        vector.y =
            Mathf.Clamp(
                vector.y,
                bottomBoundary,
                1000f
            );


        return vector;
    }


    // ============================================================
    // SHOW TRAJECTORY
    // ============================================================

    void ShowTrajectory()
    {
        if (currentJelly == null)
            return;


        float pullDistance =
            Vector2.Distance(
                center.position,
                currentPosition
            );


        float forcePercent =
            Mathf.Clamp01(
                pullDistance /
                maxLength
            );


        float launchForce =
            Mathf.Lerp(
                minForce,
                maxForce,
                forcePercent
            );


        Vector2 launchDirection =
            (
                center.position -
                currentPosition
            ).normalized;


        Vector2 velocity =
            launchDirection *
            launchForce;


        Vector2 startPos =
            currentJelly.transform.position;


        for (int i = 0; i < number; i++)
        {
            float t =
                i * 0.07f;


            Vector2 point =
                startPos +
                velocity * t +
                0.5f *
                Physics2D.gravity *
                (t * t);


            TrajectoryDots[i].transform.position =
                point;


            TrajectoryDots[i].SetActive(true);
        }
    }


    // ============================================================
    // HIDE TRAJECTORY
    // ============================================================

    void HideTrajectory()
    {
        if (TrajectoryDots == null)
            return;


        for (int i = 0; i < number; i++)
        {
            if (TrajectoryDots[i] != null)
            {
                TrajectoryDots[i].SetActive(false);
            }
        }
    }
}