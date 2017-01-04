using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class AppController : MonoBehaviour {

	public static AppController appController;

	private string recoURL;

	void Awake(){
		if (appController == null) {
			DontDestroyOnLoad (gameObject);
			appController = this;
		}
		else if(appController != null){
			Destroy (gameObject);
		}
	}

	public void SetRecoURL (string newURL){
		recoURL = newURL;
	}

	public string GetRecoURL (){
		return recoURL;
	}

	public void SceneChange () {
		SceneManager.LoadScene ("Main");
		Destroy (gameObject);
	}
}
