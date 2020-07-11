/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;

public class AirVRControllerInputDevice : AirVRInputSender {
    // implements AirVRInputSender
    public override byte id => (byte)AirVRInputDeviceID.Controller;

    public override void PendInputsPerFrame(AirVRInputStream inputStream) {
        const OVRInput.Controller ltouch = OVRInput.Controller.LTouch;
        const OVRInput.Controller rtouch = OVRInput.Controller.RTouch;
        const OVRInput.Controller lremote = OVRInput.Controller.LTrackedRemote;
        const OVRInput.Controller rremote = OVRInput.Controller.RTrackedRemote;

        inputStream.PendAxis2D(this, (byte)AirVRControllerControl.Axis2DLThumbstick, combineAxes(OVRInput.Get(OVRInput.RawAxis2D.LThumbstick, ltouch),
                                                                                                 OVRInput.Get(OVRInput.RawAxis2D.LTouchpad, lremote)));
        inputStream.PendAxis2D(this, (byte)AirVRControllerControl.Axis2DRThumbstick, combineAxes(OVRInput.Get(OVRInput.RawAxis2D.RThumbstick, rtouch),
                                                                                                 OVRInput.Get(OVRInput.RawAxis2D.RTouchpad, rremote)));
        inputStream.PendAxis(this, (byte)AirVRControllerControl.AxisLIndexTrigger, combineAxes(OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger, ltouch),
                                                                                               OVRInput.Get(OVRInput.RawButton.LIndexTrigger, lremote) ? 1 : 0));
        inputStream.PendAxis(this, (byte)AirVRControllerControl.AxisRIndexTrigger, combineAxes(OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger, rtouch),
                                                                                               OVRInput.Get(OVRInput.RawButton.RIndexTrigger, rremote) ? 1 : 0));
        inputStream.PendAxis(this, (byte)AirVRControllerControl.AxisLHandTrigger, OVRInput.Get(OVRInput.RawAxis1D.LHandTrigger, ltouch));
        inputStream.PendAxis(this, (byte)AirVRControllerControl.AxisRHandTrigger, OVRInput.Get(OVRInput.RawAxis1D.RHandTrigger, rtouch));
        inputStream.PendByteAxis(this, (byte)AirVRControllerControl.ButtonA, getByteAxis(OVRInput.Get(OVRInput.RawButton.A, rtouch) ||
                                                                                         OVRInput.Get(OVRInput.RawButton.RTouchpad, rremote)));
        inputStream.PendByteAxis(this, (byte)AirVRControllerControl.ButtonB, getByteAxis(OVRInput.Get(OVRInput.RawButton.B, rtouch)));
        inputStream.PendByteAxis(this, (byte)AirVRControllerControl.ButtonX, getByteAxis(OVRInput.Get(OVRInput.RawButton.X, ltouch) ||
                                                                                         OVRInput.Get(OVRInput.RawButton.LTouchpad, lremote)));
        inputStream.PendByteAxis(this, (byte)AirVRControllerControl.ButtonY, getByteAxis(OVRInput.Get(OVRInput.RawButton.Y, ltouch)));
        inputStream.PendByteAxis(this, (byte)AirVRControllerControl.ButtonStart, getByteAxis(OVRInput.Get(OVRInput.RawButton.Start, ltouch)));
        inputStream.PendByteAxis(this, (byte)AirVRControllerControl.ButtonBack, getByteAxis(OVRInput.Get(OVRInput.RawButton.Back, lremote | rremote)));
        inputStream.PendByteAxis(this, (byte)AirVRControllerControl.ButtonLThumbstick, getByteAxis(OVRInput.Get(OVRInput.RawButton.LThumbstick, ltouch) ||
                                                                                                   OVRInput.Get(OVRInput.RawButton.LTouchpad, lremote)));
        inputStream.PendByteAxis(this, (byte)AirVRControllerControl.ButtonRThumbstick, getByteAxis(OVRInput.Get(OVRInput.RawButton.RThumbstick, rtouch) ||
                                                                                                   OVRInput.Get(OVRInput.RawButton.RTouchpad, rremote)));
    }

    private byte getByteAxis(bool button) {
        return button ? (byte)1 : (byte)0;
    }

    private Vector2 combineAxes(params Vector2[] axes) {
        Vector2 sum = Vector2.zero;
        foreach (var axis in axes) {
            sum += axis;
        }
        return new Vector2(
            Mathf.Clamp(sum.x, -1.0f, 1.0f),
            Mathf.Clamp(sum.y, -1.0f, 1.0f)
        );
    }

    private float combineAxes(params float[] axes) {
        var sum = 0.0f;
        foreach (var axis in axes) {
            sum += axis;
        }
        return Mathf.Clamp(sum, 0.0f, 1.0f);
    }
}
