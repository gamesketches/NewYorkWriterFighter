using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : Attack {

	public int activeFrame;
	public float speed;
	public GameObject projectile;

	public Sprite[] projectileFrames;

	FighterController player;

	// Use this for initialization
	void Awake () {
		base.Awake();
		player = transform.parent.parent.GetComponent<FighterController>();
	}
	
	void FixedUpdate () {
		frameCounter--;
		if(frameCounter < 0) {
			curFrame++;
			if(curFrame >= frames.Count) {
				hurtBoxController.EndAttack();
				gameObject.SetActive(false);
			}
			else if(curFrame == activeFrame) {
				GameObject tempProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
				int sourceLayer = player.transform.GetChild(1).gameObject.layer;
				int direction = player.leftSide ? 1 : -1;
				tempProjectile.GetComponent<ProjectileController>().SetValues(projectileFrames, speed, direction, attackData, sourceLayer);
				frameCounter = CharacterAnimator.frameSpeed;
			}
			frameCounter = CharacterAnimator.frameSpeed;
		}
		
	}
}
