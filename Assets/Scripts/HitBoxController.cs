using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour {

	BoxCollider2D[] colliders;
	// Use this for initialization
	void Start () {
		colliders = GetComponents<BoxCollider2D>();
		Debug.Log(colliders);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateHitBoxes(Rect[] dimensions) {
		Debug.Log(dimensions.Length);
		Debug.Log(colliders);
		for(int i = 0; i < colliders.Length; i++) {
			if(i < dimensions.Length) {
				colliders[i].enabled = true;
				colliders[i].offset = dimensions[i].position;
				colliders[i].size = dimensions[i].size;
			}
			else {
				colliders[i].enabled = false;
			}
		}
	}

}
