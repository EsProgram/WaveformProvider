using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.InkPainter;

//TODO:InkCanvasを動的に追加するよう変更(HeightPaintのみ。WaveInput用のRenderTextureを強制的に生成してセットしたい)
//TODO:waveMaterialをResources.Loadで呼ぶよう変更
//TODO:名前空間
//TODO:waveMaterialにセットする変数をそれぞれインスペクターから設定できるよう変更
[RequireComponent(typeof(InkCanvas))]
public class Wave : MonoBehaviour
{
	public Material waveMaterial;
	private Texture2D init;
	private RenderTexture input;
	private RenderTexture prev;
	private RenderTexture prev2;
	private RenderTexture result;
	private new Renderer renderer;

	private readonly int ShaderPropertyInputTex = Shader.PropertyToID("_InputTex");
	private readonly int ShaderPropertyPrevTex = Shader.PropertyToID("_PrevTex");
	private readonly int ShaderPropertyPrev2Tex = Shader.PropertyToID("_Prev2Tex");
	private readonly int ShaderPropertyWaveTex = Shader.PropertyToID("_WaveTex");

	[Range(1, 10)]
	public int updateFrameTiming = 3;

	public bool debug;

	private void Awake()
	{
		//初期化処理
		//InkPanterで波動方程式計算時の入力テクスチャへ書き込みを行うので
		//InkPainterでキャンバスの初期化終了後のタイミングでペイント用のテクスチャを取得しておく。
		GetComponent<InkCanvas>().OnInitializedAfter += canvas =>
		{
			//入力初期化用のテクスチャを作っておく
			init = new Texture2D(1, 1);
			init.SetPixel(0, 0, new Color(0, 0, 0, 0));
			init.Apply();

			//入力用テクスチャを取得し、波動方程式を求めるのに必要なバッファを生成
			input = canvas.GetPaintHeightTexture("Reflect");
			prev = new RenderTexture(input.width, input.height, 0, RenderTextureFormat.R8);
			prev2 = new RenderTexture(input.width, input.height, 0, RenderTextureFormat.R8);
			result = new RenderTexture(input.width, input.height, 0, RenderTextureFormat.R8);

			//バッファの初期化
			var r8Init = new Texture2D(1, 1);
			r8Init.SetPixel(0, 0, new Color(0.5f, 0, 0, 1));
			r8Init.Apply();
			Graphics.Blit(r8Init, prev);
			Graphics.Blit(r8Init, prev2);
		};

		renderer = GetComponent<Renderer>();
	}

	private void OnWillRenderObject()
	{
		WaveUpdate();
	}

	private void WaveUpdate()
	{
		if (Time.frameCount % updateFrameTiming != 0)
			return;

		if (input == null)
			return;

		waveMaterial.SetTexture(ShaderPropertyInputTex, input);
		waveMaterial.SetTexture(ShaderPropertyPrevTex, prev);
		waveMaterial.SetTexture(ShaderPropertyPrev2Tex, prev2);

		//波動方程式を解いてresultに格納
		Graphics.Blit(null, result, waveMaterial);

		var tmp = prev2;
		prev2 = prev;
		prev = result;
		result = tmp;

		//入力用テクスチャを初期化
		Graphics.Blit(init, input);

		renderer.sharedMaterial.SetTexture(ShaderPropertyWaveTex, prev);
	}

	private void OnGUI()
	{
		if (debug)
		{
			var h = Screen.height / 3;
			const int StrWidth = 20;
			GUI.Box(new Rect(0, 0, h, h * 3), "");
			GUI.DrawTexture(new Rect(0, 0 * h, h, h), Texture2D.whiteTexture);
			GUI.DrawTexture(new Rect(0, 0 * h, h, h), input);
			GUI.DrawTexture(new Rect(0, 1 * h, h, h), prev);
			GUI.DrawTexture(new Rect(0, 2 * h, h, h), prev2);
			GUI.Box(new Rect(0, 1 * h - StrWidth, h, StrWidth), "INPUT");
			GUI.Box(new Rect(0, 2 * h - StrWidth, h, StrWidth), "PREV");
			GUI.Box(new Rect(0, 3 * h - StrWidth, h, StrWidth), "PREV2");
		}
	}
}