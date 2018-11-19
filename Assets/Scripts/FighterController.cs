using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpriteGlow;

public enum PlayerNumber {P1, P2};
public enum MovementState {Standing, Crouching, Jumping, Attacking, KnockedDown, Recoiling, Blocking, CrouchBlocking, BlockStun, Victory, Thrown};
public class FighterController : MonoBehaviour {

	static float jumpHeight = 2.37f;
	static int inputLeniency = 3;
	static GameManager gameManager;
	[HideInInspector]
	public bool leftSide;
	CharacterAnimator animator;
	public float walkSpeed = 1;
	public float throwDistance = 2;
	[HideInInspector]
	public PlayerNumber identity;
	MovementState state;
	public AnimationCurve jumpY;
	public float jumpX = 1.9f;
	float baseY;
	[HideInInspector]
	public FighterController opponent;
	Transform attacks;
	SpriteRenderer renderer;
	[HideInInspector]
	public bool locked = true;
	AIController aiController;
	
	bool superAvailable;
	AudioSource audio;
	AudioSource blockSound;
	public Character characterIdentity;
	public bool freeSpecialOn;

	// Use this for initialization
	void Awake () {
		superAvailable = false;
		animator = GetComponent<CharacterAnimator>();
		audio = GetComponents<AudioSource>()[1];
		blockSound = gameObject.AddComponent<AudioSource>();
		blockSound.clip = Resources.Load<AudioClip>("HeavyBlock");
		state = MovementState.Standing;
		attacks = transform.GetChild(2);
		leftSide = identity == PlayerNumber.P1;
		foreach(Transform child in attacks) {
			child.gameObject.SetActive(false);
		}
		renderer = GetComponent<SpriteRenderer>();
		if(gameManager == null) gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		baseY = transform.position.y;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(locked) return;
		if(state == MovementState.Attacking ) { 
			if(animator.animationFinished) {
				if(VerticalInput() < 0) {
					animator.SwitchAnimation("Crouch"); 
					state = MovementState.Crouching;
				}
				else state = MovementState.Standing;
			}
		}
		else if(state == MovementState.Thrown && !animator.animationFinished) {
			if(leftSide) {
				MoveLeft(walkSpeed * Time.fixedDeltaTime * 2);
			}
			else {
				MoveRight(walkSpeed * Time.fixedDeltaTime *2);
			}
		}
		else if(state != MovementState.KnockedDown && state != MovementState.Recoiling && state != MovementState.BlockStun && state != MovementState.Victory){
			CheckButtonInput();
			CheckDirectionalInput();
			if(state != MovementState.Jumping) AdjustFacing();
		}
	}

	void CheckDirectionalInput() {
		if(state == MovementState.Attacking || state == MovementState.Victory) return;
		if(state == MovementState.Jumping) {
		//	Debug.Log("Cancel into attacks here");
		}
		else if(state == MovementState.Recoiling || state == MovementState.BlockStun ) {
		//	Debug.Log("taking damage");
		}
		else if((opponent.GetState() == MovementState.Attacking || OpponentProjectileExists()) && HoldingBack()) {
			Block();
		}
		else if(VerticalInput() < 0) {
			animator.SwitchAnimation("Crouch");
			state = MovementState.Crouching;
		}
		else if(VerticalInput() > 0) {
			StartCoroutine(Jump());
		}
		else if(HorizontalInput() > 0){
			animator.SwitchAnimation("Walk");
			state = MovementState.Standing;
			MoveRight(walkSpeed * Time.deltaTime);
		}
		else if(HorizontalInput() < 0) {
			animator.SwitchAnimation("Walk");
			state = MovementState.Standing;
			MoveLeft(walkSpeed * Time.deltaTime);
		}
		else {
			animator.SwitchAnimation("Idle");
			state = MovementState.Standing;
		}
	}

