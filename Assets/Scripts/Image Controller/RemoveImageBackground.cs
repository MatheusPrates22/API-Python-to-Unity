using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
// using System.Diagnostics as SD;

public class RemoveImageBackground : MonoBehaviour
{
    private static RemoveImageBackground Instance;

    public Color colorToChange = Color.clear;
    public Color backgroundColor = Color.magenta;
    [Range(0f, 1f)] public float tolerance = 0.1f;


    private void Awake() {
        if (Instance == null) Instance = this; else Destroy(Instance);
    }

    public static Texture2D RemoveBackground(Texture2D image, Color backgroundColor=default(Color), Color colorToChange=default(Color), float tolerance=default(float)) {
        Debug.Log("Removing background");
        backgroundColor = backgroundColor != default(Color) ? backgroundColor : Instance.backgroundColor;
        colorToChange = colorToChange != default(Color) ? colorToChange : Instance.colorToChange;
        tolerance = tolerance != default(float) ? tolerance : Instance.tolerance;

        Texture2D modifiedTexture = new Texture2D(image.width, image.height);

        for (int x = 0; x < image.width; x++)
        {
            for (int y = 0; y < image.height; y++)
            {
                Color pixelColor = image.GetPixel(x, y);
                float colorDistance = ColorDistance(pixelColor, backgroundColor);

                if (colorDistance <= tolerance)
                {
                    // Define o pixel como transparente
                    // pixelColor = new Color(0f, 0f, 0f, 0f);
                    // pixelColor = Color.clear;
                    pixelColor = colorToChange;
                }

                modifiedTexture.SetPixel(x, y, pixelColor);
            }
        }

        modifiedTexture.Apply();
        Debug.Log("Background removed");
        return modifiedTexture;
    }

    private  static float ColorDistance(Color color1, Color color2)
    {
        float distance = Mathf.Sqrt(Mathf.Pow(color1.r - color2.r, 2) +
                                    Mathf.Pow(color1.g - color2.g, 2) +
                                    Mathf.Pow(color1.b - color2.b, 2));
        return distance;
    }

    public static void SaveTextureToFile(Texture2D texture, string filePath, bool openAfterSave=false)
    {
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Saved texture!");
        if (openAfterSave) {
            Application.OpenURL(filePath);
        }
    }
}
