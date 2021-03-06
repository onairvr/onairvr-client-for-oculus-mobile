﻿/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;

public class AirVRProfile : AirVRProfileBase {
    public AirVRProfile(VideoBitrate bitrate) : base(bitrate) {}

	private bool _userPresent;

    public override (int width, int height) videoResolution {
        get {
            return AirVROVRInputHelper.GetHeadsetType() == AirVROVRInputHelper.HeadsetType.Quest ? (3200, 1600) : (2560, 1280);
        }
    }

    public override float defaultVideoFrameRate {
        get {
#if !UNITY_EDITOR && UNITY_ANDROID
            AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject activity = jc.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject display = activity.Call<AndroidJavaObject>("getSystemService", "window").Call<AndroidJavaObject>("getDefaultDisplay");
            return display.Call<float>("getRefreshRate");
#else
            return 59.2f;
#endif
        }
    }

	public override bool stereoscopy { 
		get {
			return true;
		}
	}

    public override float[] leftEyeCameraNearPlane { 
        get {
            OVRDisplay.EyeRenderDesc desc = OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.LeftEye);

            return new float[] {
                -Mathf.Tan(desc.fullFov.LeftFov / 180.0f * Mathf.PI),
                Mathf.Tan(desc.fullFov.UpFov / 180.0f * Mathf.PI),
                Mathf.Tan(desc.fullFov.RightFov / 180.0f * Mathf.PI),
                -Mathf.Tan(desc.fullFov.DownFov / 180.0f * Mathf.PI),
            };
        }
    }

    public override Vector3 eyeCenterPosition { 
        get {
            return new Vector3(0.0f, OVRManager.profile.eyeHeight - OVRManager.profile.neckHeight, OVRManager.profile.eyeDepth);
        }
    }

    public override float ipd { 
        get {
            return OVRManager.profile.ipd;
        }
    }

	public override bool hasInput {
		get {
			return true;
		}
	}

	public override RenderType renderType {
		get {
			return RenderType.DirectOnTwoEyeTextures;
		}
	}

	public override int[] leftEyeViewport { 
		get {
			OVRDisplay.EyeRenderDesc desc = OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.LeftEye);
			return new int[] { 0, 0, (int)desc.resolution.x, (int)desc.resolution.y };
		}
	}

	public override int[] rightEyeViewport { 
		get {
			return leftEyeViewport;
		}
	}

	public override float[] videoScale {
		get {
            //OVRDisplay.EyeRenderDesc desc = OVRManager.display.GetEyeRenderDesc(UnityEngine.XR.XRNode.LeftEye);
            //return new float[] { (float)videoWidth / 2 / desc.resolution.x, (float)videoHeight / desc.resolution.y };
            return new float[] { 1.0f, 1.0f };
		}
	}

	public override bool isUserPresent {
		get {
			return OVRManager.instance.isUserPresent;
		}
	}

	public override float delayToResumePlayback {
		get {
			return 1.5f;
		}
	}
}
