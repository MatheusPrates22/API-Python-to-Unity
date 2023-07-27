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

    [Header("Illumination")]
    [SerializeField] private Light lightToChange;

    private static MyObjectData _myObjectData;
    private static MyCameraData _myCameraData;
    private static MyIlluminationData _myIlluminationData;
    private static bool needUpdateObject;
    private static bool needUpdateCamera;
    private static bool needUpdateIllumination;

    private void Awake() {
        if (Instance == null) Instance = this; else Destroy(Instance);
    }

    private void Update() {
        if (needUpdateObject){
            UpdateAtObject();
            needUpdateObject = false;
        }
        if (needUpdateCamera){
            UpdateAtCamera();
            needUpdateCamera = false;
        }
        if (needUpdateIllumination){
            UpdateAtIllumination();
            needUpdateIllumination = false;
        }
    }

    public static void NeedUpdateObject(MyObjectData myObject) {
        _myObjectData = myObject;
        needUpdateObject = true;
    }

    public static void NeedUpdateCamera(MyCameraData myCamera) {
        _myCameraData = myCamera;
        needUpdateCamera = true;
    }

    public static void NeedUpdateIllumination(MyIlluminationData myIllumination) {
        _myIlluminationData = myIllumination;
        needUpdateIllumination = true; 
    }

    private void UpdateAtObject() {
        if (!string.IsNullOrEmpty(_myObjectData.asset_path)){
            // Precisa carregar o prefab pelo caminho fornecido
            Debug.Log(_myObjectData.asset_path);
            GameObject prefabGO = GLBLoader.LoadGLBFromFile(_myObjectData.asset_path);
            if (objectToChangeTransform != null) Destroy(objectToChangeTransform.gameObject);
            objectToChangeTransform = prefabGO.transform;
        } else if(objectToChangeTransform == null){
            // Como não há nenhuma caminho fornecido e o objectToChangeTransform está nulo (não foi atribuido através do Inspector) então carregar o prefab default
            Debug.Log("No asset path specified");
            GameObject prefabGO = GLBLoader.LoadDefaultGLB();
            objectToChangeTransform = prefabGO.transform;
        } else {
            objectToChangeTransform.gameObject.SetActive(true);
        }

        if(objectToChangeTransform != null) {
            objectToChangeTransform.position = _myObjectData.position;
            // objectToChangeTransform.Rotate(dataAPI._UnityAPI__objectTransform._Transform__rotation);
            // objectToChangeTransform.eulerAngles = dataAPI._UnityAPI__objectTransform._Transform__rotation;
            objectToChangeTransform.rotation = Quaternion.Euler(_myObjectData.rotation);
            objectToChangeTransform.localScale = _myObjectData.scale;
        } else {
            Debug.Log("Failed to load asset and failed to use default asset. Check provided path.");
        }
    }

    private void UpdateAtCamera() {
        cameraToChange.transform.position = _myCameraData.position;
        cameraToChange.transform.rotation = Quaternion.Euler(_myCameraData.rotation);
        cameraToChange.transform.localScale = _myCameraData.scale;
        cameraToChange.fieldOfView = _myCameraData.fov;
        Screen.SetResolution((int)_myCameraData.resolution.x, (int)_myCameraData.resolution.y, false);
    }

    private void UpdateAtIllumination() {
        lightToChange.transform.rotation = Quaternion.Euler(_myIlluminationData.rotation);
        lightToChange.intensity = _myIlluminationData.intensity;
        lightToChange.color = new Color(_myIlluminationData.color.x, _myIlluminationData.color.y, _myIlluminationData.color.z);
    }
}
