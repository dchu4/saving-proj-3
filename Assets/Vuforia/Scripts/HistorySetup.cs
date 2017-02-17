using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HistorySetup : MonoBehaviour {

	void Start () {
		GameObject.FindGameObjectWithTag("AppController").GetComponent<AppController> ().Load ();
		GameObject.FindGameObjectWithTag("HistoryController").GetComponent<ButtonGenerator>().SetupHistoryScene ();
	}

}
