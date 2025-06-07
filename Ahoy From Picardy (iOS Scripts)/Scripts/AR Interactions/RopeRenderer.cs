using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class RopeRenderer : MonoBehaviour
{
    // These will now be set by DisablePinataPlacer, or derived from the joint
    // [SerializeField] private Transform pinataAnchorPoint; // No longer directly used for rendering
    // [SerializeField] private Transform hangingHookPoint; // No longer directly used for rendering

    [SerializeField] private int numberOfSegments = 15; 
    [SerializeField] private float sagStrength = 0.5f; 
    [Tooltip("Adjusts the maximum sag for visual purposes. Only affects appearance, not physics.")]
    [SerializeField] private float maxVisualSag = 0.5f;

    private Camera mainCamera;
    private LineRenderer lineRenderer;
    private Transform anchor1Transform;
    private Transform anchor2Transform;

    private bool shouldDrawRope = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfSegments;
        lineRenderer.alignment = LineAlignment.TransformZ;
        lineRenderer.enabled = false; 

        FindMainCamera();
    }

    void Update()
    {
        if (!shouldDrawRope) return;

        if (mainCamera == null)
        {
            FindMainCamera();
            if (mainCamera == null)
            {
                Debug.LogWarning("RopeRenderer: Main Camera not found. Ensure your AR Camera is tagged 'MainCamera'.", this);
                return;
            }
        }

        if (anchor1Transform == null || anchor2Transform == null)
        {
            // Debug.LogWarning("RopeRenderer: Anchor points not set for drawing!", this);
            return; 
        }

        transform.forward = mainCamera.transform.forward;

        DrawRope();
    }

    private void FindMainCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; 
        }
    }

    // This method is called by DisablePinataPlacer
    public void SetRopePointsAndStartDrawing(Transform startPointTransform, Transform endPointTransform)
    {
        this.anchor1Transform = startPointTransform;
        this.anchor2Transform = endPointTransform;

        lineRenderer.positionCount = numberOfSegments; // Ensure correct segment count
        lineRenderer.enabled = true;
        shouldDrawRope = true;
        Debug.Log("RopeRenderer: Set to draw between provided points with sag enabled.");
    }

    void DrawRope()
    {
        Vector3 startPos = anchor1Transform.position;
        Vector3 endPos = anchor2Transform.position;
        Vector3 straightLineVector = endPos - startPos;
        float distance = straightLineVector.magnitude;

        // Calculate a dynamic sag based on distance
        // The sag should be more pronounced for longer ropes, but capped
        float calculatedSag = Mathf.Min(sagStrength * distance * 0.5f, maxVisualSag); // Adjust multiplier as needed

        for (int i = 0; i < numberOfSegments; i++)
        {
            float t = (float)i / (numberOfSegments - 1);
            Vector3 pointOnStraightLine = Vector3.Lerp(startPos, endPos, t);
            
            // This is the parabolic curve factor
            float curveFactor = 4 * t * (1 - t); 
            
            // Apply the sag offset
            Vector3 finalPoint = pointOnStraightLine + Vector3.down * (calculatedSag * curveFactor);
            lineRenderer.SetPosition(i, finalPoint);
        }
    }

    public void StopDrawingRope()
    {
        shouldDrawRope = false;
        lineRenderer.enabled = false;
    }
}