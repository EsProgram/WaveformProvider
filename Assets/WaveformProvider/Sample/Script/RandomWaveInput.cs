using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Es.WaveformProvider.Sample
{
	/// <summary>
	/// Give input of waveform to random position.
	/// </summary>
	public class RandomWaveInput : MonoBehaviour
	{
		[SerializeField]
		private float waitTime = 0.5f;

		[SerializeField]
		private Texture2D waveform;

		[SerializeField, Range(0f, 1f)]
		private float inputScale = 0.05f;

		[SerializeField, Range(0f, 1f)]
		private float inputStrength = 0.1f;

		[SerializeField]
		private List<WaveConductor> targets;

		private void Start()
		{
			StartCoroutine(RandomInput());
		}

		private IEnumerator RandomInput()
		{
			while (true)
			{
				yield return new WaitForSeconds(waitTime);
				foreach (var t in targets)
				{
					var randomUV = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
					t.Input(waveform, randomUV, inputScale, inputStrength);
				}
			}
		}
	}
}