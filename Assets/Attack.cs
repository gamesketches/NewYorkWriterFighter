using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType {Mid, Low, Overhead};
public enum AttackButtons {LP, MP, HP, LK, MK, HK};

public class Attack : MonoBehaviour {

	public Sprite[] frames;
	public Rect[][] hitBoxes;
	public Rect[][] hurtBoxes;

	public int damage;
	public BlockType blockType;
	public bool knockdown;
	public float knockBack;
	public float hitStop;
	public float hitStun;

	HitBoxController hitBoxController;
	HurtBoxController hurtBoxController;
	int curFrame;

	// Use this for initialization
	void Start () {
		hitBoxController = GetComponentInChildren<HitBoxController>();
		hurtBoxController = GetComponentInChildren<HurtBoxController>();	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		curFrame++;
		if(curFrame >= frames.Length) {
			this.enabled = false;
		}
		else {
			hitBoxController.UpdateHitBoxes(hitBoxes[curFrame]);
			hurtBoxController.UpdateHurtBoxes(hurtBoxes[curFrame]);
		}
	}

	void OnEnable() {
		curFrame = 0;
		transform.parent.GetComponent<CharacterAnimator>().AttackAnimation(frames);
	}
}
