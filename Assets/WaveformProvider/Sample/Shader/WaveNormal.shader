Shader "Es/WaveformProvider/Sample/WaveNormal"
{
	Properties
	{
		//this property is populated with the wave's RenderTexture.
		_WaveTex("Wave",2D) = "gray" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Assets/WaveformProvider/Shader/Lib/WaveUtil.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			//wave texture definition.
			WAVE_TEX_DEFINE(_WaveTex)

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _WaveTex);
				return o;
			}

			float4 frag (v2f i) : SV_Target
			{
				//compute wave normal.
				float3 normal01 = WAVE_NORMAL(_WaveTex, i.uv) * 0.5 + 0.5;
				return float4(normal01, 1);
			}
			ENDCG
		}
	}
}
