using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;

public class BurstLight : MonoBehaviour
{
    public Light2D Light;
    public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    public float MaxIntensity = 2f;
    public float Duration = 1f;

    private float timer;

    private void UponSpawn()
    {
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float p = Mathf.Clamp01(timer / Duration);
        float intensity = Curve.Evaluate(p) * MaxIntensity;

        Light.intensity = intensity;
    }
}
