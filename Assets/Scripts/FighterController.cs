using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterController : MonoBehaviour {

	public bool leftSide;
	CharacterAnimator animator;
	public float walkSpeed = 1;

	// Use this for initialization
	void Start () {
		animator = GetComponent<CharacterAnimator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.DownArrow)) {
			animator.SwitchAnimation("Crouch");
		}
		else if(Input.GetKey(KeyCode.RightArrow)){
			animator.SwitchAnimation("Walk");
			MoveRight(walkSpeed * Time.deltaTime);
		}
		else if(Input.GetKey(KeyCode.LeftArrow)) {
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
