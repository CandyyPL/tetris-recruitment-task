using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private string gameplaySceneName = "Gameplay";

    IEnumerator Start()
    {
        AsyncOperation gameplaySceneLoading = SceneManager.LoadSceneAsync(gameplaySceneName, LoadSceneMode.Additive);
        yield return gameplaySceneLoading;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(gameplaySceneName));
    }
}
