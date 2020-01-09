/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRTrackedControllerPointer : AirVRPointer {
	// implements AirVRPointerBase
    protected override string inputDeviceName {
        get {
            return AirVRInputDeviceName.RightHandTracker;
        }
    }

    protected override byte raycastHitResultKey {
        get {
            return (byte)AirVRRightHandTrackerKey.RaycastHitResult;
        }
    }

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
