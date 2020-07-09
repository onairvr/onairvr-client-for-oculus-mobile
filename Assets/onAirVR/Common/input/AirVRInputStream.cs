/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections.Generic;
using UnityEngine;

public abstract class AirVRInputBase {
    public abstract byte id { get; }
}

public abstract class AirVRInputSender : AirVRInputBase {
    public abstract void PendInputsPerFrame(AirVRInputStream inputStream);
}

public abstract class AirVRInputStream {
    private bool _streaming = false;
    private FixedRateTimer _timer;

    protected Dictionary<byte, AirVRInputSender> senders { get; private set; }

    public AirVRInputStream() {
        _timer = new FixedRateTimer();

        senders = new Dictionary<byte, AirVRInputSender>();
    }

    public void RegisterInputSender(AirVRInputSender sender) {
        senders[sender.id] = sender;
    }

    public void UnregisterInputSender(AirVRInputSender sender) {
        senders.Remove(sender.id);
    }

    public void Init() {
        _timer.Set(maxSendingRatePerSec);
    }

    public void Start() {
        _streaming = true;
    }

    public void Stop() {
        _streaming = false;

        ClearInputImpl();
    }

    public void Cleanup() {
        _streaming = false;
        _timer.Reset();

        ClearInputImpl();
    }

    public void UpdateInputFrame() {
        UpdateInputFrameImpl();
    }

    public void UpdateSenders() {
        if (_streaming == false) { return; }

        _timer.UpdatePerFrame();

        if (_timer.expired) {
            long timestamp = 0;
            BeginPendInputImpl(ref timestamp);

            foreach (var key in senders.Keys) {
                senders[key].PendInputsPerFrame(this);
            }
            SendPendingInputEventsImpl(timestamp);
        }
    }

    public void PendState(AirVRInputSender sender, byte control, byte state) {
        PendStateImpl(sender.id, control, state);
    }

    public void PendByteAxis(AirVRInputSender sender, byte control, byte axis) {
        PendByteAxisImpl(sender.id, control, axis);
    }

    public void PendAxis(AirVRInputSender sender, byte control, float axis) {
        PendAxisImpl(sender.id, control, axis);
    }

    public void PendAxis2D(AirVRInputSender sender, byte control, Vector2 axis2D) {
        PendAxis2DImpl(sender.id, control, axis2D);
    }

    public void PendPose(AirVRInputSender sender, byte control, Vector3 position, Quaternion rotation) {
        PendPoseImpl(sender.id, control, position, rotation);
    }

    public void PendRaycastHit(AirVRInputSender sender, byte control, Vector3 origin, Vector3 hitPosition, Vector3 hitNormal) {
        PendRaycastHitImpl(sender.id, control, origin, hitPosition, hitNormal);
    }

    public void PendVibration(byte device, byte control, float frequency, float amplitude) {
        PendVibrationImpl(device, control, frequency, amplitude);
    }

    public void PendTouch2D(byte device, byte control, Vector2 position, byte state, bool active) {

    }

    public byte GetState(byte device, byte control) {
        byte state = 0;
        return GetStateImpl(device, control, ref state) ? state : (byte)0;
    }

    public byte GetByteAxis(byte device, byte control) {
        byte axis = 0;
        return GetByteAxisImpl(device, control, ref axis) ? axis : (byte)0;
    }

    public float GetAxis(byte device, byte control) {
        float axis = 0;
        return GetAxisImpl(device, control, ref axis) ? axis : 0.0f;
    }

    public Vector2 GetAxis2D(byte device, byte control) {
        var axis2D = Vector2.zero;
        return GetAxis2DImpl(device, control, ref axis2D) ? axis2D : Vector2.zero;
    }

    public Pose GetPose(byte device, byte control) {
        var position = Vector3.zero;
        var rotation = Quaternion.identity;

        return GetPoseImpl(device, control, ref position, ref rotation) ? new Pose(position, rotation) : Pose.identity;
    }