	void CheckButtonInput() {
		string playerID = identity.ToString();
		string attackButton = "none";
		if(superAvailable && Input.GetButton(playerID + "LP") && Input.GetButton(playerID + "MP") && Input.GetButton(playerID + "HP")) {
			if(state != MovementState.Jumping) {
				attacks.GetChild(attacks.childCount - 1).gameObject.SetActive(true);
				state = MovementState.Attacking;
				superAvailable = false;
				float poseTime = animator.victoryAnimation.Length * Time.fixedDeltaTime * CharacterAnimator.frameSpeed;
				Debug.Log(poseTime);
				GetComponent<SpriteGlowEffect>().OutlineWidth = 0;
				//StartCoroutine(HitStop(poseTime));
				Vector3 zoomPoint = transform.position;
				zoomPoint.y += 0.4f;
				StartCoroutine(Camera.main.GetComponent<CameraController>().ZoomCamera(zoomPoint, poseTime));
			}
		}
		else if(CheckThrow()) {
			Debug.Log("Throwin time");
		}
		else if(Input.GetButtonDown(playerID + "MP")) {
			attackButton = "MP";
		}
		else if(Input.GetButtonDown(playerID + "LP")) {
			attackButton = "LP";
		}

		else if(Input.GetButtonDown(playerID + "HP")) {
			attackButton = "HP";
		}
		else if(Input.GetButtonDown(playerID + "HK")) {
			attackButton = "HK";
		}
		else if(Input.GetButtonDown(playerID + "MK")) {
			attackButton = "MK";
		}
		else if(Input.GetButtonDown(playerID + "LK")) {
			attackButton = "LK";
		}
		if(aiController != null) attackButton = aiController.attackButton;
		if(attackButton != "none") {
			if(!IsJumpAttack(attackButton) && !IsCrouchAttack(attackButton)) {
				attacks.GetChild(GetButtonIndex(attackButton)).gameObject.SetActive(true);
			}
			state = MovementState.Attacking;
		}
		if(freeSpecialOn && Input.GetKey(KeyCode.Space)) MakeSuperAvailable();
	}

	public void MoveRight(float distance) {
		transform.Translate(distance, 0, 0);
	}

	public void MoveLeft(float distance) {
		transform.Translate(-distance, 0, 0);
	}

	IEnumerator Jump() {
		float groundedY = transform.position.y;
		state = MovementState.Jumping;
		Vector3 temp = transform.position;
		animator.SwitchAnimation("Jump");
		int leniencyTimer = inputLeniency;
		int jumpDirection = 0;
		float jumpTime = jumpY.keys[jumpY.length -1].time * 2;
		for(float t = 0; t < jumpTime; t += Time.deltaTime) {
			temp.y = Mathf.Lerp(groundedY, jumpHeight, jumpY.Evaluate(t)); 
			if(leniencyTimer > 0) {
				leniencyTimer--;
				if(HorizontalInput() > 0) jumpDirection = 1;
				else if(HorizontalInput() < 0) jumpDirection = -1;
			}
			if(jumpDirection != 0) temp.x = transform.position.x + (walkSpeed * Time.deltaTime * jumpDirection * jumpX);
			transform.Translate(temp - transform.position);
			if(animator.animationFinished) {
				animator.SwitchAnimation("Jump");
				state = MovementState.Jumping;
			}
			yield return null;
		}
		temp.y = baseY;
		transform.position = temp;
		state = MovementState.Standing;
		animator.SwitchAnimation("Idle");
		animator.nextState = AnimationType.Idle;
		
	} 

	void Block() {
		if(VerticalInput() < 0) {
			state = MovementState.CrouchBlocking;
			animator.SwitchAnimation("CrouchBlock");
		}
		else {
			state = MovementState.Blocking;
			animator.SwitchAnimation("Block");
		}
	}

	bool OpponentProjectileExists() {
		GameObject[] projectiles = GameObject.FindGameObjectsWithTag("Projectile");
		for(int i = 0; i < projectiles.Length; i++) {
			if(identity == PlayerNumber.P1 && projectiles[i].GetComponent<ProjectileController>().sourceLayer != 10) {
					return true;
			}
			else if(identity == PlayerNumber.P2 && projectiles[i].GetComponent<ProjectileController>().sourceLayer != 12) {
					return true;
			}
		}	
		return false;
	}


