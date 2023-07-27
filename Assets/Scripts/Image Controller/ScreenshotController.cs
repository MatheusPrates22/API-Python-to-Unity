using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScreenshotController : MonoBehaviour
{
    private static ScreenshotController Instance;

    public static event EventHandler<Texture2D> OnScreenshotTaken;

    [SerializeField] private bool saveScreenshotOnLocal;
    [SerializeField] private string pathToSaveScreenshot = "/Photos/ScreenshotUnity.png";

    private static bool needTakeScreenshot = false;

    private void Awake() {
        if (Instance == null) Instance = this; else Destroy(Instance);
    }

    private void Update() {
        if (needTakeScreenshot) {
            Instance.StartCoroutine(TakeScreenshotCoroutine());
            needTakeScreenshot = false;
        }
    }

    public static void TakeScreenshot(bool saveLocal = false) {
        needTakeScreenshot = true;
        if (saveLocal || Instance.saveScreenshotOnLocal) {
            Instance.saveScreenshotOnLocal = true;
        }
    }

    private static IEnumerator TakeScreenshotCoroutine()
    {
        yield return new WaitForEndOfFrame();
        Texture2D screenshotTexture = ScreenCapture.CaptureScreenshotAsTexture();

        OnScreenshotTaken?.Invoke(null, screenshotTexture);

        if (Instance.saveScreenshotOnLocal) {
            byte[] imageBytes = screenshotTexture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + Instance.pathToSaveScreenshot, imageBytes);
        }
    }
}
