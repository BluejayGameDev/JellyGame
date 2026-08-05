using UnityEngine;

public class Launcher : MonoBehaviour
{
    public LineRenderer[] lineRenderers;
    public Transform[] stripPositions;
    public Transform center;
    public Transform idlePosition;

    public GameObject[] JellyPrefabs;

    public float maxLength = 3f;
    public float bottomBoundary = -5f;
    public float jellyPositionOffset = 0.5f;

    public float minForce = 5f;
    public float maxForce = 15f;

    private GameObject currentJelly;

    private Rigidbody2D[] jellyBodies;
    private Collider2D[] jellyColliders;

    private Vector3 currentPosition;

    private bool isMouseDown;


    void Start()
    {
        lineRenderers[0].positionCount = 2;
        lineRenderers[1].positionCount = 2;

        lineRenderers[0].SetPosition(0, stripPositions[0].position);
        lineRenderers[1].SetPosition(0, stripPositions[1].position);

        CreateJelly();
    }


    void CreateJelly()
    {
        // Pick a random jelly from the array
        GameObject selectedJelly =
            JellyPrefabs[Random.Range(0, JellyPrefabs.Length)];


        currentJelly = Instantiate(
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


        foreach (Collider2D col in jellyColliders)
        {
            col.enabled = false;
        }
    }


    void Update()
    {
        if (currentJelly == null)
            return;


        if (isMouseDown)
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;


            currentPosition =
                Camera.main.ScreenToWorldPoint(mousePosition);


            currentPosition =
                center.position +
                Vector3.ClampMagnitude(
                    currentPosition - center.position,
                    maxLength
                );


            currentPosition =
                ClampBoundary(currentPosition);


            MoveJelly(currentPosition);

            SetStrips(currentPosition);
        }
        else
        {
            ResetStrips();
        }
    }


    void MoveJelly(Vector3 position)
    {
        Vector3 direction =
            position - center.position;


        Vector3 targetPosition =
            position + direction.normalized * jellyPositionOffset;


        currentJelly.transform.position =
            targetPosition;


        currentJelly.transform.rotation =
            Quaternion.Euler(
                0,
                0,
                -direction.x * 15f
            );
    }


    void Shoot()
    {
        float pullDistance =
            Vector2.Distance(
                center.position,
                currentPosition
            );


        float forcePercent =
            Mathf.Clamp01(
                pullDistance / maxLength
            );


        float launchForce =
            Mathf.Lerp(
                minForce,
                maxForce,
                forcePercent
            );


        Vector2 launchDirection =
            (center.position - currentPosition).normalized;


        foreach (Rigidbody2D rb in jellyBodies)
        {
            rb.simulated = true;

            rb.linearVelocity =
                launchDirection * launchForce;
        }


        foreach (Collider2D col in jellyColliders)
        {
            col.enabled = true;
        }


        currentJelly = null;
        jellyBodies = null;
        jellyColliders = null;


        Invoke(nameof(CreateJelly), 2f);
    }


    void ResetStrips()
    {
        currentPosition = idlePosition.position;
        SetStrips(currentPosition);
    }


    void SetStrips(Vector3 position)
    {
        lineRenderers[0].SetPosition(1, position);
        lineRenderers[1].SetPosition(1, position);
    }


    Vector3 ClampBoundary(Vector3 vector)
    {
        vector.y =
            Mathf.Clamp(
                vector.y,
                bottomBoundary,
                1000
            );

        return vector;
    }


    private void OnMouseDown()
    {
        isMouseDown = true;
    }


    private void OnMouseUp()
    {
        isMouseDown = false;


        if (currentJelly != null)
        {
            Shoot();
        }
    }
}