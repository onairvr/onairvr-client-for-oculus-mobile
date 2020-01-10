/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;

public abstract class AirVRRealWorldSpaceBase {
    protected AirVRCameraBase cameraRig { get; private set; }

    public Matrix4x4 realWorldToWorldMatrix { get; private set; }

    public AirVRRealWorldSpaceBase(AirVRCameraBase cameraRig) {
        this.cameraRig = cameraRig;
    }

    public void Update() {
        realWorldToWorldMatrix = CalcRealWorldToWorldMatrix(UpdateInputToResetWorldOrigin());
    }

    protected abstract bool UpdateInputToResetWorldOrigin();
    protected abstract Matrix4x4 CalcRealWorldToWorldMatrix(bool resetRealWorldOrigin);
}
