using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType {Idle, Walk, PreJump, Jump, Damage, Fall, Throw, Block};
public class CharacterAnimator : MonoBehaviour {

	public Sprite[] walkAnimation;
	public Sprite[] idleAnimation;

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
			case AnimationType.Walk:
				if(TimesUp()) {
					NextFrame();
					frameCounter = walkAnimation.Length - 1;
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
			case AnimationType.Idle:
				if(TimesUp()) {
					NextFrame();
					frameCounter = idleAnimation.Length - 1;
				}
				break;
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
				frameCounter = 4;
				animationFinished = true;
				break;
			/*case AnimationState.Jump:
				curAnimation = new Sprite[] {jumpAnimation};
				animationFinished = true;
				break;
			case AnimationState.PreJump:
				curAnimation = preJumpAnimation;
				animationFinished = false;
				frameCounter = 7;
				break;
			case AnimationState.Land:
				curAnimation = new Sprite[] {landingFrame};
				animationFinished = false;
				frameCounter = 4;
				break;
			case AnimationState.Glide:
				curAnimation = glideAnimation;
				animationFinished = true;
				frameCounter = 4;
				break;
			*/
		}
		curFrame = 0;
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
