using UnityEngine;

namespace SteeringBehaviours
{
    [System.Serializable]
    public class SteeringActuatorRigidbody : SteeringActuator
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private ForceMode _forceMode = ForceMode.Force;
        
        // Facing/rotation
        public enum FacingDirection { None, ToMoveDirection, ToTarget }
        [SerializeField] private FacingDirection _facingDirection = FacingDirection.ToMoveDirection;
        [SerializeField] private float _rotationSpeed = 10;
        [SerializeField] private bool _bankingTurn = true;


        void OnValidate()
        {
            if (_rigidbody == null)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
        }


        void Start()
        {
            if (_rigidbody == null)
            {
                Debug.LogError($"Missing Rigidbody reference of {gameObject.name}.");
            }
        }


        public override Vector3 GetLinearVelocity()
        {
            return _rigidbody.linearVelocity;
        }


        public override void ApplySteering(SteeringResult steeringResult)
        {
            // Apply the force and clamp the speed.
            _rigidbody.AddForce(steeringResult.SteeringForceTruncated, _forceMode);
            ClampSpeed(_rigidbody, _minSpeed, _maxSpeed);

            // Update the rotation.
            UpdateFacingDirection(steeringResult);
        }


        void ClampSpeed(Rigidbody rb, float minVal, float maxVal)
        {
            float speed = rb.linearVelocity.magnitude;

            if (speed < minVal)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * minVal;
            }
            else if (speed > maxVal) // Maybe can be replaced by Rigibody.maxLinearVelocity.
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxVal;
            }
        }


        void UpdateFacingDirection(SteeringResult steeringResult)
        {
            switch (_facingDirection)
            {
                case FacingDirection.ToMoveDirection:
                    UpdateRotation(steeringResult.SteeringForceNormalized, steeringResult.Context.AgentUp);
                    break;
                case FacingDirection.ToTarget:
                    UpdateRotation(steeringResult.Context.TargetDirection, steeringResult.Context.AgentUp);
                    break;
                case FacingDirection.None:
                default:
                    break;
            }
        }


        void UpdateRotation(Vector3 direction, Vector3 up)
        {
            if (_rigidbody.linearVelocity.sqrMagnitude < 0.01f) return;

            Quaternion newRotation;

            if (_bankingTurn)
            {
                Vector3 estimatedUp = direction + Vector3.up + up;
                newRotation = Quaternion.LookRotation(_rigidbody.linearVelocity.normalized, estimatedUp.normalized);
            }
            else
            {
                newRotation = Quaternion.LookRotation(_rigidbody.linearVelocity.normalized, Vector3.up);
            }

            Quaternion smoothRotation = Quaternion.Lerp(_rigidbody.rotation, newRotation, _rotationSpeed * Time.fixedDeltaTime);
            //Quaternion smoothRotation = Quaternion.Slerp(_rigidbody.rotation, newRotation, _rotationSmoothSpeed * Time.fixedDeltaTime);

            // _rigidbody.rotation = smoothRotation;
            _rigidbody.MoveRotation(smoothRotation);
        }
    }
}
