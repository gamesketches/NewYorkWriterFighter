using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour {

	Sprite[] stages;
	PlayerNumber playerNum;
	int selectedStage = 0;
	Image stageImage;
	Text stageName;

	// Use this for initialization
	void Start () {
		stages = Resources.LoadAll<Sprite>("stages");	
	}
	
	// Update is called once per frame
	void Update () {
		if(playerNum == null) {
			if(GameManager.player1Character != Character.None) {
				playerNum = PlayerNumber.P1;
			}
			else if(GameManager.player2Character != Character.None) {
				playerNum = PlayerNumber.P2;
			}
		}
		//else if(playerNum = PlayerNumber.P1) {
			
	}
}
