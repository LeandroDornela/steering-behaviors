using UnityEngine;

namespace SteeringBehaviours
{
    [System.Serializable]
    public class SBSeek : SteeringBehavior
    {
        public SBSeek() : base(){}

        public override Vector3 GetSteering(in SteeringContext context)
        {
            // desiredVelocity = context.MaxSpeed * context.TargetDirection
            Vector3 force = context.MaxSpeed * context.TargetDirection - context.LinearVelocity;

            // Cache current steering force.
            _cachedSteeringForce = force;

            // Debug
            DrawRay(context.Position, force);

            return force;
        }


#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (!_enabled) return;
        }
#endif
    }
}
