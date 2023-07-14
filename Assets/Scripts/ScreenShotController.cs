using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenShotController : MonoBehaviour
{
    [SerializeField] private bool forceChangesByPythonCall = false;
    [SerializeField] private Transform screenshotTransform;

    [SerializeField] private Color colorToChange = Color.clear;
    
    // [SerializeField] private Vector2Int screenshotSize;
    // [SerializeField] Camera myCamera;

    public string screenshotPath = "Assets\\Photos\\screenshotCoroutine.png";

    private string _name;
    private float _photoNumber;
    
    private UnityAPIJsonFormat unityAPIInfo;
    private readonly Color _backgroundColor = new Color(255, 255, 255);

    // private bool takeScreenshotOnNextFrame = false;


    private void OnEnable() {
        SocketReceiver.OnReceiveCalled += SocialReceiver_OnReceiveCalled;
    }

    private void OnDisable() {
        SocketReceiver.OnReceiveCalled -= SocialReceiver_OnReceiveCalled;
    }

    private void SocialReceiver_OnReceiveCalled(object sender, string receivedString)
    {
        unityAPIInfo = JsonUtility.FromJson<UnityAPIJsonFormat>(receivedString);
        forceChangesByPythonCall = true;
    }

    private void Update() {
        if (forceChangesByPythonCall){
            ChangeAllVariablesToTakeScreenshot();
            StartCoroutine(TakeScreenshot());
            forceChangesByPythonCall = false;
        }
        if (Input.GetKeyDown(KeyCode.T)) 
        {
            StartCoroutine(TakeScreenshotAndRemoveBackground());
            // StartCoroutine(CoroutineScreenshot());
        }
    }

    private IEnumerator TakeScreenshotAndRemoveBackground(){
        yield return new WaitForEndOfFrame();
        Texture2D modifiedTexture = ScreenCapture.CaptureScreenshotAsTexture();
        
        // float tolerance = 0.1f;
        // Texture2D modifiedTexture = new Texture2D(originalTexture.width, originalTexture.height);

        // Debug.Log(2);
        // for (int x = 0; x < originalTexture.width; x++)
        // {
        //     for (int y = 0; y < originalTexture.height; y++)
        //     {
        //         Color pixelColor = originalTexture.GetPixel(x, y);
        //         Debug.Log(pixelColor);
        //         float colorDistance = ColorDistance(pixelColor, Color.black);

        //         if (colorDistance <= tolerance)
        //         {
        //             // Define o pixel como transparente
        //             // pixelColor = new Color(0f, 0f, 0f, 0f);
        //             pixelColor = colorToChange;
        //         }

        //         modifiedTexture.SetPixel(x, y, pixelColor);
        //     }
        // }

        // modifiedTexture.Apply();


        byte[] data = modifiedTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Photos/CameraScreenshot2.png", data);
    }

    private IEnumerator CoroutineScreenshot()
    {
        yield return new WaitForEndOfFrame();

        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenshotTexture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Rect rect = new Rect(0, 0, width, height);
        screenshotTexture.ReadPixels(rect, 0, 0);
        screenshotTexture.Apply();

        Debug.Log("screenshot");
        // Color backgroundColor = new Color(255, 0, 255);
        // float tolerance = 1f;
        // Texture2D imageWithoutBackground = RemoveBackground(screenshotTexture, backgroundColor, tolerance);
        // screenshotTexture = RemoveBackground(screenshotTexture);
        // Debug.Log("remove background");

        byte[] data = screenshotTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/Photos/CameraScreenshot2.png", data);
        Debug.Log("saved");

    }

    private void SaveTextureToFile(Texture2D texture, string filePath)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }

    private IEnumerator TakeScreenshot()
    {
        yield return new WaitForEndOfFrame();

        screenshotPath = "Assets\\Photos\\screenshotCoroutine.png";
        ScreenCapture.CaptureScreenshot(screenshotPath);

        yield return new WaitForSeconds(0.2f);

        byte[] imageBytes = File.ReadAllBytes(screenshotPath);
        Texture2D texture = new Texture2D(720, 720);
        texture.LoadImage(imageBytes);

        Color backgroundColor = new Color(255, 0, 255);
        float tolerance = 1f;
        Texture2D imageWithoutBackground = RemoveBackground(texture, backgroundColor, tolerance);

        SocketReceiver.SendMessageCallback(imageWithoutBackground, true);
    }

    #region ajust picture
    private void ChangeAllVariablesToTakeScreenshot()
    {
        this._name = unityAPIInfo._UnityAPI__name;
        this._photoNumber = unityAPIInfo._UnityAPI__photoNumber;
        ChangeObjectTransform(unityAPIInfo._UnityAPI__objectTransform);
        ChangeCameraSettings(unityAPIInfo._UnityAPI__camera);

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
    #endregion

    public Texture2D RemoveBackground(Texture2D originalTexture, Color backgroundColor, float tolerance = 0.1f)
    {
        Debug.Log(1);
        // if (backgroundColor == default)
        // {
        //     backgroundColor = Color.magenta;
        // }
        Texture2D modifiedTexture = new Texture2D(originalTexture.width, originalTexture.height);

        Debug.Log(2);
        for (int x = 0; x < originalTexture.width; x++)
        {
            for (int y = 0; y < originalTexture.height; y++)
            {
                Color pixelColor = originalTexture.GetPixel(x, y);
                Debug.Log(pixelColor);
                float colorDistance = ColorDistance(pixelColor, backgroundColor);

                if (colorDistance <= tolerance)
                {
                    // Define o pixel como transparente
                    // pixelColor = new Color(0f, 0f, 0f, 0f);
                    pixelColor = Color.clear;
                }

                modifiedTexture.SetPixel(x, y, pixelColor);
            }
        }

        modifiedTexture.Apply();

        return modifiedTexture;
    }

    private float ColorDistance(Color color1, Color color2)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(color1.r - color2.r, 2) +
                                    Mathf.Pow(color1.g - color2.g, 2) +
                                    Mathf.Pow(color1.b - color2.b, 2));
        return distance;
    }
}
