using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour {

	static bool player1Active = false;
	static bool player2Active = false;
	bool selectingCharacters;
	FadeInScene sceneFader;
	public GameObject movie;
	public GameObject theCanvas; 
	public GameObject background;
	public GameObject portraits;
	public GameObject p1Elements;
	public GameObject p2Elements;
	public GameObject vsScreenElements;

	// Use this for initialization
	void Awake () {
		sceneFader = GetComponent<FadeInScene>();
		if(!player1Active && !player2Active) {
			theCanvas.SetActive(false);
			selectingCharacters = false;
		}
		else {
			selectingCharacters = true;
			movie.SetActive(false);
		}
		
		vsScreenElements.SetActive(false);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Input.GetButtonDown("P1Start")) {
			player1Active = true;
			if(!selectingCharacters) StartCoroutine(OpenCharacterSelect());
		}
		else if(Input.GetButtonDown("P2Start")) {
			player2Active = true;
			if(!selectingCharacters) StartCoroutine(OpenCharacterSelect());
		}
		if(GameManager.player1Character != Character.None && GameManager.player2Character != Character.None && selectingCharacters) {	
			StartCoroutine(VsScreen());
			selectingCharacters = false;
		}
	}

	IEnumerator OpenCharacterSelect(){
		StartCoroutine(sceneFader.FadeInOut(0.3f));
		selectingCharacters = true;
		yield return new WaitForSeconds(0.3f);
		movie.SetActive(false);
		theCanvas.SetActive(true);
	}
	
	IEnumerator VsScreen() {
		StartCoroutine(sceneFader.FadeInOut(0.3f));
		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = Resources.Load<AudioClip>("VsScreen");
		yield return new WaitForSeconds(0.3f);
		vsScreenElements.SetActive(true);
		portraits.SetActive(false);
		background.SetActive(false);
		p1Elements.SetActive(false);	
		p2Elements.SetActive(false);
		audio.Play();
		yield return new WaitForSeconds(audio.clip.length - 0.3f);
		sceneFader.BeginFade(1);
		yield return new WaitForSeconds(0.3f);
		SceneManager.LoadScene("main");
	}
}
