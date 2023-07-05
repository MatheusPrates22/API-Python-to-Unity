using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShotController : MonoBehaviour
{
    [SerializeField] private Transform screenshotTransform;

    private string _name;
    private float _photoNumber;
    
    public Vector3 objectPosition;
    public Vector3 objectRotation;
    public Vector3 objectScale = Vector3.one;

    private void Update() {
        screenshotTransform.position = objectPosition;
        screenshotTransform.rotation = Quaternion.Euler(objectRotation);
        screenshotTransform.localScale = objectScale;
    }

    public void Initialize(UnityAPIJsonFormat format){
        this._name = format._UnityAPI__name;
        this._photoNumber = format._UnityAPI__photoNumber;
        ChangeObjectTransform(format._UnityAPI__objectTransform);
        // ChangeCameraSettings(format._UnityAPI__camera);
    }

    private void ChangeObjectTransform(MyTransform myTransform){
        objectPosition = myTransform._Transform__position;
        objectRotation = myTransform._Transform__rotation;
        objectScale = myTransform._Transform__scale;
    }

    private void ChangeCameraSettings(MyCamera myCamera){
        Camera.main.transform.position = myCamera._Transform__position;
        Camera.main.transform.rotation = Quaternion.Euler(myCamera._Transform__rotation);
        Camera.main.transform.localScale = myCamera._Transform__scale;
        Camera.main.fieldOfView = myCamera._Camera__fov;
    }
}