	public IEnumerator GetHit(AttackData attackData, Vector3 contactPoint) {
		Debug.Log("Hit");
		if(state != MovementState.KnockedDown) {
			bool airborne = state == MovementState.Jumping;
			if(SuccessfulBlock(attackData.blockType)) {
				StartCoroutine(GetPushed(attackData.knockBack, attackData.blockStun));
				blockSound.Play();
				state = MovementState.BlockStun;
				gameManager.PlayHitSpark(contactPoint, true, attackData.damage);
				yield return new WaitForSeconds(attackData.blockStun);
			}
			else {
				StartCoroutine(HitStop(attackData.hitStop));
				if(gameManager.UpdateLifeBarCheckDeath(identity, attackData.damage)) {
					Debug.Log("Killed");
					yield return StartCoroutine(DeathAnimation());
	
				}	
				else if(attackData.knockdown) {
					animator.SwitchAnimation("Fall");
					state = MovementState.KnockedDown;
					gameManager.PlayHitSpark(contactPoint, false, attackData.damage);
					StartCoroutine(GetPushed(attackData.knockBack, attackData.hitStun));
					while(!animator.animationFinished) yield return null;
					yield return new WaitForSeconds(1);
				}
				else {
					animator.SwitchAnimation("Damage");
					if(state == MovementState.Jumping) {
						animator.nextState = AnimationType.Jump;
					}
					else {
						animator.nextState = AnimationType.Idle;
					}
					state = MovementState.Recoiling;
					gameManager.PlayHitSpark(contactPoint, false, attackData.damage);
					if(attackData.hitSFX != null) {
						audio.clip = attackData.hitSFX;
						audio.Play();
					}
					StartCoroutine(GetPushed(attackData.knockBack, attackData.hitStun));
					yield return new WaitForSeconds(attackData.hitStun);
				}
			}
		if(animator.state == AnimationType.Jump) state = MovementState.Jumping;
		else state = MovementState.Standing;
		}
	}

	public IEnumerator GetThrown(AttackData throwData) {
		Debug.Log("Throw");
		animator.SwitchAnimation("Damage");
		state = MovementState.Recoiling;
		StartCoroutine(Camera.main.GetComponent<CameraController>().ZoomCamera(transform.position, throwData.hitStun));
		yield return new WaitForSeconds(throwData.hitStun);
		if(gameManager.UpdateLifeBarCheckDeath(identity, throwData.damage)) {
			Debug.Log("Killed");
			yield return StartCoroutine(DeathAnimation());
		}
		else{
			animator.SwitchAnimation("Fall");
			state = MovementState.Thrown;
			while(!animator.animationFinished) {
				if(leftSide) MoveLeft(walkSpeed * Time.deltaTime);
				else MoveRight(walkSpeed *Time.deltaTime);
				yield return null;
			}
			yield return new WaitForSeconds(1);
		}

		state = MovementState.Standing;
	}

	IEnumerator DeathAnimation() {
		locked = true;
		Debug.Log("Killed");
		Debug.Log("In Death Animation");
		animator.SwitchAnimation("Fall");
		state = MovementState.KnockedDown;
		Time.timeScale = 0.2f;
		GetComponent<AudioSource>().Play();
		while(!animator.animationFinished) {
			if(leftSide) MoveLeft(walkSpeed * Time.deltaTime);
			else MoveRight(walkSpeed *Time.deltaTime);
			yield return null;
		}
		Time.timeScale = 1;
		while(state == MovementState.KnockedDown) yield return null;
	}


	bool SuccessfulBlock(BlockType blockType) {
		if(state != MovementState.Blocking && state != MovementState.CrouchBlocking && state != MovementState.BlockStun) return false;
		if(state == MovementState.BlockStun) Block();
		switch(blockType) {
			case BlockType.Mid:
				return true;
			case BlockType.Overhead:
				if(state == MovementState.Blocking) return true;
				break;
			case BlockType.Low:
				if(state == MovementState.CrouchBlocking) return true;
				break;
		}
		return false;
	}

	IEnumerator GetPushed(float distance, float stunTime) {
		float previous = 0;
		for(float i = 0; i < stunTime; i += Time.fixedDeltaTime){
			float newX = Mathf.SmoothStep(0, distance, i / stunTime);
			if(leftSide) MoveLeft(newX - previous);
			else MoveRight(newX - previous);
			yield return null;
			previous = newX;
		}
	}
	
