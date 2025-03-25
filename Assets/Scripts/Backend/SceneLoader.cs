using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneAsset gameplayScene;

    IEnumerator Start()
    {
        AsyncOperation gameplaySceneLoading = SceneManager.LoadSceneAsync(gameplayScene.name, LoadSceneMode.Additive);
        yield return gameplaySceneLoading;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameplayScene.name));
    }
}
