Shader "Hidden/Screen Warp"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _Blend;
		int _PointCount;
		float4 _Points[32];
		float2 _ScreenWorldSize;
		float2 _ScreenWorldPos;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
			float2 coord = i.texcoord;
			float2 worldPos = _ScreenWorldPos + coord * _ScreenWorldSize;

			for(int j = 0; j < _PointCount; j++){
				float4 p = _Points[j];

				float2 pos = p.xy;

				float2 diff = pos - worldPos;
				float scale = (p.z - length(diff)) / p.z;

				if(scale < 0)
					scale = 0;
				if(scale > 1)
					scale = 1;

				float2 move = normalize(diff) * scale * p.w;
				
				coord += float2(move.x / _ScreenWorldSize.x, move.y / _ScreenWorldSize.y);
			}

			//return float4(worldPos.x, worldPos.y, 0, 1);
            return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coord);
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}