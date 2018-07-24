using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBoxController : MonoBehaviour {

	BoxCollider2D[] colliders;
	// Use this for initialization
	void Start () {
		colliders = GetComponents<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateHurtBoxes(Rect[] dimensions) {
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

	void OnTriggerEnter2D(Collider2D other) {
		Debug.Log("Hit!");
	}
}
