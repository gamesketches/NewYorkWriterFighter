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
	public Text roundTimer;
	float roundTime;
	
	FighterController player1;
	FighterController player2;

	AudioSource audio;
	public Text RoundText;

	// Use this for initialization
	void Start () {
		roundTime = 99.99f;
		player1Bar.fillAmount = 0;
		player2Bar.fillAmount = 0;
		player1Life = totalLife;
		player2Life = totalLife;
		StartCoroutine(StartRound());
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		roundTime -= Time.fixedDeltaTime;
		roundTimer.text = Mathf.Floor(roundTime).ToString();
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

	public bool UpdateLifeBarCheckDeath(PlayerNumber playerNum, float lifeChange) {
		if(playerNum == PlayerNumber.P1) {
			player1Life -= lifeChange;
			if(player1Life < 0) {
				Debug.Log("Round over");
				return true;
			}	
			else {
				StartCoroutine(ChangeLifeAmount(player1Bar, (player1Life - lifeChange) / totalLife, 0.1f));
			}
		}
		else if(playerNum == PlayerNumber.P2) {
			player2Life -= lifeChange;
			if(player2Life < 0){
				Debug.Log("Round over");
				return true;
			}	
			else {
				StartCoroutine(ChangeLifeAmount(player2Bar, (player2Life - lifeChange) / totalLife, 0.1f));
			}
		}	
		return false;
	}
}
