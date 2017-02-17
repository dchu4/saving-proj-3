using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AppController : MonoBehaviour {

	public static AppController appController;

	private string recoURL;
	private string recoProductName;
	private string recoProductRetailer;
	private Dictionary<string, List<Product>> productHistory = new Dictionary<string, List<Product>> ();
	private Dictionary<string, List<Product>> productWishlist = new Dictionary<string, List<Product>> ();

	void Awake(){
		Load ();
		if (appController == null) {
			DontDestroyOnLoad (gameObject);
			appController = this;
		}
		else if(appController != null){
			Destroy (gameObject);
		}

//		List<Product> holder = new List<Product> ();
//		holder.Add (new Product("rrerehreh","egehrehreh"));
//		holder.Add (new Product("rrerehvrebreh","egebreberbhrehreh"));
//		holder.Add (new Product("brebre","egehrehbreberreh"));
//
//		productWishlist.Add ("Test 1", holder);
//
//		Save ();
	}

	//Setter Methods --Beginning--
	public void SetRecoURL (string newURL){
		recoURL = newURL;
	}

	public void SetRecoProductName (string newProductName){
		recoProductName = newProductName;
	}

	public void SetRecoProductRetailer (string newProductRetailer){
		recoProductRetailer = newProductRetailer;
	}
	//Setter Methods --End--

	//Gets product url
	public string GetRecoURL (){
		return recoURL;
	}

	public Dictionary<string, List<Product>> GetProductHistory(){
		return productHistory;
	}

	public Dictionary<string, List<Product>> GetProductWishlist(){
		return productWishlist;
	}

	public void AddToHistory(){
		if (!productHistory.ContainsKey (recoProductRetailer)) {
			productHistory.Add (recoProductRetailer, new List<Product> ());

			productHistory [recoProductRetailer].Add (CreateHistoryLog ());
		} else {
			productHistory [recoProductRetailer].Add (CreateHistoryLog ());
		}

		Save ();
	}

	public void AddToWishlist(){
		if (!productWishlist.ContainsKey (recoProductRetailer)) {
			productWishlist.Add (recoProductRetailer, new List<Product> ());

			productWishlist [recoProductRetailer].Add (CreateWishlistLog ());
		} else {
			productWishlist [recoProductRetailer].Add (CreateWishlistLog ());
		}

		Save ();
	}

	Product CreateHistoryLog(){
		Product newHistoryLog = new Product (recoProductName, recoURL);

		return newHistoryLog;
	}

	Product CreateWishlistLog(){
		Product newWishlistProduct = new Product (recoProductName, recoURL);

		return newWishlistProduct;
	}

	public void Save(){
		Debug.Log ("Saved");
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/wishlistHistoryData.dat");

		WishlistHistoryData data = new WishlistHistoryData ();
		data.SetProductHistory (productHistory);
		data.SetProductWishlist (productWishlist);

		bf.Serialize (file, data);
		file.Close ();
	}

	public void Load(){
		if(File.Exists(Application.persistentDataPath + "/wishlistHistoryData.dat")){
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/wishlistHistoryData.dat", FileMode.Open);

			WishlistHistoryData data = (WishlistHistoryData)bf.Deserialize(file);
			file.Close ();

			productHistory = data.getProductHistory ();
			productWishlist = data.getProductWishlist ();
		}
	}
}

[Serializable]
public class Product{
	string productName = "";
	string productURL = "";

	public Product(string productName, string productURL){
		this.productName = productName;
		this.productURL = productURL;
	}

	public string GetProductName(){
		return productName;
	}

	public string GetProductURL(){
		return productURL;
	}
}

[Serializable]
class WishlistHistoryData{
	Dictionary<string, List<Product>> productWishlist = new Dictionary<string, List<Product>> ();
	Dictionary<string, List<Product>> productHistory = new Dictionary<string, List<Product>> ();

	public Dictionary<string, List<Product>> getProductWishlist(){
		return productWishlist;
	}

	public void SetProductWishlist(Dictionary<string, List<Product>> newWishlist){
		productWishlist = newWishlist;
	}

	public Dictionary<string, List<Product>> getProductHistory(){
		return productHistory;
	}

	public void SetProductHistory(Dictionary<string, List<Product>> newHistory){
		productHistory = newHistory;
	}
}