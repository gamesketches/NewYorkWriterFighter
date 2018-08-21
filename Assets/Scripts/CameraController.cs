﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform Player1;
	public Transform Player2;

	public float maxSize;
	public float minSize;
	public float sizeOffset;
	public float cornerOffset;
	Camera camera;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		float newXPosition = Mathf.Lerp(Player1.position.x, Player2.position.x, 0.5f);
		if(newXPosition < -GameManager.stageXEnds + cornerOffset) {
			newXPosition = -GameManager.stageXEnds + cornerOffset;
		}
		else if(newXPosition > GameManager.stageXEnds - cornerOffset) {
			newXPosition = GameManager.stageXEnds - cornerOffset;
		}
		transform.position = new Vector3(newXPosition, transform.position.y, transform.position.z);
		camera.orthographicSize = Mathf.Clamp(Mathf.Abs(Player1.position.x - Player2.position.x) - sizeOffset, minSize, maxSize);
	}

	public void SetPlayerTransforms(Transform player1, Transform player2) {
		Player1 = player1;
		Player2 = player2;
	}
}
