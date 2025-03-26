using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        EventManager.Instance.OnLineClear += OnLineClear;
    }

    private void OnLineClear()
    {
        scoreText.text = (GameManager.Instance.playerScore + 100).ToString();
    }
}
