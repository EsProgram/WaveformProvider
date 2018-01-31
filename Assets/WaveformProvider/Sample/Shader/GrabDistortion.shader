Shader "Es/WaveformProvider/Sample/GrabDistortion"
{
	Properties
	{
		//this property is populated with the wave's RenderTexture.
		_WaveTex("Wave",2D) = "gray" {}
		_NormalScale("NormalScale", Float) = 0.01
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" }

		GrabPass
		{
			"_BackgroundTexture"
		}

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
				float4 grabPos : TEXCOORD1;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 grabPos : TEXCOORD1;
			};

			sampler2D _BackgroundTexture;
			float4 _BackgroundTexture_TexelSize;
			float _NormalScale;
			//wave texture definition.
			WAVE_TEX_DEFINE(_WaveTex)

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _WaveTex);
				o.grabPos = ComputeGrabScreenPos(o.pos);
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				//compute wave normal.
				float3 normal = WAVE_NORMAL(_WaveTex, i.uv);
				float2 offset = normal.xy * _NormalScale - _BackgroundTexture_TexelSize.xy;
				float4 bgcolor = tex2D(_BackgroundTexture, i.grabPos.xy / i.grabPos.w + offset);

				return bgcolor;
			}
			ENDCG
		}

	}
}