﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

	[HideInInspector] 
	public float verticalAxis;

	[HideInInspector]
	public float horizontalAxis;

	[HideInInspector]
	public string attackButton;

	public float actionTimer = 1;
	public float verticalTimer = 2;
	float horizontalTimer = 0.5f;
	
	string[] attackButtons;

	// Use this for initialization
	void Start () {
		attackButtons = new string[] {"LP", "MP", "HP", "LK", "MK", "HK"};

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateAttackStatus();
		UpdateVerticalStatus();
		UpdateHorizontalStatus();
		horizontalAxis = Random.Range(-1, 1);
	}

	void UpdateAttackStatus() {
		actionTimer -= Time.fixedDeltaTime;
		if(actionTimer <= 0) {
			attackButton = 	attackButtons[Random.Range(0, attackButtons.Length)];
			actionTimer = 1f;
		}	
		else { attackButton = "none";};
	}

	void UpdateVerticalStatus() {
		verticalTimer -= Time.fixedDeltaTime;
		if(verticalTimer <= 0) {
			verticalTimer = 2f;
			int roll = (int) (Random.value * 3);
			switch(roll) {
				case 0: 
					verticalAxis = 0;
					break;
				case 1:
					verticalAxis = 1;
					break;
				case 2:
					verticalAxis = -1;
					break;
			}
		}
	}

	void UpdateHorizontalStatus() {
		horizontalTimer -= Time.fixedDeltaTime;
		if(horizontalTimer <= 0) {
			horizontalTimer = 1f;
			int roll = (int) (Random.value * 3);
			Debug.Log(roll);
			switch(roll) {
				case 0: 
					horizontalAxis = 0;
					break;
				case 1:
					horizontalAxis = 1;
					break;
				case 2:
					horizontalAxis = -1;
					break;
			}
		}
	}
}
