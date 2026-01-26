using UnityEngine;
using UnityEngine.UI;

public class ButtonShader : MonoBehaviour
{
    public Material materialPersonalizado;

    void Start()
    {
        Image img = GetComponent<Image>();
        img.material = materialPersonalizado;
    }
}

