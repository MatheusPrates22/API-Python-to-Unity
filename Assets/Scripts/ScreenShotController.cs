using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotController : MonoBehaviour
{
    [SerializeField] private bool forceChangesByPythonCall = false;
    [SerializeField] private Transform screenshotTransform;

    private string _name;
    private float _photoNumber;
    
    private UnityAPIJsonFormat unityAPIInfo;


    private void Update() {
        if (forceChangesByPythonCall){
            ChangeObjectTransform(unityAPIInfo._UnityAPI__objectTransform);
            ChangeCameraSettings(unityAPIInfo._UnityAPI__camera);
        }
    }

    public void Initialize(UnityAPIJsonFormat format){
        forceChangesByPythonCall = true;
        this.unityAPIInfo = format;
        this._name = format._UnityAPI__name;
        this._photoNumber = format._UnityAPI__photoNumber;
        // ChangeObjectTransform(format._UnityAPI__objectTransform);
        // ChangeCameraSettings(format._UnityAPI__camera);
    }

    private void ChangeObjectTransform(MyTransform myTransform){
       screenshotTransform.position = unityAPIInfo._UnityAPI__objectTransform._Transform__position;
        screenshotTransform.rotation = Quaternion.Euler(unityAPIInfo._UnityAPI__objectTransform._Transform__rotation);
        screenshotTransform.localScale = unityAPIInfo._UnityAPI__objectTransform._Transform__scale;
    }

    private void ChangeCameraSettings(MyCamera myCamera){
        Camera.main.transform.position = myCamera._Transform__position;
        Camera.main.transform.rotation = Quaternion.Euler(myCamera._Transform__rotation);
        Camera.main.transform.localScale = myCamera._Transform__scale;
        Camera.main.fieldOfView = myCamera._Camera__fov;
        Screen.SetResolution((int)myCamera._Camera__resolution.x, (int)myCamera._Camera__resolution.y, false);
    }
}
