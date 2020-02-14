/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[System.Serializable]
public abstract class AirVRProfileBase {
    private const int ProfilerMaskFrame = 0x01;
    private const int ProfilerMaskReport = 0x02;

    public enum RenderType {
        DirectOnTwoEyeTextures,
        UseSeperateVideoRenderTarget
    }

#pragma warning disable CS0414
    [SerializeField] private string UserID;
    [SerializeField] private int VideoBitrate;
    [SerializeField] private int Profilers;
    [SerializeField] private string ProfilerLogPostfix;
    [SerializeField] private string[] SupportedVideoCodecs;
    [SerializeField] private string[] SupportedAudioCodecs;
    [SerializeField] private int VideoWidth;
    [SerializeField] private int VideoHeight;
    [SerializeField] private float VideoFrameRate;
    [SerializeField] private float IPD;
    [SerializeField] private bool Stereoscopy;
    [SerializeField] private float[] LeftEyeCameraNearPlane;
    [SerializeField] private Vector3 EyeCenterPosition;

    [SerializeField] private int[] LeftEyeViewport;
    [SerializeField] private int[] RightEyeViewport;
    [SerializeField] private float[] VideoScale;
#pragma warning restore CS0414

    private string[] supportedVideoCodecs {
        get {
            bool supportAVC = false;
            bool supportHEVC = false;

            if (Application.isEditor == false && Application.platform == RuntimePlatform.Android) {
                AndroidJavaObject mediaCodecList = new AndroidJavaObject("android.media.MediaCodecList", 0);
                AndroidJavaObject[] mediaCodecInfos = mediaCodecList.Call<AndroidJavaObject[]>("getCodecInfos");
                foreach (AndroidJavaObject codecInfo in mediaCodecInfos) {
                    string[] types = codecInfo.Call<string[]>("getSupportedTypes");
                    foreach (string type in types) {
                        if (type.Equals("video/avc")) {
                            supportAVC = true;
                        } else if (type.Equals("video/hevc")) {
                            supportHEVC = this.supportHEVC;
                        }
                    }
                }
            } else {
                supportAVC = supportHEVC = true;
            }
            Assert.IsTrue(supportHEVC || supportAVC);
            string[] result = new string[(supportHEVC && supportAVC) ? 2 : 1];
            if (supportHEVC) {
                result[0] = "H265";
            }
            if (supportAVC) {
                result[result.Length - 1] = "H264";
            }

            return result;
        }
    }

    private string[] supportedAudioCodecs {
        get {
            return new string[] { "opus" };
        }
    }

    private float[] leftEyeCameraNearPlaneScaled {
        get {
            float[] result = leftEyeCameraNearPlane;
            float[] scale = videoScale;
            result[0] *= scale[0];
            result[1] *= scale[1];
            result[2] *= scale[0];
            result[3] *= scale[1];

            return result;
        }
    }

    public abstract (int width, int height) videoResolution { get; }
    public abstract float defaultVideoFrameRate { get; }
    public abstract bool stereoscopy { get; }
    public abstract float[] leftEyeCameraNearPlane { get; }
    public abstract Vector3 eyeCenterPosition { get; }
    public abstract float ipd { get; }
    public abstract bool hasInput { get; }

    public abstract RenderType renderType { get; }
    public abstract int[] leftEyeViewport { get; }
    public abstract int[] rightEyeViewport { get; }
    public abstract float[] videoScale { get; }   // ratio of the size of the whole video rendered to the size of the area visible to an eye camera

    public abstract bool isUserPresent { get; }
    public abstract float delayToResumePlayback { get; }

    public virtual float[] videoRenderMeshVertices {
        get {
            return new float[] {
                -0.5f,  0.5f, 0.0f,
                0.5f,  0.5f, 0.0f,
                -0.5f, -0.5f, 0.0f,
                0.5f, -0.5f, 0.0f
            };
        }
    }

    public virtual float[] videoRenderMeshTexCoords {
        get {
            return new float[] {
                0.0f, 1.0f,
                1.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 0.0f
            };
        }
    }

