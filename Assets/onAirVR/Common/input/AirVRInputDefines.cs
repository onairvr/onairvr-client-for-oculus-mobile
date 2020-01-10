/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

public static class AirVRInputDeviceName {
    public const string HeadTracker = "HeadTracker";
    public const string LeftHandTracker = "LeftHandTracker";
    public const string RightHandTracker = "RightHandTracker";
    public const string Controller = "Controller";
}

public enum AirVRHeadTrackerKey {
    Transform = 0,
    RaycastHitResult,

    // ADD ADDITIONAL KEYS HERE

    Max
}

public enum AirVRLeftHandTrackerKey {
    Transform = 0,
    RaycastHitResult,

    // ADD ADDITIONAL KEYS HERE

    Max
}

public enum AirVRRightHandTrackerKey {
    Transform = 0,
    RaycastHitResult,

    // ADD ADDITIONAL KEYS HERE

    Max
}

public enum AirVRControllerKey {
    Touchpad = 0,

    ButtonTouchpad,
    ButtonUp,
    ButtonDown,
    ButtonLeft,
    ButtonRight,

    Axis2DLThumbstick,
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
    ButtonRThumbstick,
    ButtonLShoulder,
    ButtonRShoulder,

    // ADD ADDITIONAL KEYS HERE

    ExtAxis2DTouchpad,
    ExtTouchTouchpad,
    ExtButtonLIndexTrigger,
    ExtButtonRIndexTrigger,
    ExtButtonLHandTrigger,
    ExtButtonRHandTrigger,
    ExtButtonLThumbstickUp,
    ExtButtonLThumbstickDown,
    ExtButtonLThumbstickLeft,
    ExtButtonLThumbstickRight,
    ExtButtonRThumbstickUp,
    ExtButtonRThumbstickDown,
    ExtButtonRThumbstickLeft,
    ExtButtonRThumbstickRight,

    Max
}
