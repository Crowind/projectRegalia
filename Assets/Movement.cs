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


	private float sidewaysInput;
	
	
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
		if(IsGrounded()) {
			Transform b = body.transform;
			float delta = observer.transform.rotation.eulerAngles.y - b.rotation.eulerAngles.y;
			b.Rotate(b.up,delta);
			//Calculate the transform based on the body but which faces the direction of the camera
			projInput = Vector3.ProjectOnPlane(b.TransformDirection(input), b.transform.up); //TODO Solve this shit
			//forwardInput = Vector3.Project(projInput, body.forward);
			forwardInput = projInput;
			forwardInput[0] = 0;
			forwardInput[1] = 0;
			//forwardInput[2] = forwardInput[2]>0? forwardInput[2] : 0;
			sidewaysInput = projInput.x;
			Debug.Log(sidewaysInput);
		}
		else {
			projInput = 0.01f * Vector3.ProjectOnPlane(observer.transform.TransformVector(input), observer.transform.up);
			forwardInput = projInput;
			sidewaysInput = 0;;
		}

		if (rigidBody.velocity.magnitude < maxSpeed) {
			//rigidBody.AddForce(projInput * (forceAmount * Time.fixedDeltaTime),ForceMode.Acceleration);
			rigidBody.AddForce(forwardInput * (forceAmount * Time.fixedDeltaTime),ForceMode.Acceleration);
			
			rigidBody.centerOfMass = transform.InverseTransformPoint(body.transform.position + body.transform.right * (sphereCollider.radius * sidewaysInput));
			
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
			
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(body.transform.position + body.transform.right * (sidewaysInput * sphereCollider.radius), 0.25f);
			
		}
		
	}

	

}