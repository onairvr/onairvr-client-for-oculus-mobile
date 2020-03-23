using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRLeftHandTrackerPointer : AirVRPointer {
    // implements AirVRPointerBase
    protected override string inputDeviceName => AirVRInputDeviceName.LeftHandTracker;
    protected override OVRInput.Controller ovrController => AirVROVRInputHelper.ParseController(OVRInput.Controller.LTouch);
    protected override byte renderOnClientKey => (byte)AirVRLeftHandTrackerKey.RenderOnClient;
    protected override byte raycastHitResultKey => (byte)AirVRLeftHandTrackerKey.RaycastHitResult;
    protected override byte vibrateKey => (byte)AirVRLeftHandTrackerKey.RaycastHitResult;

    protected override Vector3 worldOriginPosition {
        get {
            return trackingOriginLocalToWorldMatrix.MultiplyPoint(
                OVRInput.GetLocalControllerPosition(AirVROVRInputHelper.ParseController(OVRInput.Controller.LTouch))
            );
        }
    }

    protected override Quaternion worldOriginOrientation {
        get {
            return cameraRoot.rotation * OVRInput.GetLocalControllerRotation(AirVROVRInputHelper.ParseController(OVRInput.Controller.LTouch));
        }
    }
}
