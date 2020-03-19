using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRRightHandTrackerPointer : AirVRPointer {
    // implements AirVRPointerBase
    protected override string inputDeviceName => AirVRInputDeviceName.RightHandTracker;

    protected override byte raycastHitResultKey => (byte)AirVRRightHandTrackerKey.RaycastHitResult;

    protected override Vector3 worldOriginPosition {
        get {
            return trackingOriginLocalToWorldMatrix.MultiplyPoint(
                OVRInput.GetLocalControllerPosition(AirVROVRInputHelper.ParseController(OVRInput.Controller.RTouch))
            );
        }
    }

    protected override Quaternion worldOriginOrientation {
        get {
            return cameraRoot.rotation * OVRInput.GetLocalControllerRotation(AirVROVRInputHelper.ParseController(OVRInput.Controller.RTouch));
        }
    }
}
