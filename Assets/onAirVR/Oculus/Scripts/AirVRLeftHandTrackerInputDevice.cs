/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;

public class AirVRLeftHandTrackerInputDevice : AirVRTrackerInputDevice {
    // implements AirVRInputDevice
    protected override string deviceName => AirVRInputDeviceName.LeftHandTracker;

    protected override bool connected => AirVROVRInputHelper.IsConnected(OVRInput.Controller.LTouch);

    protected override void PendInputs(AirVRInputStream inputStream) {
        var (position, rotation) = getPose();

        inputStream.PendTransform(this, (byte)AirVRLeftHandTrackerKey.Transform, position, rotation);
    }

    private (Vector3 position, Quaternion rotation) getPose() {
        const OVRInput.Controller controller = OVRInput.Controller.LTouch;

        var position = OVRInput.GetLocalControllerPosition(controller);
        var rotation = OVRInput.GetLocalControllerRotation(controller);

        if (realWorldSpace != null) {
            var trackingSpaceToRealWorldMatrix = (realWorldSpace as AirVRRealWorldSpace).trackingSpaceToRealWorldMatrix;

            return (
                trackingSpaceToRealWorldMatrix.MultiplyPoint(position),
                trackingSpaceToRealWorldMatrix.rotation * rotation
            );
        }
        else {
            return (position, rotation);
        }
    }
}
