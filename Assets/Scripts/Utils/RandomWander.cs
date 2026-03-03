// AI GENERATED CLASS FOR TESTING.

using UnityEngine;

/// <summary>
/// Randomly repositions the attached GameObject within a given radius at regular intervals.
/// Includes a Gizmo debug view of the wander radius.
/// </summary>
public class RandomWander : MonoBehaviour
{
    [Header("Wander Settings")]
    [Tooltip("Maximum distance from the origin the object can be placed.")]
    [SerializeField] private float wanderRadius = 5f;

    [Tooltip("How often (in seconds) the object picks a new random position.")]
    [SerializeField] private float interval = 2f;

    [Tooltip("If true, the wander origin is the object's starting world position. " +
             "If false, the wander origin is always Vector3.zero.")]
    [SerializeField] private bool useStartPositionAsOrigin = true;

    [Tooltip("If true, movement is instant. If false, the object lerps to the target.")]
    [SerializeField] private bool instantMove = false;

    [Tooltip("Speed at which the object moves towards the target when instantMove is false.")]
    [SerializeField] private float moveSpeed = 3f;

    [Tooltip("Lock movement on the Y axis (useful for 2D or flat 3D scenes).")]
    [SerializeField] private bool lockY = false;

    [Header("Debug")]
    [Tooltip("Color of the radius gizmo drawn in the Scene view.")]
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0.5f, 0.25f);
    [SerializeField] private Color gizmoOutlineColor = new Color(0f, 1f, 0.5f, 1f);

    // ── Runtime state ──────────────────────────────────────────────────────────
    private Vector3 _origin;
    private Vector3 _targetPosition;
    private float   _timer;

    // ── Unity lifecycle ────────────────────────────────────────────────────────
    private void Start()
    {
        _origin         = useStartPositionAsOrigin ? transform.position : Vector3.zero;
        _targetPosition = transform.position;
        _timer          = interval; // trigger immediately on first frame
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= interval)
        {
            _timer = 0f;
            _targetPosition = PickRandomPosition();
        }

        MoveTowardsTarget();
    }

    // ── Private helpers ────────────────────────────────────────────────────────

    /// <summary>Returns a random point inside the wander radius.</summary>
    private Vector3 PickRandomPosition()
    {
        // Random point in unit circle, scaled to wander radius
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;

        Vector3 newPos = new Vector3(
            _origin.x + randomCircle.x,
            lockY ? transform.position.y : _origin.y + Random.Range(-wanderRadius, wanderRadius),
            _origin.z + randomCircle.y
        );

        return newPos;
    }

    private void MoveTowardsTarget()
    {
        if (instantMove)
        {
            transform.position = _targetPosition;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPosition,
                moveSpeed * Time.deltaTime
            );
        }
    }

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>Change the wander radius at runtime.</summary>
    public void SetWanderRadius(float radius) => wanderRadius = Mathf.Max(0f, radius);

    /// <summary>Change the wander interval at runtime.</summary>
    public void SetInterval(float seconds)
    {
        interval = Mathf.Max(0.05f, seconds);
        _timer   = Mathf.Min(_timer, interval);
    }

    /// <summary>Manually force the object to move to a new random position immediately.</summary>
    public void ForceWander()
    {
        _timer          = 0f;
        _targetPosition = PickRandomPosition();
    }

    // ── Gizmos ─────────────────────────────────────────────────────────────────
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 origin = (useStartPositionAsOrigin && Application.isPlaying)
            ? _origin
            : transform.position;

        // Filled disc
        UnityEditor.Handles.color = gizmoColor;
        UnityEditor.Handles.DrawSolidDisc(origin, Vector3.up, wanderRadius);

        // Outline circle
        UnityEditor.Handles.color = gizmoOutlineColor;
        UnityEditor.Handles.DrawWireDisc(origin, Vector3.up, wanderRadius);

        // Draw a wire sphere for 3D context (when Y is not locked)
        if (!lockY)
        {
            Gizmos.color = gizmoOutlineColor;
            Gizmos.DrawWireSphere(origin, wanderRadius);
        }

        // Line from origin to current target
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(origin, _targetPosition);
            Gizmos.DrawSphere(_targetPosition, 0.15f);
        }
    }
#endif
}