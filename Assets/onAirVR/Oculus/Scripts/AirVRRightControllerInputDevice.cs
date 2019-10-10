using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRRightControllerInputDevice : AirVRInputDevice {
    protected override string deviceName {
        get {
            return AirVRInputDeviceName.RightController;
        }
    }

    protected override bool connected {
        get {
            return (OVRInput.GetConnectedControllers() & OVRInput.Controller.RTouch) == OVRInput.Controller.RTouch;
        }
    }

    protected override void PendInputs(AirVRInputStream inputStream) {
        var controller = OVRInput.Controller.RTouch;

        inputStream.PendTransform(this, (byte)AirVRControllerKey.Transform, OVRInput.GetLocalControllerPosition(controller), OVRInput.GetLocalControllerRotation(controller));
        inputStream.PendAxis2D(this, (byte)AirVRControllerKey.Axis2DThumbstick, OVRInput.Get(OVRInput.RawAxis2D.RThumbstick));
        inputStream.PendAxis(this, (byte)AirVRControllerKey.Axis1DIndexTrigger, OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger));
        inputStream.PendAxis(this, (byte)AirVRControllerKey.Axis1DHandTrigger, OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonOne, OVRInput.Get(OVRInput.RawButton.A));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonTwo, OVRInput.Get(OVRInput.RawButton.B));
    }
}
