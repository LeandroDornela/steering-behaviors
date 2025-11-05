using System.Collections;
using UnityEngine;

public class ChangeColorOnCollision : MonoBehaviour
{
    public string detectionTag = "Player";
    public Color color = Color.red;
    public float resetTime = 3f;

    private Color _originalColor;
    private Renderer _renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _originalColor = _renderer.material.color;
    }

    IEnumerator ResetColor()
    {
        yield return new WaitForSeconds(resetTime);
        _renderer.material.color = _originalColor;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == detectionTag)
        {
            _renderer.material.color = color;
            StartCoroutine(ResetColor());
        }
    }
}
