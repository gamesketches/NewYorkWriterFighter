using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorLogic : MonoBehaviour {

	public PlayerNumber identity;
	public Transform portraits;
	int childIndex;
	
	private string verticalAxis;
	private string horizontalAxis;
	AudioSource audio;
	public static float dragMoveTime = 0.3f;
	float dragMoveTimer = 0;
	Image cursorImage;
	public Image largePortrait;
	public Text characterName;
	public Image vsScreenPortrait;
	public Text vsScreenName;
	
	Dictionary<string, string> fullNames;

	[HideInInspector]
	public bool selected;

	// Use this for initialization
	void Start () {
		selected = false;
		MakeFullNameDict();
		cursorImage = GetComponent<Image>();
		audio = GetComponent<AudioSource>();
		if(identity == PlayerNumber.P1) {
			childIndex = 0;
		}
		else {
			childIndex = portraits.childCount - 1;
		}
		verticalAxis = identity.ToString() + "Vertical";
		horizontalAxis = identity.ToString() + "Horizontal";
		UpdateCharacterInfo();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(dragMoveTimer > 0 || selected) {
			dragMoveTimer -= Time.fixedDeltaTime;
			cursorImage.color = selected ? Color.white : GetRandomColor();
			return;
		}

		if(Input.GetAxisRaw(verticalAxis) == 1) {
			if(childIndex - 4 < 0) {
				childIndex = portraits.childCount - childIndex;
			}
			else {
				childIndex -= 4;
			}
			MoveCursor();
		}
		else if(Input.GetAxisRaw(verticalAxis) == -1) {
			childIndex = (childIndex + 4) % portraits.childCount;
			MoveCursor();
		}
		if(Input.GetAxisRaw(horizontalAxis) == 1) {
			childIndex = (childIndex + 1) % portraits.childCount;
			MoveCursor();
		}
		else if(Input.GetAxisRaw(horizontalAxis) == -1) {
			childIndex -= 1;
			if(childIndex < 0) {
				childIndex = portraits.childCount - 1;
			}
			MoveCursor();
		}

		if(AnyButtonHit()) {
			selected = true;
			string characterName = portraits.GetChild(childIndex).gameObject.name;
			Character theCharacter = (Character)System.Enum.Parse(typeof(Character), characterName);
			audio.clip = Resources.Load<AudioClip>("charSelected");
			audio.Play();
			vsScreenPortrait.sprite = largePortrait.sprite;
			vsScreenName.text =portraits.GetChild(childIndex).name;
			if(identity == PlayerNumber.P1) {
				GameManager.player1Character = theCharacter;
			}
			else {
				GameManager.player2Character = theCharacter;
			}
		}
		
		cursorImage.color = GetRandomColor();
		transform.position = portraits.GetChild(childIndex).position;
	}
	
	void MoveCursor() {
		audio.Play();
		dragMoveTimer = dragMoveTime;
		UpdateCharacterInfo();
	}

	bool AnyButtonHit() {
		string id = identity.ToString();
		if(Input.GetButtonDown(id + "LP") || Input.GetButtonDown(id + "MP")|| Input.GetButtonDown(id + "HP")
			|| Input.GetButtonDown(id + "LK")|| Input.GetButtonDown(id + "MK")|| Input.GetButtonDown(id + "HK")){
			return true;
		}
		return false;
	}

	void UpdateCharacterInfo() {
		Transform portrait = portraits.GetChild(childIndex);
		largePortrait.sprite = portrait.GetComponent<Image>().sprite;
		characterName.text = fullNames[portrait.name];
	}

	Color GetRandomColor(){
		return new Color(Random.value, Random.value, Random.value);
	}

	void MakeFullNameDict() {
		fullNames = new Dictionary<string, string>();
		fullNames.Add("Alexandra", "<color=#ff8c52>Alexandra Kleeman</color>\nNovelist\n<i>You Too Can Have A Body Like Mine</i>\n<color=#55acee>@AlexKleeman</color>");
		fullNames.Add("Amy", "<color=#ff8c52>Amy Rose Speigel</color>\nWriter and Editor\n<i>Action: A Book About Sex</i>\nCreator of Enormous Eye\n<color=#55acee>@AmyRosary</color>");
		fullNames.Add("Tony", "<color=#ff8c52>Tony Tuliathimutte</color>\nNovelist\n<i>Private Citizens</i>\n<color=#55acee>@tonytula</color>");
		fullNames.Add("Arabelle", "<color=#ff8c52>Arabelle Sicardi</color>\nBeauty and Fashion Writer\nAllure, Sense, Racked, DAZED, BuzzFeed, Jezebel\n<color=#55acee>@arabellesicardi</color>");
		fullNames.Add("Chelsea", "<color=#ff8c52>Chelsea Hodson</color>\nEssayist\n<i>Tonight I’m Someone Else</i>\n<color=#55acee>@ChelseaHodson</color>");
		fullNames.Add("Jia", "<color=#ff8c52>Jia Tolentino</color>\nThe New Yorker Staff Writer\nFormer editor Jezebel and The Hairpin\n<color=#55acee>@jiatolentino</color>");
		fullNames.Add("Saeed", "<color=#ff8c52>Saeed Jones</color>\nPoet and Memoirist\n<i>Prelude to Bruise</i>\nBuzzFeed Executive Editor and AM to DM Host\n<color=#55acee>@theferocity</color>");
	}
}
