using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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


    [Header("Others")]
    [SerializeField] private KeyCode printObjectSizeKeyCode = KeyCode.P;


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

        // Others
        if (Input.GetKeyDown(printObjectSizeKeyCode)) {
            PrintObjectSize();
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


    // Others
    private void PrintObjectSize() {
        Debug.Log("Printing object size");
        Transform objectTransform = SceneModifier.GetObjectTransform();
        
        // Obtém o objeto pai com as maiores dimensões.
        Transform parentWithMaxDimensions = GetParentWithMaxDimensions(objectTransform);

        // Verifica se o objeto pai foi encontrado.
        if (parentWithMaxDimensions != null)
        {
            // Obtém o componente Renderer do objeto pai com as maiores dimensões.
            Renderer parentRenderer = parentWithMaxDimensions.GetComponent<Renderer>();

            // Obtém as dimensões máximas do objeto pai em forma de um objeto Bounds.
            Bounds parentBounds = parentRenderer.bounds;

            // Obtém as dimensões máximas do objeto pai em forma de Vector3 (largura, altura e comprimento).
            Vector3 parentSize = parentBounds.size;

            // Exibe as dimensões máximas do objeto pai no console.
            // Debug.Log($"Largura: {parentSize.x}, Altura: {parentSize.y}, Comprimento: {parentSize.z}");
            Debug.Log($"Object Dimension: {parentBounds.size}");
        }
        else
        {
            // Caso o objeto pai não seja encontrado, exibe uma mensagem de erro no console.
            Debug.Log($"Test -> Printing object size: {nameof(objectTransform)} is null!");
        }

                // Debug.Log($"Test -> Printing object size: object Renderer is null!");
    }

    // Função recursiva para encontrar o objeto pai com as maiores dimensões.
    private Transform GetParentWithMaxDimensions(Transform currentTransform)
    {
        // Obtém o componente Renderer do objeto atual, se existir.
        Renderer renderer = currentTransform.GetComponent<Renderer>();

        // Inicializa o objeto pai com as maiores dimensões como o objeto atual.
        Transform parentWithMaxDimensions = currentTransform;

        // Inicializa as maiores dimensões como zero.
        float maxDimensions = 0f;

        // Verifica se o objeto atual tem um componente Renderer.
        if (renderer != null)
        {
            // Obtém as dimensões do objeto atual.
            float currentDimensions = GetObjectDimensions(renderer.bounds);

            // Se as dimensões do objeto atual forem maiores que as maiores dimensões encontradas até agora,
            // atualiza o objeto pai com as maiores dimensões e o valor máximo das dimensões.
            if (currentDimensions > maxDimensions)
            {
                parentWithMaxDimensions = currentTransform;
                maxDimensions = currentDimensions;
            }
        }

        // Verifica os filhos recursivamente para encontrar o objeto com as maiores dimensões.
        foreach (Transform child in currentTransform)
        {
            Transform childParentWithMaxDimensions = GetParentWithMaxDimensions(child);
            float childDimensions = GetObjectDimensions(childParentWithMaxDimensions.GetComponent<Renderer>().bounds);

            // Se as dimensões do filho forem maiores que as maiores dimensões encontradas até agora,
            // atualiza o objeto pai com as maiores dimensões e o valor máximo das dimensões.
            if (childDimensions > maxDimensions)
            {
                parentWithMaxDimensions = childParentWithMaxDimensions;
                maxDimensions = childDimensions;
            }
        }

        return parentWithMaxDimensions;
    }

    // Função auxiliar para calcular as dimensões do objeto a partir de Bounds.
    private float GetObjectDimensions(Bounds bounds)
    {
        // Debug.Log($"Largura: {bounds.x}, Altura: {bounds.y}, Comprimento: {bounds.z}");
        Debug.Log($"Children Dimension: {bounds.size}");
        return bounds.size.x * bounds.size.y * bounds.size.z;
    }

    public struct ObjectDimension {
        public float maxX;
        public float maxY;
        public float maxZ;
        public float minX;
        public float minY;
        public float minZ;
    }
}
