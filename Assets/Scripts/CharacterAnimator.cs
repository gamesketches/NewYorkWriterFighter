using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType {Idle, Walk, Crouch, PreJump, Jump, Damage, Fall, Throw, Block, CrouchBlock, Attacking};
public class CharacterAnimator : MonoBehaviour {

	public Sprite[] walkAnimation;
	public Sprite[] idleAnimation;
	public Sprite[] crouchAnimation;
	public Sprite[] jumpAnimation;
	public Sprite[] damageAnimation;
	public Sprite[] blockAnimation;
	public Sprite[] crouchBlockAnimation;
	public Sprite[] fallAnimation;

	Sprite[] curAnimation;

	int curFrame;
	int frameCounter;

	public bool animationFinished;

	SpriteRenderer renderer;
	AnimationType state;

	public static int frameSpeed = 4;

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
			case AnimationType.Damage:
				if(TimesUp()) {
					curFrame++;
					if(curFrame >= curAnimation.Length) {
						curFrame = 0;
						animationFinished = true;
						SwitchAnimation("Idle");
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
		if(state == newState || !animationFinished) return;
		else state = newState;
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
				animationFinished = true;
				break;
			case AnimationType.Damage:
				curAnimation = damageAnimation;
				Debug.Log("Changed to damage state");
				animationFinished = true;
				break;
			case AnimationType.Fall:
				curAnimation = fallAnimation;
				frameCounter = frameSpeed;
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
	
	
}
