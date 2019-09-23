using UnityEngine;

namespace ProjectB.Effects
{
    [RequireComponent(typeof(PoolObject))]
    public class MuzzleFlash : MonoBehaviour
    {
        public PoolObject PoolObject
        {
            get
            {
                if (_po == null)
                    _po = GetComponent<PoolObject>();
                return _po;
            }
        }
        private PoolObject _po;

        public SpriteRenderer Renderer;
        public Sprite[] Sprites;
        public Vector2 Scales = new Vector2(0.7f, 1.1f);
        public float Time;

        private float timer;

        private void UponSpawn()
        {
            timer = 0f;
            Renderer.sprite = Sprites[Random.Range(0, Sprites.Length)];
            transform.localScale = Vector2.one * Random.Range(Scales.x, Scales.y);
        }

        private void Update()
        {
            timer += UnityEngine.Time.deltaTime;

            float p = timer / Time;
            if (p >= 1f)
            {
                PoolObject.Despawn(this.PoolObject);
            }

            var c = Renderer.color;
            c.a = 1f - p;
            Renderer.color = c;
        }
    }
}
