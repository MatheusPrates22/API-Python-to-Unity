using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

// [System.Serializable]
// public class APIPythonUnityJsonFormat 
// {
//     // AO ADICIONAR ALGO AQUI, ADD NO PYTHON TBM -data.py-
//     public string message;
//     public APIData data; //precisa ser string para verificar se é nulo, depois transform em DataAPI
//     public bool take_screenshot;
//     public bool remove_background;
//     public float tolerance;
// }

[System.Serializable]
public class APIData 
{
    // AO ADICIONAR ALGO AQUI, ADD NO PYTHON TBM -data.py-
    public MyObjectData unity_object;
    public MyCameraData camera;
    public MyScreenshotData screenshot;
    public MyIlluminationData illumination;
    public string message;

    public bool _APIData__update_unity_object;
    public bool _APIData__update_camera;
    public bool _APIData__update_screenshot;
    public bool _APIData__update_illumination;
}

[System.Serializable]
public class MyTransformData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
}

[System.Serializable]
public class MyObjectData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public string asset_path;
}

[System.Serializable]
public class MyCameraData
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    public float fov;
    public Vector2 resolution;

}

[System.Serializable]
public class MyIlluminationData
{
    public Vector3 rotation;
    public float intensity;
    public Vector3 color;
}

[System.Serializable]
public class MyScreenshotData
{
    public bool take_screenshot;
    public bool remove_background;
    public float tolerance;
}