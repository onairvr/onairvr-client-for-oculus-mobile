/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using System.Runtime.InteropServices;

public abstract class AirVRCameraBase : MonoBehaviour {
    private Transform _thisTransform;
    private Camera _camera;
    private int _viewNumber;
    private RenderCommand _renderCommand;
    private bool _renderingRight;
    private bool _aboutToDestroy;
    private Vector2 _savedCameraClipPlanes;

    [SerializeField] private bool _enableAudio = true;
    [SerializeField] private AudioMixerGroup _audioMixerGroup;
    [SerializeField] private AirVRProfileBase.VideoBitrate _videoBitrate = AirVRProfileBase.VideoBitrate.Normal;
    [SerializeField] private ControllerOverlay _controllerOverlay = ControllerOverlay.Default;
    [SerializeField] private GameObject _leftControllerModel = null;
    [SerializeField] private GameObject _rightControllerModel = null;
    [SerializeField] private bool _enablePointer = true;
    [SerializeField] private Color _colorLaser = Color.white;
    [SerializeField] private Texture2D _pointerCookie = null;
    [SerializeField] private float _pointerCookieDepthScaleMultiplier = 0.015f;

    protected AirVRProfileBase.VideoBitrate videoBitrate => _videoBitrate;
    protected HeadTrackerInputDevice headTracker { get; private set; }

    protected GameObject leftControllerModel {
        get {
            return _controllerOverlay == ControllerOverlay.Custom ? _leftControllerModel : Resources.Load<GameObject>("LeftControllerModel");
        }
    }
    protected GameObject rightControllerModel {
        get {
            return _controllerOverlay == ControllerOverlay.Custom ? _rightControllerModel : Resources.Load<GameObject>("RightControllerModel");
        }
    }

    protected AirVRTrackerFeedbackBase.PointerDesc pointerDesc {
        get {
            return new AirVRTrackerFeedbackBase.PointerDesc {
                enabled = _controllerOverlay == ControllerOverlay.Custom ? _enablePointer : true,
                colorLaser = _controllerOverlay == ControllerOverlay.Custom ? _colorLaser : Color.white,
                cookie = _controllerOverlay == ControllerOverlay.Custom ? _pointerCookie : Resources.Load<Texture2D>("PointerCookie"),
                cookieDepthScaleMultiplier = _controllerOverlay == ControllerOverlay.Custom ? _pointerCookieDepthScaleMultiplier : 0.015f
            };
        }
    }

    protected abstract void RecenterPose();

    public abstract AirVRProfileBase profile { get; }
    public abstract Matrix4x4 trackingSpaceToWorldMatrix { get; }

    protected virtual void Awake() {
        _thisTransform = transform;
        _camera = gameObject.GetComponent<Camera>();

        if (_enableAudio) {
            GameObject go = new GameObject("AirVRAudioSource");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            AudioSource audioSource = go.AddComponent<AudioSource>();
            audioSource.loop = false;

            if (_audioMixerGroup != null) {
                audioSource.outputAudioMixerGroup = _audioMixerGroup;

                // workaround for applying audio mixer group change
                audioSource.Stop();
                audioSource.Play();
            }

            go.AddComponent<AirVRClientAudioSource>();
        }
        headTracker = new HeadTrackerInputDevice(_thisTransform);
    }

    protected virtual void Start() {
        _renderCommand = RenderCommand.Create(profile, _camera);

        AirVRClient.LoadOnce(profile, this);
        AirVRInputManager.LoadOnce();

		AirVRClient.MessageReceived += onAirVRMesageReceived;

        AirVRInputManager.RegisterInputSender(headTracker);

        StartCoroutine(CallEndOfFrame());

        saveCameraClipPlanes(); // workaround for the very first disconnected event
    }

    private void OnPreRender() {
        if (Application.isEditor) {
            return;
        }

		if (AirVRClient.playing) {
            if (_renderingRight == false) {
                var pose = headTracker.currentPose;

                ocs_SetCameraOrientation(new AirVRVector4D(pose.rotation), ref _viewNumber);
                GL.IssuePluginEvent(ocs_PreRenderVideoFrame_RenderThread_Func(), _viewNumber);
            }

            // clear color the texture only for the right eye when using single texture for the two eyes
            _renderCommand.Issue(ocs_RenderVideoFrame_RenderThread_Func(),
                                 renderEvent(_renderingRight ? FrameType.StereoRight : FrameType.StereoLeft, 
                                             profile.useSingleTextureForEyes == false || _renderingRight == false));
        }

        _renderingRight = true;
    }

    private void OnPostRender() {
        _renderCommand.Clear();
    }

    private void OnDestroy() {
        _aboutToDestroy = true;

		AirVRClient.MessageReceived -= onAirVRMesageReceived;
    }

    private IEnumerator CallEndOfFrame() {
        while (_aboutToDestroy == false) {
            yield return null;

            _renderingRight = false;

#if !UNITY_EDITOR && UNITY_ANDROID
            GL.IssuePluginEvent(ocs_EndOfRenderFrame_RenderThread_Func(), 0);
#endif
        }
    }

