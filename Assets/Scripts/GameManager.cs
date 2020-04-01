using System.Collections;
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
	bool player1LifeBarFlashing;
	bool player2LifeBarFlashing;
	public Vector3 player1StartPos;
	public Vector3 player2StartPos;
	public static Character player1Character = Character.None;
	public static Character player2Character = Character.None;
	public static int stageID = 0;
	public static int AIPlayer = -1;
	List<GameObject> hitSparks;

	public static float stageXEnds = 12.68f;
	
	FighterController player1;
	FighterController player2;

	AudioSource audio;
	AudioSource bgm;
	public Text RoundText;

	public Character debugP1, debugP2;

    public static GameManager _instance;

	// Use this for initialization
	void Awake () {
        _instance = this;
		audio = GetComponent<AudioSource>();
		bgm = GetComponents<AudioSource>()[1];
		roundCounter = 0;
		SpriteRenderer background = GameObject.Find("Battlezone").GetComponent<SpriteRenderer>();
		
		background.sprite = Resources.LoadAll<Sprite>("stages")[stageID];
		if(stageID == 0) AdjustForWiderStage();
		bgm.clip = Resources.Load<AudioClip>("music/" + background.sprite.name);
		bgm.Play();
		#if UNITY_EDITOR
		player1Character = debugP1;
		player2Character = debugP2;
		#endif
		player1WinIcons.transform.GetChild(0).gameObject.SetActive(false);
		player1WinIcons.transform.GetChild(1).gameObject.SetActive(false);
		player2WinIcons.transform.GetChild(0).gameObject.SetActive(false);
		player2WinIcons.transform.GetChild(1).gameObject.SetActive(false);
		StartCoroutine(StartRound());
		hitSparks = new List<GameObject>();
		hitSparks.Add(Instantiate(Resources.Load<GameObject>("HitSpark")));
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(RoundText.text == "") {
			roundTime -= Time.fixedDeltaTime; 
			if(roundTime > 0) {
				roundTimer.text = Mathf.Floor(roundTime).ToString();
			}
			else if(!player1.locked) {
				if(player2Life > player1Life) {
					StartCoroutine(EndRound(PlayerNumber.P2));
				}
				else {
					StartCoroutine(EndRound(PlayerNumber.P1));
				}
			}
		}
	}

	IEnumerator StartRound() {
		if(roundCounter == 0)
			LoadCharacters();
		else 
			ResetPlayers();
		#if UNITY_EDITOR
		  roundTime = 999.99f;
		#else 
		  roundTime = 99.99f;
		#endif
		player1Bar.fillAmount = 0;
		player2Bar.fillAmount = 0;
		player1Life = totalLife;
		player2Life = totalLife;
		StartCoroutine(ChangeLifeAmount(player1Bar, 1, 0.6f));
		StartCoroutine(ChangeLifeAmount(player2Bar, 1, 0.6f));
		player1LifeBarFlashing = false;
		player2LifeBarFlashing = false;
		Attack.attackID = 0;
		AudioClip round = Resources.Load<AudioClip>("AnnouncerClips/round");
		AudioClip fight = Resources.Load<AudioClip>("AnnouncerClips/fight");
		if(roundCounter < 2) {
			AudioClip roundNum = Resources.Load<AudioClip>("AnnouncerClips/" + (roundCounter+ 1).ToString());
			RoundText.text = "Round " + (roundCounter+1).ToString();
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
		player1.locked = false;
		player2.locked = false;
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
			StartCoroutine(ChangeLifeAmount(player1Bar, (player1Life - lifeChange) / totalLife, 0.1f));
			player1Life -= lifeChange;
			if(player1Life < 0) {
				//UpdateRoundCounters(PlayerNumber.P2);
				StartCoroutine(EndRound(PlayerNumber.P2));
				return true;
			}
			else if(!player1LifeBarFlashing && player1Life < (totalLife / 3)) {
				player1LifeBarFlashing = true;
				StartCoroutine(FlashLifeBar(player1Bar, PlayerNumber.P1));
				player1.MakeSuperAvailable();
			}
		}
		else if(playerNum == PlayerNumber.P2) {
			StartCoroutine(ChangeLifeAmount(player2Bar, (player2Life - lifeChange) / totalLife, 0.1f));
			player2Life -= lifeChange;
			if(player2Life < 0){
				StartCoroutine(EndRound(PlayerNumber.P1));
			//	UpdateRoundCounters(PlayerNumber.P1);
				return true;
			}
			else if(!player2LifeBarFlashing && player2Life < (totalLife / 3)) {
				player2LifeBarFlashing = true;
				StartCoroutine(FlashLifeBar(player2Bar, PlayerNumber.P2));
				player2.MakeSuperAvailable();
			}	
		}	
		return false;
	}

	bool UpdateRoundCounters(PlayerNumber playerNum){
		if(playerNum == PlayerNumber.P1) {
			if(player1WinIcons.transform.GetChild(0).gameObject.activeSelf) {
				player1WinIcons.transform.GetChild(1).gameObject.SetActive(true);
                Debug.Log("Player 1 Wins");
                return true;
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
                Debug.Log("Player 2 Wins");
                return true;
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
		player1.locked = true;
		player2.locked = true;
		FighterController winningPlayer;
		FadeInScene sceneFader = GetComponent<FadeInScene>();
		winningPlayer = playerNum == PlayerNumber.P1 ? player1 : player2;

		StartCoroutine(winningPlayer.VictoryPose());
		Debug.Log(winningPlayer.gameObject.name);
		AudioClip you = Resources.Load<AudioClip>("AnnouncerClips/roundEndYou");
		AudioClip win = Resources.Load<AudioClip>("AnnouncerClips/win");
		if(roundTime < 0) {
			RoundText.text = "TIME OVER";
		}
		else {
			RoundText.text = playerNum.ToString() + " Wins";
		}
		yield return StartCoroutine(PlayAudioAndYield(you));
		yield return StartCoroutine(PlayAudioAndYield(win));
		yield return new WaitForSecondsRealtime(1.5f);
		if(UpdateRoundCounters(playerNum)){
			CharacterSelectManager.winner = (int)playerNum;
			SceneManager.LoadScene("CharacterSelect");
		}
		else {
			yield return StartCoroutine(sceneFader.FadeInOut(2f));
			StartCoroutine(StartRound());
		}
	}


	void LoadCharacters() {
		GameObject player1Obj = Instantiate((GameObject)Resources.Load(player1Character.ToString()));
		player1Obj.transform.position = new Vector3(player1StartPos.x, player1Obj.transform.position.y, player1StartPos.z);
		player1StartPos = player1Obj.transform.position;
		player1Obj.SetActive (true);
		player1 = player1Obj.GetComponent<FighterController>();
		player1.SetPlayerIdentity(PlayerNumber.P1);
		if(AIPlayer == 0) player1.AddAI();
		GameObject player2Obj = Instantiate((GameObject)Resources.Load(player2Character.ToString()));
		player2Obj.transform.position = new Vector3(player2StartPos.x, player2Obj.transform.position.y, player2StartPos.z);
		player2StartPos = player2Obj.transform.position;
		player2Obj.SetActive (true);
		player2 = player2Obj.GetComponent<FighterController>();
		player2.SetPlayerIdentity(PlayerNumber.P2);
		if(player1Character == player2Character) {
			player2Obj.GetComponent<SpriteRenderer>().color = new Color(0.7f, 0.7f, 0.7f);
		}
		if(AIPlayer == 1) player2.AddAI();
		player2.opponent = player1;
		player1.opponent = player2;

		player1.locked = true;
		player2.locked = true;
		Camera.main.GetComponent<CameraController>().SetPlayerTransforms(player1.transform, player2.transform);

	}

	void ResetPlayers() {
        player1.transform.position = player1StartPos;
        player2.transform.position = player2StartPos;
        player1.ResetPlayer();
		player2.ResetPlayer();
	}

	IEnumerator PlayAudioAndYield(AudioClip clip) {
		audio.clip = clip;
		audio.Play();
		while(audio.isPlaying) yield return null;
	}

	IEnumerator FlashLifeBar(Image lifeBar, PlayerNumber num) {
		while(num == PlayerNumber.P1 ? player1LifeBarFlashing : player2LifeBarFlashing){
			lifeBar.color = Color.red;
			yield return new WaitForSecondsRealtime(0.2f);
			lifeBar.color = Color.white;
			yield return new WaitForSecondsRealtime(0.2f);
		}
	}

	public void PlayHitSpark(Vector3 position, bool blockSparks, float damage) {
		GameObject sparks = null;
		for(int i = 0; i <	hitSparks.Count; i++) {
			if(!hitSparks[i].activeSelf) {
				sparks = hitSparks[i];
				sparks.SetActive(true);
				break;
			}
		}
		if(sparks == null) {
			sparks = Instantiate(Resources.Load<GameObject>("HitSpark"));
			hitSparks.Add(sparks);
		}
		float scaleFactor = Mathf.Clamp(damage / 90 + 1, 1, 3);
		sparks.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
		sparks.transform.position = position;
		if(blockSparks) sparks.GetComponent<SpriteRenderer>().color = Color.blue;
		else sparks.GetComponent<SpriteRenderer>().color = Color.white;
	}	

    public IEnumerator PlayThrowSparks(Vector3 position, int[] frameCounts)
    {
        audio.clip = Resources.Load<AudioClip>("Grab");
        audio.Play();
        GameObject newSparks = Instantiate(Resources.Load<GameObject>("HitSpark"));
        HitSparkBehavior sparkBehavior = newSparks.GetComponent<HitSparkBehavior>();
        sparkBehavior.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
        sparkBehavior.transform.position = position;
        yield return new WaitForSeconds(frameCounts[0] * CharacterAnimator.frameSpeed * Time.deltaTime);
        audio.clip = Resources.Load<AudioClip>("ThrowHit");
        sparkBehavior.transform.localScale = new Vector3(2f, 2f, 2f);
        for (int i = 1;  i < frameCounts.Length; i++)
        {
            yield return new WaitForSeconds(frameCounts[i] * CharacterAnimator.frameSpeed * Time.deltaTime);
            StartCoroutine(sparkBehavior.PlayAnimation());
            newSparks.SetActive(true);
            audio.Play();
        }
        yield return new WaitForSeconds(0.2f);
        Destroy(newSparks);
    }

    public void AdjustBGMVolume(float newVolume)
    {
        bgm.volume = newVolume;
    }

    public IEnumerator AdjustBGMVolume(float newVolume, float time)
    {
        float startVolume = bgm.volume;
        for(float t = 0; t < time; t += Time.deltaTime)
        {
            bgm.volume = Mathf.Lerp(startVolume, newVolume, t / time);
            yield return null;
        }
        bgm.volume = newVolume;
    }

    void AdjustForWiderStage(){
		BoxCollider2D[] colliders = GameObject.Find("Battlezone").GetComponents<BoxCollider2D>();

		colliders[0].offset = new Vector2(-12.8f, 0);
		colliders[1].offset = new Vector2(12.8f, 0);

		Camera.main.GetComponent<CameraController>().cornerOffset = 6.41f;
	}
				
}
