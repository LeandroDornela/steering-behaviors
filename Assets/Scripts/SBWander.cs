using UnityEngine;

namespace SteeringBehaviours
{
    [System.Serializable]
    public class SBWander : SteeringBehavior
    {
        [Header("Wander")]
        [SerializeField] private Vector2 _updateInterval = Vector2.one;
        [SerializeField] private float _distance = 4f;
        [SerializeField] private float _radius = 2f;

        [Header("Interpolation")]
        [SerializeField] private bool _interpolate = false;
        [SerializeField] private float _interpolationSpeed = 1;

        private float _lastUpdate = 0;
        private float _currentUpdateInterval = 0;
        private Vector3 _targetSphereDirection = Vector3.zero;
        private Vector3 _sphereDirection = Vector3.zero;


        public SBWander() : base() { }

        public override Vector3 GetSteering(in SteeringContext context)
        {
            float _timeSinceLastUp = Time.time - _lastUpdate;
            if(_timeSinceLastUp >= _currentUpdateInterval)
            {
                _targetSphereDirection = Random.onUnitSphere * _radius;
                
                if(!_interpolate) _sphereDirection = _targetSphereDirection;

                _lastUpdate = Time.time;
                _currentUpdateInterval = Random.Range(_updateInterval.x, _updateInterval.y);
            }

            if(_interpolate)
            {
                _sphereDirection = Vector3.Slerp(_sphereDirection, _targetSphereDirection, _interpolationSpeed * Time.fixedDeltaTime);
            }

            Vector3 wanderDirection = context.AgentLinearVelocity.normalized * _distance + _sphereDirection;
            //Vector3 force = Vector3.ClampMagnitude(wanderDirection - context.LinearVelocity, context.MaxForce);
            Vector3 force = wanderDirection - context.AgentLinearVelocity;

            Debug.DrawRay(context.AgentPosition, wanderDirection, _color);

            return force;
        }


        public override void OnDrawGizmosSelected(Transform transform)
        {
            if (!_enabled) return;

            Gizmos.color = _color*0.7f;
            Gizmos.DrawWireSphere(transform.position + transform.forward * _distance, _radius);
        }
    }
}
