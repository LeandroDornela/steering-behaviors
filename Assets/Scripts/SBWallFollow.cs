using UnityEditor;
using UnityEngine;

namespace SteeringBehaviours
{
    [System.Serializable]
    public class SBWallFollow : SteeringBehavior
    {
        [Header("Wall Follow settings")]
        [SerializeField] private float _wallDistance = 2;
        [SerializeField] private LayerMask _collisionLayers;

        private Vector3 _force;
        private Vector3 _desiredVelocity;
        private Vector3 _wallTangent;

        public SBWallFollow() : base() { }

        public override Vector3 GetSteering(in SteeringContext context)
        {
            Vector3 origin = context.AgentPosition;
            Vector3 force = Vector3.zero;

            // Forward ray to detect walls
            Collider[] collidersHit = Physics.OverlapSphere(context.AgentPosition, _wallDistance, _collisionLayers);

            if (collidersHit.Length == 0) return force;

            // Get closest collider.
            Collider closestCollider = collidersHit[0];
            // NOTE: ClosestPointOnBounds is less precise but more performant.
            Vector3 closestPoint = closestCollider.ClosestPoint(origin);
            // Obviously, don't need to check if only one collider.
            if (collidersHit.Length > 1)
            {
                foreach (Collider collider in collidersHit)
                {
                    Vector3 point = collider.ClosestPoint(origin);
                    if ((point - origin).sqrMagnitude < (closestPoint - origin).sqrMagnitude) // sqrtMag is faster and enough for that application.
                    {
                        closestCollider = collider;
                        closestPoint = point;
                    }
                }
            }

            // TODO: when ray is to close to be parallel to agent velocity, randonly change the direction.

            if (Physics.Raycast(origin, closestPoint - origin, out RaycastHit hit, _wallDistance, _collisionLayers))
            {
                Vector3 wallNormal = hit.normal;
                Vector3 wallTangent = (context.AgentLinearVelocity - (Vector3.Dot(context.AgentLinearVelocity, wallNormal) * wallNormal)).normalized;
                float distanceToWall = hit.distance;
                Vector3 tangentVelocity = wallTangent * context.AgentMaxSpeed;

                // Min in 0 to avoid attraction force.
                float distanceError = Mathf.Max(0, _wallDistance - distanceToWall);
                Vector3 separationForce = wallNormal * distanceError;

                Vector3 desiredVelocity = tangentVelocity + separationForce;

                //force = Vector3.ClampMagnitude(desiredVelocity - context.LinearVelocity, context.MaxForce);
                force = desiredVelocity - context.AgentLinearVelocity;

                Debug.DrawRay(hit.point, wallNormal * _wallDistance, _color);
                Debug.DrawRay(context.AgentPosition, force, _color);
                Debug.DrawRay(context.AgentPosition, desiredVelocity, _color);
                Debug.DrawRay(context.AgentPosition, wallTangent, _color);

                _desiredVelocity = desiredVelocity;
                _force = force;
                _wallTangent = wallTangent;
            }

            return force;
            //return _desiredVelocity;
        }


        public override void OnDrawGizmosSelected(Transform transform)
        {
            if (!_enabled) return;

            Gizmos.color = new Color(_color.r, _color.g, _color.b, _color.a / 3f);
            Gizmos.DrawSphere(transform.position, _wallDistance);

            Handles.Label(transform.position + _desiredVelocity, "WF_Vd");
            Handles.Label(transform.position + _force, "WF_F");
            Handles.Label(transform.position + _wallTangent, "WF_Tg");
        }
    }
}
