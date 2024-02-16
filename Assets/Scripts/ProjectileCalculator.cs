using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCalculator : MonoBehaviour
{
    public float initialSpeed = 10f;
    public float gravity = 9.8f;
    public Transform LineOffset;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 1;
    }
    public void CalculateTrajectory()
    {
        Vector3 launchDirection = LineOffset.forward;
        float timeStep = 0.1f;

        Vector3 velocity = initialSpeed * launchDirection;

        Vector3 currentPosition = transform.position;

        Vector3[] positions = new Vector3[100];
        positions[0] = currentPosition;

        for (int i = 1; i < positions.Length; i++)
        {
            currentPosition += velocity * timeStep;
            velocity.y -= gravity * timeStep;
            positions[i] = currentPosition;
        }

        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    public void ResetLine()
    {
        lineRenderer.positionCount = 0;
    }
}
