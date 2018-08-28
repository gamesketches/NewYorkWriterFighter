using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour {

	static bool player1Active = false;
	static bool player2Active = false;
	public static int winner = -1;
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
		if(winner != -1) {
			movie.SetActive(false);
			selectingCharacters = false;
			StartCoroutine(VictoryScreen());
			return;
		}
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
		if(GameManager.player1Character != Character.None && GameManager.player2Character != Character.None && selectingCharacters && winner == -1) {	
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

	IEnumerator VictoryScreen(){
		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = Resources.Load<AudioClip>("LoseScreen");
		vsScreenElements.SetActive(true);
		string p1Name = GameManager.player1Character.ToString();
		string p2Name = GameManager.player2Character.ToString();
		Image p1Portrait = vsScreenElements.transform.GetChild(1).GetComponent<Image>();
		p1Portrait.sprite = Resources.Load<Sprite>("headshots/" + p1Name);
		if(winner == 1) p1Portrait.color = new Color(0.5f, 0.5f, 0.5f);
		vsScreenElements.transform.GetChild(2).GetComponent<Text>().text = p1Name;
		Image p2Portrait = vsScreenElements.transform.GetChild(3).GetComponent<Image>();
		p2Portrait.sprite = Resources.Load<Sprite>("headshots/" +p2Name);
		if(winner == 0) p2Portrait.color = new Color(0.5f, 0.5f, 0.5f);
		vsScreenElements.transform.GetChild(4).GetComponent<Text>().text = p2Name;
		portraits.SetActive(false);
		background.SetActive(false);
		p1Elements.SetActive(false);	
		p2Elements.SetActive(false);
		player1Active = false;
		player2Active = false;
		audio.Play();
		winner = -1;
		yield return new WaitForSeconds(audio.clip.length - 0.3f);
		sceneFader.BeginFade(1);
		yield return new WaitForSeconds(0.3f);
		SceneManager.LoadScene("CharacterSelect");
	}
}