    private void onAirVRMesageReceived(AirVRClientMessage message) {
        if (message.IsSessionEvent()) {
            if (message.Name.Equals(AirVRClientMessage.NameConnected)) {
                saveCameraClipPlanes();
                
                ocs_EnableNetworkTimeWarp(true);
            }
            else if (message.Name.Equals(AirVRClientMessage.NameDisconnected)) {
                restoreCameraClipPlanes();
            }
        }
        else if (message.IsMediaStreamEvent()) {
            if (message.Name.Equals(AirVRClientMessage.NameCameraClipPlanes)) {
                setCameraClipPlanes(message.NearClip, message.FarClip);
            }
            else if (message.Name.Equals(AirVRClientMessage.NameEnableNetworkTimeWarp)) {
                ocs_EnableNetworkTimeWarp(message.Enable);
            }
        }
        else if (message.IsInputStreamEvent() && message.Name.Equals(AirVRClientMessage.NameRecenterPose)) {
            RecenterPose();
        }
	}

    private int renderEvent(FrameType frameType, bool clearColor) {
        return (int)(((int)frameType << 24) + (clearColor ? RenderEventMaskClearColor : 0));
    }

    private void saveCameraClipPlanes() {
        _savedCameraClipPlanes.x = _camera.nearClipPlane;
        _savedCameraClipPlanes.y = _camera.farClipPlane;
    }

    private void restoreCameraClipPlanes() {
        _camera.nearClipPlane = _savedCameraClipPlanes.x;
        _camera.farClipPlane = _savedCameraClipPlanes.y;
    }

    private void setCameraClipPlanes(float nearClip, float farClip) {
        _camera.nearClipPlane = Mathf.Min(nearClip, _camera.nearClipPlane);
        _camera.farClipPlane = Mathf.Max(farClip, _camera.farClipPlane);
    }

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_EnableNetworkTimeWarp(bool enable);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern void ocs_SetCameraOrientation(AirVRVector4D rotation, ref int viewNumber);

    [DllImport(AirVRClient.LibPluginName)]
    private static extern System.IntPtr ocs_PreRenderVideoFrame_RenderThread_Func();

    [DllImport(AirVRClient.LibPluginName)]
    private static extern System.IntPtr ocs_RenderVideoFrame_RenderThread_Func();

    [DllImport(AirVRClient.LibPluginName)]
    private static extern System.IntPtr ocs_EndOfRenderFrame_RenderThread_Func();

    private const uint RenderEventMaskClearColor = 0x00800000U;

    private enum FrameType {
        StereoLeft = 0,
        StereoRight,
        Mono
    }

    private enum ControllerOverlay {
        Default,
        Custom
    }

    private abstract class RenderCommand {
        public static RenderCommand Create(AirVRProfileBase profile, Camera camera) {
            return profile.useSeperateVideoRenderTarget ? new RenderCommandImmediate() as RenderCommand :
                                                          new RenderCommandOnCameraEvent(camera, CameraEvent.BeforeForwardOpaque) as RenderCommand;
        }

        public abstract void Issue(System.IntPtr renderFuncPtr, int arg);
        public abstract void Clear();
    }

    private class RenderCommandOnCameraEvent : RenderCommand {
        public RenderCommandOnCameraEvent(Camera camera, CameraEvent cameraEvent) {
            _commandBuffer = new CommandBuffer();
            camera.AddCommandBuffer(cameraEvent, _commandBuffer);
        }

        public override void Issue(System.IntPtr renderFuncPtr, int arg) {
            _commandBuffer.IssuePluginEvent(renderFuncPtr, arg);
        }

        public override void Clear() {
            _commandBuffer.Clear();
        }

        private CommandBuffer _commandBuffer;
    }

    private class RenderCommandImmediate : RenderCommand {
        public override void Issue(System.IntPtr renderFuncPtr, int arg) {
            GL.IssuePluginEvent(renderFuncPtr, arg);
        }

        public override void Clear() { }
    }

    protected class HeadTrackerInputDevice : AirVRTrackerInputDevice {
        private Transform _head;

        public Pose currentPose {
            get {
                if (realWorldSpace != null) {
                    var worldToRealWorldMatrix = realWorldSpace.realWorldToWorldMatrix.inverse;

                    return new Pose(
                        worldToRealWorldMatrix.MultiplyPoint(_head.position),
                        worldToRealWorldMatrix.rotation * _head.rotation
                    );
                }
                else {
                    return new Pose(_head.localPosition, _head.localRotation);
                }
            }
        }

        public HeadTrackerInputDevice(Transform head) {
            _head = head;
        }

        // implements AirVRTrackerInputDevice
        public override byte id => (byte)AirVRInputDeviceID.HeadTracker;

        public override void PendInputsPerFrame(AirVRInputStream inputStream) {
            var pose = currentPose;

            inputStream.PendPose(this, (byte)AirVRHeadTrackerControl.Pose, pose.position, pose.rotation);
        }
    }
}
