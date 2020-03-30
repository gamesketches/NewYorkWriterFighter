using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaeedLungeKick : MonoBehaviour {

	public AnimationCurve xTravel;
	FighterController player;
	public float delay;

	// Use this for initialization
	void Awake () {
		player = transform.parent.parent.gameObject.GetComponent<FighterController>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnEnable() {
		StartCoroutine(MoveSaeed());
	}

	IEnumerator MoveSaeed(){
		yield return new WaitForSeconds(delay);
		float travelTime = xTravel.keys[xTravel.length - 1].time;
		for(float t = 0; t < travelTime; t += Time.fixedDeltaTime) {
			if(player.leftSide) {
				player.MoveRight(xTravel.Evaluate(t));
			}
			else {
				player.MoveLeft(xTravel.Evaluate(t));
			}
			yield return null;
		}
	}
}
