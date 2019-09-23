
using UnityEngine;

namespace ProjectB
{
    public class LitColor : MonoBehaviour
    {
        public Color Colour = Color.white;

        private void Awake()
        {
            GetComponent<Renderer>().material.SetColor("_Color", Colour);
        }
    }
}

