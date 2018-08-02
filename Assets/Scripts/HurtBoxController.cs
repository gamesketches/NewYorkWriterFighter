using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBoxController : MonoBehaviour {

	BoxCollider2D[] colliders;
	FighterController player;
	bool hitThisFrame;

	// Use this for initialization
	void Start () {
		hitThisFrame = false;
		colliders = GetComponents<BoxCollider2D>();
		player = transform.parent.gameObject.GetComponent<FighterController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		hitThisFrame = false;
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
				data = other.gameObject.GetComponent<ProjectileController>().GetAttackData();
				Destroy(other.gameObject);
			}
			else {
				data = other.gameObject.GetComponent<HitBoxController>().GetAttackData();
			}
			StartCoroutine(player.GetHit(data));
			StartCoroutine(HitStop(data.hitStop));
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		Debug.Log("Trigger exited");
	}

	IEnumerator HitStop(float hitStopTime) {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(hitStopTime);
		Time.timeScale = 1;
	}
}
