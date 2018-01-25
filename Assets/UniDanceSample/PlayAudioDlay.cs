using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayAudioDlay : MonoBehaviour
{
	// Use this for initialization
	public float delay;

	private void Start()
	{
		GetComponent<AudioSource>().Play((ulong)(44100 * delay));
	}
}