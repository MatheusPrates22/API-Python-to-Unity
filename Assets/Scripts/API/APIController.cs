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

    private static string protocol;

    private void Awake() {
        if (Instance == null) Instance = this; else Destroy(Instance);
        transform.eulerAngles = new Vector3(0, 45, 20);
        transform.rotation =  Quaternion.Euler(new Vector3(10, 5, 2));
        ReadConfigJsonFile();
        EnableServer(protocol);

    }

    private void OnEnable() {
        ScreenshotController.OnScreenshotTaken += ScreenshotController_OnScreenshotTaken;
    }

    private void OnDisable() {
        ScreenshotController.OnScreenshotTaken -= ScreenshotController_OnScreenshotTaken;
    }

    #region JSON RECEIVED
    public static void ReceiveJsonString(string jsonReceived) {
        if (Instance.showJsonString) {
            Debug.Log(jsonReceived);
        }
        try {
            APIPythonUnityJsonFormat unityAPIInfo = JsonUtility.FromJson<APIPythonUnityJsonFormat>(jsonReceived);
            ShowMessage(unityAPIInfo.message);
            UpdateScene(unityAPIInfo.data);
            TakeScreenshot(unityAPIInfo.takeScreenshot);
        } catch (Exception e) {
            Debug.Log("Exception in change string to json: " + e.Message);
        }
    }

    private static void ShowMessage(string message) {
        if (message != "") {
            Debug.Log("Message: " + message);
        }
    }

    private static void UpdateScene(string data) {
        if (!string.IsNullOrEmpty(data)) {
            DataAPI dataAPI = JsonUtility.FromJson<DataAPI>(data);
            SceneModifier.UpdateScene(dataAPI);
        }
    }

    private static void TakeScreenshot(bool takeScreenshot) {
        if (takeScreenshot) {
            ScreenshotController.TakeScreenshot();
            Debug.Log("Taking screenshot");
        }
    }
    #endregion

    private void ScreenshotController_OnScreenshotTaken(object sender, Texture2D screenshotTexture)
    {
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
            protocol = configData.protocol;
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
