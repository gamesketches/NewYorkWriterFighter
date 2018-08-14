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
	public Image player1Bar;
	public Image player2Bar;
	
	FighterController player1;
	FighterController player2;

	AudioSource audio;
	public Text RoundText;

	// Use this for initialization
	void Start () {
		player1Bar.fillAmount = 0;
		player2Bar.fillAmount = 0;
		StartCoroutine(StartRound());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator StartRound() {
		StartCoroutine(ChangeLifeAmount(player1Bar, 1, 0.6f));
		StartCoroutine(ChangeLifeAmount(player2Bar, 1, 0.6f));
		RoundText.text = "Round 1";
		yield return new WaitForSeconds(0.4f);
		RoundText.text = "Fight!";
		yield return new WaitForSeconds(0.2f);
		RoundText.text = "";
	}

	IEnumerator ChangeLifeAmount(Image lifeBar, float targetFill, float duration) {
		float startFill = lifeBar.fillAmount;
		for(float t = 0; t < duration; t += Time.fixedDeltaTime) {
			lifeBar.fillAmount = Mathf.Lerp(startFill, targetFill, t / duration);
			yield return null;
		}
		lifeBar.fillAmount = targetFill;
	}
}
