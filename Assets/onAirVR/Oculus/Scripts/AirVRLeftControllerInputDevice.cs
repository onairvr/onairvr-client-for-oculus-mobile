using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRLeftControllerInputDevice : AirVRInputDevice {
    protected override string deviceName {
        get {
            return AirVRInputDeviceName.LeftController;
        }
    }

    protected override bool connected {
        get {
            return (OVRInput.GetConnectedControllers() & OVRInput.Controller.LTouch) == OVRInput.Controller.LTouch;
        }
    }

    protected override void PendInputs(AirVRInputStream inputStream) {
        var controller = OVRInput.Controller.LTouch;

        inputStream.PendTransform(this, (byte)AirVRControllerKey.Transform, OVRInput.GetLocalControllerPosition(controller), OVRInput.GetLocalControllerRotation(controller));
        inputStream.PendAxis2D(this, (byte)AirVRControllerKey.Axis2DThumbstick, OVRInput.Get(OVRInput.RawAxis2D.LThumbstick));
        inputStream.PendAxis(this, (byte)AirVRControllerKey.Axis1DIndexTrigger, OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger));
        inputStream.PendAxis(this, (byte)AirVRControllerKey.Axis1DHandTrigger, OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonOne, OVRInput.Get(OVRInput.RawButton.X));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonTwo, OVRInput.Get(OVRInput.RawButton.Y));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonSystem, OVRInput.Get(OVRInput.RawButton.Start));
    }
}
