using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [Header("Screenshot")]
    [SerializeField] private bool openAfterSave = true;
    [SerializeField] private KeyCode takeScreenshotKeyCode = KeyCode.T;
    [SerializeField] private KeyCode removeBackgroundKeyCode = KeyCode.B;
    [SerializeField] private KeyCode saveScreenshotKeyCode = KeyCode.S;
    [SerializeField] private KeyCode sceneSnapshotKeyCode = KeyCode.Y;

    [SerializeField] private string savePath = "\\Photos\\screenshotSaveUnity.png";
    [SerializeField] private Color colorToChange = Color.clear;
    [SerializeField] private Color backgroundColor = Color.magenta;
    [SerializeField, Range(0f, 1f)] private float tolerance = 0.7f;

    [Header("Illumination")]
    [SerializeField] private KeyCode changeIlluminationKeyCode = KeyCode.Y;
    [SerializeField] private Vector3 rotationIllumination;
    [SerializeField] private float intensityIllumination;

    private Texture2D image;
    private bool isSceneSnapshotRequested = false;

    private void Update() {
        // Screenshot
        if (Input.GetKeyDown(sceneSnapshotKeyCode)) {
            ScreenshotController.TakeScreenshot();
            isSceneSnapshotRequested = true;
        }
        if (Input.GetKeyDown(takeScreenshotKeyCode)) {
            ScreenshotController.TakeScreenshot();
        }
        if (Input.GetKeyDown(removeBackgroundKeyCode)) {
            image = RemoveImageBackground.RemoveBackground(image, backgroundColor, colorToChange, tolerance);
        }
        if (Input.GetKeyDown(saveScreenshotKeyCode)) {
            RemoveImageBackground.SaveTextureToFile(image, Application.dataPath + savePath, openAfterSave);
        }

        // Illumination
        if (Input.GetKeyDown(changeIlluminationKeyCode)) {
            MyIlluminationData illuminationData = new MyIlluminationData();
            illuminationData.rotation = rotationIllumination;
            illuminationData.intensity = intensityIllumination;
            SceneModifier.NeedUpdateIllumination(illuminationData);
        }
    }

    private void OnEnable() {
        ScreenshotController.OnScreenshotTaken += ScreenshotController_OnScreenshotTaken;
    }

    private void OnDisable() {
        ScreenshotController.OnScreenshotTaken -= ScreenshotController_OnScreenshotTaken;
    }

    private void ScreenshotController_OnScreenshotTaken(object sender, Texture2D screenshotTexture)
    {
        image = screenshotTexture;
        Debug.Log("Screenshot taken!");
        if (isSceneSnapshotRequested) {
            isSceneSnapshotRequested = false;
            image = RemoveImageBackground.RemoveBackground(image, backgroundColor, colorToChange, tolerance);
            RemoveImageBackground.SaveTextureToFile(image, Application.dataPath + savePath, openAfterSave);
        }
    }
}
