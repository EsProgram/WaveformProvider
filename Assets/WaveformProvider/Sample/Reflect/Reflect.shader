Shader "WaterSurface"
{
	Properties
	{
		//TODO:入力用テクスチャを定義させる。マクロで定義するように。Inspectorでは表示しないほうが良さそう
		//プロパティではマクロ使えねぇ・・・どうするか・・・
		//InkCanvasで無理矢理描画できるようにするべきか？
		[HideInInspector]
		_WaveInputTex("Wave Input Texture", 2D) = "black" {}
		_RefTex("Ref",2D) = "black" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_BumpAmt("BumpAmt", Range(0,100)) = 0
		//this property is populated with the wave's RenderTexture.
		_WaveTex("Wave",2D) = "gray" {}
		_ParallaxScale("Parallax Scale", Float) = 1
		_NormalScaleFactor("Normal Scale Factor", Float) = 1
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite On
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha

		CGINCLUDE
		#include "UnityCG.cginc"
		//
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
		float _ParallaxScale;
		float _NormalScaleFactor;
		//wave texture definition.
		WAVE_TEX_DEFINE(_WaveTex)

		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.ref = mul(_RefVP, mul(_RefW, v.vertex));
			o.uv = TRANSFORM_TEX(v.uv, _BumpMap);
			return o;
		}

		fixed4 frag (v2f i) : SV_Target
		{
			float2 bump = UnpackNormal(tex2D( _BumpMap, i.uv + _Time.x / 2 )).rg;
			//compute wave normal.
			// bump += WAVE_NORMAL(_WaveTex, i.uv);
			bump += WAVE_NORMAL_ADJ(_WaveTex, i.uv, _ParallaxScale, _NormalScaleFactor);

			//(通常の法線マップ+波の法線)から、投影するテクスチャのオフセットを計算
			float2 offset = bump * _BumpAmt - _BumpAmt * 0.5;
			i.ref.xy = offset * i.ref.z + i.ref.xy;
			float4 ref = tex2D(_RefTex, i.ref.xy / i.ref.w * 0.5 + 0.5);

			float4 ret = ref;
			ret.a = 1;
			return ret;
		}

		ENDCG

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
}