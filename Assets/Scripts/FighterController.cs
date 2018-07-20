using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerNumber {P1, P2};
public enum MovementState {Standing, Crouching, Jumping, Attacking, KnockedDown, Recoiling};
public class FighterController : MonoBehaviour {

	static float jumpHeight = 2.37f;
	public bool leftSide;
	CharacterAnimator animator;
	public float walkSpeed = 1;
	public playerNumber identity;
	MovementState state;

	// Use this for initialization
	void Start () {
		animator = GetComponent<CharacterAnimator>();
		state = MovementState.Standing;
	}
	
	// Update is called once per frame
	void Update () {
		CheckDirectionalInput();
		
	}

	void CheckDirectionalInput() {
		string playerID = identity.ToString();
		if(state == MovementState.Jumping) {
			Debug.Log("Cancel into attacks here");
		}
		else if(Input.GetAxisRaw(playerID + "Vertical") < 0) {
			animator.SwitchAnimation("Crouch");
		}
		else if(Input.GetAxisRaw(playerID + "Vertical") > 0) {
			StartCoroutine(Jump());
		}
		else if(Input.GetAxisRaw(playerID + "Horizontal") > 0){
			animator.SwitchAnimation("Walk");
			MoveRight(walkSpeed * Time.deltaTime);
		}
		else if(Input.GetAxisRaw(playerID + "Horizontal") < 0) {
			animator.SwitchAnimation("Walk");
			MoveLeft(walkSpeed * Time.deltaTime);
		}
		else animator.SwitchAnimation("Idle");
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
		Vector3 startPos = transform.position;
		Vector3 endPos = transform.position;
		endPos.y = 2.37f;
		for(float t = 0; t < 2; t += Time.deltaTime) {
			transform.position = Vector3.Lerp(startPos, endPos, Mathf.PingPong(t, 1));
			yield return null;
		}
		transform.position = startPos;
		state = MovementState.Standing;
	} 
}
