using UnityEngine;

public abstract class SteeringPerception : MonoBehaviour
{
    public abstract bool TryGetTargetPosition(out Vector3 targetPosition);
}
