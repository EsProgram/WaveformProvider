Shader "Es/WaveformProvider/Sample/WaterSurface"
{
	Properties
	{
		[HideInInspector]
		_WaveInputTex("Wave Input Texture", 2D) = "black" {}
		_RefTex("Ref",2D) = "black" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_BumpAmt("BumpAmt", Range(0,10000)) = 0

		//this property is populated with the wave's RenderTexture.
		_WaveTex("Wave",2D) = "gray" {}

		_VertexDisplacementScale("Displacement Scale", Float) = 0.5
		_ParallaxScale("Parallax Scale", Float) = 1
		_NormalScaleFactor("Normal Scale Factor", Float) = 1
	}

	Category
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite On
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha

		CGINCLUDE
		#pragma fragment frag
		#include "UnityCG.cginc"

		//include wave utility.
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
			float4 ref : TEXCOORD1;
		};

		sampler2D _WaveInputTex;
		float4 _WaveInputTex_ST;
		sampler2D _RefTex;
		float4 _RefTex_TexelSize;
		sampler2D _BumpMap;
		float4 _BumpMap_ST;
		float4x4 _RefW;
		float4x4 _RefVP;
		float _BumpAmt;
		float _VertexDisplacementScale;
		float _ParallaxScale;
		float _NormalScaleFactor;

		//wave texture definition.
		WAVE_TEX_DEFINE(_WaveTex)

		fixed4 frag (v2f i) : SV_Target
		{
			float2 bump = UnpackNormal(tex2D( _BumpMap, i.uv + _Time.x / 2 )).rg;

			//compute wave normal.
			bump += WAVE_NORMAL_ADJ(_WaveTex, i.uv, _ParallaxScale, _NormalScaleFactor);

			float2 offset = bump * _BumpAmt * _RefTex_TexelSize.xy;
			i.ref.xy = offset * i.ref.z + i.ref.xy;
			float4 ref = tex2D(_RefTex, i.ref.xy / i.ref.w * 0.5 + 0.5);

			float4 ret = ref;
			ret.a = 1;
			return ret;
		}

		ENDCG
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert

			v2f vert (appdata v)
			{
				v2f o;

				//Move the vertex position up and down with the wave height.
				v.vertex.y += WAVE_HEIGHT(_WaveTex, v.uv) * _VertexDisplacementScale;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.ref = mul(_RefVP, mul(_RefW, v.vertex));
				o.uv = TRANSFORM_TEX(v.uv, _BumpMap);
				return o;
			}

			ENDCG
		}
	}

	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma target 2.0
			#pragma vertex vert

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.ref = mul(_RefVP, mul(_RefW, v.vertex));
				o.uv = TRANSFORM_TEX(v.uv, _BumpMap);
				return o;
			}

			ENDCG
		}
	}
}