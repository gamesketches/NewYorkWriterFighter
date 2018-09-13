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
			vsScreenName.text = fullNames[characterName];
			if(identity == PlayerNumber.P1) {
				GameManager.player1Character = theCharacter;
			}
			else {
				GameManager.player2Character = theCharacter;
			}
		}

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

	void MakeFullNameDict() {
		fullNames = new Dictionary<string, string>();
		fullNames.Add("Alexandra", "Alexandra Kleeman\nNovelist\nAuthor of You Too Can Have A Body Like Mine\n@AlexKleeman");
		fullNames.Add("Amy", "Amy Rose Speigel\nWriter and Editor\nAuthor of Action: A Book About Sex\nCreator of Enormous Eye\n@AmyRosary");
		fullNames.Add("Tony", "Tony Tuliathimutte\nNovelist\nAuthor of Private Citizens\n@tonytula");
		fullNames.Add("Arabelle", "Arabelle Sicardi\nBeauty and Fashion Writer\nAllure, Ssense, Racked, DAZED, BuzzFeed, Jezebel\n@arabellesicardi");
		fullNames.Add("Chelsea", "Chelsea Hodson\nEssayist\nAuthor of Tonight I’m Someone Else\n@ChelseaHodson");
		fullNames.Add("Jia", "Jia Tolentino\nThe New Yorker Staff Writer\nFormer editor Jezebel and The Hairpin\n@jiatolentino");
		fullNames.Add("Saeed", "Saeed Jones\nPoet and Memoirist\nAuthor of Prelude to Bruise\nBuzzFeed Executive Editor and AM to DM Host\n@theferocity");
	}
}
