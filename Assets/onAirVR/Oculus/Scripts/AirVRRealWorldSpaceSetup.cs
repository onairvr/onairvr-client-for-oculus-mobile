using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AirVRRealWorldSpaceSetup : MonoBehaviour {
    private const int MaxDistance = 50;
    private const float BoundaryLineWidth = 0.05f;
    private const float OriginIndicatorLineWidth = 0.025f;

    private Transform _thisTransform;
    private AirVRCamera _cameraRig;
    private Mesh _mesh;
    private Vector3[] _vertices;
    private Color[] _colors;
    private int[] _gridIndices;
    private int[] _boundaryIndices;
    private int[] _originIndicatorIndices;

    private int gridSize => MaxDistance * 2 + 1;
    private int gridVerticeCount => gridSize * 4;

    [SerializeField] private Color _colorGrid = new Color(0.357f, 0.357f, 0.357f);
    [SerializeField] private Color _colorBoundary = new Color(0.6f, 0.196f, 0.8f);
    [SerializeField] private Color _colorForward = new Color(0.255f, 0.412f, 0.882f);
    [SerializeField] private Color _colorRightward = new Color(0.698f, 0.133f, 0.133f);

    private void Awake() {
        _thisTransform = transform;

        _mesh = new Mesh();

        if (Application.isEditor) {
            return;
        }

        _cameraRig = FindObjectOfType<AirVRCamera>();
    }

    private void Update() {
        const OVRInput.Controller controller = OVRInput.Controller.RTouch;
        const OVRInput.RawButton trigger = OVRInput.RawButton.RIndexTrigger;
        const OVRInput.RawButton buttonOrigin = OVRInput.RawButton.A;
        const OVRInput.RawButton buttonFront = OVRInput.RawButton.B;

        ensureComponentIntegrity();
        updateMesh();

        if (OVRInput.GetDown(buttonOrigin, controller) && OVRInput.Get(trigger, controller)) {
            var position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            _cameraRig.realWorldSpace.SetRealWorldOrigin(
                _cameraRig.trackingSpaceToWorldMatrix.MultiplyPoint(position)
            );
        }
        else if (OVRInput.GetDown(buttonFront, controller) && OVRInput.Get(trigger, controller)) {
            var position = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            _cameraRig.realWorldSpace.SetRealWorldFront(
                _cameraRig.trackingSpaceToWorldMatrix.MultiplyPoint(position)
            );
        }
    }

    private void ensureComponentIntegrity() {
        var meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter == null) {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (meshFilter.sharedMesh != _mesh) {
            meshFilter.sharedMesh = _mesh;
        }
        var renderer = gameObject.GetComponent<MeshRenderer>();
        if (renderer == null) {
            renderer = gameObject.AddComponent<MeshRenderer>();
        }
        if (renderer.sharedMaterials == null || 
            renderer.sharedMaterials.Length < 2 ||
            renderer.sharedMaterials[0] == null) {
            var material = new Material(Shader.Find("onAirVR/Unlit world space"));
            material.hideFlags = HideFlags.HideAndDontSave;

            renderer.sharedMaterials = new Material[] { material, material, material };
        }
    }

    private void updateMesh() {
        _mesh.Clear();
        _mesh.subMeshCount = 3;

        var boundary = Application.isEditor ? new Vector3[] {
            new Vector3(-1, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, -1),
            new Vector3(-1, 0, -1)
        } : OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.OuterBoundary);

        if (_vertices == null || _vertices.Length != verticeCount(boundary)) {
            _vertices = new Vector3[verticeCount(boundary)];
            _colors = new Color[verticeCount(boundary)];
        }

        _mesh.vertices = _vertices;
        _mesh.colors = _colors;

        var offset = updateGridLines(_colorGrid);
        offset = updateBoundary(offset, boundary, _colorBoundary);
        updateOriginIndicator(offset, _colorForward, _colorRightward);
    }

    private int updateGridLines(Color color) {
        if (_gridIndices == null || _gridIndices.Length != gridVerticeCount) {
            _gridIndices = new int[gridVerticeCount];
        }

        for (int i = 0; i < gridSize; i++) {
            var first = i * 4;

            _vertices[first] = new Vector3(i - MaxDistance, 0, -MaxDistance);
            _vertices[first + 1] = new Vector3(i - MaxDistance, 0, MaxDistance);
            _vertices[first + 2] = new Vector3(-MaxDistance, 0, i - MaxDistance);
            _vertices[first + 3] = new Vector3(MaxDistance, 0, i - MaxDistance);

            _colors[first] = _colors[first + 1] = _colors[first + 2] = _colors[first + 3] = color;

            _gridIndices[first] = first;
            _gridIndices[first + 1] = first + 1;
            _gridIndices[first + 2] = first + 2;
            _gridIndices[first + 3] = first + 3;
        }

        _mesh.SetIndices(_gridIndices, MeshTopology.Lines, 0);
        return gridVerticeCount;
    }

    private int updateBoundary(int offset, Vector3[] boundary, Color color) {
        if (boundary == null || boundary.Length < 3) {
            return offset;
        }

        if (_boundaryIndices == null || _boundaryIndices.Length != boundary.Length * 6) {
            _boundaryIndices = new int[boundary.Length * 6];
        }

        for (int i = 0; i < boundary.Length; i++) {
            var current = calcEdgeVertices(
                boundaryVertex(boundary, i - 1),
                boundaryVertex(boundary, i),
                boundaryVertex(boundary, i + 1)
            );
            var next = calcEdgeVertices(
                boundaryVertex(boundary, i),
                boundaryVertex(boundary, i + 1),
                boundaryVertex(boundary, i + 2)
            );

            var first = i * 4;
            _vertices[offset + first] = current.outer;
            _vertices[offset + first + 1] = next.outer;

            _vertices[offset + first + 2] = current.inner;
            _vertices[offset + first + 3] = next.inner;

            _colors[offset + first] = _colors[offset + first + 1] = _colors[offset + first + 2] = _colors[offset + first + 3] = color;

            var firstIndex = i * 6;
            _boundaryIndices[firstIndex] = offset + first;
            _boundaryIndices[firstIndex + 1] = offset + first + 1;
            _boundaryIndices[firstIndex + 2] = offset + first + 2;
            _boundaryIndices[firstIndex + 3] = offset + first + 2;
            _boundaryIndices[firstIndex + 4] = offset + first + 1;
            _boundaryIndices[firstIndex + 5] = offset + first + 3;
        }

        _mesh.SetIndices(_boundaryIndices, MeshTopology.Triangles, 1);
        return offset + boundary.Length * 4;
    }
    
    private Vector3 boundaryVertex(Vector3[] boundary, int index) {
        var result = boundary[(index + boundary.Length) % boundary.Length];
        return _cameraRig != null ? _cameraRig.trackingSpaceToWorldMatrix.MultiplyPoint(result) : result;
    }

    private (Vector3 inner, Vector3 outer) calcEdgeVertices(Vector3 prev, Vector3 target, Vector3 next) {
        var backward = (prev - target).normalized;
        var forward = (next - target).normalized;

        var innerward = (backward + forward).normalized;
        if (Vector3.Cross(forward, innerward).y < 0.0f) {
            innerward = -innerward;
        }

        return (
            target + innerward * BoundaryLineWidth / 2,
            target - innerward.normalized * BoundaryLineWidth / 2
        );
    }

    private void updateOriginIndicator(int offset, Color colorForward, Color colorRight) {
        if (_originIndicatorIndices == null) {
            _originIndicatorIndices = new int[12];
        }

        var origin = Application.isEditor == false ? _cameraRig.realWorldSpace.realWorldToWorldMatrix.MultiplyPoint(Vector3.zero) : Vector3.zero;
        var front = Application.isEditor == false ? _cameraRig.realWorldSpace.realWorldToWorldMatrix.MultiplyPoint(Vector3.forward) : Vector3.forward;
        var right = Application.isEditor == false ? _cameraRig.realWorldSpace.realWorldToWorldMatrix.MultiplyPoint(Vector3.right * 0.5f) : Vector3.right * 0.5f;

        var rightward = (right - origin).normalized;
        var forward = (front - origin).normalized;

        _vertices[offset] = front - rightward * OriginIndicatorLineWidth / 2;
        _vertices[offset + 1] = front + rightward * OriginIndicatorLineWidth / 2;
        _vertices[offset + 2] = origin - (forward + rightward) * OriginIndicatorLineWidth / 2;
        _vertices[offset + 3] = origin + (forward + rightward) * OriginIndicatorLineWidth / 2;
        _vertices[offset + 4] = origin - (forward + rightward) * OriginIndicatorLineWidth / 2;
        _vertices[offset + 5] = origin + (forward + rightward) * OriginIndicatorLineWidth / 2;
        _vertices[offset + 6] = right - forward * OriginIndicatorLineWidth / 2;
        _vertices[offset + 7] = right + forward * OriginIndicatorLineWidth / 2;

        _colors[offset] = _colors[offset + 1] = _colors[offset + 2] = _colors[offset + 3] = colorForward;
        _colors[offset + 4] = _colors[offset + 5] = _colors[offset + 6] = _colors[offset + 7] = colorRight;

        _originIndicatorIndices[0] = offset;
        _originIndicatorIndices[1] = offset + 1;
        _originIndicatorIndices[2] = offset + 2;
        _originIndicatorIndices[3] = offset + 2;
        _originIndicatorIndices[4] = offset + 1;
        _originIndicatorIndices[5] = offset + 3;
        _originIndicatorIndices[6] = offset + 4;
        _originIndicatorIndices[7] = offset + 5;
        _originIndicatorIndices[8] = offset + 6;
        _originIndicatorIndices[9] = offset + 6;
        _originIndicatorIndices[10] = offset + 5;
        _originIndicatorIndices[11] = offset + 7;

        _mesh.SetIndices(_originIndicatorIndices, MeshTopology.Triangles, 2);
    }

    private int verticeCount(Vector3[] boundary) {
        return gridVerticeCount + 
            (boundary != null ? boundary.Length * 4 : 0) +      // boundary
            8;                                                  // origin indicator
    }
}
