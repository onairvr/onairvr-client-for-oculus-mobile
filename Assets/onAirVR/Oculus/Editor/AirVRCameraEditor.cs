/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AirVRCamera))]

public class AirVRCameraEditor : Editor {
    private SerializedProperty _propEnableAudio;
    private SerializedProperty _propAudioMixerGroup;
    private SerializedProperty _propVideoBitrate;
    private SerializedProperty _propPreferRealWorldSpace;
    private SerializedProperty _propLeftControllerModel;
    private SerializedProperty _propRightControllerModel;
    private SerializedProperty _propControllerOverlay;
    private SerializedProperty _propEnablePointer;
    private SerializedProperty _propColorLaser;
    private SerializedProperty _propPointerCookie;
    private SerializedProperty _propPointerCookieDepthScaleMultiplier;

    private void OnEnable() {
        _propEnableAudio = serializedObject.FindProperty("_enableAudio");
        _propAudioMixerGroup = serializedObject.FindProperty("_audioMixerGroup");
        _propVideoBitrate = serializedObject.FindProperty("_videoBitrate");
        _propPreferRealWorldSpace = serializedObject.FindProperty("_preferRealWorldSpace");
        _propControllerOverlay = serializedObject.FindProperty("_controllerOverlay");
        _propLeftControllerModel = serializedObject.FindProperty("_leftControllerModel");
        _propRightControllerModel = serializedObject.FindProperty("_rightControllerModel");
        _propEnablePointer = serializedObject.FindProperty("_enablePointer");
        _propColorLaser = serializedObject.FindProperty("_colorLaser");
        _propPointerCookie = serializedObject.FindProperty("_pointerCookie");
        _propPointerCookieDepthScaleMultiplier = serializedObject.FindProperty("_pointerCookieDepthScaleMultiplier");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_propVideoBitrate);
        EditorGUILayout.PropertyField(_propEnableAudio);
        if (_propEnableAudio.boolValue) {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(_propAudioMixerGroup);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.PropertyField(_propControllerOverlay);
        if (_propControllerOverlay.boolValue) {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(_propLeftControllerModel);
            EditorGUILayout.PropertyField(_propRightControllerModel);
            EditorGUILayout.PropertyField(_propEnablePointer);

            GUI.enabled = _propEnablePointer.boolValue;
            EditorGUILayout.PropertyField(_propColorLaser);
            EditorGUILayout.PropertyField(_propPointerCookie);
            EditorGUILayout.PropertyField(_propPointerCookieDepthScaleMultiplier);
            GUI.enabled = true;

            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.PropertyField(_propPreferRealWorldSpace);

        serializedObject.ApplyModifiedProperties();
    }
}
