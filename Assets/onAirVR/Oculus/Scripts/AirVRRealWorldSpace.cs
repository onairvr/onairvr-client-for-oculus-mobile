/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;

public class AirVRRealWorldSpace : AirVRRealWorldSpaceBase {
    private const string KeySettings = "onairvr.realworldspace";

    private bool _shouldResetWorldOrigin = false;
    private Vector3 _requestedOriginInGameWorldSpace = Vector3.zero;
    private Vector3 _requestedFrontInGameWorldSpace = Vector3.forward;
    private Vector3 _originOnPlayArea;
    private Vector3 _frontOnPlayArea;
    private Matrix4x4 _realWorldToPlayAreaMatrix = Matrix4x4.identity;

    public Matrix4x4 trackingSpaceToRealWorldMatrix => realWorldToWorldMatrix.inverse * cameraRig.trackingSpaceToWorldMatrix;

    public AirVRRealWorldSpace(AirVRCamera cameraRig) : base(cameraRig) {
        load();

        _originOnPlayArea = _realWorldToPlayAreaMatrix.MultiplyPoint(Vector3.zero);
        _frontOnPlayArea = _realWorldToPlayAreaMatrix.MultiplyPoint(Vector3.forward);
    }

    public void SetRealWorldOrigin(Vector3 positionInGameWorldSpace) {
        _requestedOriginInGameWorldSpace = positionInGameWorldSpace;
        _shouldResetWorldOrigin = true;
    }

    public void SetRealWorldFront(Vector3 positionInGameWorldSpace) {
        _requestedFrontInGameWorldSpace = positionInGameWorldSpace;
        _shouldResetWorldOrigin = true;
    }

    protected override bool UpdateInputToResetWorldOrigin() {
        if (_shouldResetWorldOrigin) {
            _shouldResetWorldOrigin = false;
            return true;
        }
        return false;
    }

    protected override Matrix4x4 CalcRealWorldToWorldMatrix(bool resetRealWorldOrigin) {
        var playAreaToWorldMatrix = calcPlayAreaToWorldMatrix();

        if (resetRealWorldOrigin) {
            _realWorldToPlayAreaMatrix = calcRealWorldToPlayAreaMatrix(playAreaToWorldMatrix);

            save();
        }

        return playAreaToWorldMatrix * _realWorldToPlayAreaMatrix;
    }

    private Matrix4x4 calcPlayAreaToWorldMatrix() {
        var origin = Vector3.zero;

        var vertices = OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea);
        foreach (var vertice in vertices) {
            origin += cameraRig.trackingSpaceToWorldMatrix.MultiplyPoint(vertice);
        }
        origin /= vertices.Length;

        var front = cameraRig.trackingSpaceToWorldMatrix.MultiplyPoint(vertices[0]);
        front.y = origin.y;

