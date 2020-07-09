/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEditor;

[CustomEditor(typeof(AirVRCamera))]

public class AirVRCameraEditor : Editor {
    private SerializedProperty _propEnableAudio;
    private SerializedProperty _propAudioMixerGroup;
    private SerializedProperty _propVideoBitrate;
    private SerializedProperty _propPreferRealWorldSpace;

    private void OnEnable() {
        _propEnableAudio = serializedObject.FindProperty("_enableAudio");
        _propAudioMixerGroup = serializedObject.FindProperty("_audioMixerGroup");
        _propVideoBitrate = serializedObject.FindProperty("_videoBitrate");
        _propPreferRealWorldSpace = serializedObject.FindProperty("_preferRealWorldSpace");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_propEnableAudio);
        if (_propEnableAudio.boolValue) {
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.PropertyField(_propAudioMixerGroup);
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.PropertyField(_propVideoBitrate);
        EditorGUILayout.PropertyField(_propPreferRealWorldSpace);

        serializedObject.ApplyModifiedProperties();
    }
}
