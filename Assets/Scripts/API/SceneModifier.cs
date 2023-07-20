using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneModifier : MonoBehaviour
{
    public static SceneModifier Instance;

    [Header("Python")]
    [Header("Object")]
    [SerializeField] private Transform objectToChangeTransform;
    [Header("Camera")]
    [SerializeField] private Camera cameraToChange;

    private static DataAPI dataAPI;
    private static bool needChangesByPythonCall = false;

    private void Awake() {
        if (Instance == null) Instance = this; else Destroy(Instance);
    }

    private void Update() {
        if (needChangesByPythonCall){
            MakeChangesToTheScene();
            // StartCoroutine(TakeScreenshot());
            needChangesByPythonCall = false;
        }
    }

    public static void UpdateScene(DataAPI dataAPI_to_update) {
        dataAPI = dataAPI_to_update;
        needChangesByPythonCall = true;
    }

    private void MakeChangesToTheScene() {
        ChangeObjectTransform(dataAPI._UnityAPI__objectTransform);
        ChangeCameraSettings(dataAPI._UnityAPI__camera);
    }

    private void ChangeObjectTransform(MyTransform unityAPI__objectTransform) {
        objectToChangeTransform.position = dataAPI._UnityAPI__objectTransform._Transform__position;
        // objectToChangeTransform.Rotate(dataAPI._UnityAPI__objectTransform._Transform__rotation);
        // objectToChangeTransform.eulerAngles = dataAPI._UnityAPI__objectTransform._Transform__rotation;
        objectToChangeTransform.rotation = Quaternion.Euler(dataAPI._UnityAPI__objectTransform._Transform__rotation);
        objectToChangeTransform.localScale = dataAPI._UnityAPI__objectTransform._Transform__scale;
    }

    private void ChangeCameraSettings(MyCamera unityAPI__camera)
    {
        cameraToChange.transform.position = unityAPI__camera._Transform__position;
        cameraToChange.transform.rotation = Quaternion.Euler(unityAPI__camera._Transform__rotation);
        cameraToChange.transform.localScale = unityAPI__camera._Transform__scale;
        cameraToChange.fieldOfView = unityAPI__camera._Camera__fov;
        Screen.SetResolution((int)unityAPI__camera._Camera__resolution.x, (int)unityAPI__camera._Camera__resolution.y, false);
    }
}
