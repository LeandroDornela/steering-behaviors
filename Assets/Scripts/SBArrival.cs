using UnityEngine;

namespace SteeringBehaviours
{
    [System.Serializable]
    public class SBArrival : SteeringBehavior
    {
        [Header("Arrival settings")]
        [SerializeField] private float _minDistanceRadius = 1;
        [SerializeField] private float _slowRadius = 2;

        public SBArrival() : base(){}

        public override void OnValidate()
        {
            if (_minDistanceRadius > _slowRadius)
            {
                _minDistanceRadius = _slowRadius;
            }
        }


        public override Vector3 GetSteering(in SteeringContext context)
        {
            float speed;
            if (context.TargetDistance > _slowRadius)
            {
                speed = context.AgentMaxSpeed;
            }
            else // If inside slow radius, linearly decelerate.
            {
                speed = context.AgentMaxSpeed * (Mathf.Max(0, context.TargetDistance - _minDistanceRadius) / _slowRadius);
            }

            //Vector3 force = Vector3.ClampMagnitude(speed * context.TargetDirection - context.LinearVelocity, context.MaxForce);
            Vector3 force = speed * context.TargetDirection - context.AgentLinearVelocity;

            // Cache current steering force.
            _cachedSteeringForce = force;

            // Debug
            Debug.DrawRay(context.AgentPosition, force, _color);

            return force;
        }


        public override void OnDrawGizmosSelected(Transform transform)
        {
            if (!_enabled) return;

            Gizmos.color = new Color(_color.r, _color.g, _color.b, _color.a/3f);
            Gizmos.DrawSphere(transform.position, _minDistanceRadius);
            Gizmos.DrawSphere(transform.position, _slowRadius);
        }
    }
}