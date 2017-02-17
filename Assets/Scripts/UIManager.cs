using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

	public Animator menu;
	bool menuShow;
	AppController appController;

	void Start () {
		menuShow = false;
		appController = GameObject.FindGameObjectWithTag ("AppController").GetComponent<AppController> ();
	}

	public void MenuToggle () {
		if (menuShow == false) {
			menu.SetBool ("isHidden", false);
			menuShow = true;
		}
		else {
			menu.SetBool ("isHidden", true);
			menuShow = false;
		}
	}

	public void HomeTransition(){
		SceneManager.LoadScene ("main-menu");
	}

	public void WishlistTransition(){
		SceneManager.LoadScene ("Wishlist");
	}

	public void HistoryTransition(){
		SceneManager.LoadScene ("History");
	}

	public void Starred(){
		appController.AddToWishlist();
	}
}
