﻿/*===============================================================================
Copyright (c) 2015-2016 PTC Inc. All Rights Reserved.
 
Copyright (c) 2012-2015 Qualcomm Connected Experiences, Inc. All Rights Reserved.
 
Vuforia is a trademark of PTC Inc., registered in the United States and other 
countries.
===============================================================================*/
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using Vuforia;

/// <summary>
/// This MonoBehaviour implements the Cloud Reco Event handling for this sample.
/// It registers itself at the CloudRecoBehaviour and is notified of new search results as well as error messages
/// The current state is visualized and new results are enabled using the TargetFinder API.
/// </summary>
public class CloudRecognitionEventHandler : MonoBehaviour, ICloudRecoEventHandler
{
	#region PRIVATE_MEMBERS
	// ObjectTracker reference to avoid lookups
	private ObjectTracker mObjectTracker;
	private ContentManager mContentManager;
	private TrackableSettings mTrackableSettings;
	private bool mMustRestartApp = false;

	// the parent gameobject of the referenced ImageTargetTemplate - reused for all target search results
	private GameObject mParentOfImageTargetTemplate;
	private string recoURL;
	private AudioSource audioSource;
	private string ahoyVisit = "";
	private string ahoyVisitor = "";
	#endregion // PRIVATE_MEMBERS


	#region PUBLIC_MEMBERS
	/// <summary>
	/// Can be set in the Unity inspector to reference a ImageTargetBehaviour that is used for augmentations of new cloud reco results.
	/// </summary>
	public ImageTargetBehaviour ImageTargetTemplate;
	/// <summary>
	/// The scan-line rendered in overlay when Cloud Reco is in scanning mode.
	/// </summary>
	public ScanLine scanLine;
	/// <summary>
	/// Cloud Reco error UI elements.
	/// </summary>
	public Canvas cloudErrorCanvas;
	public UnityEngine.UI.Text cloudErrorTitle;
	public UnityEngine.UI.Text cloudErrorText;
	public GameObject tapToStart;
	public GameObject scanningAnimation;
	public AudioClip scanningRegistered;
	AppController appController;
	#endregion //PUBLIC_MEMBERS

	#region MONOBEHAVIOUR_METHODS
	/// <summary>
	/// register for events at the CloudRecoBehaviour
	/// </summary>
	void Start()
	{
		audioSource = GetComponent<AudioSource> ();
		appController = GameObject.FindGameObjectWithTag ("AppController").GetComponent<AppController> ();
		mTrackableSettings = FindObjectOfType<TrackableSettings>();

		// look up the gameobject containing the ImageTargetTemplate:
		mParentOfImageTargetTemplate = ImageTargetTemplate.gameObject;
	}
	#endregion //MONOBEHAVIOUR_METHODS

	public void StartScanning (){
		CloudRecoBehaviour cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
		if (cloudRecoBehaviour)
		{
			cloudRecoBehaviour.RegisterEventHandler(this);
			tapToStart.SetActive (false);
			scanningAnimation.SetActive (true);
		}
	}

	public void StopScanning (){
		CloudRecoBehaviour cloudRecoBehaviour = GetComponent<CloudRecoBehaviour>();
		if (cloudRecoBehaviour)
		{
			cloudRecoBehaviour.UnregisterEventHandler(this);
		}
	}

	private string StringCleanUp(string jsonString){
		int stringLength = jsonString.Length - 2;
		string stringOutput = jsonString.Substring(1, stringLength);

		return stringOutput;
	}

	public string GetRecoURL (){
		return recoURL;
	}

