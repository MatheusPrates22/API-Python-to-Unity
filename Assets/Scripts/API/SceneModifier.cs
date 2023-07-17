using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneModifier : MonoBehaviour
{
    [Header("Python")]
    [SerializeField] private bool forceChangesByPythonCall = false;
    [Header("Object")]
    [SerializeField] private Transform objectToChangeTransform;
    [Header("Camera")]
    [SerializeField] private Camera cameraToChange;

    private UnityAPIJsonFormat unityAPIInfo;

    private void OnEnable() {
        TCPServer.OnReceiveCalled += TCPServer_OnReceiveCalled;
        UDPServer.OnReceiveCalled += UDPServer_OnReceiveCalled;
    }

    private void OnDisable() {
        TCPServer.OnReceiveCalled -= TCPServer_OnReceiveCalled;
        UDPServer.OnReceiveCalled -= UDPServer_OnReceiveCalled;
    }

    private void TCPServer_OnReceiveCalled(object sender, string receivedString)
    {
        unityAPIInfo = JsonUtility.FromJson<UnityAPIJsonFormat>(receivedString);
        forceChangesByPythonCall = true;
    }

    private void UDPServer_OnReceiveCalled(object sender, string receivedString)
    {
        unityAPIInfo = JsonUtility.FromJson<UnityAPIJsonFormat>(receivedString);
        forceChangesByPythonCall = true;
    }

    private void Update() {
        if (forceChangesByPythonCall){
            MakeChangesToTheScene();
            // StartCoroutine(TakeScreenshot());
            forceChangesByPythonCall = false;
        }
    }

    private void MakeChangesToTheScene()
    {
        ChangeObjectTransform(unityAPIInfo._UnityAPI__objectTransform);
        ChangeCameraSettings(unityAPIInfo._UnityAPI__camera);
    }

    private void ChangeObjectTransform(MyTransform myTransform){
        objectToChangeTransform.position = unityAPIInfo._UnityAPI__objectTransform._Transform__position;
        objectToChangeTransform.rotation = Quaternion.Euler(unityAPIInfo._UnityAPI__objectTransform._Transform__rotation);
        objectToChangeTransform.localScale = unityAPIInfo._UnityAPI__objectTransform._Transform__scale;
    }

    private void ChangeCameraSettings(MyCamera myCamera){
        cameraToChange.transform.position = myCamera._Transform__position;
        cameraToChange.transform.rotation = Quaternion.Euler(myCamera._Transform__rotation);
        cameraToChange.transform.localScale = myCamera._Transform__scale;
        cameraToChange.fieldOfView = myCamera._Camera__fov;
        Screen.SetResolution((int)myCamera._Camera__resolution.x, (int)myCamera._Camera__resolution.y, false);
    }
}
