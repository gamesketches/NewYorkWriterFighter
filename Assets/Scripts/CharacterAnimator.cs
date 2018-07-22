﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType {Idle, Walk, Crouch, PreJump, Jump, Damage, Fall, Throw, Block, Attacking};
public class CharacterAnimator : MonoBehaviour {

	public Sprite[] walkAnimation;
	public Sprite[] idleAnimation;
	public Sprite[] crouchAnimation;
	public Sprite[] jumpAnimation;
	Sprite[] curAnimation;

	int curFrame;
	int frameCounter;

	public bool animationFinished;

	SpriteRenderer renderer;
	AnimationType state;

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
				if(TimesUp()) {
					NextFrame();
					frameCounter = idleAnimation.Length - 1;
				}
				break;
			case AnimationType.Walk:
				if(TimesUp()) {
					NextFrame();
					frameCounter = walkAnimation.Length;
				}
				break;
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
						SwitchAnimation("Idle");
					}
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
			case AnimationState.Glide:
				if(TimesUp()) {
					NextFrame();
					frameCounter = 4;
				}
				break;
			*/
			
		}
		renderer.sprite = curAnimation[curFrame];
	}

	public void SwitchAnimation(string animationType) {
		AnimationType newState = (AnimationType)System.Enum.Parse(typeof(AnimationType), animationType);
		if(state == newState || !animationFinished) return;
		else state = newState;
		switch(state) {
			case AnimationType.Idle:
				curAnimation = idleAnimation;
				frameCounter = idleAnimation.Length;
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
				animationFinished = true;
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
			case AnimationType.Glide:
				curAnimation = glideAnimation;
				animationFinished = true;
				frameCounter = 4;
				break;
			*/
		}
		curFrame = 0;
	}

	public void AttackAnimation(Sprite[] frames) {
		state = AnimationType.Attacking;
		curAnimation = frames;
		frameCounter = frames.Length;
		animationFinished = false;
	}

	bool TimesUp() {
		frameCounter--;
		return frameCounter < 0;
	}

	void NextFrame() {
		curFrame++;
		if(curFrame >= curAnimation.Length) curFrame = 0;
	}
}
