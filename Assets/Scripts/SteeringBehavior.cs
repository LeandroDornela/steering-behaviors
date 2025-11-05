using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SteeringBehavior : MonoBehaviour
{
    [SerializeField] private Transform[] _targets;
    [SerializeField] private Rigidbody _rigidbody;

    [Header("Seek and Arrive")]
    [SerializeField] private float _slowRadius = 3f;
    [SerializeField] private float _minDistanceRadius = 1.5f;
    //[SerializeField] private float _maxSteeringForce = 10f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _minSpeed = 0f;

    [Header("Avoid")]
    [SerializeField] private float _avoidWeight = 3f;
    //[SerializeField] private float avoidDistance = 2f;
    private float lookahead = 5f;
    [SerializeField] private float obstacleAvoidRadius = 1f;

    [Header("Wander")]
    [SerializeField] private float _wanderingUpdateInterval = 2f;
    [SerializeField] private float _wanderingRadius = 2f;
    [SerializeField] private float _wanderWeight = 1;

    [Header("Projectile testing")]
    public bool shoot = false;
    public GameObject projectile;
    public Transform projectileOrigin;
    public float shootDistance = 300;
    public float shootInterval = 0.3f;
    private float shootTimer = 0;

    private Vector3 _desiredDirection;
    private Vector3 _steeringForce;

    private Vector3 _avoidTarget;
    private bool _evading = false; // uma alternativa para saber se esta evitando um obstaculo é saber se persegue um alvo e a posição do alvo não é a posição para onde se está indo(posição de evazao)

    Vector3 _lastSteeringForce;
    Vector3 _lastWanderDirection;

    Vector3 randomSphereDirection = Vector3.zero;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("UpdateRandomSphereDirection", 0, _wanderingUpdateInterval);

        randomSphereDirection = transform.forward;
    }

    void FixedUpdate()
    {
        _steeringForce = Vector3.zero;
        _desiredDirection = Vector3.zero;
        float lookahead = _rigidbody.linearVelocity.magnitude;
        float avoidDistance = lookahead/5;

        Vector3 averageTargetsPosition = GetAverageTargetPosition();
        //_avoidTarget = averageTargetsPosition;

        // WANDER
        _lastWanderDirection = Vector3.Slerp(_lastWanderDirection, randomSphereDirection, 1 * Time.fixedDeltaTime);
        Vector3 wanderPosition = transform.position + _lastWanderDirection + transform.forward * 3;

        // Stop evading close to evade position. Do this before obstacle check to avoid ignoring evading position.
        if (_evading && (transform.position - _avoidTarget).magnitude < 2)
        {
            //Debug.Log((transform.position - _avoidTarget).magnitude);
            _evading = false;
        }

        // OBSTACLE AVOIDANCE
        RaycastHit hit;
        Vector3 p1 = transform.position;
        if (Physics.SphereCast(p1, obstacleAvoidRadius, _rigidbody.linearVelocity, out hit, lookahead))
        {
            _avoidTarget = hit.point + (hit.normal * avoidDistance);
            //Vector3 normalProjectionOnRight = Vector3.Project(hit.normal, transform.right).normalized;
            //_avoidTarget = hit.point + (normalProjectionOnRight * avoidDistance);
            Debug.DrawLine(hit.point, _avoidTarget, Color.magenta, 0.1f);
            _evading = true;
        }
        else
        {
            //_evading = false;
        }

        // Check free path to the target.
        //if (!Physics.Raycast(transform.position, averageTargetsPosition - transform.position, 999f))
        if (!Physics.SphereCast(p1, obstacleAvoidRadius, transform.forward, out hit, (averageTargetsPosition - transform.position).magnitude))
        {
            _evading = false;
        }

        //Testing shoot
        if(shoot)
        {
            if (shootTimer <= 0)
            {
                shootTimer = shootInterval;

                if (Physics.Raycast(p1 + projectileOrigin.forward * 2, projectileOrigin.forward, out hit, shootDistance))
                {
                    if (hit.transform.gameObject.tag == "Target")
                    {
                        Instantiate(projectile, projectileOrigin.position, projectileOrigin.rotation).GetComponent<Rigidbody>().AddForce(Random.onUnitSphere / 10 + projectileOrigin.forward * 100, ForceMode.Impulse);
                    }
                }
            }
            else
            {
                shootTimer -= Time.fixedDeltaTime;
            }
        }


        if (_evading) _steeringForce += _avoidWeight * GetArriveForce(_avoidTarget);
        if(!_evading)_steeringForce += GetArriveForce(averageTargetsPosition);
        _steeringForce += _wanderWeight * GetArriveForce(wanderPosition);
        //_steeringForce.Normalize();
        //_steeringForce = _steeringForce * _maxSteeringForce;

        // Play the steering force.
        _steeringForce = Vector3.Slerp(_lastSteeringForce, _steeringForce, 5 * Time.fixedDeltaTime);
        _rigidbody.AddForce(_steeringForce, ForceMode.Force);

        // Correct min speed.
        if (_rigidbody.linearVelocity.magnitude < _minSpeed)
        {
            _rigidbody.linearVelocity = _rigidbody.linearVelocity.normalized * _minSpeed;
        }

        _lastSteeringForce = _steeringForce;

        // Update rotation.
        Vector3 estimatedUp = _steeringForce.normalized + Vector3.up + transform.up;
        Quaternion newRotation = Quaternion.LookRotation(_rigidbody.linearVelocity, estimatedUp);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, 0.8f * lookahead * Time.fixedDeltaTime); // lookahead provisorio pra fazer a velocidade influenciar a suavidade da interpolação.
    }

    Vector3 GetAverageTargetPosition()
    {
        Vector3 averageTargetsPosition = Vector3.zero;

        foreach (Transform target in _targets)
        {
            Debug.DrawLine(transform.position, target.position, Color.white);
            averageTargetsPosition += target.position;
        }

        return averageTargetsPosition /= _targets.Length;
    }

    // Arrive is seek with slow radius.
    // TODO: Add Flee when refactoring.
    Vector3 GetArriveForce(Vector3 targetPosition)
    {
        Vector3 targetDirection = targetPosition - transform.position;
        float targetDistance = targetDirection.magnitude;

        // ARRIVE
        // Get the module of the desired velocity.
        float desiredSpeed;
        desiredSpeed = _maxSpeed * ((targetDistance - _minDistanceRadius) / _slowRadius); // Quanto menor a target distance em relação ao slow radius menor será a aceleração.
        if (desiredSpeed > _maxSpeed)
        {
            desiredSpeed = _maxSpeed;
        }
        else if(desiredSpeed < _minSpeed)
        {
            desiredSpeed = _minSpeed;
        }
        //desiredSpeed = Mathf.Min(_maxSpeed, desiredSpeed); // Limita a velocidade a velocidade maxima.

        // Get the scaled velocity.
        Vector3 desiredVelocity = desiredSpeed * targetDirection.normalized;

        _desiredDirection += desiredVelocity;

        Debug.DrawRay(transform.position, desiredVelocity, Color.red);
        return desiredVelocity - _rigidbody.linearVelocity;
    }

    
    void UpdateRandomSphereDirection()
    {
        randomSphereDirection = UnityEngine.Random.onUnitSphere * _wanderingRadius;
    }


    void OnDrawGizmos()
    {
        Vector3 velocity;
        Vector3 forward;
        if (_rigidbody != null)
        {
            velocity = _rigidbody.linearVelocity;
            forward = velocity.normalized;
        }
        else
        {
            velocity = transform.forward;
            forward = velocity;
        }

        Vector3 start = transform.position;
        Vector3 end = start + forward * lookahead; // O raio é um extra e não é considerado

        Gizmos.color = Color.cyan;
        //Gizmos.DrawWireSphere(start, obstacleAvoidRadius);
        Gizmos.DrawWireSphere(end, obstacleAvoidRadius);
        Gizmos.DrawLine(start, end);

        Handles.Label(transform.position + _desiredDirection, $"Desired Direction");
        Debug.DrawRay(transform.position, _desiredDirection, Color.red);

        Handles.Label(transform.position + _steeringForce, $"SF:{_steeringForce.magnitude}");
        Debug.DrawRay(transform.position, _steeringForce, Color.yellow);

        Gizmos.color = Color.magenta;
        if(_evading) Gizmos.DrawWireSphere(_avoidTarget, 1);

        Handles.Label(transform.position + velocity, $"V: {velocity.magnitude}");
        Debug.DrawRay(transform.position, velocity, Color.green);

        //Handles.Label(_target.position, $"Target");
        //Debug.DrawRay(transform.position, _targetDirection, Color.white);

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(_target.position, _slowRadius);
        //Gizmos.DrawWireSphere(_target.position, _minDistanceRadius);
    }
}
