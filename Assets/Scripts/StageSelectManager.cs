using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectManager : MonoBehaviour {

	Sprite[] stages;
	PlayerNumber playerNum;
	float dragMoveTimer;
	int selectedStage = 0;
	Image stageImage;
	Text stageName;
	public string[] StageNames;
	bool selectingStage = false;

	// Use this for initialization
	void Start () {
		stageImage = GetComponent<Image>();
		stages = Resources.LoadAll<Sprite>("stages");	
		GameManager.stageID = -1;
		stageName = GetComponentInChildren<Text>();
		stageName.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		if(!selectingStage) {
			if(GameManager.stageID > -1) {
				return;
			}
			else if(GameManager.player1Character != Character.None) {
				playerNum = PlayerNumber.P1;
				selectingStage = true;
			}
			else if(GameManager.player2Character != Character.None) {
				playerNum = PlayerNumber.P2;
				selectingStage = true;
			}
		}
		else if(dragMoveTimer > 0) {
			dragMoveTimer -= Time.fixedDeltaTime;
		}
		else {
			CheckChangeStage(Input.GetAxisRaw(playerNum.ToString() + "Horizontal"));
			string colorString = GetRandomColorString();
			stageName.text = colorString + "<</color> " + StageNames[selectedStage] + colorString +  " ></color>";
			if(AnyButtonHit()) {
				GameManager.stageID = selectedStage;
				selectingStage = false;
				stageName.text = StageNames[selectedStage];

			}
		}
		
		stageImage.sprite = stages[selectedStage];
	}

	void CheckChangeStage(float direction) {
		if(direction >0 ) {
			selectedStage++;
			if(selectedStage >= stages.Length)	selectedStage = 0;
			dragMoveTimer = CursorLogic.dragMoveTime;
		}
		else if(direction < 0) {
			selectedStage--;
			if(selectedStage < 0) selectedStage = stages.Length - 1;
			dragMoveTimer = CursorLogic.dragMoveTime;
		}
	}

	bool AnyButtonHit() {
		string id = playerNum.ToString();
		if(Input.GetButtonDown(id + "LP") || Input.GetButtonDown(id + "MP")|| Input.GetButtonDown(id + "HP")
			|| Input.GetButtonDown(id + "LK")|| Input.GetButtonDown(id + "MK")|| Input.GetButtonDown(id + "HK")){
			return true;
		}
		return false;
	}

	string GetRandomColorString(){
		Color newColor = new Color(Random.value, Random.value, Random.value);
		return "<color=#" + ColorUtility.ToHtmlStringRGB(newColor) + ">";
	}
}
