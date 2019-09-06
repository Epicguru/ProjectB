using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
public class HitscanEffect : MonoBehaviour
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

    public LineRenderer Renderer;
    public float StartFadeTime = 0.7f, EndFadeTime = 1f;
    public float Scale = 0.1f;

    private float timer;

    private void UponSpawn()
    {
        timer = 0f;
        Renderer.widthMultiplier = Scale;
    }

    public void Set(Vector2 start, Vector2 end)
    {
        Renderer.SetPosition(0, start);
        Renderer.SetPosition(1, end);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer > StartFadeTime)
        {
            float range = EndFadeTime - StartFadeTime;
            float amount = timer - StartFadeTime;
            float p = amount / range;
            Renderer.widthMultiplier = (1f - p) * Scale;
        }
        if(timer > EndFadeTime)
        {
            PoolObject.Despawn(this.PoolObject);
        }
    }
}
