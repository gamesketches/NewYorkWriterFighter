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
	public GameObject vsText;

	// Use this for initialization
	void Awake () {
		if(!player1Active && !player2Active) {
			theCanvas.SetActive(false);
			selectingCharacters = false;
		}
		else {
			selectingCharacters = true;
			movie.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(Input.GetButtonDown("P1Start")) {
			player1Active = true;
			if(!selectingCharacters) OpenCharacterSelect();
		}
		else if(Input.GetButtonDown("P2Start")) {
			player2Active = true;
			if(!selectingCharacters)OpenCharacterSelect();
		}
		if(GameManager.player1Character != Character.None && GameManager.player2Character != Character.None && selectingCharacters) {	
			StartCoroutine(VsScreen());
			selectingCharacters = false;
		}
	}

	void OpenCharacterSelect(){
		selectingCharacters = true;
		theCanvas.SetActive(true);
		movie.SetActive(false);
	}
	
	IEnumerator VsScreen() {
		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = Resources.Load<AudioClip>("VsScreen");
		vsText.SetActive(true);
		portraits.SetActive(false);
		background.SetActive(false);
		p1Elements.transform.GetChild(0).gameObject.SetActive(false);	
		p2Elements.transform.GetChild(0).gameObject.SetActive(false);
		audio.Play();
		yield return new WaitForSeconds(audio.clip.length);
		SceneManager.LoadScene("main");
	}
}
