/***********************************************************

  Copyright (c) 2017-present Clicked, Inc.

  Licensed under the MIT license found in the LICENSE file 
  in the root folder of the project.

 ***********************************************************/

using UnityEngine;
using UnityEngine.UI;

public class AirVRClientNetworkIndicator : MonoBehaviour
{
    [SerializeField] private Image[] signalBars;
    [SerializeField] private float frequencyTime = 1;

    private Text _networkName;
    private float _elapsedTime;

    private void Awake() 
    {
        _networkName = GetComponentInChildren<Text>();
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void Start()
    {
        _elapsedTime = frequencyTime;
    }

    private void Update()
    {
        if (_elapsedTime >= frequencyTime)
        {
            _elapsedTime = 0f;

            var (name, level) = GetNetworkStatus(4);

            _networkName.text = name.Trim('\"');

            for (int i = 0; i < signalBars.Length; i++)
            {
                signalBars[i].enabled = i < level;
            }
        }

        _elapsedTime += Time.deltaTime;
    }
#endif


    private (string name, int level) GetNetworkStatus(int numberOfLevels)
    {
        using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity")) {
            var connectivityManager = activity.Call<AndroidJavaObject>("getSystemService", "connectivity");
            var activeNetwork = connectivityManager.Call<AndroidJavaObject>("getActiveNetwork");
            if (activeNetwork == null) {
                return ("N/A", 0);
            }

            var capabilities = connectivityManager.Call<AndroidJavaObject>("getNetworkCapabilities", activeNetwork);
            if (capabilities == null) {
                return ("N/A", 0);
            }

            var wifiManagerClass = new AndroidJavaClass("android.net.wifi.WifiManager");
            if (capabilities.Call<bool>("hasTransport", 0)) { // cellular
                var strength = capabilities.Call<int>("getSignalStrength");
                return (
                    "Cellular",
                    wifiManagerClass.CallStatic<int>("calculateSignalLevel", strength, numberOfLevels)
                );
            }
            else if (capabilities.Call<bool>("hasTransport", 1)) { // wifi
                var wifiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi");
                var wifiInfo = wifiManager.Call<AndroidJavaObject>("getConnectionInfo");
                int rssi = wifiInfo.Call<int>("getRssi");

                return (
                    wifiInfo.Call<string>("getSSID"),
                    wifiManagerClass.CallStatic<int>("calculateSignalLevel", rssi, numberOfLevels)
                );
            }
        }

        return ("N/A", 0);
    }  
}
