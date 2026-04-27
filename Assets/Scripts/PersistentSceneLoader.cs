using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Maintaining for documentation purposes. Used to load do work before and after a scene loads
/// </summary>
public class PersistentSceneLoader
{
    const string kAdditiveManagerScene = "PersistentManagerScene";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        // Load the Manager scene
        //SceneManager.LoadScene(kAdditiveManagerScene, LoadSceneMode.Additive);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
        // Unload the manager scene since all pieces persist between scenes.
        //SceneManager.UnloadSceneAsync(kAdditiveManagerScene);
    }
}
