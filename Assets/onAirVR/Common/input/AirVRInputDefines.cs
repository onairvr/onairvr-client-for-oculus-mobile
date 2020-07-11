/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct AirVRVector2D {
    public float x;
    public float y;

    public AirVRVector2D(Vector2 value) {
        x = value.x;
        y = value.y;
    }

    public Vector3 toVector2() {
        return new Vector2(x, y);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct AirVRVector3D {
    public float x;
    public float y;
    public float z;

    public AirVRVector3D(Vector3 value) {
        x = value.x;
        y = value.y;
        z = value.z;
    }

    public Vector3 toVector3() {
        return new Vector3(x, y, z);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct AirVRVector4D {
    public float x;
    public float y;
    public float z;
    public float w;

    public AirVRVector4D(Quaternion value) {
        x = value.x;
        y = value.y;
        z = value.z;
        w = value.w;
    }

    public Quaternion toQuaternion() {
        return new Quaternion(x, y, z, w);
    }
}

public enum AirVRInputDeviceID : byte {
    HeadTracker = 0,
    LeftHandTracker = 1,
    RightHandTracker = 2,
    Controller = 3,
    TouchScreen = 4
}

public enum AirVRInputDirection : byte {
    Up = 0,
    Down,
    Left,
    Right
}

public enum AirVRDeviceStatus : byte {
    Unavailable = 0,
    Ready
}

public enum AirVRHeadTrackerControl : byte {
    Pose = 0
}

public enum AirVRHandTrackerControl : byte {
    Status = 0,
    Pose
}

public enum AirVRHandTrackerFeedbackControl : byte {
    RenderOnClient = 0,
    RaycastHit,
    Vibration
}

public enum AirVRControllerControl : byte {
    Axis2DLThumbstick = 0,
    Axis2DRThumbstick,
    AxisLIndexTrigger,
    AxisRIndexTrigger,
    AxisLHandTrigger,
    AxisRHandTrigger,
    ButtonA,
    ButtonB,
    ButtonX,
    ButtonY,
    ButtonStart,
    ButtonBack,
    ButtonLThumbstick,
    ButtonRThumbstick
}

public enum AirVRTouchScreenControl : byte {
    TouchIndexStart = 0,
    TouchIndexEnd = 9
}

public enum AirVRTouchPhase {
    Ended = 0,
    Canceled,
    Stationary,
    Moved
}
