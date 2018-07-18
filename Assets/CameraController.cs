using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform Player1;
	public Transform Player2;

	public float maxSize;
	public float minSize;
	public float sizeOffset;
	Camera camera;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(Mathf.Lerp(Player1.position.x, Player2.position.x, 0.5f), transform.position.y, transform.position.z);
		camera.orthographicSize = Mathf.Clamp(Mathf.Abs(Player1.position.x - Player2.position.x) - sizeOffset, minSize, maxSize);
	}
}
