using System;
using System.Collections;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
// ReSharper disable Unity.InefficientPropertyAccess

public class Movement : MonoBehaviour {

	public float forceAmount;
	public float maxSpeed;
	public Transform body;
	public Transform observer;

	[Range(0 , 0.5f )]public float groundSphereCastRadius;
	[Range(0,5)] public float gravityMultiplier;
	
	private Player player;
	private Rigidbody rigidBody;
	private SphereCollider sphereCollider;

	
	private void Awake() {
		player = ReInput.players.GetPlayer(0);
		rigidBody = GetComponent<Rigidbody>();
		sphereCollider = GetComponent<SphereCollider>();
		Cursor.lockState = CursorLockMode.Locked;
	}

	private void Update() {
		body.position = transform.position;
		body.LookAt(transform.position + rigidBody.velocity.normalized);
	}

	private void FixedUpdate() {
		ApplyForces();

	}

	private void ApplyForces() {
		
		//Input Force
		var input = new Vector3(player.GetAxis("XAxis"), 0, player.GetAxis("YAxis"));
		Vector3 projInput;
		Vector3 forwardInput;
		Vector3 sidewaysInput;
		if(IsGrounded()) {
			Transform b = body.transform;
			float delta = observer.transform.rotation.eulerAngles.y - b.rotation.eulerAngles.y;
			b.Rotate(b.up,delta);
			//Calculate the transform based on the body but which faces the direction of the camera
			projInput = Vector3.ProjectOnPlane(b.TransformDirection(input), b.transform.up);
			forwardInput = Vector3.Project(projInput, b.forward);
			sidewaysInput = projInput - forwardInput;
		}
		else {
			projInput = 0.01f * Vector3.ProjectOnPlane(observer.transform.TransformVector(input), observer.transform.up);
			forwardInput = projInput;
			sidewaysInput = Vector3.zero;;
		}

		if (rigidBody.velocity.magnitude < maxSpeed) {
			//rigidBody.AddForce(projInput * (forceAmount * Time.fixedDeltaTime),ForceMode.Acceleration);
			rigidBody.AddForce(forwardInput * (forceAmount * Time.fixedDeltaTime),ForceMode.Acceleration);
			rigidBody.AddTorque(0,sidewaysInput.magnitude,0,ForceMode.Acceleration);
		}
		if (rigidBody.velocity.magnitude < Mathf.Epsilon) {
			rigidBody.velocity = Vector3.zero;
		}
		
		//Gravity
		rigidBody.AddForce(gravityMultiplier * Physics.gravity, ForceMode.Acceleration);
	}

	private bool IsGrounded() {
		return Physics.SphereCast(transform.position, groundSphereCastRadius, - body.up.normalized, out RaycastHit hit, sphereCollider.radius + groundSphereCastRadius);
	}

	private void OnDrawGizmos() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(transform.position,groundSphereCastRadius);
		if(Application.isPlaying) {
			Gizmos.DrawLine(transform.position, transform.position - body.up.normalized * (sphereCollider.radius + groundSphereCastRadius));
			Gizmos.DrawWireSphere(transform.position - body.up.normalized * (sphereCollider.radius + groundSphereCastRadius), groundSphereCastRadius );
		}
		
	}

	

}