using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	float player1Life;
	float player2Life;
	public float totalLife;
	int player1Rounds;
	int player1Wins;
	int player2Rounds;
	int player2Wins;
	
	FighterController player1;
	FighterController player2;

	AudioSource audio;
	public Text RoundText;

	// Use this for initialization
	void Start () {
		StartCoroutine(StartRound());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator StartRound() {
		RoundText.text = "Round 1";
		yield return new WaitForSeconds(0.4f);
		RoundText.text = "Fight!";
		yield return new WaitForSeconds(0.2f);
		RoundText.text = "";
	}
}
