using UnityEngine;

// NOTE: Using a struct for the behaviors make then independent of SteeringBehavioursManager.
// This can avoid changes on behaviors when changing the accessed parameters or make testing easier.
public readonly struct SteeringContext
{
    public readonly Vector3 TargetPosition;
    public readonly Vector3 Position;
    public readonly Vector3 Forward;
    public readonly Vector3 Up;
    public readonly Vector3 TargetOffSet;
    public readonly Vector3 TargetDirection;
    public readonly float TargetDistance;
    public readonly Vector3 LinearVelocity;
    public readonly float MaxSpeed;

    public SteeringContext(Vector3 targetPosition, Vector3 position, Vector3 forward, Vector3 up, Vector3 linearVelocity, float maxSpeed)
    {
        TargetPosition = targetPosition;
        
        Position = position;
        Forward = forward;
        Up = up;

        LinearVelocity = linearVelocity;
        MaxSpeed = maxSpeed;

        TargetOffSet = targetPosition - position;
        TargetDirection = TargetOffSet.normalized;
        TargetDistance = TargetOffSet.magnitude;
    }
}