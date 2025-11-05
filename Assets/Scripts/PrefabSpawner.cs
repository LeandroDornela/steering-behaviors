using UnityEngine;

// TODO: Temp AI code. Refactor or delete.
public class PrefabSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("The prefab to spawn.")]
    public GameObject prefab;

    [Tooltip("Number of prefabs to spawn.")]
    public int spawnCount = 10;

    [Tooltip("Size of the cube area to spawn in.")]
    public Vector3 spawnAreaSize = new Vector3(10, 10, 10);

    [Tooltip("If true, draw a wire cube in the editor.")]
    public bool showGizmo = true;

    public float initialImpulse = 5;

    void Start()
    {
        SpawnPrefabs();
    }


    [NaughtyAttributes.Button]
    public void SpawnPrefabs()
    {
        if (prefab == null)
        {
            Debug.LogWarning("Prefab not assigned to PrefabSpawner.");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            // Random position within cube centered at this GameObject
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
                Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
                Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
            );

            // Spawn the prefab
            GameObject obj = Instantiate(prefab, randomPos, Quaternion.identity, transform);
            obj.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.onUnitSphere * initialImpulse, ForceMode.Impulse);
        }
    }

    // Optional: visualize spawn area in the editor
    void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, spawnAreaSize);
        }
    }
}