        return Matrix4x4.TRS(
            origin,
            Quaternion.LookRotation(front - origin, Vector3.up),
            Vector3.one
        );
    }

    private Matrix4x4 calcRealWorldToPlayAreaMatrix(Matrix4x4 playAreaToWorldMatrix) {
        const float MaxHeightOfValidRequestedPosition = 0.1f;

        var requestedOriginOnPlayArea = playAreaToWorldMatrix.inverse.MultiplyPoint(_requestedOriginInGameWorldSpace);
        if (Mathf.Abs(requestedOriginOnPlayArea.y) < MaxHeightOfValidRequestedPosition) {
            _originOnPlayArea = requestedOriginOnPlayArea;
            _originOnPlayArea.y = 0;
        }

        var requestedFrontOnPlayArea = playAreaToWorldMatrix.inverse.MultiplyPoint(_requestedFrontInGameWorldSpace);
        if (Mathf.Abs(requestedFrontOnPlayArea.y) < MaxHeightOfValidRequestedPosition) {
            _frontOnPlayArea = requestedFrontOnPlayArea;
            _frontOnPlayArea.y = 0;
        }

        return Matrix4x4.TRS(
            _originOnPlayArea,
            Quaternion.LookRotation(_frontOnPlayArea - _originOnPlayArea, Vector3.up),
            Vector3.one
        );
    }

    private void load() {
        var matrix = Matrix4x4.identity;

        matrix.m00 = PlayerPrefs.GetFloat(KeySettings + ".m00", 1.0f);
        matrix.m01 = PlayerPrefs.GetFloat(KeySettings + ".m01", 0.0f);
        matrix.m02 = PlayerPrefs.GetFloat(KeySettings + ".m02", 0.0f);
        matrix.m03 = PlayerPrefs.GetFloat(KeySettings + ".m03", 0.0f);
        matrix.m10 = PlayerPrefs.GetFloat(KeySettings + ".m10", 0.0f);
        matrix.m11 = PlayerPrefs.GetFloat(KeySettings + ".m11", 1.0f);
        matrix.m12 = PlayerPrefs.GetFloat(KeySettings + ".m12", 0.0f);
        matrix.m13 = PlayerPrefs.GetFloat(KeySettings + ".m13", 0.0f);
        matrix.m20 = PlayerPrefs.GetFloat(KeySettings + ".m20", 0.0f);
        matrix.m21 = PlayerPrefs.GetFloat(KeySettings + ".m21", 0.0f);
        matrix.m22 = PlayerPrefs.GetFloat(KeySettings + ".m22", 1.0f);
        matrix.m23 = PlayerPrefs.GetFloat(KeySettings + ".m23", 0.0f);
        matrix.m30 = PlayerPrefs.GetFloat(KeySettings + ".m30", 0.0f);
        matrix.m31 = PlayerPrefs.GetFloat(KeySettings + ".m31", 0.0f);
        matrix.m32 = PlayerPrefs.GetFloat(KeySettings + ".m32", 0.0f);
        matrix.m33 = PlayerPrefs.GetFloat(KeySettings + ".m33", 1.0f);

        _realWorldToPlayAreaMatrix = matrix;
    }

    private void save() {
        PlayerPrefs.SetFloat(KeySettings + ".m00", _realWorldToPlayAreaMatrix.m00);
        PlayerPrefs.SetFloat(KeySettings + ".m01", _realWorldToPlayAreaMatrix.m01);
        PlayerPrefs.SetFloat(KeySettings + ".m02", _realWorldToPlayAreaMatrix.m02);
        PlayerPrefs.SetFloat(KeySettings + ".m03", _realWorldToPlayAreaMatrix.m03);
        PlayerPrefs.SetFloat(KeySettings + ".m10", _realWorldToPlayAreaMatrix.m10);
        PlayerPrefs.SetFloat(KeySettings + ".m11", _realWorldToPlayAreaMatrix.m11);
        PlayerPrefs.SetFloat(KeySettings + ".m12", _realWorldToPlayAreaMatrix.m12);
        PlayerPrefs.SetFloat(KeySettings + ".m13", _realWorldToPlayAreaMatrix.m13);
        PlayerPrefs.SetFloat(KeySettings + ".m20", _realWorldToPlayAreaMatrix.m20);
        PlayerPrefs.SetFloat(KeySettings + ".m21", _realWorldToPlayAreaMatrix.m21);
        PlayerPrefs.SetFloat(KeySettings + ".m22", _realWorldToPlayAreaMatrix.m22);
        PlayerPrefs.SetFloat(KeySettings + ".m23", _realWorldToPlayAreaMatrix.m23);
        PlayerPrefs.SetFloat(KeySettings + ".m30", _realWorldToPlayAreaMatrix.m30);
        PlayerPrefs.SetFloat(KeySettings + ".m31", _realWorldToPlayAreaMatrix.m31);
        PlayerPrefs.SetFloat(KeySettings + ".m32", _realWorldToPlayAreaMatrix.m32);
        PlayerPrefs.SetFloat(KeySettings + ".m33", _realWorldToPlayAreaMatrix.m33);

        PlayerPrefs.Save();
    }
}
