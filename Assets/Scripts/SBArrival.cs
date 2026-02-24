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

        void OnValidate()
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
                speed = context.MaxSpeed;
            }
            else // If inside slow radius, linearly decelerate.
            {
                speed = context.MaxSpeed * ((context.TargetDistance - _minDistanceRadius) / _slowRadius);
            }

            //desiredVelocity = speed * context.TargetDirection
            Vector3 force = speed * context.TargetDirection - context.LinearVelocity;

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

            Gizmos.color = _color;
            Gizmos.DrawWireSphere(transform.position, _minDistanceRadius);
            Gizmos.DrawWireSphere(transform.position, _slowRadius);
        }
#endif
    }
}