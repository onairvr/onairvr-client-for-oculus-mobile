/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;

public abstract class AirVRTrackerFeedbackBase : MonoBehaviour {
    public struct PointerDesc {
        public bool enabled;
        public Color colorLaser;
        public Texture2D cookie;
        public float cookieDepthScaleMultiplier;
    } 

    private const float DefaultRayLength = 0.15f;
    private const float MaxRayLength = 1.5f;
    private const float RatioOfRayLengthToHit = 0.3f;
    private static float[] RayPositions = { 0.0f, 0.1f, 1.0f };
    private static GradientAlphaKey[] RayAlphaKeys = {
        new GradientAlphaKey(0.0f, 0.0f),
        new GradientAlphaKey(0.8f, 0.1f),
        new GradientAlphaKey(0.0f, 1.0f)
    };

    private Transform _cookie;
    private MeshRenderer _cookieRenderer;
    private PointerDesc _pointerDesc = new PointerDesc();
    private LineRenderer _ray;
    private Transform _body;
    private bool _shouldRenderVisuals;
    private Vector3 _rayOrigin;
    private Vector3 _raycastHitPosition;
    private Vector3 _raycastHitNormal;

    private FilteredPose _worldOriginPose = new FilteredPose();
    private FixedRateTimer _vibrationTimer = new FixedRateTimer();

    private bool shouldRenderVisuals {
        get { return _shouldRenderVisuals; }
        set {
            _shouldRenderVisuals = value;
            updateVisuals();
        }
    }

    protected abstract AirVRInputDeviceID srcDevice { get; }
    protected abstract bool srcDeviceConnected { get; }
    protected abstract void recalculatePointerRoot();
    protected abstract Matrix4x4 trackingOriginLocalToWorldMatrix { get; }
    protected abstract Vector3 worldOriginPosition { get; }
    protected abstract Quaternion worldOriginOrientation { get; }

    protected virtual Vector3 worldBodyPosition => worldOriginPosition;
    protected virtual Quaternion worldBodyOrientation => worldOriginOrientation;

    protected abstract void SetVibration(float frequency, float amplitude);

