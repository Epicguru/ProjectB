
using ProjectB.Effects.PostProcessing;
using UnityEngine;

namespace ProjectB.Effects
{
    [ExecuteInEditMode]
    public class WarpPoint : MonoBehaviour
    {
        public Animator Anim;
        [Range(0, 1)]
        public int Type = 0;

        [Header("Radius")]
        public float Radius = 5f;
        public float RadiusScale = 1f;

        [Header("Intensity")]
        public float Intensity = 1.5f;
        public float IntensityScale = 1f;

        public void Run()
        {
            Anim.SetInteger("Type", Type);
            Anim.SetTrigger("Run");
        }

        private void LateUpdate()
        {
            ScreenWarpRenderer.AddPoint(transform.position, Radius * RadiusScale, Intensity * IntensityScale);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, Radius);
        }
    }
}