	public IEnumerator PostVisit(string product_id){
		WWWForm form = new WWWForm();

		form.AddField ("device_unique_id", SystemInfo.deviceUniqueIdentifier.ToString());
		form.AddField ("device_os", SystemInfo.operatingSystem.ToString());
		form.AddField ("device_type", SystemInfo.deviceType.ToString());
		form.AddField ("product_id", product_id);

		WWW www = new WWW("http://tde-analytics.herokuapp.com/visits", form);
		yield return www;

		Debug.Log (www.text);
	}

//	public IEnumerator PostEvent(){
//		WWWForm form = new WWWForm();
//
//		Dictionary<string, string> headers = new Dictionary<string, string>();
//
//		headers.Add ("Content-Type", "application/x-www-form-urlencoded");
//		headers.Add ("Ahoy-Visit", ahoyVisit);
//		headers.Add ("Ahoy-Visitor", ahoyVisitor);
//
//		form.AddField ("name", "Track event from Unity ME!!!!!!!!!!!!!!");
//		form.AddField ("user_id", 42);
//
//		WWW www = new WWW("http://unity-analytics.herokuapp.com/ahoy/events", form.data, headers);
//		yield return www;
//	}

	#region ICloudRecoEventHandler_implementation
	/// <summary>
	/// Called when TargetFinder has been initialized successfully
	/// </summary>
	public void OnInitialized()
	{
		Debug.Log("Cloud Reco initialized successfully.");

		// get a reference to the Object Tracker, remember it
		mObjectTracker = TrackerManager.Instance.GetTracker<ObjectTracker>();
		mContentManager = FindObjectOfType<ContentManager>();
	}

	/// <summary>
	/// Called if Cloud Reco initialization fails
	/// </summary>
	public void OnInitError(TargetFinder.InitState initError)
	{
		Debug.Log("Cloud Reco initialization error: " + initError.ToString());
		switch (initError)
		{
		case TargetFinder.InitState.INIT_ERROR_NO_NETWORK_CONNECTION:
			{
				mMustRestartApp = true;
				ShowError("Network Unavailable", "Please check your internet connection and try again.");
				break;
			}
		case TargetFinder.InitState.INIT_ERROR_SERVICE_NOT_AVAILABLE:
			ShowError("Service Unavailable", "Failed to initialize app because the service is not available.");
			break;
		}
	}

