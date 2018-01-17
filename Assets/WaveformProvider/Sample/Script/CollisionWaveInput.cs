using UnityEngine;

namespace Es.InkPainter.Sample
{
	[RequireComponent(typeof(Collider), typeof(MeshRenderer))]
	public class CollisionWaveInput : MonoBehaviour
	{
		[SerializeField]
		private Brush brush = null;

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
				var canvas = p.otherCollider.GetComponent<InkCanvas>();
				if (canvas != null)
					canvas.Paint(brush, p.point);
			}
		}
	}
}