	bool IsJumpAttack(string button) {
		if(state != MovementState.Jumping) return false;
		else if(button == "LP" || button == "MP" || button == "HP") {
			attacks.GetChild(GetButtonIndex("JP")).gameObject.SetActive(true);
		}
		else if(button == "LK" || button == "MK" || button == "HK") {
			attacks.GetChild(GetButtonIndex("JK")).gameObject.SetActive(true);
		}

		return true;
	}

	bool IsCrouchAttack(string button) {
		if(state != MovementState.Crouching) return false;
		else if(button == "LP" || button == "MP" || button == "HP") {
			attacks.GetChild(GetButtonIndex("CRP")).gameObject.SetActive(true);
		}
		else if(button == "LK" || button == "MK" || button == "HK") {
			attacks.GetChild(GetButtonIndex("CRK")).gameObject.SetActive(true);
		}
		animator.nextState = AnimationType.Crouch;

		return true;
	}

	bool CheckThrow() {
		string playerID = identity.ToString();
		if(opponent.state == MovementState.Jumping || opponent.state == MovementState.KnockedDown ||
			opponent.state == MovementState.Recoiling || !Input.GetButtonDown(playerID + "HP")) return false;
		float distance = Vector3.Distance(transform.position, opponent.transform.position);
		if(distance < throwDistance && (
			(leftSide && HorizontalInput() >0) || (!leftSide && HorizontalInput() < 0))) {
			animator.SwitchAnimation("Throw");
			AttackData throwData = new AttackData(100, BlockType.Mid, true, 0, 0, animator.GetAnimationLength(), 0, null);
			StartCoroutine(opponent.GetThrown(throwData));
			state = MovementState.Attacking;
			return true;
		}
		return false;
	}

	void AdjustFacing() {
		leftSide =  transform.position.x < opponent.transform.position.x; 
		renderer.flipX = !leftSide;
	}

	float HorizontalInput() {
		if(aiController != null) return aiController.horizontalAxis;
		
		return Input.GetAxisRaw(identity.ToString() + "Horizontal");
	}

	float VerticalInput() {
		if(aiController != null) return aiController.verticalAxis;
		return Input.GetAxisRaw(identity.ToString() + "Vertical");
	}

	bool HoldingBack() {
		if(leftSide) return HorizontalInput() < 0;
		else return HorizontalInput() > 0;
	} 

	public MovementState GetState() {
		return state;
}

	int GetButtonIndex(string button) {
		AttackButton theButton = (AttackButton)System.Enum.Parse(typeof(AttackButton), button);
		return (int)theButton;
	}

	public void SetPlayerIdentity(PlayerNumber num) {
		identity = num;
		GameObject hitBoxes = transform.GetChild(0).gameObject;
		GameObject hurtBoxes = transform.GetChild(1).gameObject;
		if(num == PlayerNumber.P1) {
			hitBoxes.layer = 9;
			hurtBoxes.layer = 10;
		}
		else {
			hitBoxes.layer = 11;
			hurtBoxes.layer = 12;
			renderer.flipX = true;
		}
	}	

	public IEnumerator VictoryPose() {
		locked = true;
		state = MovementState.Victory;
		Debug.Log("Victory pose");
		animator.SwitchAnimation("Victory");
		animator.nextState = AnimationType.Victory;
		while(state == MovementState.Victory) yield return null;
	}

	public void ResetPlayer() {
		state = MovementState.Standing;
		animator.SwitchAnimation("Idle");
		superAvailable = false;
		GetComponent<SpriteGlowEffect>().OutlineWidth = 0;
	}

	public void AddAI() {
		aiController = gameObject.AddComponent<AIController>();
	}

	IEnumerator HitStop(float hitStopTime) {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(hitStopTime);
		Time.timeScale = 1;
	}

	public void MakeSuperAvailable() {
		superAvailable = true;
		GetComponent<SpriteGlowEffect>().OutlineWidth = 1;
	}
}
