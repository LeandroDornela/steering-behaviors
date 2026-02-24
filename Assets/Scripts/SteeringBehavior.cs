using Unity.Collections;
using UnityEngine;

namespace SteeringBehaviours
{
    [System.Serializable]
    public abstract class SteeringBehavior : MonoBehaviour
    {
        [SerializeField] protected bool _enabled = true;
        [SerializeField] protected float _weight = 1;

#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] protected Color _color = Color.lightGray;
#endif

        protected Vector3 _cachedSteeringForce;

        public bool Enabled => _enabled;
        public float Weight => _weight;
        public Vector3 CachedSteeringForce => _cachedSteeringForce;

        public SteeringBehavior() { }

        public abstract Vector3 GetSteering(in SteeringContext context);


        #region  Debug

        protected void DrawRay(Vector3 pos, Vector3 dir, float duration = 0)
        {
#if UNITY_EDITOR
            Debug.DrawRay(pos, dir, _color, duration: duration);
#endif
        }

        #endregion
    }
}