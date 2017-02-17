using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIMenu : MonoBehaviour {

	public Texture2D menuTexture;

	void OnGUI() {
		GUIStyle customButton = new GUIStyle();
		customButton.fontSize = 60;
		customButton.normal.background = menuTexture;

		GUILayout.BeginArea(new Rect(100, 250, 500, 500));
		GUILayout.Button("Wishlist", customButton, GUILayout.Width(500), GUILayout.Height(150));
		GUILayout.Button("History", customButton, GUILayout.Width(500), GUILayout.Height(150));
		GUILayout.EndArea();
	}
}
