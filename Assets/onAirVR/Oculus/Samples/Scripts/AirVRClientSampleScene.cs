/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRClientSampleScene : MonoBehaviour, AirVRClient.EventHandler {
    private AirVRCamera _camera;

    [SerializeField] private string _serverAddress;
    [SerializeField] private int _serverPort;
	[SerializeField] private int _userID;

    private void Awake() {
        AirVRClient.Delegate = this;

        _camera = FindObjectOfType<AirVRCamera>();
    }

	private void Update() {
        if (OVRInput.GetDown(OVRInput.Button.Back) || Input.GetKeyDown(KeyCode.Escape)) {
            if (AirVRClient.connected == false) {
                _camera.profile.userID = (_userID++).ToString();

                AirVRClient.Connect(_serverAddress, _serverPort);
			}
			else {
                AirVRClient.Disconnect();
            }
		}
	}

    private void OnApplicationPause(bool pauseStatus) {
		if (pauseStatus && AirVRClient.playing) {
			AirVRClient.Stop();
		}
		else if (pauseStatus == false && AirVRClient.connected) {
			AirVRClient.Play();
		}
    }

    // implements AirVRClient.EventHandler
    public void AirVRClientFailed (string reason) {
        Debug.Log("[ERROR] AirVRClient failed to initialize : " + reason);
    }

    public void AirVRClientConnected() {
		AirVRClient.Play();
    }

    public void AirVRClientPlaybackStarted() {}
    public void AirVRClientPlaybackStopped() {}
    public void AirVRClientDisconnected() {}
    public void AirVRClientUserDataReceived(byte[] userData) {}
}
