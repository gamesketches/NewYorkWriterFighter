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
		if(GameManager.player1Character != Character.None && GameManager.player2Character != Character.None) {
			SceneManager.LoadScene("main");
		}
	}

	void OpenCharacterSelect(){
		selectingCharacters = true;
		theCanvas.SetActive(true);
		movie.SetActive(false);
	}
}
