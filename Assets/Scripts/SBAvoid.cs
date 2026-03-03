using UnityEngine;

namespace SteeringBehaviours
{
    /// <summary>
    /// Uses a Capsule projected along the forward direction of the object to detect possible collisions.
    /// The a ray is cast on the direction of the closest point of the closest possible collider and a force
    /// is generate in opposition of the normal of th hit point of that ray.
    /// </summary>
    [System.Serializable]
    public class SBAvoid : SteeringBehavior
    {
        [Header("Avoid settings")]
        [Tooltip("How far ahead it will test collisions. The collision capsule length. Big values for closed ambients with too many obstacles cam cause chaotic movement.")]
        [SerializeField, Min(0)] private float _lookAhead = 5f;

        [Tooltip("Where it will start to test collisions. The collision capsule origin offset in object forward direction.")]
        [SerializeField] private float _startOffset = 0f;

        [Tooltip("Radius of the collision test capsule.")]
        [SerializeField, Min(0)] private float _avoidRadius = 1f;

        [Tooltip("The 'force' that push the object away from a obstacle.")]
        [SerializeField, Min(0)] private float _avoidForce = 10f;

        [Tooltip("The layers that will be tested to avoid collision.")]
        [SerializeField] private LayerMask _collisionLayers;

        public SBAvoid() : base() { }

        public override Vector3 GetSteering(in SteeringContext context)
        {
            // Using SphereCast can result in misses since when the object target of the collision test is already overlapping the
            // sphere cast it will not hit, what is common when the objects are to close an colliding over each other for a time,
            // like a character stuck in a wall.

            // Check collisions.
            Vector3 origin = context.AgentPosition + context.AgentLinearVelocity.normalized * _startOffset;
            Vector3 end = origin + context.AgentLinearVelocity.normalized * _lookAhead;
            Collider[] collidersHit = Physics.OverlapCapsule(origin, end, _avoidRadius, _collisionLayers);

            // Exit if no collision.
            if (collidersHit.Length == 0) return Vector3.zero;

            // Get closest collider.
            Collider closestCollider = null;
            Vector3 closestPoint = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);//closestCollider.ClosestPoint(origin);
            // Obviously, don't need to check if only one collider.
            if (collidersHit.Length > 1)
            {
                foreach (Collider collider in collidersHit)
                {
                    if (collider.gameObject == context.AgentGameObject) continue;

                    // NOTE: ClosestPointOnBounds is less precise but more performant.
                    Vector3 point = collider.ClosestPoint(origin);
                    if ((point - origin).sqrMagnitude < (closestPoint - origin).sqrMagnitude) // sqrtMag is faster and enough for that application.
                    {
                        closestCollider = collider;
                        closestPoint = point;
                    }
                }
            }

            // Exit when the only collider belongs to the object.
            if (closestCollider == null) return Vector3.zero;

            if (_enableVisualDebug) Debug.DrawLine(origin, closestPoint, _color);

            // Raycast filtering self.
            RaycastHit[] hits = Physics.RaycastAll(origin, closestPoint - origin, _lookAhead + _startOffset, _collisionLayers);
            RaycastHit closestHit = new RaycastHit();
            if (hits.Length == 0) return Vector3.zero;
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.gameObject == context.AgentGameObject) continue;

                if (closestHit.collider == null || hit.distance < closestHit.distance)
                {
                    closestHit = hit;
                }
            }

            // =================================
            // FINAL STEERING FORCE CALCULATION
            // =================================

            Vector3 force = closestHit.normal * _avoidForce + context.AgentLinearVelocity;
            _cachedSteeringForce = force;

            if (_enableVisualDebug)
            {
                Debug.DrawRay(closestHit.point, closestHit.normal, _color);
                Debug.DrawRay(origin, force, Color.yellow);
            }

            return force;
        }


        public override void OnDrawGizmosSelected(Transform transform)
        {
            if (!_enabled) return;

            Gizmos.color = _color * 0.7f;

            Vector3 start = transform.position + transform.forward * _startOffset;
            Vector3 end = start + transform.forward * _lookAhead;
            Debug.DrawLine(start, end, _color * 0.7f);
            Gizmos.color = new Color(_color.r, _color.g, _color.b, _color.a / 3f);
            Gizmos.DrawSphere(start, _avoidRadius);
            Gizmos.DrawSphere(end, _avoidRadius);
        }
    }
}