    public void GetRaycastHit(byte device, byte control, ref Vector3 origin, ref Vector3 hitPosition, ref Vector3 hitNormal) {
        if (GetRaycastHitImpl(device, control, ref origin, ref hitPosition, ref hitNormal) == false) {
            origin = hitPosition = hitNormal = Vector3.zero;
        }
    }

    public bool GetVibrationFrame(byte device, byte control, ref float frequency, ref float amplitude) {
        return GetVibrationImpl(device, control, ref frequency, ref amplitude);
    }

    public bool GetTouch2D(byte device, byte control, ref Vector2 position, ref byte state) {
        return GetTouch2DImpl(device, control, ref position, ref state);
    }

    public bool IsActive(byte device, byte control) {
        return IsActiveImpl(device, control);
    }

    public bool IsActive(byte device, byte control, AirVRInputDirection direction) {
        return IsActiveImpl(device, control, direction);
    }

    public bool GetActivated(byte device, byte control) {
        return GetActivatedImpl(device, control);
    }

    public bool GetActivated(byte device, byte control, AirVRInputDirection direction) {
        return GetActivatedImpl(device, control, direction);
    }

    public bool GetDeactivated(byte device, byte control) {
        return GetDeactivatedImpl(device, control);
    }

    public bool GetDeactivated(byte device, byte control, AirVRInputDirection direction) {
        return GetDeactivatedImpl(device, control, direction);
    }

    // abstract methods
    protected abstract float maxSendingRatePerSec { get; }

    protected abstract void BeginPendInputImpl(ref long timestamp);
    protected abstract void PendStateImpl(byte device, byte control, byte state);
    protected abstract void PendByteAxisImpl(byte device, byte control, byte axis);
    protected abstract void PendAxisImpl(byte device, byte control, float axis);
    protected abstract void PendAxis2DImpl(byte device, byte control, Vector2 axis2D);
    protected abstract void PendPoseImpl(byte device, byte control, Vector3 position, Quaternion rotation);
    protected abstract void PendRaycastHitImpl(byte device, byte control, Vector3 origin, Vector3 hitPosition, Vector3 hitNormal);
    protected abstract void PendVibrationImpl(byte device, byte control, float frequency, float amplitude);
    protected abstract void PendTouch2DImpl(byte device, byte control, Vector2 position, byte state, bool active);
    protected abstract void SendPendingInputEventsImpl(long timestamp);

    protected abstract bool GetStateImpl(byte device, byte control, ref byte state);
    protected abstract bool GetByteAxisImpl(byte device, byte control, ref byte axis);
    protected abstract bool GetAxisImpl(byte device, byte control, ref float axis);
    protected abstract bool GetAxis2DImpl(byte device, byte control, ref Vector2 axis2D);
    protected abstract bool GetPoseImpl(byte device, byte control, ref Vector3 position, ref Quaternion rotation);
    protected abstract bool GetRaycastHitImpl(byte device, byte control, ref Vector3 origin, ref Vector3 hitPosition, ref Vector3 hitNormal);
    protected abstract bool GetVibrationImpl(byte device, byte control, ref float frequency, ref float amplitude);
    protected abstract bool GetTouch2DImpl(byte device, byte control, ref Vector2 position, ref byte state);
    protected abstract bool IsActiveImpl(byte device, byte control);
    protected abstract bool IsActiveImpl(byte device, byte control, AirVRInputDirection direction);
    protected abstract bool GetActivatedImpl(byte device, byte control);
    protected abstract bool GetActivatedImpl(byte device, byte control, AirVRInputDirection direction);
    protected abstract bool GetDeactivatedImpl(byte device, byte control);
    protected abstract bool GetDeactivatedImpl(byte device, byte control, AirVRInputDirection direction);

    protected abstract void UpdateInputFrameImpl();
    protected abstract void ClearInputImpl();
}
