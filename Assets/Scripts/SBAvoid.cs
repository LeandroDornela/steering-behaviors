using UnityEngine;

namespace SteeringBehaviours
{
    [System.Serializable]
    public class SBAvoid : SteeringBehavior
    {
        [Header("Avoid settings")]
        [SerializeField] private float _avoidDistance = 5f;
        [SerializeField] private float _avoidRadius = 1f;
        [SerializeField] private LayerMask _collisionLayers;

        public SBAvoid() : base(){}

        public override Vector3 GetSteering(in SteeringContext context)
        {
            RaycastHit hit;
            if (Physics.SphereCast(context.Position, _avoidRadius, context.LinearVelocity, out hit, _avoidDistance, _collisionLayers))
            {
                Vector3 force = hit.normal * _avoidDistance;

                // Cache current steering force.
                _cachedSteeringForce = force;

                // Debug
                DrawRay(hit.point, hit.normal);
                DrawRay(context.Position, force);

                return force;
            }

            return Vector3.zero;
        }


#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            if (!_enabled) return;

            Gizmos.color = _color * 0.7f;

            Vector3 start = transform.position;
            Vector3 end = transform.position + transform.forward * _avoidDistance;
            Gizmos.DrawWireSphere(end, _avoidRadius);
            Debug.DrawLine(start, end, _color * 0.7f);
        }
#endif
    }
}
