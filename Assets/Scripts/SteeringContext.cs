using UnityEngine;

// NOTE: Using a struct for the behaviors make then independent of SteeringBehavioursManager.
// This can avoid changes on behaviors when changing the accessed parameters or make testing easier.
public struct SteeringContext
{
    // Agent generals.
    public readonly GameObject AgentGameObject;
    public readonly Vector3 AgentPosition;
    public readonly Vector3 AgentForward;
    public readonly Vector3 AgentUp;
    
    // Physics.
    public readonly Vector3 AgentLinearVelocity;
    public readonly float AgentMaxSpeed;
    public readonly float AgentMaxForce;

    // Target.
    public readonly Vector3 TargetPosition;
    public readonly Vector3 TargetOffSet;
    public readonly Vector3 TargetDirection;
    public readonly float TargetDistance;

    

    public SteeringContext(Vector3 targetPosition, GameObject gameObject, Vector3 position, Vector3 forward, Vector3 up, Vector3 linearVelocity, float maxSpeed, float maxForce)
    {
        TargetPosition = targetPosition;

        AgentGameObject = gameObject;
        
        AgentPosition = position;
        AgentForward = forward;
        AgentUp = up;

        AgentLinearVelocity = linearVelocity;
        AgentMaxSpeed = maxSpeed;
        AgentMaxForce = maxForce;

        TargetOffSet = targetPosition - position;
        TargetDirection = TargetOffSet.normalized;
        TargetDistance = TargetOffSet.magnitude;
    }
}