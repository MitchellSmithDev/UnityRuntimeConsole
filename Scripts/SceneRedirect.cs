using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRedirect : MonoBehaviour
{
    [SerializeField] private bool loadOnStart = false;
    [SerializeField] private SceneReference redirectToScene = null;
    [SerializeField] private LoadSceneMode sceneMode = LoadSceneMode.Single;

    void Start()
    {
        if(loadOnStart)
            LoadScene();
    }

    public void LoadScene()
    {
        LoadScene(redirectToScene, sceneMode);
    }

    public void LoadScene(SceneReference scene, LoadSceneMode sceneMode = LoadSceneMode.Single)
    {
        if(scene != null)
            SceneManager.LoadScene(scene, sceneMode);
    }
}