    public virtual int[] videoRenderMeshIndices {
        get {
            return new int[] {
                0, 1, 2, 2, 1, 3
            };
        }
    }

    public virtual bool supportHEVC {
        get {
            return true;
        }
    }

    public bool useSeperateVideoRenderTarget {
        get {
            return renderType == RenderType.UseSeperateVideoRenderTarget;
        }
    }

    public bool useSingleTextureForEyes {
        get {
            return renderType == RenderType.UseSeperateVideoRenderTarget;
        }
    }

    public string userID {
        get {
            return UserID;
        }
        set {
            UserID = value;
        }
    }

    public float videoFrameRate {
        get {
            return VideoFrameRate;
        }
        set {
            VideoFrameRate = value;
        }
    }

    public int videoBitrate {
        get {
            return VideoBitrate;
        }
        set {
            VideoBitrate = value;
        }
    }

    public string profiler {
        get {
            var profileFrame = (Profilers & ProfilerMaskFrame) != 0;
            var profileReport = (Profilers & ProfilerMaskReport) != 0;

            if (profileFrame && profileReport) {
                return "full";
            }
            else if (profileFrame) {
                return "frame";
            }
            else if (profileReport) {
                return "report";
            }
            else {
                return "";
            }
        }
        set {
            switch (value) {
                case "full":
                    Profilers = ProfilerMaskFrame | ProfilerMaskReport;
                    break;
                case "frame":
                    Profilers = ProfilerMaskFrame;
                    break;
                case "report":
                    Profilers = ProfilerMaskReport;
                    break;
                default:
                    Profilers = 0;
                    break;
            }
        }
    }

    public string profilerLogPostfix {
        get {
            return ProfilerLogPostfix;
        }
        set {
            ProfilerLogPostfix = value;
        }
    }

	public AirVRProfileBase GetSerializable() {
        var resolution = videoResolution;

		SupportedVideoCodecs = supportedVideoCodecs;
		SupportedAudioCodecs = supportedAudioCodecs;
		VideoWidth = resolution.width;
		VideoHeight = resolution.height;
		VideoFrameRate = videoFrameRate;
        IPD = ipd;
		Stereoscopy = stereoscopy;
		LeftEyeCameraNearPlane = leftEyeCameraNearPlane;
		EyeCenterPosition = eyeCenterPosition;

		LeftEyeViewport = leftEyeViewport;
		RightEyeViewport = rightEyeViewport;
		VideoScale = videoScale;

        if (VideoFrameRate <= 0.0f) {
            VideoFrameRate = defaultVideoFrameRate;
        }

        return this;
	}

    public override string ToString () {
        var resolution = videoResolution;

        return string.Format("[AirVRProfile]\n" +
                             "    videoWidth={0}\n" +
                             "    videoHeight={1}\n" +
                             "    videoFrameRate={2}\n" + 
                             "    videoScale=({3}, {4})\n" + 
                             "    render type={5}\n" +
                             "    leftEyeViewport=({6}, {7}, {8}, {9})\n" + 
                             "    rightEyeViewport=({10}, {11}, {12}, {13})\n" + 
                             "    leftEyeCameraNearPlane=({14}, {15}, {16}, {17})\n" +
                             "    eyeCenterPosition={18}\n" + 
                             "    ipd={19}\n" + 
                             "    stereoscopy={20}\n", 
                             resolution.width, 
                             resolution.height, 
                             videoFrameRate, 
                             videoScale[0], videoScale[1], 
                             renderType, 
                             leftEyeViewport[0], leftEyeViewport[1], leftEyeViewport[2], leftEyeViewport[3], 
                             rightEyeViewport[0], rightEyeViewport[1], rightEyeViewport[2], rightEyeViewport[3], 
                             leftEyeCameraNearPlane[0], leftEyeCameraNearPlane[1], leftEyeCameraNearPlane[2], leftEyeCameraNearPlane[3], 
                             eyeCenterPosition, 
                             ipd, 
                             stereoscopy);
    }
}
