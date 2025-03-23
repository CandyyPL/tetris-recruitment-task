using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void PlayAgain()
    {
        Scene gameplayScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(gameplayScene.name, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(gameplayScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
