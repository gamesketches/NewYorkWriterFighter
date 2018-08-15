using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Character {Alexandra, Amy, Arabelle, Chelsea, Jia, Saeed, Tony, Rembert};

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
	public Vector3 player1StartPos;
	public Vector3 player2StartPos;
	public static Character player1Character;
	public static Character player2Character;
	
	FighterController player1;
	FighterController player2;

	AudioSource audio;
	public Text RoundText;

	// Use this for initialization
	void Awake () {
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
		LoadCharacters();
		roundTime = 99.99f;
		player1Bar.fillAmount = 0;
		player2Bar.fillAmount = 0;
		player1Life = totalLife;
		player2Life = totalLife;
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
				UpdateRoundCounters(PlayerNumber.P2);
				return true;
			}
		}
		else if(playerNum == PlayerNumber.P2) {
			player2Life -= lifeChange;
			StartCoroutine(ChangeLifeAmount(player2Bar, (player2Life - lifeChange) / totalLife, 0.1f));
			if(player2Life < 0){
				UpdateRoundCounters(PlayerNumber.P1);
				return true;
			}	
		}	
		return false;
	}

	void UpdateRoundCounters(PlayerNumber playerNum){
		if(playerNum == PlayerNumber.P1) {
			if(player1WinIcons.transform.GetChild(0).gameObject.activeSelf) {
				player1WinIcons.transform.GetChild(1).gameObject.SetActive(true);
				Debug.Log("Player 1 Wins");
			}
			else{
				StartCoroutine(StartRound());
				player1WinIcons.transform.GetChild(0).gameObject.SetActive(true);
			}
		}
		else if(playerNum == PlayerNumber.P2) {
			if(player2WinIcons.transform.GetChild(0).gameObject.activeSelf) {
				player2WinIcons.transform.GetChild(1).gameObject.SetActive(true);
				Debug.Log("Player 2 Wins");
			}
			else{
				StartCoroutine(StartRound());
				player2WinIcons.transform.GetChild(0).gameObject.SetActive(true);
			}
		}
	}

	void LoadCharacters() {
		GameObject player1Obj = Instantiate((GameObject)Resources.Load(player1Character.ToString()));
		player1Obj.transform.position = player1StartPos;
		player1 = player1Obj.GetComponent<FighterController>();
		GameObject player2Obj = Instantiate((GameObject)Resources.Load(player2Character.ToString()));
		player2Obj.transform.position = player2StartPos;
		player2 = player2Obj.GetComponent<FighterController>();
		player2.identity = PlayerNumber.P2;
		player2.opponent = player1;
		player1.opponent = player2;
		
	}
}
