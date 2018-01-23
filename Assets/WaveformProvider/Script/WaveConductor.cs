using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Es.InkPainter;
using UnityEngine;

namespace Es.WaveformProvider
{
	/// <summary>
	/// Generates a waveform from input and outputs it as a texture.
	/// </summary>
	[RequireComponent(typeof(Renderer))]
	[DisallowMultipleComponent]
	public class WaveConductor : MonoBehaviour
	{
		#region Serialized field

		/// <summary>
		/// Whether to update the waveform.
		/// </summary>
		[SerializeField]
		public bool update = true;

		/// <summary>
		/// Waveforms are updated for each specified drawing process count.
		/// </summary>
		[Range(1, 10)]
		public int updateFrameTiming = 3;

		/// <summary>
		/// To correct waveform input value.
		/// </summary>
		[Range(0f, 1f)]
		public float adjuster = 0f;

		/// <summary>
		/// Distance factor on texture space to measure displacement.
		/// </summary>
		[Range(0.01f, 2f)]
		public float stride = 1f;

		/// <summary>
		/// Adjusts the wave attenuation factor.
		/// </summary>
		[Range(0.1f, 1f)]
		public float attenuation = 0.96f;

		/// <summary>
		/// Related to wave propagation speed.
		/// </summary>
		[Range(0.01f, 0.5f)]
		public float propagationSpeed = 0.1f;

		/// <summary>
		/// Wave input texture size.
		/// </summary>
		[SerializeField]
		private int inputTextureSize = 512;

		[SerializeField]
		private RenderTexture output;

		[SerializeField]
		private bool debug;

		#endregion Serialized field

		#region private field

		private static Material waveMaterial;
		private Texture2D init;
		private RenderTexture input;
		private RenderTexture prev;
		private RenderTexture prev2;
		private RenderTexture result;
		private InkCanvas inkCanvas;
		private Brush brush;

		private readonly int ShaderPropertyAdjust = Shader.PropertyToID("_RoundAdjuster");
		private readonly int ShaderPropertyStride = Shader.PropertyToID("_Stride");
		private readonly int ShaderPropertyAttenuation = Shader.PropertyToID("_Attenuation");
		private readonly int ShaderPropertyC = Shader.PropertyToID("_C");
		private readonly int ShaderPropertyInputTex = Shader.PropertyToID("_InputTex");
		private readonly int ShaderPropertyPrevTex = Shader.PropertyToID("_PrevTex");
		private readonly int ShaderPropertyPrev2Tex = Shader.PropertyToID("_Prev2Tex");

		#endregion private field

		#region property

		/// <summary>
		/// Waveform data output texture.
		/// </summary>
		/// <returns>RenderTexture that stores the height of the wave.</returns>
		public RenderTexture Output
		{
			get { return output; }
			set { output = value; }
		}

		private Material WaveMaterial
		{
			get
			{
				if (waveMaterial == null)
					waveMaterial = new Material(Resources.Load<Material>("Es.WaveformProvider.WaveProvide"));
				return waveMaterial;
			}
		}

		#endregion property

		#region unity event method

		private void Awake()
		{
			#region create input brush

			brush = new Brush(
					brushTex: Texture2D.whiteTexture,
					scale: 0.1f,
					color: Color.white,
					normalTex: null,
					normalBlend: 0f,
					heightTex: Texture2D.whiteTexture,
					heightBlend: 1f,
					colorBlending: Brush.ColorBlendType.UseBrush,
					normalBlending: Brush.NormalBlendType.UseBrush,
					heightBlending: Brush.HeightBlendType.Add
				);

			#endregion create input brush

			#region Create InkCanvas component

			var inputMaterial = new Material(Resources.Load<Material>("Es.WaveformProvider.WaveInput"));
			InkCanvas.PaintSet paintSet = new InkCanvas.PaintSet("", "", "_ParallaxMap", false, false, true, inputMaterial);
			inkCanvas = gameObject.AddInkCanvas(paintSet);
			inkCanvas.hideFlags = HideFlags.HideInInspector;

			#endregion Create InkCanvas component

			#region Initialize texture

			inkCanvas.OnInitializedAfter += canvas =>
			{
				paintSet.paintHeightTexture = new RenderTexture(inputTextureSize, inputTextureSize, 0, RenderTextureFormat.R8);

				init = new Texture2D(1, 1);
				init.SetPixel(0, 0, new Color(0, 0, 0, 0));
				init.Apply();

				input = paintSet.paintHeightTexture;
				prev = new RenderTexture(input.width, input.height, 0, RenderTextureFormat.R8);
				prev2 = new RenderTexture(input.width, input.height, 0, RenderTextureFormat.R8);
				result = new RenderTexture(input.width, input.height, 0, RenderTextureFormat.R8);

				var r8Init = new Texture2D(1, 1);
				r8Init.SetPixel(0, 0, new Color(0.5f, 0, 0, 1));
				r8Init.Apply();
				Graphics.Blit(r8Init, prev);
				Graphics.Blit(r8Init, prev2);
			};

			#endregion Initialize texture
		}

