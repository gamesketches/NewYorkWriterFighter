using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : Attack {

	public int activeFrame;
	public float speed;
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
				int sourceLayer = transform.parent.parent.GetChild(1).gameObject.layer;
				tempProjectile.GetComponent<ProjectileController>().SetValues(projectileFrames, speed, attackData, sourceLayer);
				frameCounter = 0;
			}
			frameCounter = 0;
		}
		
	}
}
