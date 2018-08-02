using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : Attack {

	public int activeFrame;
	public GameObject projectile;

	public Sprite[] projectileFrames;

	// Use this for initialization
	void Awake () {
		base.Awake();
	}
	
	void FixedUpdate () {
		frameCounter++;
		if(frameCounter >= CharacterAnimator.frameSpeed) {
			curFrame++;
			if(curFrame >= frames.Count) {
				gameObject.SetActive(false);
			}
			else if(curFrame == activeFrame) {
				GameObject tempProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
				tempProjectile.GetComponent<ProjectileController>().SetValues(projectileFrames, attackData);
				tempProjectile.layer = transform.parent.parent.GetChild(0).gameObject.layer;
				frameCounter = 0;
			}
			frameCounter = 0;
		}
		
	}
}
