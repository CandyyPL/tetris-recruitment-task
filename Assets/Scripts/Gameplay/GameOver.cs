using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    [SerializeField] private SceneAsset backendScene;

    public void PlayAgain()
    {
        SceneManager.LoadScene(backendScene.name, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
