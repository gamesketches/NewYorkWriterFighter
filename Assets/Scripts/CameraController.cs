using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

	public Transform Player1;
	public Transform Player2;

	public float maxSize;
	public float minSize;
	public float throwSize;
	public float sizeOffset;
	public float cornerOffset;
	bool zoomMode;
	public float transitionTime = 0.2f;
	public float zoomSize = 1.5f;
	Camera camera;

	// Use this for initialization
	void Start () {
		zoomMode = false;
		zoomSize = 2.5f;
		camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		float newXPosition = Mathf.Lerp(Player1.position.x, Player2.position.x, 0.5f);
		if(zoomMode) {
			return;
		}
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

	public IEnumerator ZoomCamera(Vector3 focusPoint, float time) {
		zoomMode = true;
		Vector3 middle = Vector3.Lerp(Player1.position, Player2.position, 0.5f);
		middle.z = -10;
		middle.y = transform.position.y;
		focusPoint.z = -10;
		yield return ZoomTransition(focusPoint, zoomSize);
		yield return new WaitForSecondsRealtime(time - (transitionTime * 2));
		yield return ZoomTransition(middle, Mathf.Clamp(Mathf.Abs(Player1.position.x - Player2.position.x) - sizeOffset, minSize, maxSize));
		zoomMode = false;
		}

	IEnumerator ZoomTransition(Vector3 targetPoint, float targetZoom) {
		Vector3 startPosition = transform.position;
		float startZoom = camera.orthographicSize;
		for(float t = 0; t < transitionTime; t += Time.fixedDeltaTime) {
			transform.position = Vector3.Lerp(startPosition, targetPoint, t / transitionTime);
			camera.orthographicSize = Mathf.SmoothStep(startZoom, targetZoom, t / transitionTime);
			yield return null;
		}
		transform.position = targetPoint;
		camera.orthographicSize = targetZoom;
	}

}
