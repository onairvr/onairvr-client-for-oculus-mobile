using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRLeftHandTrackerInputDevice : AirVRInputDevice {
    // implements AirVRInputDevice
    protected override string deviceName => AirVRInputDeviceName.LeftHandTracker;

    protected override bool connected => AirVROVRInputHelper.IsConnected(OVRInput.Controller.LTouch);

    protected override void PendInputs(AirVRInputStream inputStream) {
        var controller = AirVROVRInputHelper.ParseController(OVRInput.Controller.LTouch);

        inputStream.PendTransform(this, (byte)AirVRLeftHandTrackerKey.Transform, OVRInput.GetLocalControllerPosition(controller), OVRInput.GetLocalControllerRotation(controller));
    }
}
