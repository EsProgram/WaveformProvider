using UnityEngine;
using Es.WaveformProvider;

namespace Es.InkPainter.Sample
{
	[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
	public class CollisionWaveInput : MonoBehaviour
	{
		private void OnCollisionEnter(Collision collision)
		{
			WaveInput(collision);
		}

		public void OnCollisionStay(Collision collision)
		{
			WaveInput(collision);
		}

		private void WaveInput(Collision collision)
		{
			foreach (var p in collision.contacts)
			{
				var canvas = p.otherCollider.GetComponent<WaveConductor>();
				if (canvas != null)
					canvas.Input(p.point, Random.Range(0.01f, 0.05f));
			}
		}
	}
}