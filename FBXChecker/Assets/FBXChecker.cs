using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

public class FBXChecker : OdinEditorWindow
{
    [MenuItem("Arcadia VR/TAOptimization/FBXCount")]
    private static void Init()
    {
        GetWindow<FBXChecker>().Show();
    }

    private static void CheckCountOfFBX(bool flagForFirstTypeOfMessage, bool flagForSecondTypeOfMessage)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        string word = "LOD0";

        foreach (GameObject go in allObjects)
        {
            if (PrefabUtility.IsPartOfPrefabInstance(go))
            {
                LODGroup lod = go.GetComponent<LODGroup>();
                if (lod != null)
                {
                    LOD[] lods = lod.GetLODs();

                    if (lods.Length > 3 && flagForFirstTypeOfMessage)
                    {
                        Debug.Log("<color=green>Object with 1 LOD group:</color> " + go.name);

                        foreach (LOD lodItem in lods)
                        {
                            foreach (Renderer renderer in lodItem.renderers)
                            {
                                if (renderer.name.Contains(word))
                                {
                                    Debug.Log(renderer);

                                    // Check if the mesh name ends with "LOD0"
                                    if (renderer is MeshRenderer meshRenderer)
                                    {
                                        MeshFilter meshFilter = meshRenderer.GetComponent<MeshFilter>();
                                        if (meshFilter != null && meshFilter.sharedMesh != null && meshFilter.sharedMesh.name.EndsWith("LOD0"))
                                        {
                                            int lodIndex = System.Array.IndexOf(lods, lodItem);
                                            if (lodIndex < lods.Length - 1)
                                            {
                                                Renderer nextRenderer = lods[lodIndex + 1].renderers[0];
                                                if (nextRenderer is MeshRenderer nextMeshRenderer)
                                                {
                                                    MeshFilter nextFilter = nextMeshRenderer.GetComponent<MeshFilter>();
                                                    if (nextFilter != null && nextFilter.sharedMesh != null)
                                                    {
                                                        Mesh nextMesh = nextFilter.sharedMesh;
                                                        meshFilter.sharedMesh = nextMesh;
                                                        Debug.Log("Mesh replaced with the mesh from the next LOD group.");
                                                    }
                                                }

                                                // Remove the first LOD group
                                                LOD[] newLods = new LOD[lods.Length - 1];
                                                for (int i = 1; i < lods.Length; i++)
                                                {
                                                    newLods[i - 1] = lods[i];
                                                }
                                                lod.SetLODs(newLods);
                                                Debug.Log("First LOD group removed.");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (flagForSecondTypeOfMessage)
                {
                    Debug.Log("<color=cyan>Object without LOD group:</color> " + go.name);
                }
            }
        }
    }

    [Button]
    void ShowItemsWithoutLODGroups()
    {
        CheckCountOfFBX(false, true);
    }

    [Button]
    void ShowItemsWithLODGroups()
    {
        CheckCountOfFBX(true, false);
    }
}
