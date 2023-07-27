using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Siccity.GLTFUtility;

public class GLBLoader : MonoBehaviour
{
    private static GLBLoader Instance;

    public Transform parentNewGLB;
    [SerializeField] private string defaultGLBPath = "Assets/Prefabs/exemplo.glb";

    private void Awake() {
        if (Instance == null) Instance = this; else Destroy(Instance);
    }

    public static GameObject LoadGLBFromFile(string filepath) {
        GameObject glbGO = Importer.LoadFromFile(filepath);
        if (glbGO != null) {
            glbGO.transform.SetParent(Instance.parentNewGLB);
        } else {
            Debug.Log("Falha ao carregar e instanciar arquivo GLTF ou GLB: " + filepath);
        }
        return glbGO;
    }

    public static GameObject LoadDefaultGLB() {
        return LoadGLBFromFile(Instance.defaultGLBPath);
    }
}
