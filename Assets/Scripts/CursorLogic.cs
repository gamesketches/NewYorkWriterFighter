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
		fullNames.Add("Alexandra", "Alexandra Kleeman");
		fullNames.Add("Amy", "Amy Rose Speigel");
		fullNames.Add("Tony", "Tony Tuliathimutte");
		fullNames.Add("Arabelle", "Arabelle Sicardi");
		fullNames.Add("Chelsea", "Chelsea Hodson");
		fullNames.Add("Jia", "Jia Tolentino");
		fullNames.Add("Saeed", "Saeed Jones");
	}
}
