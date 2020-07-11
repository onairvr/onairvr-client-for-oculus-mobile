/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Runtime.InteropServices;
using UnityEngine;

public class AirVRClientInputStream : AirVRInputStream {
    [DllImport(AirVRClient.LibPluginName)]
    private static extern bool ocs_GetInputState(byte device, byte control, ref byte state);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern bool ocs_GetInputRaycastHit(byte device, byte control, ref AirVRVector3D origin, ref AirVRVector3D hitPosition, ref AirVRVector3D hitNormal);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern bool ocs_GetInputVibration(byte device, byte control, ref float frequency, ref float amplitude);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_BeginPendInput(ref long timestamp);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_PendInputState(byte device, byte control, byte state);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_PendInputByteAxis(byte device, byte control, byte axis);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_PendInputAxis(byte device, byte control, float axis);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_PendInputAxis2D(byte device, byte control, AirVRVector2D axis2D);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_PendInputPose(byte device, byte control, AirVRVector3D position, AirVRVector4D rotation);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_SendPendingInputs(long timestamp);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_UpdateInputFrame();

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_ClearInput();

    // implements AirVRInputStreaming
    protected override float maxSendingRatePerSec { get { return 120.0f; } }

    protected override void BeginPendInputImpl(ref long timestamp) {
        ocs_BeginPendInput(ref timestamp);
    }

    protected override void PendStateImpl(byte device, byte control, byte state) {
        ocs_PendInputState(device, control, state);
    }

    protected override void PendByteAxisImpl(byte device, byte control, byte axis) {
        ocs_PendInputByteAxis(device, control, axis);
    }

    protected override void PendAxisImpl(byte device, byte control, float axis) {
        ocs_PendInputAxis(device, control, axis);
    }

    protected override void PendAxis2DImpl(byte device, byte control, Vector2 axis2D) {
        ocs_PendInputAxis2D(device, control, new AirVRVector2D(axis2D));
    }

    protected override void PendPoseImpl(byte device, byte control, Vector3 position, Quaternion rotation) {
        ocs_PendInputPose(device, control, new AirVRVector3D(position), new AirVRVector4D(rotation));
    }

    protected override void PendRaycastHitImpl(byte device, byte control, Vector3 origin, Vector3 hitPosition, Vector3 hitNormal) { }
    protected override void PendVibrationImpl(byte device, byte control, float frequency, float amplitude) { }
    protected override void PendTouch2DImpl(byte device, byte control, Vector2 position, byte state, bool active) { }

    protected override void SendPendingInputEventsImpl(long timestamp) {
        ocs_SendPendingInputs(timestamp);
    }

    protected override bool GetStateImpl(byte device, byte control, ref byte state) {
        return ocs_GetInputState(device, control, ref state);
    }

    protected override bool GetByteAxisImpl(byte device, byte control, ref byte axis) { return false; }
    protected override bool GetAxisImpl(byte device, byte control, ref float axis) { return false; }
    protected override bool GetAxis2DImpl(byte device, byte control, ref Vector2 axis2D) { return false; }
    protected override bool GetPoseImpl(byte device, byte control, ref Vector3 position, ref Quaternion rotation) { return false; }

    protected override bool GetRaycastHitImpl(byte device, byte control, ref Vector3 origin, ref Vector3 hitPosition, ref Vector3 hitNormal) {
        AirVRVector3D ori = new AirVRVector3D();
        AirVRVector3D pos = new AirVRVector3D();
        AirVRVector3D norm = new AirVRVector3D();

        if (ocs_GetInputRaycastHit(device, control, ref ori, ref pos, ref norm) == false) { return false; }

        origin = ori.toVector3();
        hitPosition = pos.toVector3();
        hitNormal = norm.toVector3();
        return true;
    }

    protected override bool GetVibrationImpl(byte device, byte control, ref float frequency, ref float amplitude) {
        return ocs_GetInputVibration(device, control, ref frequency, ref amplitude);
    }

    protected override bool GetTouch2DImpl(byte device, byte control, ref Vector2 position, ref byte state) { return false; }
    protected override bool IsActiveImpl(byte device, byte control) { return false; }
    protected override bool IsActiveImpl(byte device, byte control, AirVRInputDirection direction) { return false; }
    protected override bool GetActivatedImpl(byte device, byte control) { return false; }
    protected override bool GetActivatedImpl(byte device, byte control, AirVRInputDirection direction) { return false; }
    protected override bool GetDeactivatedImpl(byte device, byte control) { return false; }
    protected override bool GetDeactivatedImpl(byte device, byte control, AirVRInputDirection direction) { return false; }

    protected override void UpdateInputFrameImpl() {
        ocs_UpdateInputFrame();
    }

    protected override void ClearInputImpl() {
        ocs_ClearInput();
    }
}
