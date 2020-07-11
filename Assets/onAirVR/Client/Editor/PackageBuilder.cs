using System.Collections;
using System.Collections.Generic;
/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using System.IO;
using UnityEditor;

public class PackageBuilder {
    [MenuItem("onAirVR/Export onAirVR Client...")]
    public static void ExportAirVRClient() {
        string targetPath = EditorUtility.SaveFilePanel("Export onAirVR Client...", "", "onairvr-client", "unitypackage");
        if (string.IsNullOrEmpty(targetPath)) {
            return;
        }

        if (File.Exists(targetPath)) {
            File.Delete(targetPath);
        }
        string[] folders = { "Assets/onAirVR" };
        string[] guids = AssetDatabase.FindAssets("", folders);
        List<string> assets = new List<string>();
        foreach (string guid in guids) {
            assets.Add(AssetDatabase.GUIDToAssetPath(guid));
        }
        assets.Add("Assets/Plugins/Android/assets/client.license");
        assets.Add("Assets/Plugins/Android/libocs.so");
        assets.Add("Assets/Plugins/Android/ocs.jar");
        assets.Add("Assets/Plugins/Android/kotlin-stdlib.jar");
        AssetDatabase.ExportPackage(assets.ToArray(), targetPath);

        EditorUtility.DisplayDialog("Congratulation!", "The package is exported successfully.", "Thanks.");
    }
}
