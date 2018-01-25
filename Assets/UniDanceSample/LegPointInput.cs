using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Es.WaveformProvider;

public class LegPointInput : MonoBehaviour
{
	public Texture2D waveform;
	public int paintTiming = 3;

	[Range(0, 1)]
	public float scale = 0.01f;

	[Range(0, 1)]
	public float strength = 0.01f;

	private int count;

	private void OnCollisionStay(Collision collision)
	{
		count++;
		if (count >= paintTiming)
		{
			count = 0;
			foreach (var p in collision.contacts)
			{
				var waveConductor = p.otherCollider.GetComponent<WaveConductor>();
				if (waveConductor != null)
					waveConductor.Input(waveform, p.point, scale, strength);
			}
		}
	}
}