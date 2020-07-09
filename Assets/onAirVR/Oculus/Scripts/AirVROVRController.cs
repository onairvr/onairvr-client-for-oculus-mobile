/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the Docs folder of the distributed package.

 ***********************************************************/

using UnityEngine;

public class AirVROVRController : MonoBehaviour {
    private GameObject _controllerModel;

    private void Awake() {
        _controllerModel = transform.Find("OVRControllerPrefab").gameObject;
    }

    private void Update() {
        if (_controllerModel.activeSelf == AirVRClient.connected) {
            _controllerModel.SetActive(!AirVRClient.connected);
        }
    }
}
