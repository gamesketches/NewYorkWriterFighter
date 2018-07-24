using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerNumber {P1, P2};
public enum MovementState {Standing, Crouching, Jumping, Attacking, KnockedDown, Recoiling};
public class FighterController : MonoBehaviour {

	static float jumpHeight = 2.37f;
	static int inputLeniency = 3;
	public bool leftSide;
	CharacterAnimator animator;
	public float walkSpeed = 1;
	public playerNumber identity;
	MovementState state;
	public AnimationCurve jumpY;

	// Use this for initialization
	void Start () {
		animator = GetComponent<CharacterAnimator>();
		state = MovementState.Standing;
		//foreach(Transform child in transform) {
			transform.GetChild(2).gameObject.SetActive(false);
		//}
	}
	
	// Update is called once per frame
	void Update () {
		CheckDirectionalInput();
		CheckButtonInput();
	}

	void CheckDirectionalInput() {
		if(state == MovementState.Jumping) {
			Debug.Log("Cancel into attacks here");
		}
		else if(state == MovementState.Recoiling) {
			Debug.Log("taking damage");
		}
		else if(VerticalInput() < 0) {
			animator.SwitchAnimation("Crouch");
		}
		else if(VerticalInput() > 0) {
			StartCoroutine(Jump());
		}
		else if(HorizontalInput() > 0){
			animator.SwitchAnimation("Walk");
			MoveRight(walkSpeed * Time.deltaTime);
		}
		else if(HorizontalInput() < 0) {
			animator.SwitchAnimation("Walk");
			MoveLeft(walkSpeed * Time.deltaTime);
		}
		else animator.SwitchAnimation("Idle");
	}

	void CheckButtonInput() {
		string playerID = identity.ToString();
		if(Input.GetButtonDown(playerID + "LP")) {
			transform.GetChild(2).gameObject.SetActive(true);
		}
	}

	public void MoveRight(float distance) {
		transform.Translate(distance, 0, 0);
	}

	public void MoveLeft(float distance) {
		transform.Translate(-distance, 0, 0);
	}

	IEnumerator Jump() {
		float groundedY = transform.position.y;
		state = MovementState.Jumping;
		Vector3 temp = transform.position;
		animator.SwitchAnimation("Jump");
		int leniencyTimer = inputLeniency;
		int jumpDirection = 0;
		for(float t = 0; t < 2; t += Time.deltaTime) {
			temp.y = Mathf.Lerp(groundedY, jumpHeight, jumpY.Evaluate(t)); 
			if(leniencyTimer > 0) {
				leniencyTimer--;
				if(HorizontalInput() > 0) jumpDirection = 1;
				else if(HorizontalInput() < 0) jumpDirection = -1;
			}
			if(jumpDirection != 0) temp.x += (walkSpeed * Time.deltaTime * jumpDirection);
			transform.position = temp;
			yield return null;
		}
		temp.y = groundedY;
		transform.position = temp;
		state = MovementState.Standing;
	} 

	public IEnumerator GetHit(AttackData attackData) {
		animator.SwitchAnimation("Damage");
		state = MovementState.Recoiling;
		yield return new WaitForSeconds(attackData.hitStun);
		state = MovementState.Standing;
	}

	float HorizontalInput() {
		return Input.GetAxisRaw(identity.ToString() + "Horizontal");
	}

	float VerticalInput() {
		return Input.GetAxisRaw(identity.ToString() + "Vertical");
	}
}
