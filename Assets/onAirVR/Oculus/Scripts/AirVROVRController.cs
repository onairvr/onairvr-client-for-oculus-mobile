using System.Collections;
using System.Collections.Generic;
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
