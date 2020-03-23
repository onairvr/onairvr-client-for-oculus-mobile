/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Camera))]

public class AirVRCamera : AirVRCameraBase {
    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_InitJNI();

    private static AirVRCamera _instance;

    [SerializeField] private bool _preferRealWorldSpace = false;

    private Transform _trackingSpace;
    private AirVRLeftHandTrackerInputDevice _leftHandTracker;
    private AirVRRightHandTrackerInputDevice _rightHandTracker;
    private AirVRProfile _profile;

    public AirVRRealWorldSpace realWorldSpace { get; private set; }
    public override Matrix4x4 trackingSpaceToWorldMatrix => _trackingSpace.localToWorldMatrix;

    protected override void Awake () {
        // Work around : Only the game module can access to java classes in onAirVR client plugin.
        if (Application.isEditor == false && Application.platform == RuntimePlatform.Android) {
            ocs_InitJNI();
        }
        
        base.Awake();
        _profile = new AirVRProfile();
        _trackingSpace = transform.parent;
    }

    protected override void Start() {
        base.Start();

        _leftHandTracker = new AirVRLeftHandTrackerInputDevice();
        _rightHandTracker = new AirVRRightHandTrackerInputDevice();

        AirVRInputManager.RegisterInputDevice(_leftHandTracker);
        AirVRInputManager.RegisterInputDevice(_rightHandTracker);
        AirVRInputManager.RegisterInputDevice(new AirVRControllerInputDevice());

        gameObject.AddComponent<AirVRLeftHandTrackerPointer>().Configure(defaultLeftControllerModel, true);
        gameObject.AddComponent<AirVRRightHandTrackerPointer>().Configure(defaultRightControllerModel, true);

        if (_preferRealWorldSpace && 
            AirVROVRInputHelper.GetHeadsetType() == AirVROVRInputHelper.HeadsetType.Quest) {
            realWorldSpace = new AirVRRealWorldSpace(this);

            headTracker.setRealWorldSpace(realWorldSpace);
            _leftHandTracker.setRealWorldSpace(realWorldSpace);
            _rightHandTracker.setRealWorldSpace(realWorldSpace);
        }
    }

    private void Update() {
        if (realWorldSpace != null) {
            realWorldSpace.Update();
        }
    }

    public override AirVRProfileBase profile => _profile;

    protected override void RecenterPose() {
        OVRManager.display.RecenterPose();
    }
}
