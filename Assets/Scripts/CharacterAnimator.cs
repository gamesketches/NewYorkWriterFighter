﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType {Idle, Walk, Crouch, PreJump, Jump, Damage, Fall, Throw, Block, CrouchBlock, Attacking, Victory};
public class CharacterAnimator : MonoBehaviour {

	public Sprite[] walkAnimation;
	public Sprite[] idleAnimation;
	public Sprite[] crouchAnimation;
	public Sprite[] jumpAnimation;
	public Sprite[] damageAnimation;
	public Sprite[] blockAnimation;
	public Sprite[] crouchBlockAnimation;
	public Sprite[] fallAnimation;
	public Sprite[] victoryAnimation;
	public int[] throwAttacks;

	Sprite[] curAnimation;

	int curFrame;
	int frameCounter;

	[HideInInspector]
	public bool animationFinished;

	SpriteRenderer renderer;
	AnimationType state;
	public AnimationType nextState;

	public static int frameSpeed = 3;

	// Use this for initialization
	void Awake () {
		renderer = GetComponent<SpriteRenderer>();
		renderer.sprite = idleAnimation[0];
		curAnimation = idleAnimation;
		animationFinished = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// Leaving this for lots of animation specific timing code/frame manipulations
		switch(state) {
			case AnimationType.Idle:
			case AnimationType.Walk:
			case AnimationType.Block:
			case AnimationType.CrouchBlock:
				LoopingAnimation();
				nextState = AnimationType.Idle;
				break;
				/*if(TimesUp()) {
					NextFrame();
					frameCounter = idleAnimation.Length - 1;
				}
				break;
			case AnimationType.Walk:
				if(TimesUp()) {
					NextFrame();
					frameCounter = walkAnimation.Length;
				}
				break;*/
			case AnimationType.Crouch:
				if(TimesUp()) {
					animationFinished = true;
				}
				break;
			case AnimationType.Jump:
				if(TimesUp()) {
					animationFinished = true;
				}
				break;
			case AnimationType.Attacking:
				if(TimesUp()) {
					curFrame++;
					if(curFrame >= curAnimation.Length) {
						curFrame = 0;
						animationFinished = true;
						SwitchAnimation(nextState.ToString());
					}
					frameCounter = frameSpeed;
				}
				break;
			case AnimationType.Damage:
			case AnimationType.Victory:
				if(TimesUp()) {
					if(curFrame < curAnimation.Length -1) {
						curFrame++;
					}
					else {
						animationFinished = true;
					}
					frameCounter = frameSpeed;
				}
				break;
			case AnimationType.Fall:
				if(TimesUp()) {
					curFrame++;
					if(curFrame >= curAnimation.Length) {
						curFrame--;
						animationFinished = true;
					}
					frameCounter = frameSpeed;
				}
				break;
			/*case AnimationState.PreJump:
				if(TimesUp()) {
					SwitchAnimation("Jump");
					animationFinished = true;
				}
				break;
			case AnimationState.Land:
				if(TimesUp()) {
					animationFinished = true;
				}
				break;
			*/
			
		}
		renderer.sprite = curAnimation[curFrame];
	}

	public void SwitchAnimation(string animationType) {
		AnimationType newState = (AnimationType)System.Enum.Parse(typeof(AnimationType), animationType);
		if(state == newState || (!animationFinished && state != AnimationType.Attacking)) return;
		if(state == AnimationType.Attacking) GetComponentInChildren<HitBoxController>().EndAttack();
		state = newState;
		
		switch(state) {
			case AnimationType.Idle:
				curAnimation = idleAnimation;
				frameCounter = idleAnimation.Length;
				animationFinished = true;
				break;
			case AnimationType.Block:
				curAnimation = blockAnimation;
				frameCounter = blockAnimation.Length;
				animationFinished = true;
				break;
			case AnimationType.CrouchBlock:
				curAnimation = crouchBlockAnimation;
				frameCounter = crouchBlockAnimation.Length;
				animationFinished = true;
				break;
			case AnimationType.Walk:
				curAnimation = walkAnimation;
				frameCounter = walkAnimation.Length - 1;
				animationFinished = true;
				break;
			case AnimationType.Crouch:
				curAnimation = crouchAnimation;
				frameCounter = 1;
				animationFinished = true;
				break;
			case AnimationType.Jump:
				curAnimation = jumpAnimation;
				renderer.sprite = curAnimation[0];
				animationFinished = true;
				break;
			case AnimationType.Damage:
				curAnimation = damageAnimation;
				renderer.sprite = curAnimation[0];
				animationFinished = true;
				break;
			case AnimationType.Fall:
				curAnimation = fallAnimation;
				frameCounter = frameSpeed;
				animationFinished = false;
				break;
			case AnimationType.Victory:
				curAnimation = victoryAnimation;
				frameCounter = frameSpeed;
				animationFinished = false;
				break;
			case AnimationType.Throw:
				List<Sprite> theFrames = new List<Sprite>();
				foreach(int attack in throwAttacks) {
					Sprite[] attackFrames = transform.GetChild(2).GetChild(attack).gameObject.GetComponent<Attack>().GetSprites();
					theFrames.AddRange(attackFrames);
				}
				AttackAnimation(theFrames.ToArray());
				//curAnimation = theFrames.ToArray();
				//frameCounter = frameSpeed;
				//animationFinished = false;
				break;
			/*case AnimationType.PreJump:
				curAnimation = preJumpAnimation;
				animationFinished = false;
				frameCounter = 7;
				break;
			case AnimationType.Land:
				curAnimation = new Sprite[] {landingFrame};
				animationFinished = false;
				frameCounter = 4;
				break;
			*/
		}
		curFrame = 0;
	}

	public void AttackAnimation(Sprite[] frames) {
		state = AnimationType.Attacking;
		curAnimation = frames;
		frameCounter = frameSpeed;
		curFrame = 0;
		animationFinished = false;
	}

	void LoopingAnimation() {
		if(TimesUp()) {
			NextFrame();
			frameCounter = frameSpeed;
		}
	}

	bool TimesUp() {
		frameCounter--;
		return frameCounter < 0;
	}

	void NextFrame() {
		curFrame++;
		if(curFrame >= curAnimation.Length) curFrame = 0;
	}
	
	public int GetAnimationLength() {
		return curAnimation.Length;
	}
	
}
