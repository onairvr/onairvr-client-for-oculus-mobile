/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

public abstract class AirVRTrackerInputDevice : AirVRInputSender {
    public bool usingRealWorldSpace => realWorldSpace != null;

    public void setRealWorldSpace(AirVRRealWorldSpaceBase realWorldSpace) {
        this.realWorldSpace = realWorldSpace;
    }

    public void clearRealWorldSpace() {
        realWorldSpace = null;
    }

    protected AirVRRealWorldSpaceBase realWorldSpace { get; private set; }
}
