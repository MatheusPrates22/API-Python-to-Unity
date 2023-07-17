using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnityAPIJsonFormat 
{
    public string _UnityAPI__name;
    public float _UnityAPI__photoNumber;
    public MyTransform _UnityAPI__objectTransform;
    public MyCamera _UnityAPI__camera;
}

[System.Serializable]
public class MyCamera
{
    public Vector3 _Transform__position;
    public Vector3 _Transform__rotation;
    public Vector3 _Transform__scale;
    public float _Camera__fov;
    public Vector2 _Camera__resolution;

}

[System.Serializable]
public class MyTransform
{
    public Vector3 _Transform__position;
    public Vector3 _Transform__rotation;
    public Vector3 _Transform__scale;
}