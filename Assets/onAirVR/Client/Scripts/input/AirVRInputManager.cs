/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;
using UnityEngine.Assertions;

public class AirVRInputManager : MonoBehaviour {
    private static AirVRInputManager _instance;

    public static AirVRClientInputStream inputStream {
        get {
            return _instance != null ? _instance._inputStream : null;
        }
    }

    public static void LoadOnce() {
        if (_instance == null) {
            GameObject go = new GameObject("AirVRInputManager");
            go.AddComponent<AirVRInputManager>();
        }
    }

    public static void RegisterInputSender(AirVRInputSender sender) {
        Assert.IsNotNull(_instance);

        _instance._inputStream.RegisterInputSender(sender);
    }

    public static void UnregisterInputSender(AirVRInputSender sender) {
        Assert.IsNotNull(_instance);

        _instance._inputStream.UnregisterInputSender(sender);
    }

    private AirVRClientInputStream _inputStream;

    private void Awake() {
        Assert.IsNull(_instance);
        _instance = this;
        DontDestroyOnLoad(gameObject);

        _inputStream = new AirVRClientInputStream();

        AirVRClient.MessageReceived += onAirVRMessageReceived;
    }

    private void Update() {
        if (Application.isEditor) { return; }

        _inputStream.UpdateInputFrame();
    }

    private void LateUpdate() {
        if (Application.isEditor) { return; }

        _inputStream.UpdateSenders();
    }

    // handle AirVRMessages
    private void onAirVRMessageReceived(AirVRClientMessage message) {
        if (message.IsSessionEvent()) {
            if (message.Name.Equals(AirVRClientMessage.NameSetupResponded)) {
                _inputStream.Init();
            }
            else if (message.Name.Equals(AirVRClientMessage.NamePlayResponded)) {
                _inputStream.Start();
            }
            else if (message.Name.Equals(AirVRClientMessage.NameStopResponded)) {
                _inputStream.Stop();
            }
            else if (message.Name.Equals(AirVRClientMessage.NameDisconnected)) {
                _inputStream.Cleanup();
            }
        }
    }
}
