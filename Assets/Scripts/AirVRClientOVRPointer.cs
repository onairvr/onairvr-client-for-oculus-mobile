/***********************************************************

  Copyright (c) 2017-2018 Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class AirVRClientOVRPointer : MonoBehaviour
{
    [Header("OVR Input Module")]
    [SerializeField] private OVRInputModule _inputModule;
    [SerializeField] private OVRGazePointer _gazePointer;

    [Header("OVR Rig Transforms")]
    [SerializeField] private Transform _trackingSpace = null;
    [SerializeField] private Transform _centerEyeAnchor = null;
    [SerializeField] private Transform _leftHandAnchor = null;
    [SerializeField] private Transform _rightHandAnchor = null;

    [Header("Selection Ray")]
    public bool ShowSelectionRay = true;
    [SerializeField] private LineRenderer _lineRenderer = null;
    [SerializeField] private float _maxRayLength = 0.5f;

    private Transform _rayTransform;

    public bool isControllerConnected {
        get {
            return AirVROVRInputHelper.IsConnected(OVRInput.Controller.LTouch) || 
                AirVROVRInputHelper.IsConnected(OVRInput.Controller.RTouch);
        }
    }

    public OVRInput.Controller controller {
        get {
            OVRInput.Controller controllers = OVRInput.GetConnectedControllers();
            var right = AirVROVRInputHelper.ParseController(OVRInput.Controller.RTouch);
            if (AirVROVRInputHelper.IsConnected(right)) {
                return right;
            }
            var left = AirVROVRInputHelper.ParseController(OVRInput.Controller.LTouch);
            if (AirVROVRInputHelper.IsConnected(left)) {
                return left;
            }
            return OVRInput.GetActiveController();
        }
    }

    private void Awake() {
        _rayTransform = _centerEyeAnchor;       
    }

    private void Update() {
        if (AirVRClient.connected) {
            if (_lineRenderer != null) {
                _lineRenderer.enabled = false;
            }
            return;
        }

        Transform newRayTransform;
        if (isControllerConnected) {
            if (controller == AirVROVRInputHelper.ParseController(OVRInput.Controller.RTouch)) {
                newRayTransform = _rightHandAnchor;
            }
            else if (controller == AirVROVRInputHelper.ParseController(OVRInput.Controller.LTouch)) {
                newRayTransform = _leftHandAnchor;
            }
            else {
                newRayTransform = _centerEyeAnchor;
            }
        }
        else {
            newRayTransform = _centerEyeAnchor;
        }

        if (_rayTransform != newRayTransform) {
            _rayTransform = newRayTransform;

            _gazePointer.rayTransform = _rayTransform;
            _inputModule.rayTransform = _rayTransform;
        }

        if (_lineRenderer != null) {
            _lineRenderer.enabled = _rayTransform != _centerEyeAnchor && ShowSelectionRay;

            if (_lineRenderer.enabled) {
                Vector3 worldStartPoint = Vector3.zero;
                Vector3 worldEndPoint = Vector3.zero;

                if (_trackingSpace != null) {
                    Matrix4x4 localToWorld = _trackingSpace.localToWorldMatrix;
                    Quaternion orientation = OVRInput.GetLocalControllerRotation(controller);

                    Vector3 localStartPoint = OVRInput.GetLocalControllerPosition(controller);
                    if (AirVROVRInputHelper.GetHeadsetType() == AirVROVRInputHelper.HeadsetType.Quest) {
                        localStartPoint += orientation * Vector3.down * 0.01f;
                    }
                    Vector3 localEndPoint = localStartPoint + orientation * Vector3.forward * _maxRayLength;

                    worldStartPoint = localToWorld.MultiplyPoint(localStartPoint);
                    worldEndPoint = localToWorld.MultiplyPoint(localEndPoint);
                }

                _lineRenderer.SetPosition(0, worldStartPoint);
                _lineRenderer.SetPosition(1, worldEndPoint);
            }
        }
    }
}
