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
	public GameObject titleScreen;
	public GameObject theCanvas; 
	public GameObject background;
	public GameObject portraits;
	public GameObject p1Elements;
	public GameObject p2Elements;
	public GameObject stageSelect;
	public GameObject vsScreenElements;
	Dictionary<Character, string> winQuotes;

	// Use this for initialization
	void Awake () {
		sceneFader = GetComponent<FadeInScene>();
		MakeWinQuotes();
		p1Elements.SetActive(false);
		p2Elements.SetActive(false);
		if(winner != -1) {
			titleScreen.SetActive(false);
			selectingCharacters = false;
			StartCoroutine(VictoryScreen());
			return;
		}
		if(!player1Active && !player2Active) {
			ToggleCharacterSelectElements(false);
		//	theCanvas.SetActive(false);
			selectingCharacters = false;
			StartCoroutine(FlashStartText());
		}
		else {
			selectingCharacters = true;
			titleScreen.SetActive(false);
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
		if(selectingCharacters && winner == -1 && GameManager.stageID > -1) {
			if(player1Active && player2Active && GameManager.player1Character != Character.None && 
				GameManager.player2Character != Character.None) {	
					CloseCharacterSelect();
				}
			else if(player1Active && GameManager.player1Character != Character.None) {
				GameManager.player2Character = (Character) Random.Range(0,7);
				string opponentName = GameManager.player2Character.ToString();
				LoadVsScreenHeadshot(opponentName, 3);
				vsScreenElements.transform.GetChild(4).GetComponent<Text>().text = opponentName;
				GameManager.AIPlayer = 1;
				CloseCharacterSelect();
				}
			else if(player2Active && GameManager.player2Character != Character.None) {
				GameManager.player1Character = (Character) Random.Range(0,7);
				string opponentName = GameManager.player1Character.ToString();
				LoadVsScreenHeadshot(GameManager.player1Character.ToString(), 1);
				vsScreenElements.transform.GetChild(2).GetComponent<Text>().text = opponentName;
				GameManager.AIPlayer = 0;
				CloseCharacterSelect();
			}
		}
	}

	IEnumerator OpenCharacterSelect(){
		StartCoroutine(sceneFader.FadeInOut(0.3f));
		selectingCharacters = true;
		ToggleCharacterSelectElements(true);
		yield return new WaitForSeconds(0.3f);
		titleScreen.SetActive(false);
	}
	
	void CloseCharacterSelect() {
		ToggleCharacterSelectElements(false);
		StartCoroutine(VsScreen());
		selectingCharacters = false;
	}

	IEnumerator VsScreen() {
		StartCoroutine(sceneFader.FadeInOut(0.3f));
		AudioSource audio = GetComponent<AudioSource>();
		audio.clip = Resources.Load<AudioClip>("VsScreen");
		audio.loop = false;
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
		if(winner == 1) {
			p1Portrait.color = new Color(0.5f, 0.5f, 0.5f);
			vsScreenElements.transform.GetChild(5).GetComponent<Text>().text = winQuotes[GameManager.player2Character];
		}
		//vsScreenElements.transform.GetChild(2).GetComponent<Text>().text = p1Name;
		Image p2Portrait = vsScreenElements.transform.GetChild(3).GetComponent<Image>();
		p2Portrait.sprite = Resources.Load<Sprite>("headshots/" +p2Name);
		if(winner == 0){
			 p2Portrait.color = new Color(0.5f, 0.5f, 0.5f);
			vsScreenElements.transform.GetChild(5).GetComponent<Text>().text = winQuotes[GameManager.player1Character];
		}
		//vsScreenElements.transform.GetChild(4).GetComponent<Text>().text = p2Name;
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

	Image LoadVsScreenHeadshot(string charName, int childIndex) {
		Image portrait = vsScreenElements.transform.GetChild(childIndex).GetComponent<Image>();
		portrait.sprite = Resources.Load<Sprite>("headshots/" + charName);
		return portrait;
	}

	void ToggleCharacterSelectElements(bool state){

		background.SetActive(state);
		portraits.SetActive(state);
		if(player1Active)
			p1Elements.SetActive(state);
		if(player2Active)
			p2Elements.SetActive(state);
		stageSelect.SetActive(state);
	}

	IEnumerator FlashStartText() {
		GameObject startText = titleScreen.transform.GetChild(0).gameObject;//GetComponentInChildren<Text>();
		while(titleScreen.activeSelf) {
			yield return new WaitForSeconds(0.85f);
			startText.SetActive(!startText.activeSelf);
		}
	}
		
	void MakeWinQuotes() {
		winQuotes = new Dictionary<Character, string>();
		winQuotes.Add(Character.Alexandra, "Once a month I stumble onto snake Instagram late at night & I see dozens of baby snakes & snakes being fed snakes & black snakes & snakes sliding around like magic & I start to look up how I can get a snake for a pet but by then it's late & I fall asleep & forget everything");
		winQuotes.Add(Character.Amy, "Does anyone at the James Beard Awards have a cigarette? I'm the one in lipstick");
		winQuotes.Add(Character.Arabelle, "Every time i get a fashion invite now i just get confused. like, we really do this every season? astounding");
		winQuotes.Add(Character.Chelsea, "Trying to smuggle snacks into the metal show");
		winQuotes.Add(Character.Jia, "I will admit that I once talked to strangers in a stupid baby voice for a full day at SXSW bc I thought it would be funny but it ended up as a brutal self-own bc no one treated me any different than normal");
		winQuotes.Add(Character.Saeed, "It's hella awkward sitting next to a white dude on a flight while he's watching 12 Years A Slave. Like, don't look over here, sir");
		winQuotes.Add(Character.Tony, "I think I officially gave up on NYC glamour the moment I had to explain hentai to my agent");
	}
}
