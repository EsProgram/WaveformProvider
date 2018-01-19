using UnityEngine;

namespace Es.WaveformProvider.Sample
{
	/// <summary>
	/// Perform setup for rendering reflections.
	/// </summary>
	[ExecuteInEditMode]
	public class Reflect : MonoBehaviour
	{
		[SerializeField]
		private Camera reflectCamera;

		private new Renderer renderer;
		private Material sharedMaterial;

		private readonly int ShaderPropertyReflectTex = Shader.PropertyToID("_RefTex");

		private void Start()
		{
			renderer = GetComponent<Renderer>();
			sharedMaterial = renderer.sharedMaterial;
			reflectCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 16);
			sharedMaterial.SetTexture(ShaderPropertyReflectTex, reflectCamera.targetTexture);
		}

		private void OnWillRenderObject()
		{
			var cam = Camera.current;
			if (cam == reflectCamera)
			{
				var refVMatrix = cam.worldToCameraMatrix;
				var refPMatrix = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);
				var refVP = refPMatrix * refVMatrix;
				var refW = renderer.localToWorldMatrix;
				sharedMaterial.SetMatrix("_RefVP", refVP);
				sharedMaterial.SetMatrix("_RefW", refW);

				if (Screen.width != reflectCamera.targetTexture.width || Screen.height != reflectCamera.targetTexture.height)
				{
					reflectCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 16);
					sharedMaterial.SetTexture(ShaderPropertyReflectTex, reflectCamera.targetTexture);
				}

				if (!Application.isPlaying && sharedMaterial.GetTexture(ShaderPropertyReflectTex) == null)
				{
					sharedMaterial.SetTexture(ShaderPropertyReflectTex, reflectCamera.targetTexture);
				}
			}
		}
	}
}