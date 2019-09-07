using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ScreenWarpRenderer), PostProcessEvent.AfterStack, "Custom/Screen Warp")]
public sealed class ScreenWarp : PostProcessEffectSettings
{
    [Range(0f, 3f), Tooltip("Global warp effect size scale.")]
    public FloatParameter sizeScale = new FloatParameter { value = 1f };

    [Range(-2f, 2f), Tooltip("Global warp effect intensity scale.")]
    public FloatParameter intensityScale = new FloatParameter { value = 1f };
}

public sealed class ScreenWarpRenderer : PostProcessEffectRenderer<ScreenWarp>
{
    private static ScreenWarpRenderer Instance;
    private static readonly Vector4[] points = new Vector4[32];
    private static int lastIndex;

    public static void AddPoint(Vector2 pos, float size, float scale)
    {
        AddPoint(new Vector4(pos.x, pos.y, size, scale));
    }

    public static void AddPoint(Vector4 data)
    {
        if(lastIndex == points.Length)
        {
            Debug.LogWarning("Ran out of warp points! Soft limit is 32 for optimization purposes!");
            return;
        }

        if(Instance != null)
        {
            data.z *= Instance.settings.sizeScale.value;
            data.w *= Instance.settings.intensityScale.value;
        }

        if (data.z < 0f)
            data.z = 0f;

        points[lastIndex] = data;
        lastIndex++;
    }

    public override void Render(PostProcessRenderContext context)
    {
        if (Instance != this)
            Instance = this;

        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Screen Warp"));

        Vector2 size = new Vector2(context.camera.orthographicSize * context.camera.aspect, context.camera.orthographicSize);
        Vector2 pos = (Vector2)context.camera.transform.position - size;

        sheet.properties.SetInt("_PointCount", lastIndex);
        sheet.properties.SetVector("_ScreenWorldSize", size * 2f);
        sheet.properties.SetVector("_ScreenWorldPos", pos);
        sheet.properties.SetVectorArray("_Points", points);

        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

        lastIndex = 0;
    }
}