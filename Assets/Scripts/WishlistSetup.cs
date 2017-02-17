using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WishlistSetup : MonoBehaviour {

	void Start () {
		GameObject.FindGameObjectWithTag("AppController").GetComponent<AppController> ().Load ();
		gameObject.GetComponent<ButtonGenerator>().SetupWishlistScene ();
	}
}
