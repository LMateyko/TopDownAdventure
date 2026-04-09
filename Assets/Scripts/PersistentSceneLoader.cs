using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentSceneLoader
{
    const string kAdditiveManagerScene = "PersistentManagerScene";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoad()
    {
        // Load the Manager scene
        SceneManager.LoadScene(kAdditiveManagerScene, LoadSceneMode.Additive);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
        // Unload the manager scene since all pieces persist between scenes.
        SceneManager.UnloadSceneAsync(kAdditiveManagerScene);
    }
}
