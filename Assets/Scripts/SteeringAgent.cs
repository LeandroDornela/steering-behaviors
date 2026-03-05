using UnityEngine;

namespace SteeringBehaviours
{
    public class SteeringAgent : MonoBehaviour
    {
        [SerializeField] private SteeringBrain _steeringBrain;
        [SerializeField] public SteeringPerception _steeringPerception;
        [SerializeField] private SteeringActuator _steeringActuator;


        public void CustomUpdate()
        {
            UpdateAgent();
        }


        void UpdateAgent()
        {
            _steeringPerception.TryGetTargetPosition(out Vector3 targetPosition);

            // TODO: if no target, can't use seek, flee, arrive.

            SteeringContext context = new SteeringContext(
                targetPosition,
                gameObject,
                transform.position,
                transform.forward,
                transform.up,
                _steeringActuator.GetLinearVelocity(),
                _steeringActuator.MaxSpeed,
                _steeringActuator.MaxForce
            );

            SteeringResult result = _steeringBrain.GetResultantSteering(context);

            _steeringActuator.ApplySteering(result);
        }
    }
}
