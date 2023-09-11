using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
	public float rotationSpeed = 10f; // Speed of rotation, can be adjusted in the inspector

	// Update is called once per frame
	void FixedUpdate()
	{
		// Get the current rotation
		Vector3 currentRotation = transform.rotation.eulerAngles;

		// Calculate the new y-rotation based on the speed and time
		float newYRotation = currentRotation.y + (rotationSpeed * Time.deltaTime);

		// Apply the new rotation
		transform.rotation = Quaternion.Euler(currentRotation.x, newYRotation, currentRotation.z);
	}
}
