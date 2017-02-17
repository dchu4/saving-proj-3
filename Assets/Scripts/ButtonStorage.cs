using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStorage : MonoBehaviour {
	string productUrl = "";
	Product product;
	string dictionaryKey = "";

	public void setProductUrl(string newUrl){
		productUrl = newUrl;
	}

	public string getProductUrl(){
		return productUrl;
	}
}