/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;
using UnityEngine.Assertions;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Camera))]

public class AirVRCamera : AirVRCameraBase {
    public enum HeadsetType {
        Unknown,
        GearVR,
        Go,
        Quest
    }

    public static OVRInput.Controller ParseController(OVRInput.Controller controller) {
        switch (headsetType) {
            case HeadsetType.GearVR:
            case HeadsetType.Go:
                if (controller == OVRInput.Controller.LTouch) {
                    return OVRInput.Controller.LTrackedRemote;
                }
                else if (controller == OVRInput.Controller.RTouch) {
                    return OVRInput.Controller.RTrackedRemote;
                }
                break;
            case HeadsetType.Quest:
                if (controller == OVRInput.Controller.LTrackedRemote) {
                    return OVRInput.Controller.LTouch;
                }
                else if (controller == OVRInput.Controller.RTrackedRemote) {
                    return OVRInput.Controller.RTouch;
                }
                break;
        }
        return controller;
    }

    public static bool IsConnected(OVRInput.Controller controller) {
        return (OVRInput.GetConnectedControllers() & controller) == controller;
    }

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void onairvr_InitJNI();

    public static HeadsetType headsetType {
        get {
            if (_instance == null) { return HeadsetType.Unknown; }
            return _instance._headsetType;
        }
    }

    private static AirVRCamera _instance;

    private HeadsetType _headsetType;
    private AirVRProfile _profile;

    protected override void Awake () {
        Assert.IsNull(_instance);
        _instance = this;

        switch (OVRPlugin.GetSystemHeadsetType()) {
            case OVRPlugin.SystemHeadset.Oculus_Go:
                _headsetType = HeadsetType.Go;
                break;
            case OVRPlugin.SystemHeadset.Oculus_Quest:
                _headsetType = HeadsetType.Quest;
                break;
            case OVRPlugin.SystemHeadset.GearVR_R320:
            case OVRPlugin.SystemHeadset.GearVR_R321:
            case OVRPlugin.SystemHeadset.GearVR_R322:
            case OVRPlugin.SystemHeadset.GearVR_R323:
            case OVRPlugin.SystemHeadset.GearVR_R324:
            case OVRPlugin.SystemHeadset.GearVR_R325:
                _headsetType = HeadsetType.GearVR;
                break;
        }

        // Work around : Only the game module can access to java classes in onAirVR client plugin.
        if (Application.isEditor == false && Application.platform == RuntimePlatform.Android) {
            onairvr_InitJNI();
        }
        
        base.Awake();
        _profile = new AirVRProfile();
    }

    protected override void Start() {
        base.Start();

        if (_headsetType == HeadsetType.Quest) {
            AirVRInputManager.RegisterInputDevice(new AirVRLeftControllerInputDevice());
            AirVRInputManager.RegisterInputDevice(new AirVRRightControllerInputDevice());
        }
        else {
            AirVRInputManager.RegisterInputDevice(new AirVRTouchpadInputDevice());
            AirVRInputManager.RegisterInputDevice(new AirVRGamepadInputDevice());
            AirVRInputManager.RegisterInputDevice(new AirVRTrackedControllerInputDevice());
        }

        gameObject.AddComponent<AirVRGazePointer>();
		gameObject.AddComponent<AirVRTrackedControllerPointer>().Configure(defaultTrackedControllerModel, true);
    }

    // implements AirVRCameraBase
    protected override AirVRProfileBase profile {
        get {
            return _profile;
        }
    }

    protected override void RecenterPose() {
        OVRManager.display.RecenterPose();
    }
}
