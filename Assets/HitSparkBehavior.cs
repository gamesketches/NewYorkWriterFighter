using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSparkBehavior : MonoBehaviour {

	public Sprite[] animation;
	SpriteRenderer renderer;

	// Use this for initialization
	void Awake () {
		renderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnEnable() {
		StartCoroutine(PlayAnimation());
	}

	IEnumerator PlayAnimation(){
		for(int i = 0; i < animation.Length * CharacterAnimator.frameSpeed; i++) {
			renderer.sprite = animation[i / CharacterAnimator.frameSpeed];
			yield return null;
		}
		gameObject.SetActive(false);
	}
}
