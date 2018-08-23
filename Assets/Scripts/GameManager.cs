﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum Character {Alexandra, Amy, Arabelle, Chelsea, Jia, Saeed, Tony, Rembert, None};

public class GameManager : MonoBehaviour {

	float player1Life;
	float player2Life;
	public float totalLife;
	int player1Wins;
	int player2Wins;
	public Image player1Bar;
	public Image player2Bar;
	public GameObject player1WinIcons;
	public GameObject player2WinIcons;
	public Text roundTimer;
	float roundTime;
	int roundCounter;
	public Vector3 player1StartPos;
	public Vector3 player2StartPos;
	public static Character player1Character = Character.None;
	public static Character player2Character = Character.None;

	public static float stageXEnds = 12.68f;
	
	FighterController player1;
	FighterController player2;

	AudioSource audio;
	public Text RoundText;

	// Use this for initialization
	void Awake () {
		audio = GetComponent<AudioSource>();
		roundCounter = 0;
		player1Character = Character.Alexandra;
		player2Character = Character.Alexandra;
		player1WinIcons.transform.GetChild(0).gameObject.SetActive(false);
		player1WinIcons.transform.GetChild(1).gameObject.SetActive(false);
		player2WinIcons.transform.GetChild(0).gameObject.SetActive(false);
		player2WinIcons.transform.GetChild(1).gameObject.SetActive(false);
		StartCoroutine(StartRound());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		roundTime -= Time.fixedDeltaTime;
		roundTimer.text = Mathf.Floor(roundTime).ToString();
	}

	IEnumerator StartRound() {
		if(roundCounter == 0)
			LoadCharacters();
		else 
			ResetPlayers();
		roundTime = 99.99f;
		player1Bar.fillAmount = 0;
		player2Bar.fillAmount = 0;
		player1Life = totalLife;
		player2Life = totalLife;
		StartCoroutine(ChangeLifeAmount(player1Bar, 1, 0.6f));
		StartCoroutine(ChangeLifeAmount(player2Bar, 1, 0.6f));
		Attack.attackID = 0;
		AudioClip round = Resources.Load<AudioClip>("AnnouncerClips/round");
		AudioClip fight = Resources.Load<AudioClip>("AnnouncerClips/fight");
		if(roundCounter < 2) {
			AudioClip roundNum = Resources.Load<AudioClip>("AnnouncerClips/" + (roundCounter+ 1).ToString());
			RoundText.text = "Round " + roundCounter.ToString();
			yield return StartCoroutine(PlayAudioAndYield(round));
			yield return StartCoroutine(PlayAudioAndYield(roundNum));
		}
		else {
			AudioClip final = Resources.Load<AudioClip>("AnnouncerClips/final");
			RoundText.text = "Final Round";
			yield return StartCoroutine(PlayAudioAndYield(final));
			yield return StartCoroutine(PlayAudioAndYield(round));
		}
		yield return new WaitForSecondsRealtime(0.4f);
		RoundText.text = "Fight!";
		audio.clip = fight;
		audio.Play();
		yield return new WaitForSecondsRealtime(0.2f);
		RoundText.text = "";
	}

	IEnumerator ChangeLifeAmount(Image lifeBar, float targetFill, float duration) {
		float startFill = lifeBar.fillAmount;
		targetFill = Mathf.Clamp(targetFill, 0, 1);
		for(float t = 0; t < duration; t += Time.fixedDeltaTime) {
			lifeBar.fillAmount = Mathf.Lerp(startFill, targetFill, t / duration);
			yield return null;
		}
		lifeBar.fillAmount = targetFill;
	}

	public bool UpdateLifeBarCheckDeath(PlayerNumber playerNum, float lifeChange) {
		if(playerNum == PlayerNumber.P1) {
			player1Life -= lifeChange;
			StartCoroutine(ChangeLifeAmount(player1Bar, (player1Life - lifeChange) / totalLife, 0.1f));
			if(player1Life < 0) {
				//UpdateRoundCounters(PlayerNumber.P2);
				StartCoroutine(EndRound(PlayerNumber.P2));
				return true;
			}
		}
		else if(playerNum == PlayerNumber.P2) {
			player2Life -= lifeChange;
			StartCoroutine(ChangeLifeAmount(player2Bar, (player2Life - lifeChange) / totalLife, 0.1f));
			if(player2Life < 0){
				StartCoroutine(EndRound(PlayerNumber.P1));
			//	UpdateRoundCounters(PlayerNumber.P1);
				return true;
			}	
		}	
		return false;
	}

	bool UpdateRoundCounters(PlayerNumber playerNum){
		if(playerNum == PlayerNumber.P1) {
			if(player1WinIcons.transform.GetChild(0).gameObject.activeSelf) {
				player1WinIcons.transform.GetChild(1).gameObject.SetActive(true);
				return true;
				Debug.Log("Player 1 Wins");
			}
			else{
				roundCounter++;
				player1WinIcons.transform.GetChild(0).gameObject.SetActive(true);
				return false;
			}
		}
		else if(playerNum == PlayerNumber.P2) {
			if(player2WinIcons.transform.GetChild(0).gameObject.activeSelf) {
				player2WinIcons.transform.GetChild(1).gameObject.SetActive(true);
				return true;
				Debug.Log("Player 2 Wins");
			}
			else{
				roundCounter++;
				player2WinIcons.transform.GetChild(0).gameObject.SetActive(true);
				return false;
			}
		}
		return false;
	}

	IEnumerator EndRound(PlayerNumber playerNum) {
		FighterController winningPlayer;
		FadeInScene sceneFader = GetComponent<FadeInScene>();
		winningPlayer = playerNum == PlayerNumber.P1 ? player1 : player2;

		StartCoroutine(winningPlayer.VictoryPose());
		Debug.Log(winningPlayer.gameObject.name);
		AudioClip you = Resources.Load<AudioClip>("AnnouncerClips/roundEndYou");
		AudioClip win = Resources.Load<AudioClip>("AnnouncerClips/win");
		RoundText.text = playerNum.ToString() + " Wins";
		yield return StartCoroutine(PlayAudioAndYield(you));
		yield return StartCoroutine(PlayAudioAndYield(win));
		yield return new WaitForSecondsRealtime(1.5f);
		if(UpdateRoundCounters(playerNum)){
			SceneManager.LoadScene("CharacterSelect");
		}
		else {
			yield return StartCoroutine(sceneFader.FadeInOut(2f));
			StartCoroutine(StartRound());
		}
	}


	void LoadCharacters() {
		GameObject player1Obj = Instantiate((GameObject)Resources.Load(player1Character.ToString()));
		player1Obj.transform.position = player1StartPos;
		player1 = player1Obj.GetComponent<FighterController>();
		player1.SetPlayerIdentity(PlayerNumber.P1);
		GameObject player2Obj = Instantiate((GameObject)Resources.Load(player2Character.ToString()));
		player2Obj.transform.position = player2StartPos;
		player2 = player2Obj.GetComponent<FighterController>();
		player2.SetPlayerIdentity(PlayerNumber.P2);
		player2.opponent = player1;
		player1.opponent = player2;

		Camera.main.GetComponent<CameraController>().SetPlayerTransforms(player1.transform, player2.transform);
	}

	void ResetPlayers() {
		player1.ResetPlayer();
		player2.ResetPlayer();
		player1.transform.position = player1StartPos;
		player2.transform.position = player2StartPos;
	}

	IEnumerator PlayAudioAndYield(AudioClip clip) {
		audio.clip = clip;
		audio.Play();
		while(audio.isPlaying) yield return null;
	}
}
