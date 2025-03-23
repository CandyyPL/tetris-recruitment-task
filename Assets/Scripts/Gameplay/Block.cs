using System;
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

        InputActions.DefaultActions defaultInputActions = InputManager.Instance.inputActions.Default;

        defaultInputActions.MoveLeft.performed += OnMoveLeft;
        defaultInputActions.MoveRight.performed += OnMoveRight;
        defaultInputActions.Rotate.performed += OnRotate;
        // defaultInputActions.HardDrop.performed += OnHardDrop;

        baseFallTime = GameManager.Instance.baseFallTime;
        fallTime = baseFallTime;

        audioManager = AudioManager.Instance;
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= fallTime)
        {
            MoveDown();
            StartCoroutine(CheckFloor());
            timer = 0;
        }
    }

    private void Update()
    {
        if (InputManager.Instance.inputActions.Default.SoftDrop.ReadValue<float>() > 0)
        {
            fallTime = 0.1f * baseFallTime;
        }
        else
        {
            fallTime = baseFallTime;
        }

        StartCoroutine(MoveBlock(rb.position, targetPosition));
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

    /*private void OnHardDrop(InputAction.CallbackContext obj)
    {
        StartCoroutine(HardDrop());
    }*/

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

    /*private IEnumerator HardDrop()
    {
        while (isLerpActive || isDisabling)
        {
            yield return null;
        }

        int currentX = Mathf.RoundToInt(transform.position.x);
        int currentY = Mathf.RoundToInt(transform.position.y);

        Vector2 lastDropPosition = transform.position;

        for (int y = currentY; y >= 0; y--)
        {
            Vector2 dropPosition = new Vector2(currentX, y);

            if (IsValidPosition(dropPosition))
            {
                if (y == 0)
                {
                    if (blockId == 0)
                    {
                        // IDK
                    }
                    else if (blockId == 3)
                    {
                        targetPosition = dropPosition + new Vector2(0.5f, 0.5f);
                    }
                    else
                    {
                        targetPosition = dropPosition;
                    }

                    break;
                }

                lastDropPosition = dropPosition;
                continue;
            }
            else
            {
                if (!IsValidPosition(lastDropPosition)) yield return null;
                else
                {
                    if (blockId == 0)
                    {
                        // IDK
                    }
                    else if (blockId == 3)
                    {
                        targetPosition = lastDropPosition + new Vector2(0.5f, 0.5f);
                    }
                    else
                    {
                        targetPosition = lastDropPosition;
                    }

                    break;
                }
            }
        }

        StartCoroutine(Disable());
        yield return null;
    }*/

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

    private bool IsValidPosition(Vector2 newPosition)
    {
        Transform[,] boardGrid = GameManager.Instance.boardGrid;

        foreach (Transform child in blockModel.transform)
        {
            Vector2 posChange = newPosition - (Vector2)transform.position;

            int x = Mathf.RoundToInt(child.transform.position.x + posChange.x);
            int y = Mathf.RoundToInt(child.transform.position.y + posChange.y);

            x += 5;

            if (x < 0 || x >= boardWidth || y < 0 || y >= boardHeight) return false;

            if (boardGrid[x, y] != null) return false;
        }

        return true;
    }

    private void AddToGrid()
    {
        foreach (Transform child in blockModel.transform)
        {
            int x = Mathf.RoundToInt(child.transform.position.x);
            int y = Mathf.RoundToInt(child.transform.position.y);

            x += 5;

            GameManager.Instance.AddToGrid(x, y, child);
        }
    }

    private IEnumerator Disable()
    {
        isDisabling = true;
        
        while (isLerpActive || targetPosition != rb.position)
        {
            yield return null;
        }

        AddToGrid();

        Vector3 spawnPosition = BlockSpawner.Instance.spawnPosition;

        if (!IsValidPosition(spawnPosition))
        {
            EventManager.Instance.OnGameOver?.Invoke();
            audioManager.PlayGameOverSound();
        }
        else
        {
            BlockSpawner.Instance.SpawnBlock();
            EventManager.Instance.OnFallTimeDecrease?.Invoke();
            audioManager.PlayDropSound();
        }
    }
}
