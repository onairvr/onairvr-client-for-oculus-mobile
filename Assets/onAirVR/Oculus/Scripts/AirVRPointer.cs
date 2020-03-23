/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public abstract class AirVRPointer : AirVRPointerBase {
    private Transform _trackingSpace;
    private Matrix4x4 _trackingLocalToWorldMatrix;
    private Vibration _vibration;

    protected Transform cameraRoot { get; private set; }

    protected abstract OVRInput.Controller ovrController { get; }

    protected override void recalculatePointerRoot() {
        OVRCameraRig cameraRig = FindObjectOfType<OVRCameraRig>();
        _trackingSpace = cameraRig != null ? cameraRig.trackingSpace : Camera.main.transform;
        cameraRoot = cameraRig != null ? cameraRig.transform : _trackingSpace;
        _trackingLocalToWorldMatrix = Matrix4x4.identity;
    }
        
    protected override Matrix4x4 trackingOriginLocalToWorldMatrix {
        get {
            if (cameraRoot != null && _trackingSpace != null) {
                _trackingLocalToWorldMatrix.SetTRS(_trackingSpace.position, cameraRoot.rotation, cameraRoot.localScale);
                return _trackingLocalToWorldMatrix;
            }
            return Matrix4x4.identity;
        }
    }

    protected override void Awake() {
        base.Awake();

        _vibration = new Vibration(ovrController);
    }

    private void Update() {
        _vibration.Update();
    }

    protected override void SetVibration(AirVRHapticVibration vibration) {
        _vibration.Start(vibration);
    }

    private class Vibration {
        private OVRInput.Controller _controller;
        private float _remainingToEnd = -1.0f;

        public Vibration(OVRInput.Controller controller) {
            _controller = controller;
        }

        public void Start(AirVRHapticVibration vibration) {
            var duration = parseDuration(vibration);
            if (duration <= 0.0f) { return; }

            if (_remainingToEnd <= 0.0f) {
                OVRInput.SetControllerVibration(parseFrequency(vibration), parseAmplitude(vibration), _controller);
            }
            _remainingToEnd = duration;
        }

        public void Update() {
            if (_remainingToEnd <= 0.0f) { return; }

            _remainingToEnd -= Time.deltaTime;
            if (_remainingToEnd <= 0.0f) {
                OVRInput.SetControllerVibration(0.0f, 0.0f, _controller);
            }
        }

        private float parseFrequency(AirVRHapticVibration vibration) {
            return 1.0f;
        }

        private float parseAmplitude(AirVRHapticVibration vibration) {
            return 1.0f;
        }

        private float parseDuration(AirVRHapticVibration vibration) {
            switch (vibration) {
                case AirVRHapticVibration.OneTime_Short:
                    return 0.25f;
                case AirVRHapticVibration.OneTime_Long:
                    return 1.0f;
                default:
                    return 0.0f;
            }
        }
    }
}
