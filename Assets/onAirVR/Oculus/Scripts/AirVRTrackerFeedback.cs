/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;

public abstract class AirVRTrackerFeedback : AirVRTrackerFeedbackBase {
    private Transform _trackingSpace;
    private Matrix4x4 _trackingLocalToWorldMatrix;

    protected Transform cameraRoot { get; private set; }

    protected abstract OVRInput.Controller ovrController { get; }

    protected override bool srcDeviceConnected => OVRInput.IsControllerConnected(ovrController);

    protected override void recalculatePointerRoot() {
        OVRCameraRig cameraRig = FindObjectOfType<OVRCameraRig>();
        _trackingSpace = cameraRig != null ? cameraRig.trackingSpace : Camera.main.transform;
        cameraRoot = cameraRig != null ? cameraRig.transform : _trackingSpace;
        _trackingLocalToWorldMatrix = Matrix4x4.identity;
    }

    protected override Matrix4x4 trackingOriginLocalToWorldMatrix {
        get {
            if (cameraRoot != null && _trackingSpace != null) {
                _trackingLocalToWorldMatrix.SetTRS(_trackingSpace.position, cameraRoot.rotation, cameraRoot.localScale);
                return _trackingLocalToWorldMatrix;
            }
            return Matrix4x4.identity;
        }
    }

    protected override void SetVibration(float frequency, float amplitude) {
        OVRInput.SetControllerVibration(Mathf.Clamp(frequency, 0, 1.0f), Mathf.Clamp(amplitude, 0, 1.0f), ovrController);
    }

    protected override void OnStart() { }
    protected override void OnUpdate() { }
}
