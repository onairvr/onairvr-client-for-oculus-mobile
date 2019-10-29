using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirVRControllerInputDevice : AirVRInputDevice {
    // implements AirVRInputDevice
    protected override string deviceName => AirVRInputDeviceName.Controller;

    protected override bool connected => true; // assumes that at least one kind of controller is available at any time.

    protected override void PendInputs(AirVRInputStream inputStream) {
        var touchpad = OVRInput.Controller.Touchpad;
        var gamepad = OVRInput.Controller.Gamepad;
        var trackedRemotes = OVRInput.Controller.LTrackedRemote | OVRInput.Controller.RTrackedRemote;
        var lTouch = OVRInput.Controller.LTouch;
        var rTouch = OVRInput.Controller.RTouch;

        inputStream.PendTouch(this, (byte)AirVRControllerKey.Touchpad, OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad), OVRInput.Get(OVRInput.Touch.PrimaryTouchpad));

        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonTouchpad, OVRInput.Get(OVRInput.Button.PrimaryTouchpad));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonUp, OVRInput.Get(OVRInput.RawButton.DpadUp, touchpad | gamepad));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonDown, OVRInput.Get(OVRInput.RawButton.DpadDown, touchpad | gamepad));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonLeft, OVRInput.Get(OVRInput.RawButton.DpadLeft, touchpad | gamepad));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonRight, OVRInput.Get(OVRInput.RawButton.DpadRight, touchpad | gamepad));

        inputStream.PendAxis2D(this, (byte)AirVRControllerKey.Axis2DLThumbstick, OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, gamepad | lTouch));
        inputStream.PendAxis2D(this, (byte)AirVRControllerKey.Axis2DRThumbstick, OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, gamepad | rTouch));
        inputStream.PendAxis(this, (byte)AirVRControllerKey.AxisLIndexTrigger, combineAxes(OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger, gamepad | lTouch),
                                                                                           OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, trackedRemotes) ? 1.0f : 0.0f));
        inputStream.PendAxis(this, (byte)AirVRControllerKey.AxisRIndexTrigger, combineAxes(OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger, gamepad | rTouch),
                                                                                           OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, trackedRemotes) ? 1.0f : 0.0f));
        inputStream.PendAxis(this, (byte)AirVRControllerKey.AxisLHandTrigger, OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger, lTouch));
        inputStream.PendAxis(this, (byte)AirVRControllerKey.AxisRHandTrigger, OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger, rTouch));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonA, OVRInput.Get(OVRInput.RawButton.A, gamepad | rTouch) ||
                                                                       OVRInput.Get(OVRInput.RawButton.LTouchpad, OVRInput.Controller.LTrackedRemote) ||
                                                                       OVRInput.Get(OVRInput.RawButton.RTouchpad, OVRInput.Controller.RTrackedRemote));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonB, OVRInput.Get(OVRInput.RawButton.B, gamepad | rTouch) ||
                                                                       OVRInput.Get(OVRInput.RawButton.Back, trackedRemotes));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonX, OVRInput.Get(OVRInput.RawButton.X, gamepad | lTouch));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonY, OVRInput.Get(OVRInput.RawButton.Y, gamepad | lTouch));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonStart, OVRInput.Get(OVRInput.RawButton.Start, gamepad | lTouch));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonBack, Input.GetKey(KeyCode.Escape) ||
                                                                          OVRInput.Get(OVRInput.Button.Back));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonLThumbstick, OVRInput.Get(OVRInput.RawButton.LThumbstick, gamepad | lTouch));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonRThumbstick, OVRInput.Get(OVRInput.RawButton.RThumbstick, gamepad | rTouch));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonLShoulder, OVRInput.Get(OVRInput.RawButton.LShoulder, gamepad));
        inputStream.PendButton(this, (byte)AirVRControllerKey.ButtonRShoulder, OVRInput.Get(OVRInput.RawButton.RShoulder, gamepad));
    }

    private float combineAxes(params float[] axes) {
        var sum = 0.0f;
        foreach (var axis in axes) {
            sum += axis;
        }
        return Mathf.Clamp(sum, 0.0f, 1.0f);
    }

    private Vector2 combineAxis2Ds(params Vector2[] axes) {
        var sum = Vector2.zero;
        foreach (var axis in axes) {
            sum += axis;
        }
        return new Vector2(
            Mathf.Clamp(sum.x, -1.0f, 1.0f),
            Mathf.Clamp(sum.y, -1.0f, 1.0f)
        );
    }
}
