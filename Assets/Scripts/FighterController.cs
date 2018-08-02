using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerNumber {P1, P2};
public enum MovementState {Standing, Crouching, Jumping, Attacking, KnockedDown, Recoiling, Blocking};
public class FighterController : MonoBehaviour {

	static float jumpHeight = 2.37f;
	static int inputLeniency = 3;
	//[HideInInspector]
	public bool leftSide;
	CharacterAnimator animator;
	public float walkSpeed = 1;
	public float throwDistance = 2;
	public playerNumber identity;
	MovementState state;
	public AnimationCurve jumpY;
	public FighterController opponent;
	Transform attacks;

	// Use this for initialization
	void Start () {
		animator = GetComponent<CharacterAnimator>();
		state = MovementState.Standing;
		attacks = transform.GetChild(2);
		foreach(Transform child in attacks) {
			attacks.GetChild(2).gameObject.SetActive(false);
			attacks.GetChild(3).gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(state == MovementState.Attacking ) { 
			if(animator.animationFinished) {
				state = MovementState.Standing;
			}
		}
		else if(state != MovementState.KnockedDown){
			CheckButtonInput();
			CheckDirectionalInput();
		}
	}

	void CheckDirectionalInput() {
		if(state == MovementState.Attacking) return;
		if(state == MovementState.Jumping) {
			Debug.Log("Cancel into attacks here");
		}
		else if(state == MovementState.Recoiling) {
			Debug.Log("taking damage");
		}
		else if(opponent.GetState() == MovementState.Attacking && HoldingBack()) {
			state = MovementState.Blocking;
			animator.SwitchAnimation("Block");
		}
		else if(VerticalInput() < 0) {
			animator.SwitchAnimation("Crouch");
			state = MovementState.Crouching;
		}
		else if(VerticalInput() > 0) {
			StartCoroutine(Jump());
		}
		else if(HorizontalInput() > 0){
			animator.SwitchAnimation("Walk");
			state = MovementState.Standing;
			MoveRight(walkSpeed * Time.deltaTime);
		}
		else if(HorizontalInput() < 0) {
			animator.SwitchAnimation("Walk");
			state = MovementState.Standing;
			MoveLeft(walkSpeed * Time.deltaTime);
		}
		else {
			animator.SwitchAnimation("Idle");
			state = MovementState.Standing;
		}
	}

	void CheckButtonInput() {
		string playerID = identity.ToString();
		string attackButton = "none";
		if(Input.GetButtonDown(playerID + "LP") && Input.GetButtonDown(playerID + "MP")) {
			if(state != MovementState.Jumping) {
				attacks.GetChild(attacks.childCount - 1).gameObject.SetActive(true);
			}
		}
		else if(CheckThrow()) {
			Debug.Log("Throwin time");
		}
		else if(Input.GetButtonDown(playerID + "LP")) {
			attackButton = "LP";
		}
		if(Input.GetButtonDown(playerID + "HK")) {
			attackButton = "HK";
		}
		if(attackButton != "none") {
			if(!IsJumpAttack(attackButton)) {
				attacks.GetChild(GetButtonIndex(attackButton)).gameObject.SetActive(true);
			}
			state = MovementState.Attacking;
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
			if(animator.animationFinished) {
				animator.SwitchAnimation("Jump");
				state = MovementState.Jumping;
			}
			yield return null;
		}
		temp.y = groundedY;
		transform.position = temp;
		state = MovementState.Standing;
	} 

	public IEnumerator GetHit(AttackData attackData) {
		Debug.Log("Hit");
		if(SuccessfulBlock(attackData.blockType)) {
			yield return new WaitForSeconds(attackData.blockStun);
		}
		else {
			if(attackData.knockdown) {
				animator.SwitchAnimation("Fall");
				state = MovementState.KnockedDown;
				while(!animator.animationFinished) yield return null;
				yield return new WaitForSeconds(1);
			}
			else {
				animator.SwitchAnimation("Damage");
				state = MovementState.Recoiling;
				yield return new WaitForSeconds(attackData.hitStun);
			}
		}
		state = MovementState.Standing;
	}

	bool SuccessfulBlock(BlockType blockType) {
		if(state != MovementState.Blocking) return false;
	
		switch(blockType) {
			case BlockType.Overhead:
			case BlockType.Mid:
			case BlockType.Low:
				return true;
			break;
		}
		return false;
	}

	float HorizontalInput() {
		return Input.GetAxisRaw(identity.ToString() + "Horizontal");
	}

	float VerticalInput() {
		return Input.GetAxisRaw(identity.ToString() + "Vertical");
	}

	bool HoldingBack() {
		if(leftSide) return HorizontalInput() < 0;
		else return HorizontalInput() > 0;
	} 

	public MovementState GetState() {
		return state;
	}

	int GetButtonIndex(string button) {
		AttackButton theButton = (AttackButton)System.Enum.Parse(typeof(AttackButton), button);
		return (int)theButton;
	}

	bool IsJumpAttack(string button) {
		if(state != MovementState.Jumping) return false;
		else if(button == "LP" || button == "MP" || button == "HP") {
			attacks.GetChild(GetButtonIndex("JP")).gameObject.SetActive(true);
		}
		else if(button == "LK" || button == "MK" || button == "HK") {
			attacks.GetChild(GetButtonIndex("JK")).gameObject.SetActive(true);
		}

		return true;
	}

	bool CheckThrow() {
		string playerID = identity.ToString();
		if(opponent.state == MovementState.Jumping || opponent.state == MovementState.KnockedDown ||
			opponent.state == MovementState.Recoiling || !Input.GetButtonDown(playerID + "HP")) return false;
		float distance = Vector3.Distance(transform.position, opponent.transform.position);
		if(distance < throwDistance) {
			AttackData throwData = new AttackData(100, BlockType.Mid, true, 0, 0, 0, 0);
			StartCoroutine(opponent.GetHit(throwData));
			return true;
		}
		return false;
	}
	
}
