using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneManagerScript : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        Debug.Log($"Scene change to {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
}
