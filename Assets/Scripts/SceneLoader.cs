using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneToLoad;

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    
    public void LoadScene()
    {
        LoadScene(sceneToLoad);
    }
}
