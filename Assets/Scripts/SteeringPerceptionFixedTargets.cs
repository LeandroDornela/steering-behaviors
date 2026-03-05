using UnityEngine;

namespace SteeringBehaviours
{
    [System.Serializable]
    public class SteeringPerceptionFixedTargets : SteeringPerception
    {
        [SerializeField] private Transform[] _targets;

        public override bool TryGetTargetPosition(out Vector3 targetPosition)
        {
            if(_targets.Length == 0)
            {
                targetPosition = Vector3.zero;
                return false;
            }

            targetPosition = GetAverageTargetPosition();
            return true;
        }


        public void SetTargets(Transform[] targets)
        {
            _targets = targets;
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
    }
}
