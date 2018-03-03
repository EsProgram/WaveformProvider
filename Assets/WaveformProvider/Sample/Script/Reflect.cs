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

		[SerializeField]
		private float nearClipOffset;

		private new Renderer renderer;
		private Material sharedMaterial;

		private readonly int ShaderPropertyReflectTex = Shader.PropertyToID("_RefTex");

		private void Start()
		{
			renderer = GetComponent<Renderer>();
			sharedMaterial = renderer.sharedMaterial;
			reflectCamera.projectionMatrix = reflectCamera.CalculateObliqueMatrix(CalculateCameraSpacePlane(reflectCamera, transform.position, transform.up));
			reflectCamera.targetTexture = new RenderTexture(Screen.width, Screen.height, 16);
			sharedMaterial.SetTexture(ShaderPropertyReflectTex, reflectCamera.targetTexture);
		}

		private Vector4 CalculateCameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign = 1f)
		{
			Vector3 offsetPos = pos + normal * 0.07f;
			Matrix4x4 m = cam.worldToCameraMatrix;
			Vector3 cpos = m.MultiplyPoint(offsetPos);
			Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
			return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
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