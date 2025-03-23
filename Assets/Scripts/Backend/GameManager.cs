using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int boardWidth = 10;
    public int boardHeight = 20;

    public float baseFallTime;
    public float baseScore;

    public Transform[,] boardGrid;
    public Vector3 localPlayerBoardPosition;

    public int localPlayerScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        EventManager.Instance.OnFallTimeDecrease += OnFallTimeDecrease;
        EventManager.Instance.OnLineClear += OnLineClear;

        boardGrid = new Transform[boardWidth, boardHeight];
        baseFallTime = 0.8f;
        baseScore = 100;
        localPlayerBoardPosition = new Vector3(-0.5f, 9.5f, 0);
    }

    private void Update()
    {
        for (int line = 0; line < boardHeight; line++)
        {
            if (IsLineTaken(line))
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    for (int x = 0; x < boardWidth; x++)
                    {
                        if (y == line)
                        {
                            Destroy(boardGrid[x, y].gameObject);
                        }
                        else if (y > line)
                        {
                            if (boardGrid[x, y] != null)
                            {
                                boardGrid[x, y].transform.position += Vector3.down;
                                AddToGrid(x, y - 1, boardGrid[x, y]);
                                boardGrid[x, y] = null;
                            }
                        }
                    }
                }

                EventManager.Instance.OnLineClear.Invoke();
            }
        }
    }

    public void AddToGrid(int x, int y, Transform transform)
    {
        boardGrid[x, y] = transform;
    }

    private bool IsLineTaken(int y)
    {
        for (int x = 0; x < boardWidth; x++)
        {
            if (boardGrid[x, y] == null) return false;
        }

        return true;
    }

    private void OnFallTimeDecrease()
    {
        baseFallTime *= 0.98f;
    }

    private void OnLineClear()
    {
        localPlayerScore += Mathf.RoundToInt(baseScore);
        baseScore *= 1.05f;
    }
}
