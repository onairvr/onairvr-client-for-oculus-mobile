/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using System;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

public class AirVRClientAppManager : Singleton<AirVRClientAppManager>, AirVRClient.EventHandler {
    private const string DevConfigFile = "config.json";

    [SerializeField][Range(0.5f, 2.0f)] private float _renderScale = 1f;

    private AirVRCamera _camera;
    private GameObject _room;
    private Light _envLight;
    private AirVRRealWorldSpaceSetup _realWorldSpaceSetup;
    private DevConfig _devConfig;

    private bool _lastUserPresent = false;

    public bool IsConnecting { get; private set; }
    public AirVRClientNotification Notification { get; private set; }
    public AirVRClientAppConfig Config { get; private set; }
    public AirVRClientInputModule InputModule { get; private set; }

    private void Awake() {
        _camera = FindObjectOfType<AirVRCamera>();
        _room = transform.Find("Room").gameObject;
        _envLight = transform.Find("EnvLight").GetComponent<Light>();

        Notification = FindObjectOfType<AirVRClientNotification>();
        Config = new AirVRClientAppConfig();
        InputModule = FindObjectOfType<AirVRClientInputModule>();

        AirVRClient.Delegate = this;

        var configPath = Path.Combine(Application.persistentDataPath, DevConfigFile);
        if (Application.isEditor == false && File.Exists(configPath)) {
            _devConfig = JsonUtility.FromJson<DevConfig>(File.ReadAllText(configPath));
        }
        else {
            _devConfig = new DevConfig();
            _devConfig.profiler = false;
        }
    }

    private void Start() {
        XRSettings.eyeTextureResolutionScale = _renderScale;

        if (Config.FirstPlay) {            
            AirVRClientUIManager.Instance.GuidePanel.StartGuide();
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (IsConnecting) {
                OnDisconnected();
            }
        }

        // TODO remove real world space setup
        if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick)) {
            if (_realWorldSpaceSetup == null) {
                _realWorldSpaceSetup = new GameObject("RealWorldSpaceSetup").AddComponent<AirVRRealWorldSpaceSetup>();
                _realWorldSpaceSetup.transform.position = Vector3.zero;
                _realWorldSpaceSetup.transform.rotation = Quaternion.identity;

                _room.SetActive(false);
            }
            else {
                Destroy(_realWorldSpaceSetup.gameObject);
                _realWorldSpaceSetup = null;

                _room.SetActive(true);
            }
        }
    }

    public void Connect(string addressText, string portText, string userIDText) {   
        string message;

        if (!AirVRClientAppConfig.ValidateIPv4(addressText)) {
            message = "Please enter the correct ip address.";

            Notification.DisplayError(message);
            Debug.Log(message);
            return;
        }

        if (!AirVRClientAppConfig.ValidatePort(portText)) {
            message = "Please enter the correct port.";

            Notification.DisplayError(message);
            Debug.Log(message);
            return;
        }

        if (!AirVRClientAppConfig.ValidateUserID(userIDText)) {
            message = "Please enter the correct User ID.";

            Notification.DisplayError(message);
            Debug.Log(message);
            return;
        }

        string address = addressText;
        int port = int.Parse(portText);
        int userID = int.Parse(userIDText);

        Config.Save(address, port, userID, AirVRClientUIManager.Instance.SettingPanel.AutoPlay.Toggle.isOn, Config.FirstPlay);
        AirVRClientUIManager.Instance.CanvasGroup.blocksRaycasts = false;        
        AirVRClientUIManager.Instance.CanvasGroup.interactable = false;

        AirVRClientUIManager.Instance.SettingPanel.PlayButton.enabled = false;

        IsConnecting = true;
        Notification.DisplayConnecting();

        _camera.profile.userID = userID.ToString();

        if (_devConfig.profiler) {
            var pathFormat = Path.Combine(Application.persistentDataPath, DateTime.Now.ToString("yyyyMMddhhmmss") + ".%s");

            _camera.profile.profilers = AirVRProfileBase.ProfilerMaskFrame;
            _camera.profile.profilerLogPathFormat = pathFormat;
        }
        else {
            _camera.profile.profilers = 0;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        AirVRClient.Connect(address, port);
#endif
    }

    private void OnDisconnected() {
        _envLight.enabled = true;

        if (!_room.activeSelf)
            _room.SetActive(true);

        AirVRClientUIManager.Instance.Show();
        AirVRClientUIManager.Instance.SettingPanel.PlayButton.enabled = true;
        AirVRClientUIManager.Instance.CanvasGroup.blocksRaycasts = true;
        AirVRClientUIManager.Instance.CanvasGroup.interactable = true;

        if (IsConnecting)
        {
            string message = "Connection failed.";
            Notification.DisplayError(message);
            Debug.LogError(message);
        }

        IsConnecting = false;
    }

    // implements AirVRClient.EventHandler
    public void AirVRClientFailed(string reason) { }

    public void AirVRClientConnected() {
        _room.SetActive(false);
        IsConnecting = false;
        AirVRClientUIManager.Instance.Hide();
        AirVRClient.Play();

        _envLight.enabled = false;
    }

    public void AirVRClientPlaybackStarted() {}
    
    public void AirVRClientPlaybackStopped() { }

    public void AirVRClientDisconnected() {
        OnDisconnected();
    }

    public void AirVRClientUserDataReceived(byte[] userData) {
        Debug.Log("User data received: " + userData.Length);
        
        // pong received data back to the server
        AirVRClient.SendUserData(userData);
    }

    [Serializable]
    private struct DevConfig {
        public bool profiler;
    } 
}
