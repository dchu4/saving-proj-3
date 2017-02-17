using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonGenerator : MonoBehaviour {
	public Texture btnTexture;
	public Transform parentObject;
	public Button productButtonPrefab;
	public Button deleteButtonPrefab;

	AppController appController;

	void Start(){
		appController = GameObject.FindGameObjectWithTag ("AppController").GetComponent<AppController> ();
	}

	//Setup methods for wishlist/history --Beginning--
	public void SetupHistoryScene(){
		List<string> myKeys = appController.GetProductHistory ().Keys.ToList();
		List<List<Product>> myValues = appController.GetProductHistory ().Values.ToList ();

		//Loop through dictionary keys
		for(int i = 0; i < myKeys.Count; i ++){
			List<Product> historyList = appController.GetProductHistory ()[myKeys[i]];

			//Loop through the values associated with the keys
			for (int j = 0; j < historyList.Count; j++) {
				Button newButton = CreateProductButton (historyList[j]);
				CreateDeleteButton (historyList[j], myKeys[i],newButton, appController.GetProductHistory ());
			}
		}

	}

	public void SetupWishlistScene(){
		List<string> myKeys = appController.GetProductWishlist ().Keys.ToList();
		List<List<Product>> myValues = appController.GetProductWishlist ().Values.ToList ();

		Debug.Log (myKeys[0]);

		//Loop through dictionary keys
		for(int i = 0; i < myKeys.Count; i ++){
			List<Product> wishlist = appController.GetProductWishlist ()[myKeys[i]];

			//Loop through the values associated with the keys
			for (int j = 0; j < wishlist.Count; j++) {
				Button newButton = CreateProductButton (wishlist[j]);
				CreateDeleteButton (wishlist[j], myKeys[i],newButton, appController.GetProductWishlist ());
			}
		}

	}
	//Getter methods for wishlist/history --End--

	//Create buttons that link to product
	Button CreateProductButton(Product product){
		Button newButton = Instantiate (productButtonPrefab) as Button;
		newButton.transform.SetParent (parentObject, false);

		newButton.GetComponent<ButtonStorage> ().setProductUrl (product.GetProductURL());
		newButton.GetComponentInChildren<Text>().text  = product.GetProductName();

		newButton.onClick.AddListener (delegate{
			appController.SetRecoURL(newButton.GetComponent<ButtonStorage> ().getProductUrl());
		});

		newButton.onClick.AddListener (WebViewTransition);

		return newButton;
	}

	//Create product delete buttons
	void CreateDeleteButton(Product product, string key, Button remProd, Dictionary<string, List<Product>> remDictionary){
		Button deleteButton = Instantiate (deleteButtonPrefab) as Button;
		deleteButton.transform.SetParent (remProd.transform, false);

		Vector3 deletePosition = new Vector3 (remProd.transform.position.x + 200, remProd.transform.position.y);

		deleteButton.onClick.AddListener (delegate {
			remDictionary[key].Remove(product);
			appController.Save();
		});

		deleteButton.onClick.AddListener (delegate {
			Destroy(remProd.gameObject);
		});

		deleteButton.onClick.AddListener (delegate {
			Destroy(deleteButton.gameObject);
		});
	}

	void WebViewTransition(){
		SceneManager.LoadScene ("WebViewScaled");
	}

	public void ClearHistory(){
		appController.GetProductHistory().Clear ();
		Destroy (parentObject.gameObject);
		appController.Save ();
	}

	public void ClearWishlist(){
		appController.GetProductWishlist().Clear ();
		Destroy (parentObject.gameObject);
		appController.Save ();
	}
}