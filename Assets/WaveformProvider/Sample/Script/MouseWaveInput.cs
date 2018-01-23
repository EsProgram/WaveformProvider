using UnityEngine;

namespace Es.WaveformProvider.Sample
{
	/// <summary>
	/// Generate waveform with mouse input.
	/// </summary>
	public class MouseWaveInput : MonoBehaviour
	{
		[SerializeField]
		private Texture2D waveform;

		[SerializeField, Range(0f, 1f)]
		private float waveScale = 0.05f;

		[SerializeField, Range(0f, 1f)]
		private float strength = 0.1f;

		private void Update()
		{
			if (Input.GetMouseButton(0))
			{
				var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo))
				{
					var waveObject = hitInfo.transform.GetComponent<WaveConductor>();
					if (waveObject != null)
						waveObject.Input(waveform, hitInfo, waveScale, strength);
				}
			}
		}
	}
}