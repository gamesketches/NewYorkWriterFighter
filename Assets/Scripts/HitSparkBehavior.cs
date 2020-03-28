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

	public IEnumerator PlayAnimation(){
		for(int i = 0; i < animation.Length * CharacterAnimator.frameSpeed; i++) {
			renderer.sprite = animation[i / CharacterAnimator.frameSpeed];
			yield return null;
		}
		gameObject.SetActive(false);
	}

    public IEnumerator PlayAnimationBackwards()
    {
        for(int i = animation.Length - 1; i > -1; i--)
        {
            renderer.sprite = animation[i / CharacterAnimator.frameSpeed];
            yield return null;
        }
        gameObject.SetActive(false);
    }

    public IEnumerator PlayAnimationWithinFrames(int frames)
    {
        int frameSpeed = CharacterAnimator.frameSpeed;
        if(frames < animation.Length)
        {
            frameSpeed = 1;
        }
        for(int i = 0; i < animation.Length * frameSpeed; i++)
        {
            renderer.sprite = animation[i / frameSpeed];
            yield return null;
        }

    }

}
