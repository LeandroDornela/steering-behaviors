using UnityEngine;

public class DestroyOnCollide : MonoBehaviour
{
    public GameObject particle;


    void OnCollisionEnter(Collision collision)
    {
        if (!this.enabled) return;

        Instantiate(particle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
