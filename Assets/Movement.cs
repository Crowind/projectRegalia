using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

public class Movement : MonoBehaviour {

	public float forceAmount;
	public float maxSpeed;
	public float tiltMultiplier;
	public Transform body;
	public Transform observer;

	[Range(0, 0.5f)] public float groundSphereCastRadius;
	[Range(0, 5)] public float gravityMultiplier;

	private Player player;
	private Rigidbody rigidBody;
	private SphereCollider sphereCollider;

	//input get and calculated
	private Vector3 input;
	private Vector3 sidewaysInput;
	private Vector3 projInput;
	private Vector3 forwardInput;
	private Vector3 deltaTilt;

	private void Awake() {
		player = ReInput.players.GetPlayer(0);
		rigidBody = GetComponent<Rigidbody>();
		sphereCollider = GetComponent<SphereCollider>();
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update() {
		body.position = transform.position;
		body.LookAt(transform.position + rigidBody.velocity.normalized);
		body.rotation *=  Quaternion.AngleAxis( sidewaysInput.magnitude , body.forward);
	}

	private void FixedUpdate() {

		GetInput();
		ApplyForces();

	}

	private void GetInput() {

		
		Transform b = body.transform;
		float delta = observer.transform.rotation.eulerAngles.y - b.rotation.eulerAngles.y;
		b.Rotate(b.up,delta);
		
		//Input Force
		input = new Vector3(player.GetAxis("XAxis"), 0, player.GetAxis("YAxis"));

		if (IsGrounded()) {
			projInput = Vector3.ProjectOnPlane(observer.TransformDirection(input), body.transform.up).normalized;
		}
		else {
			projInput = 0.01f * Vector3.ProjectOnPlane(observer.TransformDirection(input), observer.transform.up).normalized;
		}
		forwardInput = Vector3.Project(projInput, body.transform.forward);
		sidewaysInput = projInput - forwardInput;
		
		projInput = Vector3.ProjectOnPlane(b.TransformDirection(input), b.transform.up);
		
	}

	private void ApplyForces() {


		if (rigidBody.velocity.magnitude < maxSpeed) {
			rigidBody.AddForce(projInput * (forceAmount * Time.fixedDeltaTime), ForceMode.Acceleration);

			// float tiltAmount = 3 * rigidBody.velocity.magnitude / maxSpeed;
			//
			// Vector3 dY = -Vector3.up * 0;
			// Vector3 dZ = (forwardInput * (sphereCollider.radius * tiltAmount));
			// Vector3 dX = (sidewaysInput * (sphereCollider.radius * tiltAmount * tiltMultiplier));
			//
			// deltaTilt = dY + dX + dZ;
			// rigidBody.centerOfMass = transform.InverseTransformPoint(body.transform.position + dX + dY + dZ);

		}
		if (rigidBody.velocity.magnitude < Mathf.Epsilon) {
			rigidBody.velocity = Vector3.zero;
		}

		//Gravity
		rigidBody.AddForce(gravityMultiplier * Physics.gravity, ForceMode.Acceleration);
	}

	private bool IsGrounded() {
		return Physics.SphereCast(transform.position, groundSphereCastRadius, -body.up.normalized, out RaycastHit hit, sphereCollider.radius + groundSphereCastRadius);
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position, groundSphereCastRadius);
		if (Application.isPlaying) {
			Gizmos.DrawLine(transform.position, transform.position - body.up.normalized * (sphereCollider.radius + groundSphereCastRadius));
			Gizmos.DrawWireSphere(transform.position - body.up.normalized * (sphereCollider.radius + groundSphereCastRadius), groundSphereCastRadius);

			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(body.transform.position + deltaTilt, 0.25f);
			Gizmos.color = new Color(0.5f, 0, 1);
			Gizmos.DrawRay(body.transform.position, 3 * input);
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(body.transform.position, projInput);
			Gizmos.color = new Color(1, 0.5f, 0);
			Gizmos.DrawRay(body.transform.position, forwardInput);

		}

	}

}