/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class AirVRClientInputModule : OVRInputModule
{
    protected override PointerEventData.FramePressState GetGazeButtonState()
    {
        var pressed = OVRInput.GetDown(OVRInput.Button.One) || 
            OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
            OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) ||
            Input.GetKeyDown(gazeClickKey);
        var released = OVRInput.GetUp(OVRInput.Button.One) || 
            OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger) ||
            OVRInput.GetUp(OVRInput.Button.SecondaryIndexTrigger) ||
            Input.GetKeyUp(gazeClickKey);
        
        if (pressed && released)
            return PointerEventData.FramePressState.PressedAndReleased;
        if (pressed)
            return PointerEventData.FramePressState.Pressed;
        if (released)
            return PointerEventData.FramePressState.Released;
        return PointerEventData.FramePressState.NotChanged;
    }
}
