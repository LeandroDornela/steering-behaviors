using UnityEngine;

namespace SteeringBehaviours
{
    [System.Serializable]
    public class SBSeek : SteeringBehavior
    {
        public SBSeek() : base() { }

        public override Vector3 GetSteering(in SteeringContext context)
        {
            // desiredVelocity = context.MaxSpeed * context.TargetDirection
            //Vector3 force = Vector3.ClampMagnitude(context.MaxSpeed * context.TargetDirection - context.LinearVelocity, context.MaxForce);
            Vector3 force = context.AgentMaxSpeed * context.TargetDirection - context.AgentLinearVelocity;

            // Cache current steering force.
            _cachedSteeringForce = force;

            // Debug
            Debug.DrawRay(context.AgentPosition, force, _color);

            return force;
        }


        public override void OnDrawGizmosSelected(Transform transform)
        {
            if (!_enabled) return;
        }
    }
}
