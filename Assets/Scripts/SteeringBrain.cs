using UnityEngine;

namespace SteeringBehaviours
{
    [System.Serializable]
    public class SteeringBrain : MonoBehaviour
    {
        [SerializeField] private SBSeek _seek;
        [SerializeField] private SBFlee _flee;
        [SerializeField] private SBArrival _arrival;
        [SerializeField] private SBWander _wander;
        [SerializeField] private SBAvoid _avoid;
        [SerializeField] private SBWallFollow _wallFollow;

        [SerializeField] private float _steeringSmoothSpeed = 10;
        private Vector3 _lastSteeringForce = Vector3.zero;
        private Vector3 _currentSteeringForce = Vector3.zero;
        private Transform _transform;


        void Awake()
        {
            _transform = transform;
        }


        public SteeringResult GetResultantSteering(in SteeringContext context)
        {
            Vector3 resultantForce = Vector3.zero;

            if (_seek.Enabled) resultantForce += _seek.Weight * _seek.GetSteering(in context);
            if (_flee.Enabled) resultantForce += _flee.Weight * _flee.GetSteering(in context);
            if (_arrival.Enabled) resultantForce += _arrival.Weight * _arrival.GetSteering(in context);
            if (_wander.Enabled) resultantForce += _wander.Weight * _wander.GetSteering(in context);
            if (_avoid.Enabled) resultantForce += _avoid.Weight * _avoid.GetSteering(in context);
            if (_wallFollow.Enabled) resultantForce += _wallFollow.Weight * _wallFollow.GetSteering(in context);

            _currentSteeringForce = Vector3.Lerp(_lastSteeringForce, resultantForce, _steeringSmoothSpeed * Time.fixedDeltaTime);
            _lastSteeringForce = _currentSteeringForce;

            return new SteeringResult(context, _currentSteeringForce);
        }


        public void OnValidate()
        {
            _seek.OnValidate();
            _flee.OnValidate();
            _arrival.OnValidate();
            _wander.OnValidate();
            _avoid.OnValidate();
            _wallFollow.OnValidate();
        }


        public void OnDrawGizmosSelected()
        {
            if(_transform == null) _transform = transform;
            
            _seek.OnDrawGizmosSelected(_transform);
            _flee.OnDrawGizmosSelected(_transform);
            _arrival.OnDrawGizmosSelected(_transform);
            _wander.OnDrawGizmosSelected(_transform);
            _avoid.OnDrawGizmosSelected(_transform);
            _wallFollow.OnDrawGizmosSelected(_transform);
        }
    }
}
