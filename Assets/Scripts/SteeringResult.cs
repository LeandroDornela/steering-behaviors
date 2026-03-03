using UnityEngine;

namespace SteeringBehaviours
{
    // NOTE: Could be only one struct SteeringContext + SteeringResult. But for clean separation of pre and post calculation
    // have one to be used before and on to use after.
    public struct SteeringResult
    {
        // The context used to calculate the steering forces.
        public readonly SteeringContext Context;

        // The non truncated steering force.
        public readonly Vector3 SteeringForceRaw;
        // The truncated to MaxForce steering force.
        public readonly Vector3 SteeringForceTruncated;
        // The... normalized steering force.
        public readonly Vector3 SteeringForceNormalized;


        public SteeringResult(SteeringContext context, Vector3 rawSteeringForce)
        {
            Context = context;
            SteeringForceRaw = rawSteeringForce;
            SteeringForceTruncated = Vector3.ClampMagnitude(rawSteeringForce, context.AgentMaxForce);
            SteeringForceNormalized = rawSteeringForce.normalized;
        }
    }
}
