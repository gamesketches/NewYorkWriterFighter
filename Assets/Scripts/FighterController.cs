using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum playerNumber {P1, P2};
public class FighterController : MonoBehaviour {

	public bool leftSide;
	CharacterAnimator animator;
	public float walkSpeed = 1;
	public playerNumber identity;

	// Use this for initialization
	void Start () {
		animator = GetComponent<CharacterAnimator>();
	}
	
	// Update is called once per frame
	void Update () {
		CheckDirectionalInput();
		
	}

	void CheckDirectionalInput() {
		string playerID = identity.ToString();
		if(Input.GetAxisRaw(playerID + "Vertical") < 0) {
			animator.SwitchAnimation("Crouch");
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
}