    public void Configure(AirVRProfileBase profile, GameObject bodyModelPrefab, PointerDesc pointerDesc) {
        _pointerDesc = pointerDesc;

        if (bodyModelPrefab != null) {
            _body = Instantiate(bodyModelPrefab, Vector3.zero, Quaternion.identity).transform;
        }
        if (_pointerDesc.enabled) {
            Material mat = new Material(Shader.Find("onAirVR/Unlit vertex color"));

            _ray = _body.gameObject.AddComponent<LineRenderer>();
            _ray.positionCount = RayPositions.Length;
            _ray.receiveShadows = false;
            _ray.material = mat;
            _ray.useWorldSpace = true;
            _ray.loop = false;
            _ray.widthCurve = new AnimationCurve(new Keyframe[] {
                new Keyframe(0.0f, 0.005f),
                new Keyframe(1.0f, 0.005f),
            });
            Gradient colorCurve = new Gradient();
            colorCurve.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(pointerDesc.colorLaser, 0.0f),
                    new GradientColorKey(pointerDesc.colorLaser, 1.0f)
                },
                RayAlphaKeys
            );
            _ray.colorGradient = colorCurve;
            _ray.enabled = false;
        }

        _vibrationTimer.Set(profile.videoFrameRate);
    }
    protected abstract void OnStart();
    protected abstract void OnUpdate();

    private void Start() {
        if (Application.isEditor) { return; }

        createCookie(Shader.Find("onAirVR/Unlit alpha blended"));
        recalculatePointerRoot();

        AirVRClient.MessageReceived += onAirVRMessageReceived;

        OnStart();
    }

    private void LateUpdate() {
        if (Application.isEditor) { return; }

        shouldRenderVisuals = srcDeviceConnected && AirVRInputManager.inputStream.GetState((byte)srcDevice, (byte)AirVRHandTrackerFeedbackControl.RenderOnClient) != 0;
        if (shouldRenderVisuals) {
            AirVRInputManager.inputStream.GetRaycastHit((byte)srcDevice,
                                                        (byte)AirVRHandTrackerFeedbackControl.RaycastHit,
                                                        ref _rayOrigin,
                                                        ref _raycastHitPosition,
                                                        ref _raycastHitNormal);
            updateVisuals();
        }

        updateVibration();

        OnUpdate();
    }

    private void OnDestroy() {
        if (Application.isEditor) { return; }

        AirVRClient.MessageReceived -= onAirVRMessageReceived;
    }

    private void onAirVRMessageReceived(AirVRClientMessage message) {
        if (message.IsSessionEvent() && message.Name.Equals(AirVRClientMessage.NameDisconnected)) {
            _cookieRenderer.enabled = false;
            shouldRenderVisuals = false;
        }
    }

    private void createCookie(Shader cookieShader) {
        if (_pointerDesc.enabled == false) { return; }

        GameObject goCookie = new GameObject("Cookie");
        goCookie.transform.localPosition = Vector3.zero;
        goCookie.transform.localRotation = Quaternion.identity;
        goCookie.transform.localScale = Vector3.one;

        MeshFilter meshFilter = goCookie.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.vertices = new Vector3[] {
            new Vector3(-0.5f, 0.5f, 0.0f),
            new Vector3(0.5f, 0.5f, 0.0f),
            new Vector3(-0.5f, -0.5f, 0.0f),
            new Vector3(0.5f, -0.5f, 0.0f)
        };
        meshFilter.mesh.uv = new Vector2[] {
            new Vector2(1.0f, 1.0f),
            new Vector2(0.0f, 1.0f),
            new Vector2(1.0f, 0.0f),
            new Vector2(0.0f, 0.0f)
        };
        meshFilter.mesh.triangles = new int[] {
            1, 0, 3, 3, 0, 2
        };
        meshFilter.mesh.UploadMeshData(true);

        _cookie = goCookie.transform;
        _cookieRenderer = goCookie.AddComponent<MeshRenderer>();
        _cookieRenderer.material = new Material(cookieShader);
        _cookieRenderer.material.mainTexture = _pointerDesc.cookie;

        shouldRenderVisuals = false;
    }

    private bool isRaycastHitMissed() {
        return _raycastHitNormal == Vector3.zero;
    }

    private void updateVisuals() {
        bool shouldShowPointer = _pointerDesc.enabled && shouldRenderVisuals && AirVRClient.playing;

        if (shouldShowPointer) {
            _worldOriginPose.Update(worldOriginPosition, worldOriginOrientation);
        }
        else {
            _worldOriginPose.Reset();
        }

        updateCookie(shouldShowPointer);
        updateRay(shouldShowPointer);
        updateBody(shouldShowPointer);
    }

    private void updateCookie(bool pointerVisible) {
        _cookieRenderer.enabled = pointerVisible && (isRaycastHitMissed() == false);
        if (_cookieRenderer.enabled && _cookieRenderer.material.mainTexture != null) {
            var hitPosition = trackingOriginLocalToWorldMatrix.MultiplyPoint(_raycastHitPosition);
            var origin = trackingOriginLocalToWorldMatrix.MultiplyPoint(_rayOrigin);
            var distance = (hitPosition - origin).magnitude;

            Quaternion rotation = _cookie.rotation;
            rotation.SetLookRotation(trackingOriginLocalToWorldMatrix.MultiplyVector(_raycastHitNormal),
                _worldOriginPose.rotation * Vector3.up);

            _cookie.position = _worldOriginPose.position + _worldOriginPose.rotation * (distance * Vector3.forward);
            _cookie.rotation = rotation;
            _cookie.localScale = Vector3.one * distance * _pointerDesc.cookieDepthScaleMultiplier;
        }
    }

    private void setRayPositions(Vector3 start, Vector3 end, float[] positions) {
        Debug.Assert(_ray != null);

        _ray.SetPosition(0, start);
        for (int i = 0; i < positions.Length; i++) {
            _ray.SetPosition(i, Vector3.Lerp(start, end, positions[i]));
        }
    }

    private void updateRay(bool pointerVisible) {
        if (_ray != null) {
            _ray.enabled = pointerVisible;
            if (_ray.enabled) {
                if (_cookieRenderer.enabled) {
                    Ray ray = new Ray(_worldOriginPose.position, _cookie.position - _worldOriginPose.position);
                    setRayPositions(_worldOriginPose.position,
                                    ray.GetPoint(RatioOfRayLengthToHit * Mathf.Min((_cookie.position - _worldOriginPose.position).magnitude, MaxRayLength)),
                                    RayPositions);
                }
                else {
                    setRayPositions(_worldOriginPose.position,
                                    _worldOriginPose.position + _worldOriginPose.rotation * (Vector3.forward * DefaultRayLength),
                                    RayPositions);
                }
            }
        }
    }

    private void updateBody(bool pointerVisible) {
        if (_body != null) {
            _body.gameObject.SetActive(pointerVisible);

            if (pointerVisible) {
                Vector3 posOffset = worldBodyPosition - worldOriginPosition;
                Quaternion rotOffset = worldBodyOrientation * Quaternion.Inverse(worldOriginOrientation);

                _body.position = _worldOriginPose.position + posOffset;
                _body.rotation = _worldOriginPose.rotation * rotOffset;
            }
        }
    }

    private void updateVibration() {
        _vibrationTimer.UpdatePerFrame();

        if (_vibrationTimer.expired) {
            float frequency = 0, amplitude = 0;
            if (AirVRInputManager.inputStream.GetVibrationFrame((byte)srcDevice, (byte)AirVRHandTrackerFeedbackControl.Vibration, ref frequency, ref amplitude)) {
                SetVibration(frequency, amplitude);
            }
        }
    }

    private class FilteredPose {
        private const float FilterCoef = 0.6f;

        private bool _bypassFilterOnNextUpdate = true;

        public Vector3 position { get; private set; }
        public Quaternion rotation { get; private set; }

        public void Update(Vector3 pos, Quaternion rot) {
            if (_bypassFilterOnNextUpdate) {
                position = pos;
                rotation = rot;

                _bypassFilterOnNextUpdate = false;
            }
            else {
                position = Vector3.Lerp(position, pos, FilterCoef);
                rotation = Quaternion.Slerp(rotation, rot, FilterCoef);
            }
        }

        public void Reset() {
            _bypassFilterOnNextUpdate = true;
        }
    }
}
