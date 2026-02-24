using UnityEngine;

namespace SteeringBehaviours
{
    public class SBWallFollow : SteeringBehavior
    {
        [Header("Wall Follow settings")]
        [SerializeField] private float _wallDistance = 2;
        [SerializeField] private LayerMask _collisionLayers;

        public SBWallFollow() : base() { }

        public override Vector3 GetSteering(in SteeringContext context)
        {
            Vector3 force = Vector3.zero;

            force += GetForceToDirection(context, context.Forward);
            force += GetForceToDirection(context, Vector3.Cross(context.Forward, context.Up));
            force += GetForceToDirection(context, -Vector3.Cross(context.Forward, context.Up));

            return force;
        }

        Vector3 GetForceToDirection(SteeringContext context, Vector3 dir)
        {
            RaycastHit hit;
            Vector3 force = Vector3.zero;

            // Forward ray to detect walls
            if (Physics.Raycast(context.Position, dir, out hit, _wallDistance, _collisionLayers))
            {
                Vector3 wallNormal = hit.normal;
                Vector3 wallTangent = Vector3.Cross(wallNormal, context.Up).normalized;

                // Current distance to wall
                float distanceToWall = hit.distance;

                // --- 1. Tangential force (move along the wall) ---
                Vector3 tangentVelocity = wallTangent * context.MaxSpeed;

                // --- 2. Separation force (maintain min distance) ---
                float distanceError = _wallDistance - distanceToWall;
                // Positive error = too close, push away from wall
                Vector3 separationForce = wallNormal * distanceError;

                // --- 3. Combine ---
                Vector3 desiredVelocity = tangentVelocity + separationForce;
                force = desiredVelocity - context.LinearVelocity;

                DrawRay(context.Position, dir*_wallDistance);
            }

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