	/// <summary>
	/// Called if a Cloud Reco update error occurs
	/// </summary>
	public void OnUpdateError(TargetFinder.UpdateState updateError)
	{
		Debug.Log("Cloud Reco update error: " + updateError.ToString());
		switch (updateError)
		{
		case TargetFinder.UpdateState.UPDATE_ERROR_AUTHORIZATION_FAILED:
			ShowError("Authorization Error", "The cloud recognition service access keys are incorrect or have expired.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_NO_NETWORK_CONNECTION:
			ShowError("Network Unavailable", "Please check your internet connection and try again.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_PROJECT_SUSPENDED:
			ShowError("Authorization Error", "The cloud recognition service has been suspended.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_REQUEST_TIMEOUT:
			ShowError("Request Timeout", "The network request has timed out, please check your internet connection and try again.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_SERVICE_NOT_AVAILABLE:
			ShowError("Service Unavailable", "The service is unavailable, please try again later.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_TIMESTAMP_OUT_OF_RANGE:
			ShowError("Clock Sync Error", "Please update the date and time and try again.");
			break;
		case TargetFinder.UpdateState.UPDATE_ERROR_UPDATE_SDK:
			ShowError("Unsupported Version", "The application is using an unsupported version of Vuforia.");
			break;
		}
	}

	/// <summary>
	/// when we start scanning, unregister Trackable from the ImageTargetTemplate, then delete all trackables
	/// </summary>
	public void OnStateChanged(bool scanning)
	{
		if (scanning)
		{
			// clear all known trackables
			mObjectTracker.TargetFinder.ClearTrackables(false);

			// hide the ImageTargetTemplate
			mContentManager.ShowObject(false);
		}

		ShowScanLine(scanning);
	}

	/// <summary>
	/// Handles new search results
	/// </summary>
	/// <param name="targetSearchResult"></param>
	public void OnNewSearchResult(TargetFinder.TargetSearchResult targetSearchResult)
	{
		// This code demonstrates how to reuse an ImageTargetBehaviour for new search results and modifying it according to the metadata
		// Depending on your application, it can make more sense to duplicate the ImageTargetBehaviour using Instantiate(), 
		// or to create a new ImageTargetBehaviour for each new result

		// Vuforia will return a new object with the right script automatically if you use
		// TargetFinder.EnableTracking(TargetSearchResult result, string gameObjectName)

		//Check if the metadata isn't null
		if (targetSearchResult.MetaData == null) {
			Debug.Log ("Target metadata not available.");
			return;
		} else {
			audioSource.PlayOneShot(scanningRegistered,1.0f);
			JSONObject recoData = new JSONObject (targetSearchResult.MetaData);
			string productID = StringCleanUp (recoData ["ProductID"].ToString ());
			string url = StringCleanUp(recoData ["URL"].ToString ());

			StartCoroutine(PostVisit(productID));

			appController.SetRecoURL (url);

			SceneManager.LoadScene ("WebView");
		}

		// First clear all trackables
		mObjectTracker.TargetFinder.ClearTrackables(false);

		// enable the new result with the same ImageTargetBehaviour:
		ImageTargetBehaviour imageTargetBehaviour = mObjectTracker.TargetFinder.EnableTracking(targetSearchResult, mParentOfImageTargetTemplate) as ImageTargetBehaviour;

		//if extended tracking was enabled from the menu, we need to start the extendedtracking on the newly found trackble.
		if (mTrackableSettings && mTrackableSettings.IsExtendedTrackingEnabled())
		{
			imageTargetBehaviour.ImageTarget.StartExtendedTracking();
		}
	}
	#endregion //ICloudRecoEventHandler_implementation


	#region PUBLIC_METHODS
	public void CloseErrorDialog()
	{
		if (cloudErrorCanvas)
		{
			cloudErrorCanvas.transform.parent.position = Vector3.right * 2 * Screen.width;
			cloudErrorCanvas.gameObject.SetActive(false);
			cloudErrorCanvas.enabled = false;

			if (mMustRestartApp)
			{
				mMustRestartApp = false;
				RestartApplication();
			}
		}
	}
	#endregion //PUBLIC_METHODS

	#region PRIVATE_METHODS
	private void ShowScanLine(bool show)
	{
		// Toggle scanline rendering
		if (scanLine != null)
		{
			Renderer scanLineRenderer = scanLine.GetComponent<Renderer>();
			if (show)
			{
				// Enable scan line rendering
				if (!scanLineRenderer.enabled)
					scanLineRenderer.enabled = true;

				scanLine.ResetAnimation();
			}
			else
			{
				// Disable scanline rendering
				if (scanLineRenderer.enabled)
					scanLineRenderer.enabled = false;
			}
		}
	}

	private void ShowError(string title, string msg)
	{
		if (!cloudErrorCanvas) return;

		if (cloudErrorTitle)
			cloudErrorTitle.text = title;

		if (cloudErrorText)
			cloudErrorText.text = msg;

		// Show the error canvas
		cloudErrorCanvas.transform.parent.position = Vector3.zero;
		cloudErrorCanvas.gameObject.SetActive(true);
		cloudErrorCanvas.enabled = true;
	}

	// Callback for network-not-available error message
	private void RestartApplication()
	{
		#if (UNITY_5_2 || UNITY_5_1 || UNITY_5_0)
		int startLevel = Application.loadedLevel - 2;
		if (startLevel < 0) startLevel = 0;
		Application.LoadLevel(startLevel);
		#else // UNITY_5_3 or above
		int startLevel = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 2;
		if (startLevel < 0) startLevel = 0;
		UnityEngine.SceneManagement.SceneManager.LoadScene(startLevel);
		#endif
	}
	#endregion //PRIVATE_METHODS
}