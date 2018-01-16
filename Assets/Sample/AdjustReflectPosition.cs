using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AdjustReflectPosition : MonoBehaviour
{
	[SerializeField]
	private Transform target;

	private void Update()
	{
		if (target == null)
			return;

		var pos = target.position;
		var rotate = target.rotation.eulerAngles;

		pos.y *= -1;
		rotate.x *= -1;
		rotate.z *= -1;

		transform.position = pos;
		transform.rotation = Quaternion.Euler(rotate);
	}
}