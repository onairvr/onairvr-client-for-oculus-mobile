using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AirVRTrackerInputDevice : AirVRInputDevice {
    public bool usingRealWorldSpace => realWorldSpace != null;

    public void setRealWorldSpace(AirVRRealWorldSpaceBase realWorldSpace) {
        this.realWorldSpace = realWorldSpace;
    }

    public void clearRealWorldSpace() {
        realWorldSpace = null;
    }

    protected AirVRRealWorldSpaceBase realWorldSpace { get; private set; }
}
