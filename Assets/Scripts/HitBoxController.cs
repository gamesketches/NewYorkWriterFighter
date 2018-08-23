using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxController : MonoBehaviour {

	BoxCollider2D[] colliders;
	AttackData attackData;
	FighterController player;
	// Use this for initialization
	void Start () {
		colliders = GetComponents<BoxCollider2D>();
		player = transform.parent.gameObject.GetComponent<FighterController>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		transform.localScale = new Vector3(player.leftSide ? 1 : -1, 1, 1);
	}

	public void UpdateHitBoxes(Rect[] dimensions) {
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

	public void UpdateAttackData(AttackData newData) {
		attackData = newData;
	}

	public AttackData GetAttackData() {
		return attackData;
	}

	public void EndAttack() {
		foreach(BoxCollider2D collider in colliders) {
			collider.enabled = false;
		}
		attackData = new AttackData(0, BlockType.Mid, false, 0, 0, 0, 0, null);
	}
}
