using UnityEngine;

namespace SteeringBehaviours
{
    public abstract class SteeringActuator : MonoBehaviour
    {   
        [SerializeField, Min(0)] protected float _maxSpeed = 10;
        [SerializeField, Min(0)] protected float _minSpeed = 0;
        [SerializeField, Min(0)] protected float _maxForce = 10;

        public float MaxSpeed => _maxSpeed;
        public float MinSpeed => _minSpeed;
        public float MaxForce => _maxForce;

        
        public abstract Vector3 GetLinearVelocity();
        public abstract void ApplySteering(SteeringResult steeringResult);
    }
}
