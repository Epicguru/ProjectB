
using UnityEngine;

public class LitColor : MonoBehaviour
{
    public Color Colour = Color.white;

    private void Awake()
    {
        GetComponent<Renderer>().material.SetColor("_Color", Colour);
    }
}
