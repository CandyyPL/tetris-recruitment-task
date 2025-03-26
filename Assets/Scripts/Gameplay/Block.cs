using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Block : MonoBehaviour
{
    private Rigidbody2D rb;
    public GameObject blockModel;

    [SerializeField] private int blockId;

    private float moveTime = 0.05f;
    private float baseFallTime;
    private float fallTime;
    private float timer;

    private int boardWidth;
    private int boardHeight;

    private bool isLerpActive;
    private bool isOnBottom;
    private bool isDisabling;
    private bool isInFreeFall;

    private Vector2 targetPosition;
    public Vector3 rotationPointVector;

    private AudioManager audioManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = rb.position;

        fallTime = baseFallTime;

        boardWidth = GameManager.Instance.boardWidth;
        boardHeight = GameManager.Instance.boardHeight;
        baseFallTime = GameManager.Instance.baseFallTime;
        audioManager = AudioManager.Instance;

        InputActions.DefaultActions inputActions = InputManager.Instance.inputActions.Default;

        inputActions.MoveLeft.performed += OnMoveLeft;
        inputActions.MoveRight.performed += OnMoveRight;
        inputActions.Rotate.performed += OnRotate;
        inputActions.HardDrop.performed += OnHardDrop;

        fallTime = baseFallTime;
        isInFreeFall = true;
    }

    private void Update()
    {
        // move block down when timer reaches fallTime
        timer += Time.deltaTime;
        if (timer >= fallTime)
        {
            MoveDown();
            StartCoroutine(CheckFloor());
            timer = 0;
        }

        StartCoroutine(MoveBlock(rb.position, targetPosition));

        // if block is not in free fall, do not go further
        if (!isInFreeFall) return;

        // else, handle soft drop
        if (InputManager.Instance.inputActions.Default.SoftDrop.ReadValue<float>() > 0)
        {
            fallTime = 0.1f * baseFallTime;
        }
        else
        {
            fallTime = baseFallTime;
        }
    }

    private void OnMoveLeft(InputAction.CallbackContext obj)
    {
        MoveSides(Vector2.left);
    }

    private void OnMoveRight(InputAction.CallbackContext obj)
    {
        MoveSides(Vector2.right);
    }

    private void OnRotate(InputAction.CallbackContext obj)
    {
        Rotate();
    }

    private void OnHardDrop(InputAction.CallbackContext obj)
    {
        HardDrop();
    }

    private void MoveSides(Vector2 dir)
    {
        if (!isOnBottom)
        {
            Vector2 newPosition = targetPosition + dir;

            if (IsValidPosition(newPosition))
            {
                targetPosition = newPosition;
                audioManager.PlayMoveSound();
            }
        }
    }

    private void Rotate()
    {
        if (!isOnBottom)
        {
            transform.Rotate(new Vector3(0, 0, 90));

            if (!IsValidPosition(transform.position))
            {
                transform.Rotate(new Vector3(0, 0, -90));
            }
            else
            {
                audioManager.PlayRotateSound();
            }
        }
    }

    private void MoveDown()
    {
        if (!isOnBottom)
        {
            Vector2 newPosition = targetPosition + Vector2.down;

            if (IsValidPosition(newPosition))
            {
                targetPosition = newPosition;
            }
        }
    }

    private void HardDrop()
    {
        isInFreeFall = false;
        fallTime = 0f;
    }

    // move block, and keep track of lerp
    private IEnumerator MoveBlock(Vector2 position, Vector2 target)
    {
        if (isLerpActive || position == target) yield break;

        isLerpActive = true;
        float time = 0f;

        while (time < moveTime)
        {
            float t = time / moveTime;
            Vector2 movement = Vector2.Lerp(position, target, t);
            rb.MovePosition(movement);

            time += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(target);
        isLerpActive = false;
    }

    // check for floor presence, if block reaches floor - disable it
    private IEnumerator CheckFloor()
    {
        while (isLerpActive)
        {
            yield return null;
        }

        Vector2 checkPosition = (Vector2)transform.position + Vector2.down;

        if (!IsValidPosition(checkPosition) && !isOnBottom && !isDisabling)
        {
            isOnBottom = true;
            StartCoroutine(Disable());
        }
    }

    // check for position validity, looping through each blocks' square and checking whether its new position is valid
    private bool IsValidPosition(Vector2 newPosition)
    {
        Transform[,] boardGrid = GameManager.Instance.boardGrid;

        foreach (Transform child in blockModel.transform)
        {
            Vector2 posChange = newPosition - (Vector2)transform.position;

            int x = Mathf.RoundToInt(child.transform.position.x + posChange.x);
            int y = Mathf.RoundToInt(child.transform.position.y + posChange.y);

            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight) return false;

            if (boardGrid[x, y] != null) return false;
        }

        return true;
    }

    // when block hits floor, add it to the grid
    private void AddToGrid()
    {
        foreach (Transform child in blockModel.transform)
        {
            int x = Mathf.RoundToInt(child.transform.position.x);
            int y = Mathf.RoundToInt(child.transform.position.y);

            GameManager.Instance.AddToGrid(x, y, child);
        }
    }

    // disable block when it hits floor
    private IEnumerator Disable()
    {
        isDisabling = true;
        
        // hold until lerp is active and/or position is changing
        while (isLerpActive || targetPosition != rb.position)
        {
            yield return null;
        }

        AddToGrid();

        Vector3 spawnPosition = BlockSpawner.Instance.spawnPosition;

        // if spawn position is not valid - game over
        if (!IsValidPosition(spawnPosition))
        {
            EventManager.Instance.OnGameOver?.Invoke();
            audioManager.PlayGameOverSound();
            UIManager.Instance.ShowGameOverPanel();
        }
        else
        {
            BlockSpawner.Instance.SpawnBlock();
            EventManager.Instance.OnFallTimeDecrease?.Invoke();
            audioManager.PlayDropSound();
        }
    }
}
