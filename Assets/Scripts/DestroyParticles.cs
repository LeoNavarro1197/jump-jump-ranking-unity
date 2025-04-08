using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("DestroyParticle", 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroyParticle()
    {
        Destroy(gameObject);
    }
}
