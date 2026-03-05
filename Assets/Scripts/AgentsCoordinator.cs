using SteeringBehaviours;
using UnityEngine;

public class AgentsCoordinator : MonoBehaviour
{
    [SerializeField] private GameObject _steeringAgentPrefab;
    [SerializeField] private Transform _target;
    [SerializeField] private int _numberOfAgents = 500;

    private SteeringAgent[] _steeringAgents;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: Just for testing.
        
        _steeringAgents = new SteeringAgent[_numberOfAgents];

        int columns = Mathf.CeilToInt(Mathf.Sqrt(_numberOfAgents)); // grid width
        float spacing = 1.5f; // distance between agents
        
        for (int i = 0; i < _numberOfAgents; i++)
        {
            int row = i / columns;
            int col = i % columns;

            Vector3 offset = new Vector3(col * spacing, row * spacing, 0);

            _steeringAgents[i] = Instantiate(
                _steeringAgentPrefab,
                transform.position + offset,
                Quaternion.identity,
                transform
            ).GetComponent<SteeringAgent>();
            
            _steeringAgents[i]
                .GetComponent<SteeringPerceptionFixedTargets>()
                .SetTargets(new Transform[] { _target });
        }
    }


    void FixedUpdate()
    {
        foreach (SteeringAgent agent in _steeringAgents)
        {
            agent.CustomUpdate();
        }
    }
}
