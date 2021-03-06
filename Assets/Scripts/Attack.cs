﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType {Mid, Low, Overhead};
public enum AttackButton {LP, MP, HP, LK, MK, HK, JP, JK, CRP, CRK};

[ExecuteInEditMode]
public class Attack : MonoBehaviour {

	public static int attackID = 0;

	public List<Frame> frames;
	AudioSource sfx;

	public int damage;
	public BlockType blockType;
	public bool knockdown;
	public float knockBack;
	public float hitStop;
	public float hitStun;
	public float blockStun;
	public AudioClip hitSFX;
	public bool superAttack;

	public AttackData attackData;

	HitBoxController hitBoxController;
	protected HurtBoxController hurtBoxController;
	protected int curFrame;
	protected int frameCounter = CharacterAnimator.frameSpeed;

	// Use this for initialization
	virtual public void Awake () {
		hitBoxController = transform.parent.parent.GetChild(0).gameObject.GetComponent<HitBoxController>();
		hurtBoxController = transform.parent.parent.GetChild(1).gameObject.GetComponent<HurtBoxController>();	
	
		attackData = new AttackData(damage, blockType, knockdown, knockBack, hitStop, hitStun, blockStun, hitSFX);
	
		sfx = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		frameCounter--;
		if(frameCounter < 0) {
			curFrame++;
			if(curFrame >= frames.Count) {
				hitBoxController.EndAttack();
				hurtBoxController.EndAttack();
				gameObject.SetActive(false);
			}
			else {
				hitBoxController.UpdateHitBoxes(frames[curFrame].hitBoxes);
				hurtBoxController.UpdateHurtBoxes(frames[curFrame].hurtBoxes);
			}
			frameCounter = CharacterAnimator.frameSpeed;
		}
	}

	virtual public void OnEnable() {
		attackData.id = Attack.attackID;
		curFrame = 0;
		frameCounter = transform.parent.parent.GetComponent<CharacterAnimator>().AttackAnimation(GetSprites(), superAttack);
		hitBoxController.UpdateAttackData(attackData);
		sfx.Play();
		Attack.attackID++;
	}

	public Sprite[] GetSprites() {
		Sprite[] returnVal = new Sprite[frames.Count];
		for(int i = 0; i < frames.Count; i++) {
			returnVal[i] = frames[i].sprite;
		}
		return returnVal;
	}
}

[System.Serializable]
public class Frame {
	
	public Sprite sprite;
	public Rect[] hitBoxes;
	public Rect[] hurtBoxes;
}

public struct AttackData {
	public int damage;
	public BlockType blockType;
	public bool knockdown;
	public float knockBack;
	public float hitStop;
	public float hitStun;
	public float blockStun;
	public AudioClip hitSFX;
	public int id;

	public AttackData(int attackDamage, BlockType attackBlockType, bool knocksDown, float knockBackDistance, float hitStopTime, float hitStunTime, float blockStunTime, AudioClip clip) {
		damage = attackDamage;
		blockType = attackBlockType;
		knockdown = knocksDown;
		knockBack = knockBackDistance;
		hitStop = hitStopTime * Time.fixedDeltaTime;
		hitStun = hitStunTime * Time.fixedDeltaTime * 4;
		blockStun = blockStunTime * Time.fixedDeltaTime * 4;
		hitSFX = clip;
		id = 0;
	}
}

