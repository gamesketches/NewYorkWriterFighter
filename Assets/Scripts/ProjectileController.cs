using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {

	int direction = 1;
	float speed = 1;
	AttackData attackData;
	
	public Sprite[] frames;
	int frameCounter = 0;
	int curFrame = 0;
	public int sourceLayer = 0;

	SpriteRenderer renderer;

	// Use this for initialization
	void Awake () {
		renderer = GetComponent<SpriteRenderer>();
	}
	
	void FixedUpdate () {
		frameCounter++;
		if(frameCounter >= CharacterAnimator.frameSpeed) {
			curFrame++;
			if(curFrame >= frames.Length - 1) {
				curFrame = 0;
			}
			frameCounter = 0;
		}
		renderer.sprite = frames[curFrame];
		transform.Translate(direction * speed * Time.fixedDeltaTime, 0, 0);
	}

	public void SetValues(Sprite[] fireballFrames, float newSpeed, int newDirection, AttackData fireballAttackData, int parentLayer) {
		frames = fireballFrames;
		speed = newSpeed;
		direction = newDirection;
		if(direction == 1) renderer.flipX = false;
		else renderer.flipX = true;
		attackData = fireballAttackData;
		sourceLayer = parentLayer;
	}

	public AttackData GetAttackData() {
		return attackData;
	}

	void OnCollisionEnter2D(Collision2D col) {
		if(gameObject.layer == col.gameObject.layer) {
			Destroy(col.gameObject);
			Destroy(gameObject);
		}
	}

	void OnBecameInvisible() {
		Destroy(gameObject);
	}
}
