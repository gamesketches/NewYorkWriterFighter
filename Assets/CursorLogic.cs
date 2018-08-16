using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLogic : MonoBehaviour {

	public PlayerNumber identity;
	public Transform portraits;
	int childIndex;
	
	private string verticalAxis;
	private string horizontalAxis;
	AudioSource audio;
	public static float dragMoveTime = 0.3f;
	float dragMoveTimer = 0;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
		if(identity == PlayerNumber.P1) {
			childIndex = 0;
		}
		else {
			childIndex = 7;
		}
		verticalAxis = identity.ToString() + "Vertical";
		horizontalAxis = identity.ToString() + "Horizontal";
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(dragMoveTimer > 0) {
			dragMoveTimer -= Time.fixedDeltaTime;
			return;
		}

		if(Input.GetAxisRaw(verticalAxis) == 1) {
			if(childIndex - 4 < 0) {
				childIndex = portraits.childCount - childIndex;
			}
			else {
				childIndex -= 4;
			}
			MoveCursor();
		}
		else if(Input.GetAxisRaw(verticalAxis) == -1) {
			childIndex = (childIndex + 4) % portraits.childCount;
			MoveCursor();
		}
		if(Input.GetAxisRaw(horizontalAxis) == 1) {
			childIndex = (childIndex + 1) % portraits.childCount;
			MoveCursor();
		}
		else if(Input.GetAxisRaw(horizontalAxis) == -1) {
			childIndex -= 1;
			if(childIndex < 0) {
				childIndex = portraits.childCount;
			}
			MoveCursor();
		}

		transform.position = portraits.GetChild(childIndex).position;
	}
	
	void MoveCursor() {
		audio.Play();
		dragMoveTimer = dragMoveTime;
	}
}
