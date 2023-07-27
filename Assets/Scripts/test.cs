using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;

public class test : MonoBehaviour
{
    [Header("Screenshot")]
    [SerializeField] private bool openAfterSave = true;
    [SerializeField] private KeyCode takeScreenshotKeyCode = KeyCode.Q;
    [SerializeField] private KeyCode removeBackgroundKeyCode = KeyCode.W;
    [SerializeField] private KeyCode saveScreenshotKeyCode = KeyCode.E;
    [SerializeField] private KeyCode sceneSnapshotKeyCode = KeyCode.R;

    [SerializeField] private string savePath = "\\Photos\\screenshotSaveUnity.png";
    [SerializeField] private Color colorToChange = Color.clear;
    [SerializeField] private Color backgroundColor = Color.magenta;
    [SerializeField, Range(0f, 1f)] private float tolerance = 0.7f;

    [Header("Illumination")]
    [SerializeField] private KeyCode changeIlluminationKeyCode = KeyCode.A;
    [SerializeField] private Vector3 rotationIllumination;
    [SerializeField] private float intensityIllumination;

    [Header("Instantiate GLB")]
    [SerializeField] private KeyCode instantiateGLBFromPathKeyCode = KeyCode.Z;
    [SerializeField] private KeyCode instantiateGLBFromPrefabKeyCode = KeyCode.X;
    [SerializeField] private Transform parentNewGLB;
    [SerializeField] private string pathGLB;
    [SerializeField] private GameObject glbPrefab;

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

        // Instantiate GLB
        if (Input.GetKeyDown(instantiateGLBFromPathKeyCode)) {
            LoadGLBFromPath(pathGLB);
        }
        if (Input.GetKeyDown(instantiateGLBFromPrefabKeyCode)) {
            LoadGLBFromPrefab(glbPrefab);
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

    // Instantiate GLB
    private void LoadGLBFromPath(string path) {
        GameObject glbGO = Importer.LoadFromFile(path);
        if (glbGO != null) {
            glbGO.transform.SetParent(parentNewGLB);
        } else {
            Debug.Log("Falha ao carregar e instanciar arquivo GLTF ou GLB: " + path);
        }
    }
    private void LoadGLBFromPrefab(GameObject prefabGO) {
        Instantiate(prefabGO, parentNewGLB);
    }

}
