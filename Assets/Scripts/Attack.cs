using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockType {Mid, Low, Overhead};
public enum AttackButtons {LP, MP, HP, LK, MK, HK};

[ExecuteInEditMode]
public class Attack : MonoBehaviour {

	//public Sprite[] frames;
	//public Rect[][] hitBoxes = new Rect[4][];
	//public Rect[][] hurtBoxes;
	public List<Frame> frames;

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
		hitBoxController = transform.parent.GetChild(0).gameObject.GetComponent<HitBoxController>();
		hurtBoxController = transform.parent.GetChild(1).gameObject.GetComponent<HurtBoxController>();	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		curFrame++;
		if(curFrame >= frames.Count) {
			gameObject.SetActive(false);
		}
		else {
			hitBoxController.UpdateHitBoxes(frames[curFrame].hitBoxes);
			hurtBoxController.UpdateHurtBoxes(frames[curFrame].hurtBoxes);
		}
	}

	void OnEnable() {
		curFrame = 0;
		transform.parent.GetComponent<CharacterAnimator>().AttackAnimation(GetSprites());
	}

	Sprite[] GetSprites() {
		Sprite[] sprites = new Sprite[frames.Count];
		for(int i = 0; i < frames.Count; i++) {
			sprites[i] = frames[i].sprite;
		}
		
		return sprites;
	}
}

[System.Serializable]
public class Frame {
	
	public Sprite sprite;
	public Rect[] hitBoxes;
	public Rect[] hurtBoxes;

}
