/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

public class AirVROVRInputHelper {
    public enum HeadsetType {
        Unknown,
        GearVR,
        Go,
        Quest
    }

    public static HeadsetType GetHeadsetType() {
        switch (OVRPlugin.GetSystemHeadsetType()) {
            case OVRPlugin.SystemHeadset.Oculus_Go:
                return HeadsetType.Go;
            case OVRPlugin.SystemHeadset.Oculus_Quest:
                return HeadsetType.Quest;
            case OVRPlugin.SystemHeadset.GearVR_R320:
            case OVRPlugin.SystemHeadset.GearVR_R321:
            case OVRPlugin.SystemHeadset.GearVR_R322:
            case OVRPlugin.SystemHeadset.GearVR_R323:
            case OVRPlugin.SystemHeadset.GearVR_R324:
            case OVRPlugin.SystemHeadset.GearVR_R325:
                return HeadsetType.GearVR;
            default:
                return HeadsetType.Unknown;
        }
    }

    public static OVRInput.Controller ParseController(OVRInput.Controller controller) {
        switch (GetHeadsetType()) {
            case HeadsetType.GearVR:
            case HeadsetType.Go:
                if (controller == OVRInput.Controller.LTouch) {
                    return OVRInput.Controller.LTrackedRemote;
                }
                else if (controller == OVRInput.Controller.RTouch) {
                    return OVRInput.Controller.RTrackedRemote;
                }
                break;
            case HeadsetType.Quest:
                if (controller == OVRInput.Controller.LTrackedRemote) {
                    return OVRInput.Controller.LTouch;
                }
                else if (controller == OVRInput.Controller.RTrackedRemote) {
                    return OVRInput.Controller.RTouch;
                }
                break;
        }
        return controller;
    }

    public static bool IsConnected(OVRInput.Controller controller) {
        var parsed = ParseController(controller);
        return (OVRInput.GetConnectedControllers() & parsed) == parsed;
    }
}
