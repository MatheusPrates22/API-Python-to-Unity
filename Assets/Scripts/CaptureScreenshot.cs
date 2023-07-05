using UnityEngine;

public class CaptureScreenshot : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Capture();
        }
    }

    public void Capture()
    {
        // Cria uma textura temporária com as dimensões da câmera
        RenderTexture renderTexture = new RenderTexture(Camera.main.pixelWidth, Camera.main.pixelHeight, 24);
        Camera.main.targetTexture = renderTexture;
        Camera.main.Render();

        // Ativa a renderização na textura
        RenderTexture.active = renderTexture;

        // Cria uma nova textura 2D e a lê a partir da textura ativa
        Texture2D screenshotTexture = new Texture2D(Camera.main.pixelWidth, Camera.main.pixelHeight, TextureFormat.RGB24, false);
        screenshotTexture.ReadPixels(new Rect(0, 0, Camera.main.pixelWidth, Camera.main.pixelHeight), 0, 0);
        screenshotTexture.Apply();

        // Salva a textura como um arquivo PNG
        byte[] bytes = screenshotTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes("Assets\\Photos\\screenshot3.png", bytes);

        // Limpa e libera os recursos
        Camera.main.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);
        Destroy(screenshotTexture);

        Debug.Log("Screenshot salva como screenshot.png");
    }
}
