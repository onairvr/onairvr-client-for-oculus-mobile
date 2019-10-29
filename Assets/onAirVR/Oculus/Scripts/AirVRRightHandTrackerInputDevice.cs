using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRRightHandTrackerInputDevice : AirVRInputDevice {
    // implements AirVRInputDevice
    protected override string deviceName => AirVRInputDeviceName.RightHandTracker;

    protected override bool connected => AirVROVRInputHelper.IsConnected(OVRInput.Controller.RTouch);

    protected override void PendInputs(AirVRInputStream inputStream) {
        var controller = AirVROVRInputHelper.ParseController(OVRInput.Controller.RTouch);

        inputStream.PendTransform(this, (byte)AirVRRightHandTrackerKey.Transform, OVRInput.GetLocalControllerPosition(controller), OVRInput.GetLocalControllerRotation(controller));
    }
}
