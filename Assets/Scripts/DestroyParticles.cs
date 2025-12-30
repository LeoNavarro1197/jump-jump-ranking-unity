using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("DestroyParticle", 2f);
    }

    void DestroyParticle()
    {
        Destroy(gameObject);
    }
}
