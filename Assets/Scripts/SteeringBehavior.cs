using UnityEngine;

namespace SteeringBehaviours
{
	[System.Serializable]
	public abstract class SteeringBehavior
	{
		[SerializeField] protected bool _enabled = true;
		[SerializeField] protected float _weight = 1;

		[Header("Debug")]
		[SerializeField] protected bool _enableVisualDebug = false;
		[SerializeField] protected Color _color = Color.lightGray;


		protected Vector3 _cachedSteeringForce;

		public bool Enabled => _enabled;
		public float Weight => _weight;
		public Vector3 CachedSteeringForce => _cachedSteeringForce;

		public SteeringBehavior() { }

		public virtual void OnValidate() {}
		public virtual void OnDrawGizmosSelected(Transform transform) { }

		public abstract Vector3 GetSteering(in SteeringContext context);
	}
}