		private void OnWillRenderObject()
		{
			WaveUpdate();
		}

		private void OnGUI()
		{
			if (debug)
			{
				var h = Screen.height / 3;
				const int StrWidth = 20;
				GUI.Box(new Rect(0, 0, h, h * 3), "");
				GUI.DrawTexture(new Rect(0, 0 * h, h, h), input);
				GUI.DrawTexture(new Rect(0, 1 * h, h, h), prev);
				GUI.DrawTexture(new Rect(0, 2 * h, h, h), prev2);
				GUI.Box(new Rect(0, 1 * h - StrWidth, h, StrWidth), "INPUT");
				GUI.Box(new Rect(0, 2 * h - StrWidth, h, StrWidth), "PREV");
				GUI.Box(new Rect(0, 3 * h - StrWidth, h, StrWidth), "PREV2");
			}
		}

		#endregion unity event method

		#region wave input method

		/// <summary>
		/// Make the input waveform.
		/// </summary>
		/// <param name="waveform">Waveform texture.</param>
		/// <param name="uv">UV coordinate of texture.</param>
		/// <param name="scale">waveform scale.(The ratio of the size in the texture)</param>
		/// <param name="strength">Strength of input value.</param>
		public void Input(Texture2D waveform, Vector2 uv, float scale, float strength = 0.1f)
		{
			brush.BrushTexture = waveform;
			brush.Scale = scale;
			brush.HeightBlend = strength;
			inkCanvas.PaintUVDirect(brush, uv);
		}

		/// <summary>
		/// Make the input waveform.
		/// </summary>
		/// <param name="waveform">Waveform texture.</param>
		/// <param name="worldPos">World position.</param>
		/// <param name="scale">Waveform scale.(The ratio of the size in the texture)</param>
		/// <param name="strength">Strength of input value.</param>
		public void Input(Texture2D waveform, Vector3 worldPos, float scale, float strength = 0.1f)
		{
			brush.BrushTexture = waveform;
			brush.Scale = scale;
			brush.HeightBlend = strength;
			inkCanvas.Paint(brush, worldPos);
		}

		/// <summary>
		/// Make the input waveform.
		/// </summary>
		/// <param name="waveform">Waveform texture.</param>
		/// <param name="hitInfo">Raycast result.</param>
		/// <param name="scale">Waveform scale.(The ratio of the size in the texture)</param>
		/// <param name="strength">Strength of input value.</param>
		public void Input(Texture2D waveform, RaycastHit hitInfo, float scale, float strength = 0.1f)
		{
			brush.BrushTexture = waveform;
			brush.Scale = scale;
			brush.HeightBlend = strength;
			inkCanvas.Paint(brush, hitInfo);
		}

		/// <summary>
		/// Make the input waveform.
		/// </summary>
		/// <param name="waveform">Waveform texture.</param>
		/// <param name="worldPos">World position.</param>
		/// <param name="scale">waveform scale.(The ratio of the size in the texture)</param>
		/// <param name="strength">Strength of input value.</param>
		public void InputNearestTriangleSurface(Texture2D waveform, Vector3 worldPos, float scale, float strength = 0.1f)
		{
			brush.BrushTexture = waveform;
			brush.Scale = scale;
			brush.HeightBlend = strength;
			inkCanvas.PaintNearestTriangleSurface(brush, worldPos);
		}

		#endregion wave input method

		private void WaveUpdate()
		{
			#region Check whether to process

			if (!update)
				return;

			if (Time.frameCount % updateFrameTiming != 0)
				return;

			if (input == null || output == null)
				return;

			#endregion Check whether to process

			#region Set of data

			WaveMaterial.SetFloat(ShaderPropertyAdjust, adjuster);
			WaveMaterial.SetFloat(ShaderPropertyStride, stride);
			WaveMaterial.SetFloat(ShaderPropertyAttenuation, attenuation);
			WaveMaterial.SetFloat(ShaderPropertyC, propagationSpeed);
			WaveMaterial.SetTexture(ShaderPropertyInputTex, input);
			WaveMaterial.SetTexture(ShaderPropertyPrevTex, prev);
			WaveMaterial.SetTexture(ShaderPropertyPrev2Tex, prev2);

			#endregion Set of data

			Graphics.Blit(null, result, WaveMaterial);

			var tmp = prev2;
			prev2 = prev;
			prev = result;
			result = tmp;

			Graphics.Blit(init, input);
			Graphics.Blit(prev, output);
		}
	}
}