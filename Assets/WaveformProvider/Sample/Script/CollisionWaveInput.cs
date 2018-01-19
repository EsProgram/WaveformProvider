using UnityEngine;
using Es.WaveformProvider;

namespace Es.InkPainter.Sample
{
	[RequireComponent(typeof(Collider), typeof(MeshRenderer), typeof(Rigidbody))]
	public class CollisionWaveInput : MonoBehaviour
	{
			[SerializeField]
			float inputScaleFitter = 0.01f;

		Rigidbody rigidbody;

		void Awake()
		{
			rigidbody = GetComponent<Rigidbody>();
		}

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
					canvas.Input(p.point, rigidbody.velocity.magnitude * rigidbody.mass * inputScaleFitter);
			}
		}
	}
}