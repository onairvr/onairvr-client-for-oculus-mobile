/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;

public class AirVRLeftHandTrackerInputDevice : AirVRTrackerInputDevice {
    private AirVRDeviceStatus currentStatus {
        get {
            return AirVROVRInputHelper.IsConnected(OVRInput.Controller.LTouch) ? AirVRDeviceStatus.Ready : AirVRDeviceStatus.Unavailable;
        }
    }

    private Pose currentPose {
        get {
            const OVRInput.Controller controller = OVRInput.Controller.LTouch;

            var position = OVRInput.GetLocalControllerPosition(controller);
            var rotation = OVRInput.GetLocalControllerRotation(controller);

            if (realWorldSpace != null) {
                var trackingSpaceToRealWorldMatrix = (realWorldSpace as AirVRRealWorldSpace).trackingSpaceToRealWorldMatrix;

                return new Pose(
                    trackingSpaceToRealWorldMatrix.MultiplyPoint(position),
                    trackingSpaceToRealWorldMatrix.rotation * rotation
                );
            }
            else {
                return new Pose(position, rotation);
            }
        }
    }

    // implements AirVRInputDevice
    public override byte id => (byte)AirVRInputDeviceID.LeftHandTracker;

    public override void PendInputsPerFrame(AirVRInputStream inputStream) {
        var pose = currentPose;

        inputStream.PendState(this, (byte)AirVRHandTrackerControl.Status, (byte)currentStatus);
        inputStream.PendPose(this, (byte)AirVRHandTrackerControl.Pose, pose.position, pose.rotation);
    }
}
