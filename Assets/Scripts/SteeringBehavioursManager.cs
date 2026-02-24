using UnityEngine;

namespace SteeringBehaviours
{
    public class SteeringBehavioursManager : MonoBehaviour
    {
        [SerializeField] private Transform[] _targets;
        [SerializeField] private float _maxSpeed = 10;
        [SerializeField] private float _minSpeed = 0;

        [SerializeField] private Rigidbody _rigidbody;


        //private SteeringBehavior[] _steeringBehaviors;
        public SBSeek seek;
        public SBFlee flee;
        public SBAvoid avoid;
        public SBArrival arrival;
        public SBWander wander;
        public SBWallFollow wallFollow;

        private Vector3 _targetPosition = Vector3.zero;
        private Vector3 _lastSteeringForce = Vector3.zero;
        private Vector3 _currentSteeringForce = Vector3.zero;
        private Vector3 _desiredDirection = Vector3.zero;
        private const float _steeringSmoothSpeed = 5;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //_steeringBehaviors = GetComponents<SteeringBehavior>();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.maxLinearVelocity = _maxSpeed;

            _targetPosition = GetAverageTargetPosition();
        }


        // Update is called once per frame
        void FixedUpdate()
        {
            _targetPosition = GetAverageTargetPosition();

            Vector3 steeringForce = Vector3.zero;

            SteeringContext context = new SteeringContext(
                _targetPosition,
                transform.position,
                transform.forward,
                transform.up,
                _rigidbody.linearVelocity,
                _maxSpeed
            );

            /*
            // Calculate the steering force from all behaviours.
            foreach (SteeringBehavior behavior in _steeringBehaviors)
            {
                if (!behavior.Enabled) continue;

                steeringForce += behavior.Weight * behavior.GetSteering(in context);
            }
            */
            if(seek.Enabled) steeringForce += seek.Weight * seek.GetSteering(in context);
            if(flee.Enabled) steeringForce += flee.Weight * flee.GetSteering(in context);
            if(arrival.Enabled) steeringForce += arrival.Weight * arrival.GetSteering(in context);
            if(wander.Enabled) steeringForce += wander.Weight * wander.GetSteering(in context);
            if(avoid.Enabled) steeringForce += avoid.Weight * avoid.GetSteering(in context);
            if(wallFollow.Enabled) steeringForce += wallFollow.Weight * wallFollow.GetSteering(in context);
            

            //_currentSteeringForce = Vector3.Slerp(_lastSteeringForce, steeringForce, _steeringSmoothSpeed * Time.fixedDeltaTime);
            _currentSteeringForce = steeringForce;
            _lastSteeringForce = steeringForce;

            ApplySteeringForce(_rigidbody, _currentSteeringForce);
            ClampSpeed(_rigidbody, _minSpeed);
            UpdateRotation(_rigidbody, transform, _currentSteeringForce);
        }


        Vector3 GetAverageTargetPosition()
        {
            Vector3 averageTargetsPosition = Vector3.zero;

            foreach (Transform target in _targets)
            {
                averageTargetsPosition += target.position;
            }

            return averageTargetsPosition /= _targets.Length;
        }


        void ApplySteeringForce(Rigidbody rb, Vector3 steeringForce)
        {
            rb.AddForce(steeringForce, ForceMode.Force);
        }


        void UpdateRotation(Rigidbody rb, Transform transf, Vector3 steeringForce)
        {
            Vector3 estimatedUp = steeringForce.normalized + Vector3.up + transf.up;
            Quaternion newRotation = Quaternion.LookRotation(rb.linearVelocity, estimatedUp);
            transf.rotation = newRotation;
            //transf.rotation = Quaternion.Slerp(transf.rotation, newRotation, Time.fixedDeltaTime);
        }


        void ClampSpeed(Rigidbody rb, float minVal)
        {
            float speed = rb.linearVelocity.magnitude;

            if (speed < minVal)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * minVal;
            }
            /* Don't need. Set with Rigibody.maxLinearVelocity
            else if (speed > maxVal)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxVal;
            }
            */
        }
    }
}
