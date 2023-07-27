using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization;

public class APIController : MonoBehaviour
{
    public static APIController Instance;
    
    public static string host;
    public static int port;
    public static int bufferSize;

    [SerializeField] private GameObject tcpServerGO;
    [SerializeField] private GameObject updServerGO;
    [SerializeField] private bool showJsonString;
    [SerializeField] private bool printStage;

    private static string _protocol;
    private static bool _needRemoveBackground;
    private static float _tolerance;

    private void Awake() {
        if (Instance == null) Instance = this; else Destroy(Instance);
        transform.eulerAngles = new Vector3(0, 45, 20);
        transform.rotation =  Quaternion.Euler(new Vector3(10, 5, 2));
        ReadConfigJsonFile();
        EnableServer(_protocol);

    }

    private void OnEnable() {
        ScreenshotController.OnScreenshotTaken += ScreenshotController_OnScreenshotTaken;
    }

    private void OnDisable() {
        ScreenshotController.OnScreenshotTaken -= ScreenshotController_OnScreenshotTaken;
    }

    #region JSON RECEIVED
    public static void ReadAPIJsonFile(string jsonReceived) {
        if (Instance.showJsonString) {
            Debug.Log(jsonReceived);
        }
        try {
            APIData apiData = JsonUtility.FromJson<APIData>(jsonReceived);
            ShowMessage(apiData.message);
            UpdateScene(apiData);
            TakeScreenshot(apiData.screenshot);
            // TakeScreenshot(apiData.take_screenshot, apiData.remove_background, apiData.tolerance);
        } catch (Exception e) {
            Debug.Log("Exception in change string to json: " + e.Message);
        }
    }

    private static void ShowMessage(string message) {
        if (!string.IsNullOrEmpty(message)) {
            Debug.Log("Message: " + message);
        }
    }

    private static void UpdateScene(APIData apiData) {
        if (apiData._APIData__update_unity_object) {
            if (Instance.printStage) Debug.Log("Updating object");
            SceneModifier.NeedUpdateObject(apiData.unity_object);
        }
        if (apiData._APIData__update_camera) {
            if (Instance.printStage) Debug.Log("Updating camera");
            SceneModifier.NeedUpdateCamera(apiData.camera);
        }
        if (apiData._APIData__update_illumination) {
            if (Instance.printStage) Debug.Log("Updating illumination");
            SceneModifier.NeedUpdateIllumination(apiData.illumination);
        }
    }

    private static void TakeScreenshot(MyScreenshotData myScreenshot) {
        if (myScreenshot.take_screenshot) {
            if (Instance.printStage) Debug.Log("Taking screenshot");
            _needRemoveBackground = myScreenshot.remove_background;
            _tolerance = myScreenshot.tolerance;
            ScreenshotController.TakeScreenshot();
        }
    }
    #endregion

    private void ScreenshotController_OnScreenshotTaken(object sender, Texture2D screenshotTexture)
    {
        if (_needRemoveBackground) {
            screenshotTexture = RemoveImageBackground.RemoveBackground(screenshotTexture, tolerance: _tolerance);
        }
        TCPThread.SendTexture(screenshotTexture);
    }

    #region CONFIG JSON FILE
    private void ReadConfigJsonFile()
    {

        try {
            string configFilePath = Path.Combine(Application.dataPath, "Scripts", "Python", "configAPI.json");
            string configJson = File.ReadAllText(configFilePath);

            ConfigData configData = JsonUtility.FromJson<ConfigData>(configJson);

            host = configData.host;
            port = configData.port;
            _protocol = configData.protocol;
            bufferSize = configData.buffer_size;
        } catch (Exception e) {
            Debug.Log("Exception on read json config file. Error: " + e.Message);
        }
    }

    private void EnableServer(string protocol) {
        if (protocol == "TCP") {
            tcpServerGO.SetActive(true);
            updServerGO.SetActive(false);
        } else if (protocol == "UDP") {
            tcpServerGO.SetActive(false);
            updServerGO.SetActive(true);
        }
    }

    [System.Serializable]
    private class ConfigData {
        public string host;
        public int port;
        public string protocol;
        public int buffer_size;
    }
    #endregion

}
