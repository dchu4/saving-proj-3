using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {
	public string sceneLoad = "";
	GameObject appController;
	// Update is called once per frame
	void Start(){
		appController = GameObject.FindGameObjectWithTag ("AppController");
	}

	public void SceneChange () {
		SceneManager.LoadScene (sceneLoad);
		Destroy (appController.gameObject);
	}
}
