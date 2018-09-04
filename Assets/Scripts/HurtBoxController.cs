using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBoxController : MonoBehaviour {

	BoxCollider2D[] colliders;
	FighterController player;
	bool hitThisFrame;
	int lastAttack;
	Rect[] neutralColliderBounds;

	// Use this for initialization
	void Start () {
		hitThisFrame = false;
		lastAttack = -1;
		colliders = GetComponents<BoxCollider2D>();
		neutralColliderBounds = new Rect[colliders.Length];
		for(int i = 0; i < colliders.Length; i++) {
			neutralColliderBounds[i] = new Rect(colliders[i].offset,colliders[i].size);
		}
		player = transform.parent.gameObject.GetComponent<FighterController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		hitThisFrame = false;
		transform.localScale = new Vector3(player.leftSide ? 1 : -1, 1, 1);
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
		if(!hitThisFrame){
			hitThisFrame = true;
			AttackData data;
			if(other.tag == "Projectile"){
				ProjectileController projectileController = other.gameObject.GetComponent<ProjectileController>();
				if(projectileController.sourceLayer != gameObject.layer) {
					data = other.gameObject.GetComponent<ProjectileController>().GetAttackData();
					Destroy(other.gameObject);
				}
				else return;
			}
			else {
				data = other.gameObject.GetComponent<HitBoxController>().GetAttackData();
			}
			if(data.id != lastAttack) {
				StartCoroutine(player.GetHit(data));
				lastAttack = data.id;
			}
		}
	}

	public void EndAttack() {
		for(int i = 0; i < colliders.Length; i++) {
			colliders[i].enabled = true;
			colliders[i].offset = neutralColliderBounds[i].position;
			colliders[i].size = neutralColliderBounds[i].size;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		Debug.Log("Trigger exited");
	}

}
