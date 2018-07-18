using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterController : MonoBehaviour {

	public bool leftSide;
	CharacterAnimator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<CharacterAnimator>();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.RightArrow)){
			animator.SwitchAnimation("Walk");
		}
		else animator.SwitchAnimation("Idle");
	}
}
