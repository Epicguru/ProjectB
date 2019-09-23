using ProjectB.Vehicles.Weapons;
using UnityEngine;

namespace ProjectB.Effects
{
    public class MinigunAnimator : MonoBehaviour
    {
        public MountedWeapon MountedWeapon
        {
            get
            {
                if (_mw == null)
                    _mw = GetComponentInParent<MountedWeapon>();
                return _mw;
            }
        }
        private MountedWeapon _mw;

        public Transform ChainDetail;
        public float ChainSpeed = 720f;
        public Transform Barrels;
        public float BarrelSpeed = 1000f;

        public AnimationCurve WindDownCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
        public float WindDownTime = 4f;

        private float timer = 100f;

        private void Update()
        {
            bool shooting = MountedWeapon.Fire;

            if (shooting)
            {
                timer = 0f;
            }
            else
            {
                timer += Time.deltaTime;
            }

            float scale = shooting ? 1f : WindDownCurve.Evaluate(Mathf.Clamp01(timer / WindDownTime));

            ChainDetail.Rotate(0f, 0f, Time.deltaTime * ChainSpeed * scale, Space.Self);
            Barrels.Rotate(Time.deltaTime * BarrelSpeed * scale, 0f, 0f, Space.Self);
        }
    }
}

