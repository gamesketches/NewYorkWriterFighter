using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeInScene : MonoBehaviour {

	Texture2D fadeOutTexture;
	public float fadeSpeed = 0.01f;

	private int drawDepth = -1000;
	private float alpha = 1.0f;
	private int fadeDir = -1;

	void OnGUI() {
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		alpha = Mathf.Clamp01(alpha);

		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
	}

	public float BeginFade(int direction) {
		fadeDir = direction;
		return (fadeSpeed);
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
		fadeOutTexture = new Texture2D(2, 2);
		fadeOutTexture.SetPixels(new Color[]{Color.black, Color.black, Color.black, Color.black});
		fadeOutTexture.Apply(false);
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		BeginFade(-1);
	}

}
